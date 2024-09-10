using Serilog;
using Sofar.CommunicationLib.Modbus;

namespace Sofar.CommunicationLib.Service
{
    public class ServiceBase
    {
        public static ILogger _logger => Serilog.Log.Logger;

        #region Modbus Utils

        protected static SofarModbusClient? _modbusClient;

        public static bool TryReadRegistersBytes(byte slave, ushort startAddr, byte count,
                                        out byte[]? regsBytes, int retries = 1, int timeoutMs = 1000)
        {
            regsBytes = null;
            if (_modbusClient == null)
                return false;

            try
            {
                for (int i = 0; i < retries; i++)
                {
                    var response = _modbusClient.ReadHoldingRegisters(slave, startAddr, count, timeoutMs);
                    if (response != null)
                    {
                        regsBytes = response.RegistersBytes;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
            return false;
        }

        public static bool TryReadRegisters(byte slave, ushort startAddr, byte count,
                                        out ushort[]? regsValues, int retries = 1, int timeoutMs = 1000)
        {
            regsValues = null;
            if (_modbusClient == null)
                return false;

            try
            {
                for (int i = 0; i < retries; i++)
                {
                    var response = _modbusClient.ReadHoldingRegisters(slave, startAddr, count, timeoutMs);
                    if (response != null)
                    {
                        regsValues = response.RegistersValues;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // ignored
            }

            return false;
        }

        public static bool TryWriteRegistersBytes(byte slave, ushort startAddr, in byte[] regsBytes,
                                            int retries = 1, int timeoutMs = 1000)
        {
            if (_modbusClient == null)
                return false;

            if (regsBytes.Length % 2 != 0)
                return false;

            ushort[] regsValues = new ushort[regsBytes.Length / 2];
            for (int i = 0; i < regsValues.Length; i++)
            {
                regsValues[i] = (ushort)(regsBytes[2 * i] << 8 | regsBytes[2 * i + 1]);
            }

            try
            {
                for (int i = 0; i < retries; i++)
                {
                    var response = _modbusClient.WriteHoldingRegisters(slave, startAddr, regsValues, timeoutMs);
                    if (response != null)
                        return true;
                }
            }
            catch (Exception ex)
            {
                // ignored
            }

            return false;
        }

        public static bool TryWriteRegisters(byte slave, ushort startAddr, in ushort[] regsValues,
                                            int retries = 1, int timeoutMs = 1000)
        {
            if (_modbusClient == null)
                return false;

            try
            {
                for (int i = 0; i < retries; i++)
                {
                    var response = _modbusClient.WriteHoldingRegisters(slave, startAddr, regsValues, timeoutMs);
                    if (response != null)
                        return true;
                }
            }
            catch (Exception ex)
            {
                // ignored
            }

            return false;
        }

        #endregion Modbus Utils
    }
}