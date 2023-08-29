using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using System.Net;
using System.Net.Sockets;

namespace ServerFTestfSBKis
{
    class Server
    {
        private const int ServerPort = 12345;

        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Any, ServerPort);
            server.Start();

            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                HandleClient(client);
            }
        }

        static void HandleClient(TcpClient client)
        {
            using (NetworkStream stream = client.GetStream())
            {
                byte[] data = new byte[client.ReceiveBufferSize];
                int bytesRead = stream.Read(data, 0, data.Length);             
                int startIndex = Array.IndexOf(data, (byte)0x0A);
                int endIndex = Array.IndexOf(data, (byte)0x0B, startIndex);

                if (startIndex != -1 && endIndex != -1)
                {
                    byte[] dataBytes = new byte[endIndex - startIndex - 1];
                    Array.Copy(data, startIndex + 1, dataBytes, 0, endIndex - startIndex - 1);

                    Console.WriteLine("Получен пакет данных:");
                    Console.WriteLine($"Мусорные байты в начале: {BitConverter.ToString(data, 0, startIndex)}");
                    Console.WriteLine($"Данные байты: {BitConverter.ToString(dataBytes)}");
                    // Console.WriteLine($"Мусорные байты в конце: {BitConverter.ToString(data, endIndex + 1)}");

                    int sum = 0;
                    foreach (byte b in dataBytes)
                    {
                        sum += b;
                    }

                    byte[] response = BitConverter.GetBytes(sum);
                    stream.Write(response, 0, response.Length);
                }
            }

            client.Close();
        }
    }
}

