using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MultiThreadServer
{
    class Program
    {
        public static List<Client> ConnectedClients = new List<Client>();

        private static void ProcessClientrequests(Client client)
        {
            try
            {
                client.SendMessage("You have been connected to the server");
                client.SendMessage("Enter your name:");
                client.Name = client.WaitForMessage();

                while (true)
                {
                    var message = client.WaitForMessage();
                    foreach (var c in ConnectedClients)
                    {
                        c.SendMessage(client.Name + ": " + message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem with client communication. Exiting thread " + e);
                client.Dispose();
            }
        }

        static void Main(string[] args)
        {
            TcpListener listener = null;

            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8001);
                listener.Start();
                Console.WriteLine("Multi threader started...");

                while(true)
                {
                    Console.WriteLine("Waiting for incoming client connections...");
                    var tcpClient = listener.AcceptTcpClient();

                    var client = new Client(tcpClient);
                    ConnectedClients.Add(client);

                    //startar en ny tråd (Task.Run)
                    Task.Run(() => ProcessClientrequests(client));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
