using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.BeginConnect(new IPEndPoint(IPAddress.Loopback, 8005), new AsyncCallback(ConnectCallback), null);
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
                try
                {
                    byte[] buffer = Encoding.ASCII.GetBytes(usernameTB.Text);
                    clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
                }

                catch (Exception ex)
                {
                    MessageBoxResult result = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                clientSocket.EndConnect(AR);
            }

            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
