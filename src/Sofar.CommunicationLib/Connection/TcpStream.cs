using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Sofar.CommunicationLib.Connection
{
    public class TcpStream : TinyBytesStream
    {
        public Socket Socket { get; private set; }

        public IPEndPoint RemoteEndPoint { get; }

        public TcpStream(string identifier, string ip, int port) : base(identifier)
        {
            RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        ~TcpStream()
        {
            Dispose();
        }

        public override void Dispose()
        {
            Close();
        }

        public bool Connect(int timeoutMs = 2000)
        {
            if (!Socket.ConnectAsync(RemoteEndPoint).Wait(timeoutMs))
                return false;
            Socket.Blocking = false;
            return true;
        }

        public void Close()
        {
            try
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
                Socket.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int rxLen = 0;
            try
            {
                rxLen = Socket.Receive(buffer, offset, count, SocketFlags.None);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                if (ex is SocketException se && se.SocketErrorCode == SocketError.WouldBlock)
                    return -1;

                OnExceptionRaised(new CommunicationExceptionEventArgs(ex));
                //if (ex is SocketException or ObjectDisposedException && !Reconnect(2))
                //{
                //    throw;
                //}
                throw;
            }

            if (rxLen > 0)
            {
                OnDataReceived(new DataTransactionEventArgs(buffer.Skip(offset).Take(rxLen).ToArray()));
            }
            return rxLen;
        }

        public override int Write(byte[] data, int offset, int count)
        {
            int txLen = 0;

            try
            {
                txLen = Socket.Send(data, offset, count, SocketFlags.None);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                if (ex is SocketException se && se.SocketErrorCode == SocketError.WouldBlock)
                    return -1;

                OnExceptionRaised(new CommunicationExceptionEventArgs(ex));
                //if (ex is SocketException or ObjectDisposedException && !Reconnect(2))
                //{
                //    throw;
                //}
                throw;
            }

            if (txLen > 0)
            {
                OnDataSent(new DataTransactionEventArgs(data.Skip(offset).Take(txLen).ToArray()));
            }

            return txLen;
        }

        public override bool CheckConnection()
        {
            if (Socket.Connected)
            {
                try
                {
                    Socket.Send(Array.Empty<byte>());
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
                if (Socket != null)
                {
                    Socket.Dispose();
                }

                try
                {
                    Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    return Connect();
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
                return Socket.Available;
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
            Socket.ReceiveTimeout = rxTimeoutMs;
        }
    }
}