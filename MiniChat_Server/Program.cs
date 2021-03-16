using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetHelper;

namespace MiniChat_Server
{
    class Program
    {
        private static readonly List<Client> Connections = new List<Client>();

        private static object lck = new object();
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            Console.WriteLine("Hello server!");
            //Console.Write("Port: ");
            //int port = int.Parse(Console.ReadLine() ?? string.Empty);
            int port = 57650;

            Console.WriteLine($"Your ip is {UsefulThings.GetPublicIpAddress()}:{port}");
            Socket socket = TcpSocketHelper.CreateSocket();
            EndPoint localEndPoint = TcpSocketHelper.BindSocket(socket, IPAddress.Loopback, port);
            
            socket.Listen(10);

            while (true)
            {
                Client client = new Client();
                client.Socket = socket.Accept();
                Connections.Add(client);
                Task task = Receive(client);
            }
        }

        private static async Task Receive(Client client)
        {
            await Task.Run(() =>
            {
                client.Name = TcpSocketHelper.ReceiveString(client.Socket);


                string message = $"{client.Name} connected!";
                Console.WriteLine($"{DateTime.Now}: {message}");

                SendToEveryone(message, client);

                while (true)
                {
                    try
                    {
                        string str = TcpSocketHelper.ReceiveString(client.Socket);
                        Console.WriteLine($"{DateTime.Now}: {client.Name}: {str}");

                        message = $"{client.Name}: {str}";
   
                        SendToEveryone(message, client);

                    }
                    catch (Exception e)
                    {
                        message = $"{client.Name} disconnected!";
                        
                        Console.WriteLine($"{DateTime.Now}: {message}");

                        Connections.Remove(client);

                        SendToEveryone(message,client);

                        client.Socket.Shutdown(SocketShutdown.Both);
                        client.Socket.Close();
                        return;
                    }
                }
            });
        }

        private static void SendToEveryone(string message, Client sender)
        {
            lock (lck)
            {
                foreach (var connection1 in Connections)
                {
                    if (!Equals(connection1, sender))
                    {
                        TcpSocketHelper.SendString(connection1.Socket,message);
                    }
                }
            }
        }
    }

    class Client
    {
        public string Name;
        public Socket Socket;
    }
}
