using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace MiniTorrentServer
{
    public partial class Form1 : Form
    {
        private Socket serverSocket;
        private Socket clientSocket;

        private byte[] buffer;

        public Form1()
        {
            InitializeComponent();
            startServer();
        }

        private void startServer()
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8005));
                serverSocket.Listen(10);
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

                serverTB.Text += "The server has started.\r\n";
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AcceptCallback(IAsyncResult AR)
        {
            try
            {
                clientSocket = serverSocket.EndAccept(AR);
                buffer = new byte[clientSocket.ReceiveBufferSize];
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

                serverTB.Text += "Client has connected to the server.\r\n";
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                int received = clientSocket.EndReceive(AR);

                if (received == 0)
                {
                    serverTB.Text += "Client has disconnected from the server.\r\n";
                    return;
                }

                Array.Resize(ref buffer, received);
                string text = Encoding.ASCII.GetString(buffer);
                serverTB.Text += "Client: " + text + ".\r\n";
                Array.Resize(ref buffer, clientSocket.ReceiveBufferSize);
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
