using System;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using MiniTorrentLibrary;
using Newtonsoft.Json;
using System.Data;




namespace MiniTorrentClient
{
    public partial class SearchPage : Window
    {
        private string username { get; set; }
        private int port { get; set; }
        private string ip { get; set; }
        private Socket clientSocket;
        private static string response = string.Empty;

        public SearchPage(Socket socket, string username, int port)
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

                if (!fp.Exist)
                {
                    MessageBox.Show("The File Is Missing", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    Label l = new Label();
                    l.Content="Please choose your source to download";
                    this.AddChild(l);
                    AddExistFiles(fp);

                }
            }
        }

        private void AddExistFiles(FilePackage fp)
        {
            File file;
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            DataTable table = new DataTable("ClientsTable");
                   
            table.Columns.Add("Username", typeof(string));
            table.Columns.Add("Size File", typeof(int));
            table.Columns.Add("Port", typeof(int));
            table.Columns.Add("IP", typeof(String));
            table.Columns.Add("Download", typeof(Button));
            Button download;
            for (int i = 0; i < fp.CountClients; i++)
            {
                file = (File)javaScriptSerializer.Deserialize((fp.FilesList[i]).ToString(), typeof(File));
                download = new Button();
                download.Content = "Download";
                download.Click+=(DownloadClick);
                table.Rows.Add(file.Username, file.FileSize, file.Port, file.Ip,download);
            }
            this.AddChild(table);
        }
        private void DownloadClick(object sender, RoutedEventArgs e)
        {
        }

  }
}
