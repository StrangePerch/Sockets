﻿using System;
using System.Net;
using System.Net.Sockets;

namespace Sockets_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");

            IPEndPoint endpoint = new IPEndPoint(ip, 90);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endpoint);
            socket.Listen(10);
            try
            {
                var connection = socket.Accept();
                Console.WriteLine(connection.RemoteEndPoint);
                Console.Read();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
            }
        }
    }
}