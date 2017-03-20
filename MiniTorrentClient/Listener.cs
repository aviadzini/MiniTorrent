using MiniTorrentLibrary;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace MiniTorrentClient
{
    class Listener
    {
        private Socket socket;

        private string upPath;

        private byte[] buffer = new byte[ServerConstants.BufferSize];
        private StringBuilder sb = new StringBuilder();

        public Listener(string ip, int port, string upPath)
        {
            this.upPath = upPath;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                socket.Listen(1);
                socket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = socket.EndAccept(ar);

                handler.BeginReceive(buffer, 0, ServerConstants.BufferSize, 0,
                    new AsyncCallback(ReadCallback), handler);

                socket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int received = handler.EndReceive(ar);

                if (received > 0)
                {
                    sb.Append(Encoding.ASCII.GetString(buffer, 0, received));
                    string content = sb.ToString();

                    buffer = new byte[ServerConstants.BufferSize];

                    if (content.IndexOf(ServerConstants.EOF) > -1)
                    {
                        content = content.Substring(0, content.Length - 5);

                        var deserialized = JsonConvert.DeserializeObject<PackageWrapper>(content);
                        if (deserialized.PackageType == typeof(FileSearch))
                        {
                            FileSearch fs = (FileSearch)JsonConvert.DeserializeObject(Convert.ToString(deserialized.Package), deserialized.PackageType);
                            handler.BeginSendFile(upPath + fs.FileName, new AsyncCallback(SendCallback), handler);
                        }
                    }

                    else
                        handler.BeginReceive(buffer, 0, ServerConstants.BufferSize, 0, new AsyncCallback(ReadCallback), handler);
                }

                sb.Clear();
                buffer = new byte[ServerConstants.BufferSize];
            }
            
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
}

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.EndSendFile(ar);
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Close()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
