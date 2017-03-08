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
        private string username { get; set; }
        private int port { get; set; }

        private Socket clientSocket;

        private string response = string.Empty;

        public SearchPage(Socket clientSocket, string username, int port)
        {
            InitializeComponent();

            this.username = username;
            this.port = port;
            this.clientSocket = clientSocket;

            MainLablePage.Content = "Hello " + username;
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
                dataGrid.Items.Add(new Item() { Username = file.Username, FileSize = file.FileSize, Port = file.Port, IP = file.Ip });
            }
        }

<<<<<<< HEAD
            pw.PackageType = typeof(LogoutPackage);
            pw.Package = new LogoutPackage
            {
                Username = username
            };
            clientSocket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(pw) + ServerConstants.EOF));
        }

=======
>>>>>>> parent of c3c5bd8... client logout 2.1.6
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            clientSocket.Disconnect(true);
            clientSocket.Dispose();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /////////////////////////////////////////////////////////
            
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
