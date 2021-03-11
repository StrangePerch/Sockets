using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using NetHelper;

namespace MiniChat_Client
{
    class Program
    {
        private static IPEndPoint server;
        private static Socket socket;
        private static string name;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello client!");
            Console.WriteLine("Say me you name!");
            Console.Write("Name: ");
            name = Console.ReadLine();
            Console.WriteLine("Please enter Server ip and port!");
            Console.Write("IP: ");
            string ip = Console.ReadLine();
            Console.Write("Port: ");
            int port = int.Parse(Console.ReadLine() ?? string.Empty);
            server = new IPEndPoint(IPAddress.Parse(ip ?? string.Empty), port);

            socket = TcpSocketHelper.CreateSocket();

            try
            {
                socket = TcpSocketHelper.CreateSocket();
                socket.Connect(server);

                Task listeningTask = Listen();
                listeningTask.Start();

                TcpSocketHelper.SendString(socket, name);

                while (true)
                {
                    string message = Console.ReadLine();

                    TcpSocketHelper.SendString(socket, $"{message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Close();
            }
        }

        private static async Task Listen()
        {
            await Task.Run((() =>
            {
                try
                {
                    while (true)
                    {
                        var data = TcpSocketHelper.Receive(socket);

                        Console.WriteLine(data.GetString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Close();
                }
            }));
        }

        private static void Close()
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
            }
        }
    }
}
