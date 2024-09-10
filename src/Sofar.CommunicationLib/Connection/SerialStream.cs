using System.Diagnostics;
using System.IO.Ports;

namespace Sofar.CommunicationLib.Connection
{
    public class SerialStream : TinyBytesStream
    {
        public SerialPort SerialPort { get; private set; }

        private string _portName;

        private int _baudRate;

        public SerialStream(string identifier, string portName, int baudRate) : base(identifier)
        {
            SerialPort = new SerialPort(portName, baudRate);
            _portName = portName;
            _baudRate = baudRate;
        }

        ~SerialStream()
        {
            Dispose();
        }

        public override void Dispose()
        {
            Close();
        }

        public void Connect()
        {
            SerialPort.Open();
        }

        public void Close()
        {
            SerialPort.Close();
            SerialPort.Dispose();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int rxLen = 0;
            try
            {
                rxLen = SerialPort.Read(buffer, offset, count);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                OnExceptionRaised(new CommunicationExceptionEventArgs(ex));

                if (ex is InvalidOperationException && !Reconnect(2, 2000))
                {
                    throw;
                }
            }

            if (rxLen > 0)
            {
                OnDataReceived(new DataTransactionEventArgs(buffer.Skip(offset).Take(rxLen).ToArray()));
            }
            return rxLen;
        }

        public override int Write(byte[] data, int offset, int count)
        {
            try
            {
                SerialPort.Write(data, offset, count);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                OnExceptionRaised(new CommunicationExceptionEventArgs(ex));

                if (ex is InvalidOperationException && !Reconnect(2, 2000))
                {
                    throw;
                }
            }

            OnDataSent(new DataTransactionEventArgs(data.Skip(offset).Take(count).ToArray()));
            return count;
        }

        public override bool CheckConnection()
        {
            if (SerialPort.IsOpen)
            {
                try
                {
                    SerialPort.Write(Array.Empty<byte>(), 0, 0);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public override bool Reconnect(int retries, int intervalMs)
        {
            do
            {
                if (SerialPort != null)
                {
                    SerialPort.Dispose();
                }

                try
                {
                    SerialPort = new SerialPort(_portName, _baudRate);
                    Connect();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Thread.Sleep(intervalMs);
                    continue;
                }
            } while (--retries > 0);

            return false;
        }

        public override int GetReadableCount()
        {
            try
            {
                return SerialPort.BytesToRead;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                OnExceptionRaised(new CommunicationExceptionEventArgs(ex));
                throw;
            }
        }

        public override void SetReadTimeout(int rxTimeoutMs)
        {
            SerialPort.ReadTimeout = rxTimeoutMs;
        }
    }
}