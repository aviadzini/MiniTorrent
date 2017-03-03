using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace MiniTorrentClient
{
    public partial class MainWindow : Window
    {
        private Socket clientSocket;
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private static string response = string.Empty; 
        
        public MainWindow()
        {
            InitializeComponent();
           
            StartClient();
        }

        private void StartClient()
        {
            int RemotePort = 8005;

            try
            {
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, RemotePort);
                
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), clientSocket);   
                connectDone.WaitOne();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState; 
                client.EndConnect(ar);  
                connectDone.Set();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject();
                state.workSocket = client;
                
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }

                else
                {
                    if (state.sb.Length > 1)
                        response = state.sb.ToString();

                    receiveDone.Set();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void Send(Socket client, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);

                sendDone.Set();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       
        private void ClickUserNameTB(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Please Enter Your Username")
            {
                textBox.Clear();
            }
        }

        private void ClickPassowrdTB(object sender, RoutedEventArgs e)
        {
            
            TextBox textBox = (TextBox)sender;
           
            if (textBox.Text == "Please Enter Your Password")
            {
                textBox.Clear();

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


                    Send(clientSocket, json+"<EOF>");      
                    sendDone.WaitOne();

                    Receive(clientSocket);  
                    receiveDone.WaitOne();

                    if (int.Parse(response) > 0)
                    {
                        // MessageBoxResult result = MessageBox.Show(response, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        DownlodedPage dp = new DownlodedPage(usernameTB.Text, int.Parse(response));
                        dp.Show();
                        Close();
                    }

                    else
                    {
                        usernameTB.Clear();
                        passwordTB.Clear();

                        MessageBoxResult result = MessageBox.Show("Username or Password incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                catch (Exception ex)
                {
                    MessageBoxResult result = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}