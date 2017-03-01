using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;

namespace MiniTorrentClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket clientSocket;

        public MainWindow()
        {
            InitializeComponent();
            connectToServer();
        }

        private void connectToServer()
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8005), new AsyncCallback(ConnectCallback), null);
            }

            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void loginB_Click(object sender, RoutedEventArgs e)
        {
            if (string.Compare(usernameTB.Text, "") == 0 || string.Compare(passwordTB.Text, "") == 0)
            {
                usernameTB.Clear();
                passwordTB.Clear();

                MessageBoxResult result = MessageBox.Show("Cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else
            {
                
                    if (!clientSocket.Connected)
                        connectToServer();

                    if (clientSocket.Connected)
                    {
                        try
                        {
                            string ipA = "";

                            var host = Dns.GetHostEntry(Dns.GetHostName());
                            foreach (var ip in host.AddressList)
                                if (ip.AddressFamily == AddressFamily.InterNetwork)
                                    ipA = ip.ToString();

                            LoginPackage loginPackage = new LoginPackage
                            {
                                Username = usernameTB.Text,
                                Password = passwordTB.Text,
                                IP = ipA
                            };
                            var json = new JavaScriptSerializer().Serialize(loginPackage);

                            byte[] buffer = Encoding.ASCII.GetBytes(json);
                            clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
                        }

                        catch (Exception ex)
                        {
                            MessageBoxResult result = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
              
            }

        }

        private void SendCallback(IAsyncResult AR)
        {
            clientSocket.EndSend(AR);
        }

        private void ConnectCallback(IAsyncResult AR)
        {
            try
            {
                if (clientSocket.Connected)
                {
                    clientSocket.EndConnect(AR);
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Try later\\nthe server fail.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (clientSocket != null && clientSocket.Connected)
                clientSocket.Close();
        }
       
    }
}
