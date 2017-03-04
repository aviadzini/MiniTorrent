using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MiniTorrentServer
{
    class FileTransfer
    {
        private string username;
        public FileTransfer(string username)
        {
            this.username = username;
            StartListening();
        }

    public void StartListening()
    {
        MiniTorrentDataContext db = new MiniTorrentDataContext();

        var c = (from clients in db.Clients
                  where clients.Username == username
                  select clients).FirstOrDefault();
            long ip = long.Parse(c.IP);
            int port = (int)c.Port;
            byte[] bytes = new byte[StateObject.BufferSize];

       
        IPEndPoint localEndPoint = new IPEndPoint(ip, port);

      //  serverTB.Text += string.Format("Binding port {0} at local address {1}\n", port, ipAddress.ToString());

        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
       // serverTB.Text += string.Format("Waiting for a connection...\n\n");

        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
        }

        catch (Exception e)
        {
            //serverTB.Text += string.Format(e.ToString());
        }
    }

    public void AcceptCallback(IAsyncResult ar)
    {
        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        StateObject state = new StateObject();
        state.workSocket = handler;
        //serverTB.Text += string.Format("Connected to client at {0}\n ", handler.RemoteEndPoint.ToString());
        state.FirstBit = true;

        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        //state.StartTime = DateTime.Now;
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
          //      serverTB.Text += string.Format("\nClient #{0} at {1}", ++NumberOfConnections, handler.RemoteEndPoint.ToString());
          //    serverTB.Text += string.Format("Read {0} bytes from socket. \n Data : {1}", content.Length, content);

                content = content.Substring(0, content.Length - 5);

          //      JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
          //      LoginPackage loginPacage = (LoginPackage)javaScriptSerializer.Deserialize(content, typeof(LoginPackage));

          //    string username = loginPacage.Username;
          //  string password = loginPacage.Password;

                MiniTorrentDataContext db = new MiniTorrentDataContext();

                    var c = (from Fiels in db.Files
                             join ClientsFiles in db.ClientFiles
                             on Fiels.Name equals ClientsFiles.FileName 
                             where Fiels.Name == content
                             select new { Fiels.Name,Fiels.Size,ClientsFiles.Clients } ).ToList();

                if (c != null)
                {
                        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                        FilePackage loginPacage = (FilePackage)javaScriptSerializer.Deserialize(content, typeof(LoginPackage));

                     
                        Send(handler, loginPacage+"<Eof>");
                }

                else
                    Send(handler, "Not Found!");
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
          //  serverTB.Text += string.Format("Sent {0} bytes to client.", bytesSent);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
            //serverTB.Text += string.Format("Waiting for a connection...\n\n");
        }

        catch (Exception e)
        {
            //serverTB.Text += string.Format(e.ToString());
        }
    }
}
}
