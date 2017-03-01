using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Forms;


namespace MiniTorrentServer
{
    public partial class Form1 : Form
    {
        private Socket serverSocket;
        private Socket clientSocket;

        private byte[] buffer;

        public Form1()
        {
            InitializeComponent();
            startServer();
        }

        private void startServer()
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8005));
                serverSocket.Listen(10);
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

                serverTB.Text += "The server has started.\r\n";
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AcceptCallback(IAsyncResult AR)
        {
            try
            {
                clientSocket = serverSocket.EndAccept(AR);
                buffer = new byte[clientSocket.ReceiveBufferSize];
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

                serverTB.Text += "Client has connected to the server.\r\n";
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                int received = clientSocket.EndReceive(AR);

                if (received == 0)
                {
                    serverTB.Text += "Client has disconnected from the server.\r\n";
                    return;
                }

                Array.Resize(ref buffer, received);
                string text = Encoding.ASCII.GetString(buffer);
                serverTB.Text += "Client: " + text + ".\r\n";
                Array.Resize(ref buffer, clientSocket.ReceiveBufferSize);
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                LoginPackage loginPacage = (LoginPackage)javaScriptSerializer.Deserialize(text, typeof(LoginPackage));
                string username = loginPacage.Username;
                string password = loginPacage.Password;
                MiniTorrentDataContext db = new MiniTorrentDataContext();
                var c = (from clients in db.Clients
                         where clients.Username == username
                         where clients.Password == password
                         select clients).FirstOrDefault();
               
                if (c != null)
                {
                    buffer = Encoding.ASCII.GetBytes("True");
                  //  Send(buffer);
                    MessageBox.Show("True", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    MessageBox.Show("False", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void fileExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            serverSocket.Close();
            this.Close();
        }

        /*  private  void Send(byte[] data)
          {

              serverSocket.BeginSend(data, 0,data.Length, 0,
                  new AsyncCallback(SendCallback), serverSocket);
          }
          private  void SendCallback(IAsyncResult ar)
          {
              try
              {   
                  serverSocket = (Socket)ar.AsyncState;
              }
              catch (Exception e)
              {
                  Console.WriteLine(e.ToString());
              }
          }*/
    }
}
