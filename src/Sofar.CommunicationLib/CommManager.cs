using Sofar.CommunicationLib.Connection;
using Sofar.CommunicationLib.Modbus;
using Sofar.CommunicationLib.Model;
using Sofar.ProtocolLibs.Modbus;

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

        public bool Connect(ConnectionParams connectParams)
        {
            if (connectParams == null
                || connectParams.IPAdressList == null
                || connectParams.IPAdressList.Count == 0)
                return false;

            var allConnect = new List<bool>();
            for (int i = 0; i < connectParams.IPAdressList.Count; i++)
            {
                var tcpStream = new TcpStream(i.ToString(), connectParams.IPAdressList[i], connectParams.Port);
                allConnect.Add(tcpStream.Connect());
                var ModbusClient = new SofarModbusClient(tcpStream, ModbusFrameType.TCP);
                ModbusClients.Add(ModbusClient);
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