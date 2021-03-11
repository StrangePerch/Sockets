using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetHelper;

namespace MiniChat_Server
{
    class Program
    {
        private static readonly List<Socket> Connections = new List<Socket>();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello server!");
            Console.WriteLine($"Your ip is {UsefulThings.GetPublicIpAddress()}");
            //Console.Write("Port: ");
            //int port = int.Parse(Console.ReadLine() ?? string.Empty);
            int port = 57650;
            Socket socket = TcpSocketHelper.CreateSocket();
            EndPoint localEndPoint = TcpSocketHelper.BindSocket(socket, IPAddress.Loopback, port);
            
            socket.Listen(10);
            
            while (true)
            {
                Socket connection = socket.Accept();
                Connections.Add(connection);
                Task task = Receive(connection);
            }
            
        }

        private static async Task Receive(Socket connection)
        {
            await Task.Run(() =>
            {
                Data nameData = TcpSocketHelper.Receive(connection);
                
                string name = nameData.GetString();


                string message = $"{name} connected!";
                Console.WriteLine(message);

                foreach (var connection1 in Connections)
                {
                    if (!Equals(connection1, connection))
                        connection1.Send(UsefulThings.ToBytes(message));
                }

                while (true)
                {
                    try
                    {
                        Data data = TcpSocketHelper.Receive(connection);
                        Console.WriteLine(data.GetString());
                        foreach (var connection1 in Connections)
                        {
                            if (!Equals(connection1, connection))
                            {
                                connection1.Send(UsefulThings.ToBytes($"{name}: {data.GetString()}"));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        message = $"{name} disconnected!";
                        
                        Console.WriteLine(message);

                        foreach (var connection1 in Connections)
                        {
                            if (!Equals(connection1, connection))
                                connection1.Send(UsefulThings.ToBytes(message));
                        }
                        connection.Shutdown(SocketShutdown.Both);
                        connection.Close();
                        return;
                    }
                }
            });
        }
    }
}
