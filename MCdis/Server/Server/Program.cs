using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program
    {
        private static void ProcessingRequest(object client_obj)
        {
            var bytes = new Byte[256];
            string data = null;
            TcpClient client = client_obj as TcpClient;
            data = null;

            NetworkStream stream = client.GetStream();
            int i;

            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                stream.Write(msg, 0, msg.Length);
            }
            client.Close();
        }

        static void Main(string[] args)
        {
            TcpListener server = null;
            try
            {
                var port = 32000;
                var localAddr = IPAddress.Parse("127.0.0.1");
                var counter = 0;
                server = new TcpListener(localAddr, port);

                server.Start();

                while (true)
                {
                    Console.Write("\nWaiting for a connection... ");
                    ThreadPool.QueueUserWorkItem(ProcessingRequest, server.AcceptTcpClient());
                    counter++;
                    Console.Write("\nConnection №" + counter.ToString() + "!");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }
            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}