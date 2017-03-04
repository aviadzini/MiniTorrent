using System;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using MiniTorrentLibrary;
using Newtonsoft.Json;

namespace MiniTorrentClient
{
    public partial class DownlodedPage : Window
    {
        private string username { get; set; }
        private int port { get; set; }
        private string ip { get; set; }
        private Socket clientSocket;
        private static string response = string.Empty;

        public DownlodedPage(Socket socket, string username, int port)
        {
            InitializeComponent();
            this.username = username;
            this.port = port;
            this.clientSocket = socket;
            MainLablePage.Content = "Hello " + username;
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
                var pw = new PackageWrapper();

                pw.PackageType = typeof(FileSearch);
                pw.Package = new FileSearch
                {
                    FileName = FileTB.Text
                };
                var serialized = JsonConvert.SerializeObject(pw);

                clientSocket.Send(Encoding.ASCII.GetBytes(serialized + Constants.EOF));

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                byte[] receiveBuffer = new byte[1024];
                int received = clientSocket.Receive(receiveBuffer);
                response = Encoding.ASCII.GetString(receiveBuffer, 0, received);
                response = response.Substring(0, response.Length - 5);

                FilePackage fp = (FilePackage)javaScriptSerializer.Deserialize(response, typeof(FilePackage));

                if(!fp.Exist)
                {
                    MessageBox.Show("Missing", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        //private void Receive(Socket client)
        //{
        //    try
        //    {
        //        StateObject state = new StateObject();
        //        state.workSocket = client;

        //        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
        //            new AsyncCallback(ReceiveCallback), state);
        //    }

        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void ReceiveCallback(IAsyncResult ar)
        //{
        //    try
        //    {
        //        StateObject state = (StateObject)ar.AsyncState;
        //        Socket client = state.workSocket;

        //        int bytesRead = client.EndReceive(ar);
        //        string eof = "<EOF>";

        //        if (bytesRead > 0)
        //        {
        //            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
        //            string content = state.sb.ToString();

        //            if (content.IndexOf(eof) > -1)
        //            {
        //               // serverTB.Text += string.Format("\nClient #{0} at {1}", ++NumberOfConnections, handler.RemoteEndPoint.ToString());
        //                //serverTB.Text += string.Format("Read {0} bytes from socket. \n Data : {1}", content.Length, content);

        //                content = content.Substring(0, content.Length - 5);
        //                if (string.Compare(content, "Not Found!") == 0)
        //                {
        //                    MessageBox.Show("The file is missing!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //                }
        //                else
        //                {
        //                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
        //                    FilePackage loginPacage = (FilePackage)javaScriptSerializer.Deserialize(content, typeof(FilePackage));
        //                }
                        

                        
        //            }
        //        }
        //      }

        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

    }
}
