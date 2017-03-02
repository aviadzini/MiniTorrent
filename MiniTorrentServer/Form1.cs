
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Forms;
namespace MiniTorrentServer
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            //startServer();
            StartListening();

        }

        
            // Thread signal.  
            public  ManualResetEvent allDone = new ManualResetEvent(false);

           
            public void StartListening()
            {
                // Data buffer for incoming data.  
                byte[] bytes = new Byte[1024];

                // Establish the local endpoint for the socket.  
                // The DNS name of the computer  
                // running the listener is "host.contoso.com".  
               // IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);

                // Create a TCP/IP socket.  
                Socket listener = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.  
                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(100);

                    while (true)
                    {
                        // Set the event to nonsignaled state.  
                        allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    serverTB.Text += "Waiting for a connection...";
                        listener.BeginAccept(
                            new AsyncCallback(AcceptCallback),
                            listener);

                        // Wait until a connection is made before continuing.  
                        allDone.WaitOne();
                    }

                }
                catch (Exception e)
                {
                serverTB.Text += e.ToString();
                }

                Console.WriteLine("\nPress ENTER to continue...");
                Console.Read();

            }

            public void AcceptCallback(IAsyncResult ar)
            {
                // Signal the main thread to continue.  
                allDone.Set();

                // Get the socket that handles the client request.  
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = handler;
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
            }

            public void ReadCallback(IAsyncResult ar)
            {
                String content = String.Empty;

                // Retrieve the state object and the handler socket  
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                // Read data from the client socket.   
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read   
                    // more data.  
                    content = state.sb.ToString();
                    if (content.IndexOf("<EOF>") > -1)
                    {
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    
                        // Echo the data back to the client.  
                        Send(handler, content);
                    }
                    else
                    {
                        // Not all data received. Get more.  
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    }
                }
            }

            private void Send(Socket handler, String data)
            {
                // Convert the string data to byte data using ASCII encoding.  
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.  
                handler.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), handler);
            }

            private void SendCallback(IAsyncResult ar)
            {
                try
                {
                    // Retrieve the socket from the state object.  
                    Socket handler = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.  
                    int bytesSent = handler.EndSend(ar);
                    

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                }
                catch (Exception e)
                {
                serverTB.Text += e.ToString();
                }
            }
        }
    }

//using System;
//using System.Collections;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using System.Web.Script.Serialization;
//using System.Windows;
//using System.Windows.Forms;


//namespace MiniTorrentServer
//{
//    public partial class Form1 : Form
//    {
//        //private Socket serverSocket;
//        //private Socket clientSocket;

//        //private byte[] buffer;

//        public const int port = 8005;       // local port
//                                            // Incoming data from client.
//        public static string data = null;
//        public static int NumberOfConnections = 0;
//        ArrayList ConnectionsList = new ArrayList();
//        // Thread signal.
//        public static ManualResetEvent allDone = new ManualResetEvent(false);
//        public Form1()
//        {
//            InitializeComponent();
//            //startServer();
//            StartListening();

//        }

//        public void StartListening()
//        {
//            // Data buffer for incoming data.
//            byte[] bytes = new Byte[StateObject.BufferSize];

//            // Establish the local endpoint for the  socket.
//            //   The DNS name of the computer

//            //IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
//            //IPAddress ipAddress = ipHostInfo.AddressList[0];
//            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
//            serverTB.Text += "Binding port "+ port;
//            // Create a TCP/IP  socket.
//            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//            serverTB.Text += "Waiting for a connection...\n\n";
//            // Bind the  socket to the local endpoint and listen for incoming connections.
//            try
//            {
//                listener.Bind(localEndPoint);


//                while (true)
//                {
//                    allDone.Reset();  // Set the event to  nonsignaled state.
//                    listener.Listen(200);                  // Start  an asynchronous socket to listen for connections.
//                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
//                    allDone.WaitOne();  // Wait until a connection is made before continuing.
//                }

//            }
//            catch (Exception e)
//            {
//                serverTB.Text += e.ToString();
//            }

//            serverTB.Text += "\nHit enter to continue...";


//        }

//        public void AcceptCallback(IAsyncResult ar)
//        {


//            // Get the socket that handles the client request.
//            Socket listener = (Socket)ar.AsyncState;
//            Socket handler = listener.EndAccept(ar);
//            // Signal the main thread to continue.
//            allDone.Set();
//            // Create the state object.
//            StateObject state = new StateObject();
//            state.workSocket = handler;
//            serverTB.Text += "Connected to client at " + handler.RemoteEndPoint.ToString() + "\n ";
//            state.FirstBit = true;

//            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
//            state.StartTime = DateTime.Now;
//           // listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

//        }

//        public void ReadCallback(IAsyncResult ar)
//        {

//            // Retrieve the state object and the handler socket
//            // from the async state object.


//            StateObject state = (StateObject)ar.AsyncState;

//            Socket handler = state.workSocket;

//            // Read data from the client socket. 
//            int bytesRead = handler.EndReceive(ar);
//            //state.counter+=handler.EndReceive(ar);

//            if (bytesRead > 0)
//            {
//                // There  might be more data, so store  the data received so far.
//                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

//                // Check for end-of-file tag. If  it is not there, read more data.
//                string content = state.sb.ToString();
//                if (content.IndexOf("<EOF>") > -1)
//                {

//                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
//                    LoginPackage loginPacage = (LoginPackage)javaScriptSerializer.Deserialize(content, typeof(LoginPackage));
//                    string username = loginPacage.Username;
//                    string password = loginPacage.Password;
//                    MiniTorrentDataContext db = new MiniTorrentDataContext();
//                    var c = (from clients in db.Clients
//                             where clients.Username == username
//                             where clients.Password == password
//                             select clients).FirstOrDefault();

//                    if (c != null)
//                        // Echo the data back to the client.
//                        Send(handler, bool.TrueString);
//                    else
//                        Send(handler, bool.FalseString);
//                }
//                else
//                {
//                    // Not all data received. Get more.
//                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
//                        new AsyncCallback(ReadCallback), state);
//                }
//            }


//        }

//        private void Send(Socket handler, String data)
//        {
//            // Convert the string data to byte data using ASCII encoding.
//            byte[] byteData = Encoding.ASCII.GetBytes(data);

//            // Begin sending the data to the remote device.
//            handler.BeginSend(byteData, 0, byteData.Length, 0,
//                new AsyncCallback(SendCallback), handler);
//        }

//        private static void SendCallback(IAsyncResult ar)
//        {
//            try
//            {
//                StateObject state = (StateObject)ar.AsyncState;
//                // Retrieve the socket from the state object.
//                Socket handler = state.workSocket;

//                // Complete sending the data to the remote device.
//                int bytesSent = handler.EndSend(ar);


//                //handler.Shutdown(SocketShutdown.Both);
//                //handler.Close();

//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.ToString());
//            }
//        }

//        //private void startServer()
//        //{
//        //    try
//        //    {
//        //        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//        //        serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8005));
//        //        serverSocket.Listen(10);
//        //        serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

//        //        serverTB.Text += "The server has started.\r\n";
//        //    }

//        //    catch (Exception ex)
//        //    {
//        //        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//        //    }
//        //}

//        //private void AcceptCallback(IAsyncResult AR)
//        //{
//        //    try
//        //    {
//        //        clientSocket = serverSocket.EndAccept(AR);
//        //        buffer = new byte[clientSocket.ReceiveBufferSize];
//        //        clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
//        //        serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

//        //        serverTB.Text += "Client has connected to the server.\r\n";
//        //    }

//        //    catch (Exception ex)
//        //    {
//        //        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//        //    }
//        //}

//        //private void ReceiveCallback(IAsyncResult AR)
//        //{
//        //    try
//        //    {
//        //        int received = clientSocket.EndReceive(AR);

//        //        if (received == 0)
//        //        {
//        //            serverTB.Text += "Client has disconnected from the server.\r\n";
//        //            return;
//        //        }

//        //        Array.Resize(ref buffer, received);
//        //        string text = Encoding.ASCII.GetString(buffer);
//        //        serverTB.Text += "Client: " + text + ".\r\n";
//        //        Array.Resize(ref buffer, clientSocket.ReceiveBufferSize);
//        //       // clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
//        //        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
//        //        LoginPackage loginPacage = (LoginPackage)javaScriptSerializer.Deserialize(text, typeof(LoginPackage));
//        //        string username = loginPacage.Username;
//        //        string password = loginPacage.Password;
//        //        MiniTorrentDataContext db = new MiniTorrentDataContext();
//        //        var c = (from clients in db.Clients
//        //                 where clients.Username == username
//        //                 where clients.Password == password
//        //                 select clients).FirstOrDefault();

//        //        if (c != null)
//        //        {
//        //            buffer = Encoding.ASCII.GetBytes(bool.TrueString);
//        //            serverSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
//        //        }
//        //        else
//        //        {
//        //            buffer = Encoding.ASCII.GetBytes(bool.FalseString);
//        //            serverSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
//        //        }
//        //    }

//        //    catch (Exception ex)
//        //    {
//        //        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//        //    }
//        //}


//        //private void SendCallback(IAsyncResult AR)
//        //{
//        //    serverSocket.EndSend(AR);
//        //}
//        ///*void fileExitMenuItem_Click(object sender, RoutedEventArgs e)
//        //{
//        //    serverSocket.Close();
//        //    this.Close();
//        //}*/

//        ///*  private  void Send(byte[] data)
//        //  {

//        //      serverSocket.BeginSend(data, 0,data.Length, 0,
//        //          new AsyncCallback(SendCallback), serverSocket);
//        //  }
//        //  private  void SendCallback(IAsyncResult ar)
//        //  {
//        //      try
//        //      {   
//        //          serverSocket = (Socket)ar.AsyncState;
//        //      }
//        //      catch (Exception e)
//        //      {
//        //          Console.WriteLine(e.ToString());
//        //      }
//        //  }*/
//    }
//}
