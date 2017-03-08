using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using MiniTorrentLibrary;
using Newtonsoft.Json;

namespace MiniTorrentServer
{
    public partial class ServerWindow : Form
    {
        public static int NumberOfConnections = 0;

        private Socket serverSocket;
        private List<Socket> clientSockets = new List<Socket>();

        private byte[] buffer = new byte[ServerConstants.BufferSize];
        private StringBuilder sb = new StringBuilder();

        public ServerWindow()
        {
            InitializeComponent();

            SetupServer();
        }

        public void SetupServer()
        {
            serverTB.Text += "Setting up server...\r\n";

            serverSocket = new Socket(AddressFamily.InterNetwork, 
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                serverSocket.Bind(new IPEndPoint(
                    IPAddress.Parse(ServerConstants.ServerIP), 
                    ServerConstants.ServerPort));
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
            
            serverTB.Text += string.Format("Connected to client at {0}\r\n", 
                handler.RemoteEndPoint.ToString());
            serverTB.Text += string.Format("Client #{0} at {1}\r\n", 
                ++NumberOfConnections, handler.RemoteEndPoint.ToString());

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
                    serverTB.Text += string.Format("Read {0} bytes from socket.\r\nData: {1}", 
                        content.Length, content);

                    content = content.Substring(0, content.Length - 5);

                    var deserialized = JsonConvert.DeserializeObject<PackageWrapper>(content);

                    if (deserialized.PackageType == typeof(LoginPackage))
                    {
                        LoginPackage lp = (LoginPackage)JsonConvert.DeserializeObject(Convert.ToString(deserialized.Package), deserialized.PackageType);

                        var client = ClientsDBO.getClientsByLoginPackage(lp);

                        if (client.Count > 0)
                        {
                            Random random = new Random();
                            List<Clients> portClients;
                            int randomPort;

                            do
                            {
                                randomPort = random.Next(8006, 9000);
                                portClients = ClientsDBO.getClientsByPort(randomPort);
                            }
                            while (portClients.Count > 0);

                            ClientsDBO.setClientLogin(lp, randomPort);

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
                        FileSearch fs = (FileSearch)JsonConvert.DeserializeObject(Convert.ToString(deserialized.Package), deserialized.PackageType);

                        List<ClientFiles> list = ClientFileDBO.getClientFileByName(fs.FileName);

                        if (list.Count == 0)
                        {
                            FilePackage fp = new FilePackage
                            {
                                Exist = false
                            };

                            byte[] sendAnswer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(fp) + ServerConstants.EOF);
                            handler.BeginSend(sendAnswer, 0, sendAnswer.Length, 0, new AsyncCallback(SendCallback), handler);
                        }

                        else
                        {
                            List<FileDetails> lfd = new List<FileDetails>();

                            foreach (var item in list)
                            {
                                Tuple<string, int> d = ClientsDBO.getIpPortByName(item.Username);
                                int size = FilesDBO.getFileSize(item.FileID);
                                FileDetails file = new FileDetails
                                {
                                    Username = item.Username,
                                    FileSize = size,
                                    Ip = d.Item1,
                                    Port = d.Item2
                                };

                                lfd.Add(file);
                            }

                            FilePackage fp = new FilePackage
                            {
                                Exist = true,
                                FileName = fs.FileName,
                                CountClients = list.Count,
                                FilesList = lfd
                            };

                            byte[] sendAnswer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(fp) + ServerConstants.EOF);
                            handler.BeginSend(sendAnswer, 0, sendAnswer.Length, 0, new AsyncCallback(SendCallback), handler);
                        }
                    }
<<<<<<< HEAD
                    //////////////////////////////////////
                    else
                    {
                        LogoutPackage logoutp = (LogoutPackage)JsonConvert.DeserializeObject(Convert.ToString(deserialized.Package), deserialized.PackageType);
                        ClientsDBO.setClientLogout(logoutp.Username);
                    }
=======
>>>>>>> parent of c3c5bd8... client logout 2.1.6
                }

                else
                    handler.BeginReceive(buffer, 0, ServerConstants.BufferSize, 0, new AsyncCallback(ReadCallback), handler);

                sb.Clear();
                handler.BeginReceive(buffer, 0, ServerConstants.BufferSize, 0, new AsyncCallback(ReadCallback), handler);
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