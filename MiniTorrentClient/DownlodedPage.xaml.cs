using System;
using System.Linq;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;


namespace MiniTorrentClient
{
    /// <summary>
    /// Interaction logic for DownlodedPage.xaml
    /// </summary>
    public partial class DownlodedPage : Window
    {
        private string username { get; set; }
        private int port { get; set; }
        private string ip { get; set; }
        private Socket clientSocket;
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private static string response = string.Empty;
        public DownlodedPage(string username, int port)
        {
            InitializeComponent();
            this.username = username;
            this.port = port;
         
            MainLablePage.Content = "Hello " + username;
            StartConnection();
        }

        private void StartConnection()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
               
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
        private void SearchTBClicked(object sender, RoutedEventArgs e)
        {

            TextBox textBox = (TextBox)sender;

            if (textBox.Text == "Please Enter Here")
            {
                textBox.Clear();

            }
        }
        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            if (FileTB.Text == "")
            {
                MessageBoxResult result = MessageBox.Show("Cannot be an emty field!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string eof = "<EOF>";
                Send(clientSocket, FileTB.Text + eof);
                sendDone.WaitOne();

                Receive(clientSocket);
                receiveDone.WaitOne();
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
                string eof = "<EOF>";

                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    string content = state.sb.ToString();

                    if (content.IndexOf(eof) > -1)
                    {
                       // serverTB.Text += string.Format("\nClient #{0} at {1}", ++NumberOfConnections, handler.RemoteEndPoint.ToString());
                        //serverTB.Text += string.Format("Read {0} bytes from socket. \n Data : {1}", content.Length, content);

                        content = content.Substring(0, content.Length - 5);
                        if (string.Compare(content, "Not Found!") == 0)
                        {
                            MessageBox.Show("The file is missing!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                            FilePackage loginPacage = (FilePackage)javaScriptSerializer.Deserialize(content, typeof(FilePackage));
                        }
                        

                        
                    }
                }
              }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
