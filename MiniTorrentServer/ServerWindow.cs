using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using MiniTorrentLibrary;
using Newtonsoft.Json;
using MiniTorrentClient;

namespace MiniTorrentServer
{
    public partial class ServerWindow : Form
    {
        public static int NumberOfConnections = 0;

        private Socket serverSocket;
        private List<Socket> clientSockets = new List<Socket>();

        private byte[] buffer = new byte[Constants.BufferSize];
        private StringBuilder sb = new StringBuilder();

        public ServerWindow()
        {
            InitializeComponent();

            SetupServer();
        }

        public void SetupServer()
        {
            serverTB.Text += "Setting up server...\r\n";

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                serverSocket.Bind(new IPEndPoint(IPAddress.Parse(Constants.ServerIP), Constants.ServerPort));
                serverSocket.Listen(1);

                serverTB.Text += string.Format("Waiting for a connection...\r\n\r\n");

                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }

            catch (Exception e)
            {
                serverTB.Text += e.ToString();
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            Socket handler = serverSocket.EndAccept(ar);
            clientSockets.Add(handler);
            
            serverTB.Text += string.Format("Connected to client at {0}\r\n", handler.RemoteEndPoint.ToString());
            serverTB.Text += string.Format("Client #{0} at {1}\r\n", ++NumberOfConnections, handler.RemoteEndPoint.ToString());

            handler.BeginReceive(buffer, 0, Constants.BufferSize, 0, new AsyncCallback(ReadCallback), handler);

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

                if (content.IndexOf(Constants.EOF) > -1)
                {
                    serverTB.Text += string.Format("Read {0} bytes from socket.\r\nData: {1}", content.Length, content);

                    content = content.Substring(0, content.Length - 5);

                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    MiniTorrentDataContext db = new MiniTorrentDataContext();

                    var deserialized = JsonConvert.DeserializeObject<PackageWrapper>(content);
                  

                    if (deserialized.PackageType == typeof(LoginPackage))
                    {
                        var lp = JsonConvert.DeserializeObject(Convert.ToString(deserialized.Package), deserialized.PackageType);
                        
                        var c = (from clients in db.Clients
                                 where clients.Username == ((LoginPackage)lp).Username
                                 where clients.Password == ((LoginPackage)lp).Password
                                 select clients).FirstOrDefault();
                    //    serverTB.Text += string.Format(((LoginPackage)lp).Password);
                        if (c != null)
                        {
                            Random rnd = new Random();
                            List<Clients> ports;
                            int randomPort;

                            do
                            {
                                randomPort = rnd.Next(8006, 9000);
                                ports = (from clients in db.Clients
                                         where clients.Port == randomPort
                                         select clients).ToList();
                            }

                            while (ports.Count > 0);

                            c.Active = true;
                            c.Port = randomPort;
                            c.IP = ((LoginPackage)lp).IP;
                            db.SubmitChanges();

                            byte[] sendPort = Encoding.ASCII.GetBytes(randomPort.ToString());
                            handler.BeginSend(sendPort, 0, sendPort.Length, 0, new AsyncCallback(SendCallback), handler);
                        }

                        else
                        {
                            byte[] sendPort = Encoding.ASCII.GetBytes("-1");
                            handler.BeginSend(sendPort, 0, sendPort.Length, 0, new AsyncCallback(SendCallback), handler);
                        }
                    }

                    else
                    {
                        var fs = JsonConvert.DeserializeObject(Convert.ToString(deserialized.Package), deserialized.PackageType);

                        var f = (from files in db.Files
                                 join clintsFile in db.ClientFiles
                                 on files.Name equals clintsFile.FileName
                                 join clients in db.Clients
                                 on clintsFile.Username equals clients.Username
                                 where files.Name == ((FileSearch)fs).FileName
                                 select new { clients.Username,files.Size,clients.Port,clients.IP}).ToList();

                        if (f.Count == 0)
                        {
                            FilePackage fp = new FilePackage
                            {
                                Exist = false
                            };

                            byte[] sendAnswer = Encoding.ASCII.GetBytes(javaScriptSerializer.Serialize(fp) + Constants.EOF);
                            handler.BeginSend(sendAnswer, 0, sendAnswer.Length, 0, new AsyncCallback(SendCallback), handler);
                        }
                        else
                        {
                            List<MiniTorrentLibrary.File> l = new List<MiniTorrentLibrary.File>();
                            for (int i = 0; i < f.Count; i++)
                            {
                                MiniTorrentLibrary.File file = new MiniTorrentLibrary.File
                                {
                                    Username=f.ElementAt(i).Username,
                                    FileSize=f.ElementAt(i).Size,
                                    Port=(int)f.ElementAt(i).Port,
                                    Ip=f.ElementAt(i).IP
                                };
                                l.Add(file);
                            }
                            FilePackage fp = new FilePackage
                                {
                                    Exist = true,
                                    FileName = ((FileSearch)fs).FileName,
                                    CountClients = f.Count,
                                    FilesList = l
                                };
                            byte[] sendAnswer = Encoding.ASCII.GetBytes(javaScriptSerializer.Serialize(fp) + Constants.EOF);
                            handler.BeginSend(sendAnswer, 0, sendAnswer.Length, 0, new AsyncCallback(SendCallback), handler);
                        }
                    }
                }

                else
                    handler.BeginReceive(buffer, 0, Constants.BufferSize, 0, new AsyncCallback(ReadCallback), handler);

                sb.Clear();
                handler.BeginReceive(buffer, 0, Constants.BufferSize, 0, new AsyncCallback(ReadCallback), handler);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                
                int bytesSent = handler.EndSend(ar);
                serverTB.Text += string.Format("Sent {0} bytes to client.\r\n", bytesSent);
            }

            catch (Exception e)
            {
                serverTB.Text += e.ToString();
            }
        }
    }
}