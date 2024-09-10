namespace Sofar.CommunicationLib.Connection
{
    public abstract class TinyBytesStream : IDisposable
    {
        public event EventHandler<DataTransactionEventArgs> DataReceived;

        public event EventHandler<DataTransactionEventArgs> DataSent;

        public event EventHandler<CommunicationExceptionEventArgs> ExceptionRaised;

        public string Identifier { get; }

        protected TinyBytesStream(string identifier)
        {
            Identifier = identifier;
        }

        /// <summary>
        /// 读入
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public abstract int Read(byte[] buffer, int offset, int count);

        /// <summary>
        /// 写出
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public abstract int Write(byte[] data, int offset, int count);

        public abstract bool CheckConnection();

        public abstract bool Reconnect(int retries, int intervalMs);

        public abstract int GetReadableCount();

        public abstract void SetReadTimeout(int rxTimeoutMs);

        public abstract void Dispose();

        protected virtual void OnDataReceived(DataTransactionEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        protected virtual void OnDataSent(DataTransactionEventArgs e)
        {
            DataSent?.Invoke(this, e);
        }

        protected virtual void OnExceptionRaised(CommunicationExceptionEventArgs e)
        {
            ExceptionRaised?.Invoke(this, e);
        }
    }

    public class DataTransactionEventArgs : EventArgs
    {
        public DataTransactionEventArgs(byte[] data)
        {
            Data = data;
            Timestamp = DateTime.Now;
        }

        public byte[] Data { get; private set; } = Array.Empty<byte>();

        public DateTime Timestamp { get; private set; }
    }

    public class CommunicationExceptionEventArgs : EventArgs
    {
        public CommunicationExceptionEventArgs(Exception exception)
        {
            ExceptionCaught = exception;
        }

        public Exception ExceptionCaught { get; private set; }
    }
}