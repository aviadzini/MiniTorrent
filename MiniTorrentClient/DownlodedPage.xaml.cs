using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MiniTorrentClient
{
    /// <summary>
    /// Interaction logic for DownlodedPage.xaml
    /// </summary>
    public partial class DownlodedPage : Window
    {
        private string username { get; set; }
        private int port { get; set; }
        public DownlodedPage(string username, int port)
        {
            InitializeComponent();
            this.username = username;
            this.port = port;
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

            }
        }
    }
}
