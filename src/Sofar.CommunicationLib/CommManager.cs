using Sofar.CommunicationLib.Connection;
using Sofar.CommunicationLib.Modbus;
using Sofar.CommunicationLib.Model;
using Sofar.ProtocolLibs.Modbus;
using System.Collections.Generic;

namespace Sofar.CommunicationLib
{
    public class CommManager
    {
        public ConnectionParams ConnectionParams { get; set; }

        public List<SofarModbusClient> ModbusClients { get; set; } = new();

        #region Singleton

        private static readonly Lazy<CommManager> _lazySingleton = new(() => new CommManager());

        public static CommManager Instance => _lazySingleton.Value;

        private CommManager()
        {
        }

        #endregion Singleton

        public bool Connect(ConnectionParams connectParams,out Dictionary<string,bool> pairs)
        {
            pairs = new();//初始化

            if (connectParams == null
                || connectParams.IPAdressList == null
                || connectParams.IPAdressList.Count == 0)
                return false;

            DisConnect();
            ModbusClients = new();
            var allConnect = new List<bool>();
            for (int i = 0; i < connectParams.IPAdressList.Count; i++)
            {
                var tcpStream = new TcpStream(i.ToString(), connectParams.IPAdressList[i], connectParams.Port);
                var connectState = tcpStream.Connect();
                allConnect.Add(connectState);
                var ModbusClient = new SofarModbusClient(tcpStream, ModbusFrameType.TCP);
                ModbusClients.Add(ModbusClient);

                if (!pairs.TryAdd(connectParams.IPAdressList[i], connectState))
                {
                    pairs[connectParams.IPAdressList[i]] = connectState;
                }
            }

            return allConnect.All(item => item);
        }

        public void DisConnect()
        {
            if (!ModbusClients.Any()) return;

            foreach (var client in ModbusClients)
            {
                client.CommStream.Dispose();
            }
        }
    }
}