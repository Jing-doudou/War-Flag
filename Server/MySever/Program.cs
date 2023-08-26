using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace MySever
{
    public enum ChessTurn
    {
        None, White, Black
    }
    class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];
        public Player player;

    }
    class Program
    {
        static Socket socket;
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
        static void Main(string[] args)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipep = new IPEndPoint(ipAdr, 8888);
            socket.Bind(ipep);
            socket.Listen(2);
            Console.WriteLine("服务器启动");
            List<Socket> checkRead = new List<Socket>();
            while (true)
            {
                checkRead.Clear();
                checkRead.Add(socket);
                foreach (ClientState item in clients.Values)
                {
                    checkRead.Add(item.socket);
                }
                Socket.Select(checkRead, null, null, 1000);//筛选
                foreach (Socket item in checkRead)
                {
                    if (item == socket)
                    {
                        ReadListenfd(item);
                    }
                    else
                    {
                        ReadClient(item);
                    }
                }
            }
        }

        private static bool ReadClient(Socket item)
        {
            ClientState state = clients[item];
            int count = 0;
            try
            {
                count = item.Receive(state.readBuff);
            }
            catch (SocketException ex)
            {
                MethodInfo mei = typeof(EventHandle).GetMethod("OnDisconnect");
                object[] obj = { state };
                mei.Invoke(null, obj);

                socket.Close();
                clients.Remove(item);
                Console.WriteLine("Socket");
                return false;
            }
            if (count == 0)
            {
                MethodInfo mei = typeof(EventHandle).GetMethod("OnDisconnect");
                object[] obj = { state };
                mei.Invoke(null, obj);
                item.Close();
                clients.Remove(item);
                Console.WriteLine("Socket close");
                return false;
            }
            string receStr = Encoding.UTF8.GetString(state.readBuff, 0, count);
            Console.WriteLine(receStr);
            string[] split = receStr.Split('|');
            string msgName = split[0];
            string msgArgs = split[1];
            string funName = "Msg" + msgName;
            MethodInfo mi = typeof(MsgHandle).GetMethod(funName);
            object[] o = { state, msgArgs };
            mi.Invoke(null, o);
            return true;
        }

        private static void ReadListenfd(Socket item)
        {
            Console.WriteLine("连接成功");
            Socket client = socket.Accept();
            ClientState state = new ClientState();
            state.socket = client;
            state.player = new Player(state.socket);
            clients.Add(client, state);
        }
        public static void Send(ClientState cs, string sendStr)
        {
            byte[] sendByte = Encoding.Default.GetBytes(sendStr);
            cs.socket.Send(sendByte);
        }
        public static ClientState GetOpp(ClientState c)
        {
            foreach (ClientState item in clients.Values)
            {
                if (item != c)
                {
                    return item;
                }
            }
            return null;
        }

    }
}
