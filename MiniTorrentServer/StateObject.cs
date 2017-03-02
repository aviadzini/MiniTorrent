using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentServer
{
    public class StateObject
    {
        public Socket workSocket = null;       // Client  socket.
        public const int BufferSize = 1024;       // Size of receive buffer.
        public int counter = 0;       // counter of received bytes
        public int ReceiveCounte = 0;       // counter of received bytes
        public byte[] buffer = new byte[BufferSize]; // Receive buffer.
        public StringBuilder sb = new StringBuilder();  // Received data string.
        public DateTime StartTime, EndTime;
        public bool FirstBit = true;
        public int GetBufferSize() { return BufferSize; }
    }
}
