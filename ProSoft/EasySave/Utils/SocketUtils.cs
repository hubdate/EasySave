using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using EasySave.Models.Data;
using EasySave.Utils;

using System.Threading;


namespace EasySave.Utils
{
    public static class SocketUtils
    {
        public static void StartServer(Save save)
        {
            TcpListener server = null;
            try
            {
                int port = 8888;
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                Byte[] bytes = new Byte[256];
                double progress = 0;
                TcpClient client = null;
                while (true) // boucle infinie pour le serveur
                {
                    try
                    {
                        if (client == null || !client.Connected)
                        {
                            client = server.AcceptTcpClient();
                            Console.WriteLine("Connected!");
                        }
                        NetworkStream stream = client.GetStream();
                        progress = save.CalculateProgress();
                        string progressString = progress.ToString();
                        byte[] progressBytes = Encoding.ASCII.GetBytes(progressString);
                        stream.Write(progressBytes, 0, progressBytes.Length);
                        if (progress >= 100) // condition pour arrêter le serveur
                        {
                            break;
                        }
                        Thread.Sleep(100);
                    }
                    catch (Exception e) // gestion des exceptions de déconnexion du client
                    {
                        Console.WriteLine("Client disconnected: " + e.Message);
                        client = null; // réinitialiser le client
                    }
                }
                client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}