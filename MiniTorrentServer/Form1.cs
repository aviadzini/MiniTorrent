using System;
using System.Collections;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Linq;

namespace MiniTorrentServer
{
    public partial class Form1 : Form
    {
        public const int port = 8005;      
        public static string data = null;
        public static int NumberOfConnections = 0;
        ArrayList ConnectionsList = new ArrayList();

        public Form1()
        {
            InitializeComponent();

            StartListening();
        }

        public void StartListening()
        {
            byte[] bytes = new byte[StateObject.BufferSize];

            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            serverTB.Text += string.Format("Binding port {0} at local address {1}\n", port, ipAddress.ToString());

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverTB.Text += string.Format("Waiting for a connection...\n\n");

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
            }

            catch (Exception e)
            {
                serverTB.Text += string.Format(e.ToString());
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            
            StateObject state = new StateObject();
            state.workSocket = handler;
            serverTB.Text += string.Format("Connected to client at {0}\n ", handler.RemoteEndPoint.ToString());
            state.FirstBit = true;

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            state.StartTime = DateTime.Now;
        }

        public void ReadCallback(IAsyncResult ar)
        {
            string eof = "<EOF>";

            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket; 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                string content = state.sb.ToString();

                if (content.IndexOf(eof) > -1)
                {
                    serverTB.Text += string.Format("\nClient #{0} at {1}", ++NumberOfConnections, handler.RemoteEndPoint.ToString());
                    serverTB.Text += string.Format("Read {0} bytes from socket. \n Data : {1}", content.Length, content);

                    content = content.Substring(0, content.Length - 5);

                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    LoginPackage loginPacage = (LoginPackage)javaScriptSerializer.Deserialize(content, typeof(LoginPackage));

                    string username = loginPacage.Username;
                    string password = loginPacage.Password;

                    MiniTorrentDataContext db = new MiniTorrentDataContext();

                    var c = (from clients in db.Clients
                             where clients.Username == username
                             where clients.Password == password
                             select clients).FirstOrDefault();

                    if (c != null)
                        Send(handler, bool.TrueString);

                    else
                        Send(handler, bool.FalseString);
                }

                else
                {
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private void Send(Socket handler, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                
                int bytesSent = handler.EndSend(ar);
                serverTB.Text += string.Format("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                serverTB.Text += string.Format("Waiting for a connection...\n\n");
            }

            catch (Exception e)
            {
                serverTB.Text += string.Format(e.ToString());
            }
        }
    }
}