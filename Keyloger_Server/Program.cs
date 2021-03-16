using System;
using System.Net.Sockets;

namespace Keyloger_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socket = NetHelper.TcpSocketHelper.CreateSocket();
            NetHelper.TcpSocketHelper.BindSocket(socket, "127.0.0.1", 57650);
            socket.Listen(1);
            Socket client = socket.Accept();
            while (true)
            {
                var data = NetHelper.TcpSocketHelper.Receive(client);
                Console.Write(data.GetString());
            }
        }
    }
}
