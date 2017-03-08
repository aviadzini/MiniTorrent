using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MiniTorrentLibrary;
using Newtonsoft.Json;

namespace MiniTorrentClient
{
    public partial class MainWindow : Window
    {
        private Socket clientSocket;

        private string response = string.Empty; 
        
        public MainWindow()
        {
            InitializeComponent();

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private void StartClient()
        {
            try
            {
                clientSocket.Connect(new IPEndPoint(
                    IPAddress.Parse(ServerConstants.ServerIP),
                    ServerConstants.ServerPort));
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string getJsonData()
        {
            string ipA = "";

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    ipA = ip.ToString();

            var pw = new PackageWrapper();

            pw.PackageType = typeof(LoginPackage);
            pw.Package = new LoginPackage
            {
                Username = usernameTB.Text,
                Password = passwordTB.Text,
                IP = ipA
            };

            return JsonConvert.SerializeObject(pw) + ServerConstants.EOF;
        }

        private void loginB_Click(object sender, RoutedEventArgs e)
        {
            if (string.Compare(usernameTB.Text, "") == 0 || string.Compare(passwordTB.Text, "") == 0)
            {
                usernameTB.Clear();
                passwordTB.Clear();

                MessageBoxResult result = MessageBox.Show("Username and password cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else
            {
                try
                {
                    if(!clientSocket.Connected)
                        StartClient();

                    clientSocket.Send(Encoding.ASCII.GetBytes(getJsonData()));

                    byte[] receiveBuffer = new byte[1024];
                    int received = clientSocket.Receive(receiveBuffer);
                    response = Encoding.ASCII.GetString(receiveBuffer, 0, received);

                    if (int.Parse(response) > 0)
                    {
                       SearchPage dp = new SearchPage(clientSocket, usernameTB.Text, int.Parse(response));
                        dp.Show();
                        Close();
                    }

                    else
                    {
                        usernameTB.Clear();
                        passwordTB.Clear();

                        response = "";
                        MessageBoxResult result = MessageBox.Show("Username or Password incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                catch (Exception ex)
                {
                    MessageBoxResult result = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
              
        private void ClickUserNameTB(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Please Enter Your Username")
                textBox.Clear();
        }

        private void ClickPassowrdTB(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Please Enter Your Password")
                textBox.Clear();
        }
    }
}