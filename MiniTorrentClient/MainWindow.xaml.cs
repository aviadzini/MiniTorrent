using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MiniTorrentLibrary;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MiniTorrentClient
{
    public partial class MainWindow : Window
    {
        private Socket clientSocket;

        private string Ip = "";
        private int port = 0;

        byte[] buffer = new byte[ServerConstants.BufferSize];

        private string response = string.Empty; 
        
        public MainWindow()
        {
            InitializeComponent();

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    Ip = ip.ToString();

            Random random = new Random();
            port = random.Next(8006, 9000);
        }

        private void StartClient()
        {
            try
            {
                clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse(ServerConstants.ServerIP), ServerConstants.ServerPort), new AsyncCallback(ConnectCallback), null);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            clientSocket.EndConnect(ar);
        }

        private string getJsonData()
        {
           /* List<MiniTorrentLibrary.File> list = new List<MiniTorrentLibrary.File>();

            string[] allfiles = Directory.GetFiles(@"C:\Up");
            foreach (var file in allfiles)
            {
                FileInfo info = new FileInfo(file);
                list.Add(new MiniTorrentLibrary.File (info.Name, (int)info.Length));
            }*/

            var pw = new PackageWrapper();

            pw.PackageType = typeof(LoginPackage);
            pw.Package = new LoginPackage
            {
                Username = usernameTB.Text,
                Password = passwordTB.Text,
                IP = Ip,
                Port = port/*,
                NumOfFiles = list.Count,
                FileList = new List<MiniTorrentLibrary.File>(list)*/
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

                    byte[] bufferJ = Encoding.ASCII.GetBytes(getJsonData());
                    clientSocket.BeginSend(bufferJ, 0, bufferJ.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
                }

                catch (Exception ex)
                {
                    MessageBoxResult result = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSend(ar);

                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReciveCallback), null);
            }

            catch (Exception e)
            {
                MessageBoxResult result = MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReciveCallback(IAsyncResult ar)
        {
            try
            {
                int received = clientSocket.EndReceive(ar);

                if (received > 0)
                {
                    response = Encoding.ASCII.GetString(buffer, 0, received);
                    response = response.Substring(0, response.Length - 5);

                    ClientsDetailsPackage cdp = (ClientsDetailsPackage)JsonConvert.DeserializeObject(response, typeof(ClientsDetailsPackage));

                    if (cdp.Exist)
                    {

                        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        (Action)(() =>
                        {
                            SearchPage dp = new SearchPage(clientSocket, cdp);
                            dp.Show();
                            Close();
                        }
                        ));
                    }

                    else
                    {
                        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            (Action)(() =>
                            {
                                usernameTB.Clear();
                                passwordTB.Clear();
                            }
                            ));
                        
                        response = "";
                        MessageBoxResult result = MessageBox.Show("Username or Password incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            catch (Exception e)
            {
                MessageBoxResult result = MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void signUpB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://localhost:8080/Register.aspx");
            }

            catch { }
        }
    }
}