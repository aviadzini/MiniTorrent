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

        private byte[] buffer = new byte[ServerConstants.BufferSize];
        private StringBuilder sb = new StringBuilder();

        public ServerWindow()
        {
            InitializeComponent();

            serverTB.ScrollBars = ScrollBars.Vertical;
            serverTB.WordWrap = false;

            SetupServer();
        }

        private void SetupServer()
        {
            serverTB.AppendText("Setting up server...\r\n");

            serverSocket = new Socket(AddressFamily.InterNetwork, 
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                serverSocket.Bind(new IPEndPoint(
                    IPAddress.Parse(ServerConstants.ServerIP), 
                    ServerConstants.ServerPort));
                serverSocket.Listen(1);

                serverTB.AppendText("Waiting for a connection...\r\n\r\n");

                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }

            catch (Exception e)
            {
                serverTB.AppendText(e.ToString() + "\r\n\r\n");
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket handler = serverSocket.EndAccept(ar);
            
            serverTB.AppendText("Connected to client at " + handler.RemoteEndPoint.ToString() + "\r\n");
            serverTB.AppendText("Client #" + ++NumberOfConnections + " at "+ handler.RemoteEndPoint.ToString() + "\r\n\r\n");

            handler.BeginReceive(buffer, 0, ServerConstants.BufferSize, 0, 
                new AsyncCallback(ReadCallback), handler);

            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void ReadCallback(IAsyncResult ar)
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
                    serverTB.AppendText("Read " + content.Length + " bytes from socket.\r\nData: " + content + "\r\n\r\n");

                    var deserialized = JsonConvert.DeserializeObject<PackageWrapper>(content);
                    if (deserialized.PackageType == typeof(LoginPackage))
                    {
                        LoginPackage lp = (LoginPackage)JsonConvert.DeserializeObject(Convert.ToString(deserialized.Package), deserialized.PackageType);
                        
                        ClientsDetailsPackage client = Client.getClientsDetailsPackage(lp);

                        byte[] sendClient = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(client) + ServerConstants.EOF);
                        handler.BeginSend(sendClient, 0, sendClient.Length, 0, new AsyncCallback(SendCallback), handler);
                    }

                    else if (deserialized.PackageType == typeof(FileSearch))
                    {
                        FileSearch fs = (FileSearch)JsonConvert.DeserializeObject(Convert.ToString(deserialized.Package), deserialized.PackageType);

                        List<FileDetails> lfd = new List<FileDetails>();
                        List<File> listFile = new List<File>();
                        List<ClientFile> listClient = new List<ClientFile>();
                        Client client = new Client();
                        FilePackage fp = new FilePackage();

                        if (fs.FileName.CompareTo("*") == 0)
                        {
                            listFile = File.getAllFilesList();

                            foreach (var item in listFile)
                            {
                                listClient = ClientFile.getAllFilesById(item.Id);
                                foreach (var item2 in listClient)
                                {
                                    client.getClient(item2.Username);

                                    FileDetails f = new FileDetails
                                    {
                                        Username = item2.Username,
                                        FileName = item.Name,
                                        FileSize = item.Size,
                                        Ip = client.Ip,
                                        Port = client.Port
                                    };

                                    lfd.Add(f);
                                }
                            }

                            fp.Exist = true;
                            fp.FilesList = lfd;
                        }

                        else if (!File.isFileExist(fs.FileName))
                            fp.Exist = false;

                        else
                        {
                            listFile = File.getAllFilesListByName(fs.FileName);

                            foreach (var item in listFile)
                            {
                                listClient = ClientFile.getAllFilesById(item.Id);
                                foreach (var item2 in listClient)
                                {
                                    client.getClient(item2.Username);
                                    int size = item.Size;

                                    FileDetails f = new FileDetails
                                    {
                                        Username = item2.Username,
                                        FileName = item.Name,
                                        FileSize = size,
                                        Ip = client.Ip,
                                        Port = client.Port
                                    };

                                    lfd.Add(f);
                                }
                            }

                            fp.Exist = true;
                            fp.FilesList = lfd;
                        }

                        byte[] sendAnswer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(fp) + ServerConstants.EOF);
                        handler.BeginSend(sendAnswer, 0, sendAnswer.Length, 0, new AsyncCallback(SendCallback), handler);
                    }

                    else
                    {
                        LogoutPackage lp = (LogoutPackage)JsonConvert.DeserializeObject(Convert.ToString(deserialized.Package), deserialized.PackageType);
                        Client.setLogOut(lp.Username);
                    }
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
                serverTB.AppendText("Sent "+ bytesSent + " bytes to client.\r\n");
            }

            catch (Exception e)
            {
                serverTB.AppendText(e.ToString() + "\r\n\r\n");
            }
        }
    }
}