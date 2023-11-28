using System.Diagnostics;
using System.Net.Sockets;

namespace MCProtocol
{
    public class ListenPacket
    {
        public static int Count { get; private set; }
        readonly Socket Socket;
        readonly byte[] Bytes = new byte[4096];
        readonly Func<Socket, byte[], byte[]> DoRecieveAndResponse;
        readonly IUIUpdatable Updatable;

        /// <summary>
        /// コンストラクタ処理
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="recieveAndResponse"></param>
        public ListenPacket(Socket socket, Func<Socket, byte[], byte[]> recieveAndResponse, IUIUpdatable updatable)
        {
            Count++;
            Socket = socket;
            DoRecieveAndResponse = recieveAndResponse;
            Updatable = updatable;
        }

        /// <summary>
        /// 開始
        /// </summary>
        public void Start() => BeginRecieve(this);

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            try
            {
                Socket.Disconnect(false);
                Socket.Dispose();
            }
            catch
            {
            }
            finally
            {
                Count--;
                Updatable.UpdateConnect(Count);
            }
        }

        /// <summary>
        /// 受信開始
        /// </summary>
        /// <param name="packet"></param>
        static void BeginRecieve(ListenPacket packet)
        {
            packet.Socket.BeginReceive(packet.Bytes, 0, packet.Bytes.Length, SocketFlags.None, new AsyncCallback(OnRecieve), packet);
        }

        /// <summary>
        /// 受信処理
        /// </summary>
        /// <param name="ar"></param>
        static void OnRecieve(IAsyncResult ar)
        {
            if (ar.AsyncState is ListenPacket self)
            {
                try
                {
                    var socket = self.Socket;
                    var request = self.Bytes;
                    var len = socket.EndReceive(ar);
                    if (0 < len)
                    {
                        //受信有り
                        var response = self.DoRecieveAndResponse(socket, request);
                        socket.Send(response, response.Length, SocketFlags.None);
                        BeginRecieve(self);
                    }
                    else
                    {
                        //切断検知
                        self.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    self.Stop();
                }
            }
        }
    }
}