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
        byte[] buffer = new byte[ServerConstants.BufferSize];
        private StringBuilder sb = new StringBuilder();

        private int index = 1;
      
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

                if (content.IndexOf(ServerConstants.EOF) > -1)
                {
                    content = content.Substring(0, content.Length - 5);
                    
                    var deserialized = JsonConvert.DeserializeObject<PackageWrapper>(content);

                    if (deserialized.PackageType == typeof(FileSearch))
                    {
                        FileSearch fs = (FileSearch)JsonConvert.DeserializeObject(Convert.ToString(deserialized.Package), deserialized.PackageType);
                        sendFile(fs.FileName, handler);
                    }
                }

                else
                    handler.BeginReceive(buffer, 0, ServerConstants.BufferSize, 0, new AsyncCallback(ReadCallback), handler);

                sb.Clear();
                handler.BeginReceive(buffer, 0, ServerConstants.BufferSize, 0, new AsyncCallback(ReadCallback), handler);
            }
        }

        private void SendFileCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
            }

            catch (Exception e)
            {
                MessageBoxResult result = MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void sendFile(string fileName, Socket socket)
        {
            Socket handler = socket;
            FileStream fin = null;

            try
            {
                FileInfo ftemp = new FileInfo(upPath + fileName);
                long total = ftemp.Length;
                long ToatlSent = 0;

                fin = new FileStream(upPath + fileName, FileMode.Open, FileAccess.Read);

                while (ToatlSent < total)
                {
                    index = fin.Read(buffer, 0, buffer.Length);
                    handler.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(SendFileCallback), handler);
                    ToatlSent = ToatlSent + index;
                }
            }

            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show("A Exception occured in transfer" + ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            finally
            {
                if (fin != null)
                    fin.Close();
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
                
                byte[] receiveBuffer = new byte[1024];
                int received = clientSocket.Receive(receiveBuffer);
                response = Encoding.ASCII.GetString(receiveBuffer, 0, received);
                response = response.Substring(0, response.Length - 5);

                FilePackage fp = (FilePackage)JsonConvert.DeserializeObject(response, typeof(FilePackage));

                if (!fp.Exist)
                {
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
            for (int i = 0; i < fp.CountClients; i++)
            {
                FileDetails file = fp.FilesList[i];
                dataGrid.Items.Add(new Item() { NO = (i+1), Username = file.Username, FileSize = file.FileSize, Port = file.Port, IP = file.Ip });
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

                GetFile(file);
            }

            catch (Exception e)
            {
                MessageBoxResult result = MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetFile(string FileName)
        {
            FileStream fout = new FileStream(downPath + FileName, FileMode.Create, FileAccess.Write);

            long rby = 0;
            try
            {
                downloadSocket.BeginReceive(buffer, 0, ServerConstants.BufferSize, 0, new AsyncCallback(ReadFileCallback), null);
                while (index > 0)
                {
                    fout.Write(buffer, 0, index);
                    rby = rby + index;
                    downloadSocket.BeginReceive(buffer, 0, ServerConstants.BufferSize, 0, new AsyncCallback(ReadFileCallback), null);
                }
            }

            catch (Exception ed)
            {
                Console.WriteLine("A Exception occured in file transfer" + ed.Message);
            }

            finally
            {
                if (fout != null)
                    fout.Close();
            }
        }

        private void ReadFileCallback(IAsyncResult ar)
        {
            try
            {
                index = downloadSocket.EndReceive(ar);
            }

            catch (Exception e)
            {
                MessageBoxResult result = MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class Item
    {
        public int NO { get; set; }
        public string Username { get; set; }
        public int FileSize { get; set; }
        public int Port { get; set; }
        public string IP { get; set; }
    }
}
