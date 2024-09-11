using Sofar.CommunicationLib.Connection;
using Sofar.ProtocolLibs.Modbus;
using Sofar.ProtocolLibs.Modbus.Message;
using Sofar.ProtocolLibs.Modbus.Message.Standard;
using Sofar.ProtocolLibs.Utils.CRC;
using System.Diagnostics;

namespace Sofar.CommunicationLib.Modbus
{
    public class ModbusClient
    {
        protected int _frameBaseSize;     // PDU以外的大小

        protected int _frameHeaderSize;   // 功能码之前的大小

        private ModbusFrameType _frameType;

        public TinyBytesStream CommStream { get; }

        public ModbusFrameType FrameType
        {
            get => _frameType;
            init
            {
                _frameType = value;
                switch (value)
                {
                    case ModbusFrameType.RTU:
                        _frameBaseSize = 1 + 2;   // Address + CRC16
                        _frameHeaderSize = 1;
                        break;

                    case ModbusFrameType.TCP:
                        _frameBaseSize = 6 + 1;   // MBAP Header + Address
                        _frameHeaderSize = 6 + 1;
                        break;

                    case ModbusFrameType.ASCII:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        public ModbusClient(TinyBytesStream commStream, ModbusFrameType frameType)
        {
            CommStream = commStream;
            FrameType = frameType;
        }

        public event EventHandler<DataTransactionEventArgs>? ModbusDataReceived;

        public event EventHandler<DataTransactionEventArgs>? ModbusDataSent;

        // public event EventHandler<CommunicationExceptionEventArgs>? OnExceptionRaised;

        public ReadHoldingRegistersResponse? ReadHoldingRegisters(byte slave, ushort startAddress, byte count, int rxTimeoutMs = 1000)
        {
            var requestEntity = new ReadHoldingRegistersRequest(startAddress, count);
            return Execute<ReadHoldingRegistersRequest, ReadHoldingRegistersResponse>(slave, requestEntity, rxTimeoutMs);
        }

        public WriteMultipleRegistersResponse? WriteHoldingRegisters(byte slave, ushort address, ushort[] registersValues, int rxTimeoutMs = 1000)
        {
            var requestEntity = new WriteMultipleRegistersRequest(address, registersValues);
            return Execute<WriteMultipleRegistersRequest, WriteMultipleRegistersResponse>(slave, requestEntity, rxTimeoutMs);
        }

        public ReadDeviceIdentificationResponse? ReadDeviceIdentification(byte slave, byte devIdCode, byte firstObjId, int rxTimeoutMs = 1000)
        {
            var request = new ReadDeviceIdentificationRequest(devIdCode, firstObjId);
            return Execute<ReadDeviceIdentificationRequest, ReadDeviceIdentificationResponse>(slave, request, rxTimeoutMs);
        }

        public void Send(byte[] data)
        {
            try
            {
                lock (CommStream)
                {
                    CommStream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                // OnExceptionRaised?.Invoke(this, new CommunicationExceptionEventArgs(ex));
                Debug.WriteLine(ex);
                throw;
            }

            ModbusDataSent?.Invoke(this, new DataTransactionEventArgs(data));
        }

        protected void ReceiveOneFrame(long txTimestamp, int rxTimeoutMs, out byte[] received, int? expectedLen = null)
        {
            byte[] rxBuffer = new byte[1024];
            int rxIntervalMs = 50;

            received = Array.Empty<byte>();
            long lastRxTimestamp = -1;
            bool waiting = true;
            int rxOffset = 0;
            CommStream.SetReadTimeout(rxTimeoutMs);

            do
            {
                if (CommStream.GetReadableCount() > 0)
                {
                    // Thread.Sleep(rxIntervalMs);
                    int rxLen = CommStream.Read(rxBuffer, rxOffset, rxBuffer.Length - rxOffset);
                    if (rxLen > 0)
                    {
                        lastRxTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        waiting = false;
                        rxOffset += rxLen;

                        if (expectedLen != null && rxOffset == expectedLen)
                            break;

                        if (rxOffset == rxBuffer.Length)
                            break;
                    }
                }
                else
                {
                    if (waiting && DateTimeOffset.Now.ToUnixTimeMilliseconds() - txTimestamp > rxTimeoutMs)
                    {
                        throw new TimeoutException($"The timeout of {rxTimeoutMs}ms has reached when waiting for a response.");
                        break;
                    }
                    else if (!waiting && DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastRxTimestamp > rxIntervalMs * 1.5)
                    {
                        break;
                    }
                }

                Thread.Sleep(rxIntervalMs);
            } while (true);

            received = rxBuffer.Take(rxOffset).ToArray();

            ModbusDataReceived?.Invoke(this, new DataTransactionEventArgs(received));
        }

        protected void ClearRxStream()
        {
            byte[] buffer = new byte[1024];
            CommStream.SetReadTimeout(100);

            while (CommStream.GetReadableCount() > 0)
            {
                int readLen = CommStream.Read(buffer, 0, buffer.Length);
                if (readLen > 0)
                {
                    var received = buffer.Take(readLen).ToArray();
                    // OnDataReceived?.Invoke(this, new DataTransactionEventArgs(received));
                }
            }
        }

        protected void DoModbusTransaction(byte slave, RequestPDU requestPdu, out byte[] response, int rxTimeoutMs)
        {
            lock (CommStream)
            {
                ClearRxStream();
                response = Array.Empty<byte>();
                switch (_frameType)
                {
                    case ModbusFrameType.RTU:
                        DoRtuTransaction(slave, requestPdu, out response, rxTimeoutMs);
                        break;

                    case ModbusFrameType.TCP:
                        break;

                    case ModbusFrameType.ASCII:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected void DoRtuTransaction(byte slave, RequestPDU request, out byte[] response, int rxTimeoutMs)
        {
            response = Array.Empty<byte>();
            byte[] frameBytes = ModbusFramer.ProduceRtuFrameBytes(slave, request);
            int? expectedRxLen = 1 + request.GetResponsePduSize() + 2;

            Send(frameBytes);
            if (slave == 0)
                return;

            long txTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            ReceiveOneFrame(txTimestamp, rxTimeoutMs, out byte[] rxBytes, expectedRxLen);

            if (rxBytes.Length == 0)
                return;

            bool parseResult = false;
            string? errorMsg = "";
            int msgOffset = 0, msgSize = 0;
            for (int p = 0; p < rxBytes.Length; p++)
            {
                if (rxBytes.Length - p < 5)
                {
                    errorMsg = "The incoming message is invalid.";
                    break;
                }

                if (rxBytes[p] != slave && !(rxBytes[p] == 0 && rxBytes[p + 1] == 0x52))
                    continue;

                byte rxFuncCode = rxBytes[p + 1];
                if (rxFuncCode == 0x90 || rxFuncCode == request.FunctionCode + 0x80)  // Modbus Exception Response
                {
                    if (CRCHelper.CheckModbusCrc16(rxBytes, p, 5, false))
                    {
                        parseResult = true;
                        msgOffset = p;
                        msgSize = 5;
                        RaiseModbusException(new ModbusErrorResponse(rxBytes, p + _frameHeaderSize, 5 - _frameBaseSize));
                        break;
                    }
                }
                else if (rxFuncCode == request.FunctionCode
                         && expectedRxLen != null && rxBytes.Length - p >= expectedRxLen)
                {
                    if (CRCHelper.CheckModbusCrc16(rxBytes, p, expectedRxLen.Value, false))
                    {
                        parseResult = true;
                        msgOffset = p;
                        msgSize = expectedRxLen.Value;
                        break;
                    }
                }
                else if (rxFuncCode == request.FunctionCode && expectedRxLen == null)
                {
                    if (CRCHelper.CheckModbusCrc16(rxBytes, p, rxBytes.Length - p, false))
                    {
                        parseResult = true;
                        msgOffset = p;
                        msgSize = rxBytes.Length - p;
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(errorMsg) || !parseResult)
                return;

            response = rxBytes.Skip(msgOffset).Take(msgSize).ToArray();
        }

        protected void DoTcpTransaction(byte slave, RequestPDU request, out byte[] response, int rxTimeoutMs)
        {
            throw new NotImplementedException();
        }

        // Return null if broadcast(slave=0), return response entity if finished successful transaction, else raise exception
        protected TResponse? Execute<TRequest, TResponse>(byte slave, TRequest requestEntity, int rxTimeoutMs)
            where TRequest : RequestPDU
            where TResponse : ResponsePDU
        {
            DoModbusTransaction(slave, requestEntity, out byte[] responseBytes, rxTimeoutMs);
            if (slave == 0)
                return null;

            TResponse? responseEntity = Activator.CreateInstance(typeof(TResponse),
                responseBytes, _frameHeaderSize, responseBytes.Length - _frameBaseSize) as TResponse;
            return responseEntity;
        }

        protected void RaiseModbusException(ModbusErrorResponse response)
        {
            throw new ModbusException($"Modbus error. Error code: {response.ErrorCode}", response.ModbusError);
        }

        public bool ModbusTryBroadcast<TRequest>(TRequest requestEntity) where TRequest : RequestPDU
        {
            try
            {
                DoModbusTransaction(0, requestEntity, out _, -1);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}