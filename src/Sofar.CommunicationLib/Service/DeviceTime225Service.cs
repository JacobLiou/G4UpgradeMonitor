using System.Diagnostics;

namespace Sofar.CommBoxLib.Service
{
    internal class DeviceTime225Service
    {
        public DeviceTime225Service()
        {
            
        }

        public bool ReadDevTime(byte devAddr, out string datatime)
        {
            datatime = "";
            if (!CheckConnect())
                return false;

            SofarModbusHelper comm = CommunMgr.GetInstance().GetCommHelper();

            byte[] request = ModbusHelper.ReadHoldingRegisters(devAddr, 0x042C, 6);
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(300);
                if (comm.ReadHoldingRegisters(request, out byte[] response))
                {
                    if (response != null
                        && response.Length == 3 + 2 * 6 + 2
                        && response[1] == 0x03
                        && CheckCrc(response))
                    {
                        int year = 2000 + (response[3] << 8 | response[4]);
                        int month = response[5] << 8 | response[6];
                        int day = response[7] << 8 | response[8];
                        int hour = response[9] << 8 | response[10];
                        int minute = response[11] << 8 | response[12];
                        int second = response[13] << 8 | response[14];

                        datatime = $"{year:D4}/{month:D2}/{day:D2} {hour:D2}:{minute:D2}:{second:D2}";
                        return true;
                    }
                }

            }

            return false;
        }

        public bool WriteDevTime(byte devAddr, DateTime datetime)
        {
            if (!CheckConnect())
                return false;

            SofarModbusHelper comm = CommunMgr.GetInstance().GetCommHelper();

            var dataToWrite = new byte[12];
            
            dataToWrite[0] = (byte)((datetime.Year - 2000) >> 8);
            dataToWrite[1] = (byte)(datetime.Year - 2000);

            dataToWrite[2] = (byte)(datetime.Month >> 8);
            dataToWrite[3] = (byte)datetime.Month;

            dataToWrite[4] = (byte)(datetime.Day >> 8);
            dataToWrite[5] = (byte)datetime.Day;

            dataToWrite[6] = (byte)(datetime.Hour >> 8);
            dataToWrite[7] = (byte)datetime.Hour;

            dataToWrite[8] = (byte)(datetime.Minute >> 8);
            dataToWrite[9] = (byte)datetime.Minute;

            dataToWrite[10] = (byte)(datetime.Second >> 8);
            dataToWrite[11] = (byte)datetime.Second;

            bool result = false;
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(100);
                if (comm.WriteHoldingRegisters(devAddr, 0x1004, dataToWrite, out var response))
                {
                    if (response != null
                        && response.Length == 8
                        && response[1] == 0x10
                        && CheckCrc(response))
                    {
                        result = true;
                        break;
                    }
                }
            }

            if (result)
            {
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(100);
                    if (comm.WriteHoldingRegisters(devAddr, 0x100A, new byte[]{0, 1}, out var response))
                    {
                        if (response != null
                            && response.Length == 8
                            && response[1] == 0x10
                            && CheckCrc(response))
                        {
                            result = true;
                            break;
                        }
                        result = false;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }

            return result;

        }

        public bool CheckWriteResult(byte devAddr, out ushort resultCode)
        {
            resultCode = 0xFFFF;
            if (!CheckConnect())
                return false;

            SofarModbusHelper comm = CommunMgr.GetInstance().GetCommHelper();

            byte[] request = ModbusHelper.ReadHoldingRegisters(devAddr, 0x042C, 6);
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(300);
                if (comm.ReadHoldingRegisters(request, out byte[] response))
                {
                    if (response != null
                        && response.Length == 3 + 2 * 1 + 2
                        && response[1] == 0x03
                        && CheckCrc(response))
                    {
                        resultCode = (ushort)(response[3] << 8 | response[4]);
                        return true;
                    }
                }

            }

            return false;
        }

        public bool CheckConnect()
        {
            if (CommunMgr.GetInstance().ConnectState == ConnectionStatus.UnConnected)
                return false;
            if (!TaskModeSwitch.GetInstance().Switch2PenetrateMode())
                return false;
            return true;
        }

        private bool CheckCrc(byte[] response)
        {
            if (response == null)
                return false;

            try
            {
                int checkCrc = CRCHelper.ComputeCrc16(response, response.Length - 2);
                if (response[^2] != (byte)(checkCrc % 256) || response[^1] != (byte)(checkCrc / 256))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("检查CRC数据异常，ERROR:" + ex.Message);
                return false;
            }
            return true;
        }
    }
}
