using Sofar.CommunicationLib.Service.AppModels;
using Sofar.ProtocolLibs.FirmwareInfo;
using System.Diagnostics;
using System.Text;

namespace Sofar.CommunicationLib.Service.FileTransfer
{
    public class ReadInverterInfoService : ServiceBase
    {

        #region G4读取设备信息

        public static InverterInfo? TryReadDeviceInfoG4(byte slave)
        {
            if (_modbusClient == null)
                return null;

            List<G4DeviceDescription> descriptions = new(8);
            try
            {
                var responseReadId = _modbusClient.ReadDeviceIdentification(slave, 0x04, 0x87, 1500);
                if (responseReadId == null || responseReadId.NextObjectId == 0x87)
                    return null;

                var allObjects = responseReadId.Objects;
                var descObjs = allObjects.Where(x => x.ObjectID > 0x87);

                foreach (var obj in descObjs)
                {
                    if (TryParseG4DeviceDescription(Encoding.ASCII.GetString(obj.ObjectValue), out var desc))
                    {
                        descriptions.Add(desc!);
                    }
                }

                var infos = new InverterInfo()
                {
                    SerialNumber = descriptions.Find(x => !string.IsNullOrEmpty(x.SN))?.SN,
                    ARM_Version = descriptions
                        .Find(x => (FirmwareChipRole)x.RoleCode == FirmwareChipRole.ARM
                                   || (FirmwareChipRole)x.RoleCode == FirmwareChipRole.ARM_OLD)?
                        .SoftwareVersion,
                    DSPM_Version = descriptions
                        .Find(x => (FirmwareChipRole)x.RoleCode == FirmwareChipRole.DSPM
                                   || (FirmwareChipRole)x.RoleCode == FirmwareChipRole.DSPM_OLD)?
                        .SoftwareVersion,
                    DSPS_Version = descriptions
                        .Find(x => (FirmwareChipRole)x.RoleCode == FirmwareChipRole.DSPS
                                   || (FirmwareChipRole)x.RoleCode == FirmwareChipRole.DSPS_OLD)?
                        .SoftwareVersion,
                };


                var responseStaVer = _modbusClient.ReadHoldingRegisters(slave, 8453, 15);
                if (responseStaVer != null)
                {
                    infos.PLCSTA_Version = Encoding.ASCII.GetString(responseStaVer.InfoData.Skip(1).ToArray()).Trim('\0');
                }

                return infos;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }


        }

        private static bool TryParseG4DeviceDescription(string infoString, out G4DeviceDescription? description)
        {
            description = null;
            try
            {
                int descCount = 0;
                for (int i = 0; i < infoString.Length - 1; i++)
                {
                    if (infoString[i] == '6' && infoString[i + 1] == '=')
                    {
                        int semicolonIdx = i + 1 + infoString.Substring(i + 1).IndexOf(';');
                        if (semicolonIdx <= i + 2)
                        {
                            return false;
                        }
                        string valueStr = infoString.Substring(i + 2, semicolonIdx - (i + 2));
                        if (uint.TryParse(valueStr, out uint roleCode))
                        {
                            description = new G4DeviceDescription(roleCode);
                            descCount++;
                        }
                    }
                }

                if (description == null)
                    return false;

                for (int i = 0; i < infoString.Length - 1; i++)
                {
                    if (infoString[i] == '1' && infoString[i + 1] == '=')
                    {
                        int semicolonIdx = i + 1 + infoString.Substring(i + 1).IndexOf(';');
                        if (semicolonIdx <= i + 2)
                        {
                            continue;
                        }
                        string valueStr = infoString.Substring(i + 2, semicolonIdx - (i + 2));
                        description.DeviceModel = valueStr;

                    }
                    else if (infoString[i] == '2' && infoString[i + 1] == '=')
                    {
                        int semicolonIdx = i + 1 + infoString.Substring(i + 1).IndexOf(';');
                        if (semicolonIdx <= i + 2)
                        {
                            continue;
                        }
                        string valueStr = infoString.Substring(i + 2, semicolonIdx - (i + 2));
                        description.SoftwareVersion = valueStr;

                    }
                    else if (infoString[i] == '3' && infoString[i + 1] == '=')
                    {
                        int semicolonIdx = i + 1 + infoString.Substring(i + 1).IndexOf(';');
                        if (semicolonIdx <= i + 2)
                        {
                            continue;
                        }
                        string valueStr = infoString.Substring(i + 2, semicolonIdx - (i + 2));
                        description.HardwareVersion = valueStr;

                    }
                    else if (infoString[i] == '4' && infoString[i + 1] == '=')
                    {
                        int semicolonIdx = i + 1 + infoString.Substring(i + 1).IndexOf(';');
                        if (semicolonIdx <= i + 2)
                        {
                            continue;
                        }
                        string valueStr = infoString.Substring(i + 2, semicolonIdx - (i + 2));
                        description.ProtocolVersion = valueStr;

                    }
                    else if (infoString[i] == '5' && infoString[i + 1] == '=')
                    {
                        int semicolonIdx = i + 1 + infoString.Substring(i + 1).IndexOf(';');
                        if (semicolonIdx <= i + 2)
                        {
                            continue;
                        }
                        string valueStr = infoString.Substring(i + 2, semicolonIdx - (i + 2));
                        description.SN = valueStr;

                    }
                    else if (infoString[i] == '7' && infoString[i + 1] == '=')
                    {
                        int semicolonIdx = i + 1 + infoString.Substring(i + 1).IndexOf(';');
                        if (semicolonIdx <= i + 2)
                        {
                            continue;
                        }
                        string valueStr = infoString.Substring(i + 2, semicolonIdx - (i + 2));
                        description.SpecialVersion = valueStr;
                    }

                }


                return true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        #endregion


        #region G3读取设备信息

        // 查询版本号、序列号等
        public static InverterInfo? TryReadDeviceInfoG3(byte slave)
        {

            if (!ReadG3DeviceInfo(slave, out byte[] data))
            {
                return null;
            }

            var info = new InverterInfo();

            int baseAddr = 0x0445;

            int currAddr = 0x0445;
            int regsCount = 8;
            info.SerialNumber = Encoding.ASCII.GetString(data.Skip((currAddr - baseAddr) * 2).Take(regsCount * 2).ToArray()).Trim('\0');

            currAddr = 0x044F;
            regsCount = 4;
            info.ARM_Version = Encoding.ASCII.GetString(data.Skip((currAddr - baseAddr) * 2).Take(regsCount * 2).Skip(1).ToArray()).Trim('\0');

            currAddr = 0x0453;
            regsCount = 4;
            info.DSPM_Version = Encoding.ASCII.GetString(data.Skip((currAddr - baseAddr) * 2).Take(regsCount * 2).Skip(1).ToArray()).Trim('\0');

            currAddr = 0x0457;
            regsCount = 4;
            info.DSPS_Version = Encoding.ASCII.GetString(data.Skip((currAddr - baseAddr) * 2).Take(regsCount * 2).Skip(1).ToArray()).Trim('\0');

            currAddr = 0x0460;
            regsCount = 4;
            // info.Compliance_Version = Encoding.ASCII.GetString(data.Skip((currAddr - baseAddr) * 2).Take(regsCount * 2).Skip(1).ToArray()).Trim('\0');

            currAddr = 0x0466;
            regsCount = 4;
            info.AFCI_Version = Encoding.ASCII.GetString(data.Skip((currAddr - baseAddr) * 2).Take(regsCount * 2).ToArray()).Trim('\0');

            currAddr = 0x0470;
            regsCount = 2;
            info.SerialNumber += Encoding.ASCII.GetString(data.Skip((currAddr - baseAddr) * 2).Take(regsCount * 2).ToArray()).Trim('\0');

            Thread.Sleep(150);
            if (ReadSTAVersion(slave, out data))
            {
                info.PLCSTA_Version = Encoding.ASCII.GetString(data.ToArray()).Trim('\0');
            }

            return info;
        }

        private static bool ReadG3DeviceInfo(byte slave, out byte[] regData)
        {
            regData = Array.Empty<byte>();
            if (_modbusClient == null)
                return false;
            try
            {

                var response = _modbusClient.ReadHoldingRegisters(slave, 0x0445, 45);
                if (response != null)
                {
                    regData = response.RegistersBytes;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }


            return false;
        }

        private static bool ReadSTAVersion(byte slave, out byte[] regData)
        {
            regData = Array.Empty<byte>();
            if (_modbusClient == null)
                return false;

            try
            {
                var response = _modbusClient.ReadHoldingRegisters(slave, 0x2105, 15);
                if (response != null)
                {
                    regData = response.RegistersBytes;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            return false;
        }

        #endregion



    }
}
