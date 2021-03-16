using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetHelper
{
    public class UdpSocketHelper
    {
        public static Socket CreateSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public static IPEndPoint BindSocket(Socket socket, IPAddress ip, int port)
        {
            IPEndPoint endPoint = new IPEndPoint(ip, port);
            socket.Bind(endPoint);
            return endPoint;
        }

        public static IPEndPoint BindSocket(Socket socket)
        {
            return BindSocket(socket, IPAddress.Loopback, 0);
        }

        public static IPEndPoint BindSocket(Socket socket, string ip, int port)
        {
            return BindSocket(socket, IPAddress.Parse(ip), port);
        }
        
        public static Data ReceiveFrom(Socket socket)
        {
            var buffer = new byte[256];
            EndPoint endpoint = new IPEndPoint(0, 0);
            int bytes = socket.ReceiveFrom(buffer, ref endpoint);
            return new Data(endpoint as IPEndPoint, buffer, bytes);
        }

        public static void SendStringTo(Socket socket, IPEndPoint to, string message)
        {
            socket.SendTo(Encoding.Unicode.GetBytes(message), to);
        }
    }

    public class TcpSocketHelper
    {
        public static Socket CreateSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public static IPEndPoint BindSocket(Socket socket, IPAddress ip, int port)
        {
            IPEndPoint endPoint = new IPEndPoint(ip, port);
            socket.Bind(endPoint);
            return endPoint;
        }

        public static IPEndPoint BindSocket(Socket socket)
        {
            return BindSocket(socket, IPAddress.Loopback, 0);
        }

        public static IPEndPoint BindSocket(Socket socket, string ip, int port)
        {
            return BindSocket(socket, IPAddress.Parse(ip), port);
        }

        public static Data Receive(Socket socket)
        {
            Data data = new Data();
            data.Bytes = socket.Receive(data.Buffer);
            data.EndPoint = null;
            return data;
        }

        public static void SendString(Socket socket, string message)
        {
            var bytes = Encoding.Unicode.GetBytes(message);
            socket.Send(BitConverter.GetBytes(bytes.Length));
            socket.Send(Encoding.Unicode.GetBytes(message));
        }

        public static string ReceiveString(Socket socket)
        {
            byte[] buffer = new byte[4];
            socket.Receive(buffer);
            int length = BitConverter.ToInt32(buffer);
            buffer = new byte[length];
            socket.Receive(buffer);
            return UsefulThings.ToString(buffer, length);
        }
    }

    public class UsefulThings
    {
        public static string GetPublicIpAddress()
        {
            String address = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                address = stream.ReadToEnd();
            }


            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);

            return address;
        }
        
        public static byte[] ToBytes(string str)
        {
            return Encoding.Unicode.GetBytes(str);
        }

        public static string ToString(byte[] bytes, int length)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Encoding.Unicode.GetString(bytes, 0, length));
            return builder.ToString();
        }
    }

    public class Data
    {
        public IPEndPoint EndPoint;
        public byte[] Buffer = new byte[256];
        public int Bytes;

        public Data(IPEndPoint endPoint, byte[] data, int bytes)
        {
            this.EndPoint = endPoint;
            this.Buffer = data;
            this.Bytes = bytes;
        }
        
        public Data() 
        { }

        public string GetString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Encoding.Unicode.GetString(Buffer, 0, Bytes));
            return builder.ToString();
        }
    }

    public class Message
    {
        public string name;
        public string message;
    }
}

