using MiniTorrentLibrary;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace MiniTorrentClient
{
    class Talker
    {
        private Socket socket;

        private byte[] buffer = new byte[ServerConstants.BufferSize];

        private string downPath;
        private string fileName;

        private BinaryWriter bw = new BinaryWriter(new MemoryStream());

        public Talker(string ip, int port, string downPath, string fileName)
        {
            this.downPath = downPath;
            this.fileName = fileName;

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), new AsyncCallback(ConnectCallback), null);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                socket.EndConnect(ar);

                PackageWrapper pw = new PackageWrapper
                {
                    PackageType = typeof(FileSearch),
                    Package = new FileSearch { FileName = fileName }
                };

                byte[] sendAnswer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(pw) + ServerConstants.EOF);

                socket.BeginSend(sendAnswer, 0, sendAnswer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                socket.EndSend(ar);

                socket.BeginReceive(buffer, 0, ServerConstants.BufferSize, 0, new AsyncCallback(ReadCallback), null);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                double total = 0.0;

                int bytes = socket.EndReceive(ar);
                while (bytes > 0)
                {
                    total += bytes;
                    bw.Write(buffer, 0, bytes);
                    if (bytes < buffer.Length)
                        break;
                    buffer = new byte[ServerConstants.BufferSize];
                    bytes = socket.Receive(buffer, 0, ServerConstants.BufferSize, SocketFlags.None);
                }

                bw.Flush();
                using (FileStream fs = System.IO.File.Create(downPath + fileName))
                {
                    ((MemoryStream)bw.BaseStream).WriteTo(fs);
                }

                watch.Stop();

                double elapsedS = watch.ElapsedMilliseconds / 1000.0;
                total = total / 1000.0;

                string massage = "Total time: " + elapsedS + " seconds.\r\n";
                massage += "File size: " + total + " KB.\r\n";
                massage += "Bit rate: " + total / elapsedS + " Kbps.";

                MessageBox.Show(massage, "Download information", MessageBoxButton.OK, MessageBoxImage.Information);

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
