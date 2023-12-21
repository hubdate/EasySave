using System;
using System.Net.Sockets;
using System.Text;



namespace EasySaveClient
{
    class Program
    {
        static string serverIP = "127.0.0.1";
        static int serverPort = 8888;
        static void Main(string[] args)
        {
            try
            {
                TcpClient client = new TcpClient(serverIP, serverPort);
                NetworkStream stream = client.GetStream();

                byte[] buffer = new byte[client.ReceiveBufferSize];
                string message = "";

                while (!message.Equals("100"))
                {
                    int data = stream.Read(buffer, 0, client.ReceiveBufferSize);
                    message = Encoding.ASCII.GetString(buffer, 0, data);
                    Console.WriteLine("Progress : " + message + "%");
                }

                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }
    }
}
