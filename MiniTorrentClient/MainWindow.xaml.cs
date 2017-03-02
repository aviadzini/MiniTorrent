using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace MiniTorrentClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket clientSocket;
        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private static String response = String.Empty;  // The response from the remote device.
                                                        // private Socket clientSocket;

        //private byte[] buffer;
        public MainWindow()
        {
            InitializeComponent();
           
           // StartClient();
            //     connectToServer();
        }
        private void StartClient()
        {
            string RemoteIP = "127.0.0.1";
            int RemotePort = 8005;
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.

                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                //IPAddress ipAddress = IPAddress.Parse(RemoteIP);

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, RemotePort);

                //  Create a TCP/IP  socket.
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), clientSocket);      // Connect to the remote endpoint.
                connectDone.WaitOne();
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {

                Socket client = (Socket)ar.AsyncState; // Retrieve the socket from the state object.
                client.EndConnect(ar);  // Complete the connection.
                
                connectDone.Set();// Signal that the connection has been made.
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the async state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    ////  Get the rest of the data.
                    //client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    //    new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }



        private void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
           
            
        }
        ///////////////////////////////////////////////////////////username+password textbox/////////////////////////////////
        private void ClickUserNameTB(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Please Enter Your Username")
            {
                textBox.Clear();

          
            }
            
        }
        private void ClickPassowrdTB(object sender, RoutedEventArgs e)
        {
            
            TextBox textBox = (TextBox)sender;
           
            if (textBox.Text == "Please Enter Your Password")
            {
                textBox.Clear();

            }
        }
        //    private void connectToServer()
        //    {
        //        try
        //        {
        //            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //            clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8005), new AsyncCallback(ConnectCallback), null);
        //        }

        //        catch (Exception ex)
        //        {
        //            MessageBoxResult result = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }

        private void loginB_Click(object sender, RoutedEventArgs e)
{
            if (string.Compare(usernameTB.Text, "") == 0 || string.Compare(passwordTB.Text, "") == 0)
            {
                usernameTB.Clear();
                passwordTB.Clear();
                

                MessageBoxResult result = MessageBox.Show("Cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


    

    else
    {




                try
            {
                string ipA = "";

                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        ipA = ip.ToString();

                LoginPackage loginPackage = new LoginPackage
                {
                    Username = usernameTB.Text,
                    Password = passwordTB.Text,
                    IP = ipA
                };
                var json = new JavaScriptSerializer().Serialize(loginPackage);


                    Send(clientSocket, json+"<EOF>");       // Send test data to the remote device.
                    sendDone.WaitOne();
                    Receive(clientSocket);      // Receive the response from the remote device.
                    receiveDone.WaitOne();
                    if (bool.Parse(response))
                    {
                        MessageBoxResult result = MessageBox.Show(response, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        usernameTB.Clear();
                        passwordTB.Clear();

                        MessageBoxResult result = MessageBox.Show("Username or Password incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    //clientSocket.Shutdown(SocketShutdown.Both);      // Release the socket.
                    //clientSocket.Close();

                //    byte[] buffer = Encoding.ASCII.GetBytes(json);
                //clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
                //clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }

            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

        
    }
}

//    private void SendCallback(IAsyncResult AR)
//    {
//        clientSocket.EndSend(AR);
//    }

//    private void ConnectCallback(IAsyncResult AR)
//    {
//        try
//        {
//            if (clientSocket.Connected)
//            {
//                clientSocket.EndConnect(AR);
//            }
//            else
//            {
//                MessageBoxResult result = MessageBox.Show("Try later\\nthe server fail.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        catch (Exception ex)
//        {
//            MessageBoxResult result = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//        }
//    }

//    private void ReceiveCallback(IAsyncResult AR)
//    {
//        try
//        {
//            //int received = clientSocket.EndReceive(AR);

//           // if (received == 0)
//           // {
//           //     return;
//           // }
//            buffer = new byte[50];

//            //Array.Resize(ref buffer, received);
//            bool userFound = bool.Parse(buffer.ToString());

//            //Array.Resize(ref buffer, clientSocket.ReceiveBufferSize);
//            //clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
//            if (userFound)
//            {
//                MessageBoxResult result = MessageBox.Show(userFound.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//            else
//            {
//                usernameTB.Clear();
//                passwordTB.Clear();

//                MessageBoxResult result = MessageBox.Show("Username or Password incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }

//        }

//        catch (Exception ex)
//        {
//            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//        }
//    }





//    protected override void OnClosing(CancelEventArgs e)
//    {
//        base.OnClosing(e);

//        if (clientSocket != null && clientSocket.Connected)
//            clientSocket.Close();
//    }




//}

