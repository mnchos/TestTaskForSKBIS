using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Net.Sockets;

namespace TestFSKBIs
{
    public partial class MainWindow : Window
    {
        private const int ServerPort = 12345; // Порт сервера
        private const string ServerIPAddress = "127.0.0.1"; // IP-адрес сервера

        private TcpClient client;
        private NetworkStream stream;

        public MainWindow()
        {
            InitializeComponent();
            client = new TcpClient();
            client.Connect(ServerIPAddress, ServerPort);
            stream = client.GetStream();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = PrepareData();
            stream.Write(data, 0, data.Length);

            byte[] responseBytes = new byte[4];
            stream.Read(responseBytes, 0, responseBytes.Length);
            int responseData = BitConverter.ToInt32(responseBytes, 0);

            if (responseData == 0)
            {
                ResponseTextBox.Text = "В пакете не было данных.";
            }
            else
            {
                ResponseTextBox.Text = $"Ответ сервера: 0x{responseData:X}";
            }
        }

        private byte[] PrepareData()
        {
            byte[] headerBytes = new byte[int.Parse(HeaderBytesTextBox.Text)];
            byte[] footerBytes = new byte[int.Parse(FooterBytesTextBox.Text)];
            byte[] dataBytes = Encoding.UTF8.GetBytes(DataTextBox.Text);        
            Random random = new Random();
            random.NextBytes(headerBytes);
            random.NextBytes(footerBytes);

            byte[] packet = new byte[headerBytes.Length + 1 + dataBytes.Length + 1 + footerBytes.Length];
            headerBytes.CopyTo(packet, 0);
            packet[headerBytes.Length] = 0x0A;
            dataBytes.CopyTo(packet, headerBytes.Length + 1);
            packet[headerBytes.Length + 1 + dataBytes.Length] = 0x0B;
            footerBytes.CopyTo(packet, headerBytes.Length + 1 + dataBytes.Length + 1);

            return packet;
        }
    }
}
