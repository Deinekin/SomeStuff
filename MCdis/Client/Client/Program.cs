using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Client
{
    class Program
    {
        private static void CopyFiles(string SourcePath, string DestinationPath)
        {
            Directory.CreateDirectory(DestinationPath);

            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath));
        }

        private static void Connect(String server, String message)
        {
            try
            {
                var port = 32000;
                var client = new TcpClient(server, port);

                var data = System.Text.Encoding.ASCII.GetBytes(message);
                var stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                stream.Flush();

                data = new Byte[256];

                var responseData = string.Empty;

                var bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter an IP address (you need 127.0.0.1 for success, but you can try anything else): \n");
            var address = Console.ReadLine();
            
            var match = Regex.Match(address, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");

            if (match.Success && address == "127.0.0.1")
            {
                Console.WriteLine("\n We have just received a new message!\n");
                Connect("127.0.0.1", "Hey, i have to update my folder structure\n");
                string pathServer = @"c:\\newDir";
                string pathClient = @"c:\\newDir1";

                if (Directory.Exists(pathClient))
                    Directory.Delete(pathClient, true);

                CopyFiles(pathServer, pathClient);
                Console.WriteLine("We have just updated client's folder structure");
            }
            else
            {
               if (match.Success && address != "127.0.0.1")
                   Console.WriteLine("I dont know this ip address");
               else
                   Console.WriteLine("Something strange... might be Elven");
            }
        }
    }
}