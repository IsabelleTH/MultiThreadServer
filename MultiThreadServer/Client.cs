using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace MultiThreadServer
{
    class Client : IDisposable
    {
        public TcpClient TcpClient { get; set; }
        public string Name { get; set; }

        public Client(TcpClient client)
        {
            TcpClient = client;
        }

        public void SendMessage(string message)
        {
            var stream = TcpClient.GetStream();

            // Skicka meddelandets storlek (i byte array)
            var bytes = BitConverter.GetBytes(message.Length);
            stream.Write(bytes, 0, bytes.Length);

            // Gör meddelandet till bytearray och skicka
            var messageBytes = Encoding.UTF8.GetBytes(message);
            stream.Write(messageBytes, 0, messageBytes.Length);

        }

        public string WaitForMessage()
        {
            var stream = TcpClient.GetStream();

            // Lyssna på hur stort inkommande paket är
            var sizeBuffer = new byte[10];
            stream.Read(sizeBuffer, 0, sizeBuffer.Length);

            var packageSize = BitConverter.ToInt32(sizeBuffer, 0);

            // Skapa en array med den storleken
            var messageBuffer = new byte[packageSize];

            // Läs den storleken
            stream.Read(messageBuffer, 0, messageBuffer.Length);
            
            // Convertera bytearray till string
            var message = Encoding.UTF8.GetString(messageBuffer);
            return message;
        }

        public void Dispose()
        {
            TcpClient.Close();
        }
    }
}
