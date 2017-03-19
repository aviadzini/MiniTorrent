using System;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MiniTorrentLibrary;
using Newtonsoft.Json;

namespace MiniTorrentClient
{
    public partial class SearchPage : Window
    {
        private Socket clientSocket;
        private ClientsDetailsPackage cdp;
        private Listener listener;

        private string response = string.Empty;

        public SearchPage(Socket clientSocket, ClientsDetailsPackage cdp)
        {
            InitializeComponent();

            this.clientSocket = clientSocket;
            this.cdp = cdp;
            MainLablePage.Content = "Hello " + cdp.Username;

            listener = new Listener(cdp.IP, cdp.Port, cdp.UpPath);
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
                MessageBox.Show("Cannot be an empty field!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            foreach(var item in fp.FilesList)
            {
                if(cdp.Username.CompareTo(item.Username) != 0)
                    dataGrid.Items.Add(new Item() { Username = item.Username, FileName = item.FileName, FileSize = item.FileSize, Port = item.Port, IP = item.Ip });
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Item item = dataGrid.SelectedItem as Item;
            int port = item.Port;
            string ip = item.IP;

            Talker talker = new Talker(ip, port, cdp.DownPath, item.FileName);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            PackageWrapper pw = new PackageWrapper
            {
                PackageType = typeof(LogoutPackage),
                Package = new LogoutPackage { Username = cdp.Username }
            };

            clientSocket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(pw) + ServerConstants.EOF));

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }

    public class Item
    {
        public string Username { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public int Port { get; set; }
        public string IP { get; set; }
    }
}
