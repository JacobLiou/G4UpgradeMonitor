using System.Diagnostics;

namespace Sofar.CommunicationLib
{
    public class ConnectionParams
    {
        // 管理机IP
        public string IP { get; set; } = "192.168.1.11";

        // 串口连接参数
        public string SerialPortName { get; set; } = "COM2";

        public int BaudRate { get; set; } = 115200;
        // public StopBits StopBits { get; set; }
        // public Parity Parity { get; set; }
        // public int DataBits { get; set; }

        // Sofar Http Url
        public string Url => $"https://{IP}/api";

        // 上次连接的类型
        public ConnectionType PreviousConnectionType { get; set; } = ConnectionType.None;

        // public byte[]? PreviousNormalModeTaskIds = null;
    }

    public enum GcpMaintainChannel
    {
        None = 0,
        CCO1 = 8888,
        CCO2 = 9999,
        // PID1 = 6666,
        // PID2 = 7777,
    }

    public enum GcpRunningMode
    {
        None = 0,
        Normal,
        Maintenance,
    }

    public enum ConnectionType
    {
        None,
        SerialPort,
        Gcp7428,
    }

    public class CommManager
    {
        #region Singleton

        private static readonly Lazy<CommManager> _lazySingleton = new(() => new CommManager());

        public static CommManager Instance => _lazySingleton.Value;

        private CommManager()
        {
        }

        #endregion Singleton

        public event EventHandler ConnectionStatusChanged;

        public ConnectionParams ConnectionParams { get; set; }

        public ConnectionType ConnectionStatus { get; private set; } = ConnectionType.None;

        public GcpRunningMode GcpRunningMode { get; private set; }

        public GcpMaintainChannel GcpMaintainChannel { get; private set; } = GcpMaintainChannel.None;

        private long _lastCheckModeTime = 0;
        private bool _isCheckingMode = false;
        private int _checkModeInterval = 5000;
        private Task? _checkingGcpModeTask = null;

        private void StartCheckingGcpMode()
        {
            _isCheckingMode = true;

            bool failed = false;
            _checkingGcpModeTask = Task.Factory.StartNew(() =>
            {
                while (_isCheckingMode)
                {
                    try
                    {
                        ConnectionStatusChanged?.Invoke(this, EventArgs.Empty);
                        Thread.Sleep(_checkModeInterval);
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception);
                        // if (this.GcpTasksManager != null &&
                        //     this.GcpTasksManager.CommStream.Reconnect(10, 5000))
                        // {
                        //    continue;
                        // }
                        // else
                        // {
                        //
                        // }

                        break;
                    }
                }

                _isCheckingMode = false;
            }, TaskCreationOptions.LongRunning);
        }

        public void SwitchConnection(ConnectionType none, ConnectionParams connectParams)
        {
            throw new NotImplementedException();
        }
    }
}