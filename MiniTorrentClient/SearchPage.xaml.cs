using System;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MiniTorrentLibrary;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace MiniTorrentClient
{
    public partial class SearchPage : Window
    {
        private Socket clientSocket;
        private Socket downloadSocket;
        private Socket serverSocket;

        private string upPath;
        private string downPath;

        private string file = "";
        private byte[] buffer = new byte[ServerConstants.BufferSize];
        private StringBuilder sb = new StringBuilder();

        private BinaryWriter bw = new BinaryWriter(new MemoryStream());

        private string response = string.Empty;

        public SearchPage(Socket clientSocket, ClientsDetailsPackage cdp)
        {
            InitializeComponent();

            upPath = cdp.UpPath;
            downPath = cdp.DownPath;

            this.clientSocket = clientSocket;
            
            downloadSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            serverSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                serverSocket.Bind(new IPEndPoint(IPAddress.Parse(cdp.IP), cdp.Port));
                serverSocket.Listen(1);
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }

            catch (Exception e)
            {
                MessageBoxResult result = MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            MainLablePage.Content = "Hello " + cdp.Username;
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            Socket handler = serverSocket.EndAccept(ar);

            handler.BeginReceive(buffer, 0, ServerConstants.BufferSize, 0,
                new AsyncCallback(ReadCallback), handler);

            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            Socket handler = (Socket)ar.AsyncState;
            int received = handler.EndReceive(ar);

            if (received > 0)
            {
                sb.Append(Encoding.ASCII.GetString(buffer, 0, received));
                string content = sb.ToString();

                buffer = new byte[ServerConstants.BufferSize];

                if (content.IndexOf(ServerConstants.EOF) > -1)
                {
                    content = content.Substring(0, content.Length - 5);
                    
                    var deserialized = JsonConvert.DeserializeObject<PackageWrapper>(content);

                    if (deserialized.PackageType == typeof(FileSearch))
                    {
                        FileSearch fs = (FileSearch)JsonConvert.DeserializeObject(Convert.ToString(deserialized.Package), deserialized.PackageType);
                        handler.BeginSendFile(upPath + fs.FileName, new AsyncCallback(SendFileCallback), handler);
                    }
                }

                else
                    handler.BeginReceive(buffer, 0, ServerConstants.BufferSize, 0, new AsyncCallback(ReadCallback), handler);

                buffer = new byte[ServerConstants.BufferSize];
                sb.Clear();
                handler.BeginReceive(buffer, 0, ServerConstants.BufferSize, 0, new AsyncCallback(ReadCallback), handler);
            }
        }

        private void SendFileCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.EndSendFile(ar);
            }

            catch (Exception e)
            {
                MessageBoxResult result = MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTBClicked(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text == "Please Enter Here")
                textBox.Clear();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            if (FileTB.Text == "")
            {
                MessageBoxResult result = MessageBox.Show("Cannot be an empty field!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else
            {
                file = FileTB.Text;
                var pw = new PackageWrapper();

                pw.PackageType = typeof(FileSearch);
                pw.Package = new FileSearch
                {
                    FileName = FileTB.Text
                };

                var serialized = JsonConvert.SerializeObject(pw);

                clientSocket.Send(Encoding.ASCII.GetBytes(serialized + ServerConstants.EOF));
                
                byte[] receiveBuffer = new byte[ServerConstants.BufferSize];
                int received = clientSocket.Receive(receiveBuffer);
                response = Encoding.ASCII.GetString(receiveBuffer, 0, received);
                response = response.Substring(0, response.Length - 5);

                FilePackage fp = (FilePackage)JsonConvert.DeserializeObject(response, typeof(FilePackage));

                if (!fp.Exist)
                {
                    label.Visibility = Visibility.Hidden;
                    dataGrid.Visibility = Visibility.Hidden;
                    MessageBox.Show("The File Is Missing", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                else
                {
                    label.Visibility = Visibility.Visible;
                    dataGrid.Visibility = Visibility.Visible;
                    AddExistFiles(fp);
                }
            }
        }

        private void AddExistFiles(FilePackage fp)
        {
            dataGrid.Items.Clear();

            for (int i = 0; i < fp.CountClients; i++)
            {
                FileDetails file = fp.FilesList[i];
                dataGrid.Items.Add(new Item() { Username = file.Username, FileSize = file.FileSize, Port = file.Port, IP = file.Ip });
            }
        }
        
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            clientSocket.Disconnect(true);
            clientSocket.Dispose();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Item item = dataGrid.SelectedItem as Item;
            int port = item.Port;
            string ip = item.IP;

            try
            {
                downloadSocket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), new AsyncCallback(ConnectCallback), null);
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
                downloadSocket.EndConnect(ar);

                PackageWrapper pw = new PackageWrapper();
                pw.PackageType = typeof(FileSearch);
                pw.Package = new FileSearch { FileName = file };

                byte[] sendAnswer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(pw) + ServerConstants.EOF);

                downloadSocket.BeginSend(sendAnswer, 0, sendAnswer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            }

            catch (Exception e)
            {
                MessageBoxResult result = MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                downloadSocket.EndSend(ar);

                downloadSocket.BeginReceive(buffer, 0, ServerConstants.BufferSize, 0, new AsyncCallback(ReadFileCallback), null);
            }

            catch (Exception e)
            {
                MessageBoxResult result = MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadFileCallback(IAsyncResult ar)
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                double total = 0.0;

                int bytes = downloadSocket.EndReceive(ar);
                while (bytes > 0)
                {
                    total += bytes;
                    bw.Write(buffer, 0, bytes);
                    if (bytes < buffer.Length)
                        break;
                    bytes = downloadSocket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                }

                bw.Flush();
                using (FileStream fs = System.IO.File.Create(downPath + file))
                {
                    ((MemoryStream)bw.BaseStream).WriteTo(fs);
                }

                watch.Stop();

                double elapsedS = watch.ElapsedMilliseconds / 1000.0;
                total = total / 1000.0;

                string massage = "Total time: " + elapsedS + " seconds.\r\n";
                massage += "File size: " + total + " KB.\r\n";
                massage += "Bit rate: " + total / elapsedS + " Kbps."; 

                MessageBoxResult result = MessageBox.Show(massage, "Download information", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            catch (Exception e)
            {
                MessageBoxResult result = MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            downloadSocket.Disconnect(true);
            downloadSocket.Dispose();
        }
    }

    public class Item
    {
        public string Username { get; set; }
        public int FileSize { get; set; }
        public int Port { get; set; }
        public string IP { get; set; }
    }
}
