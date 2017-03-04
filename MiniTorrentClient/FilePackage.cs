using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentClient
{

    class FilePackage
    {
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string BufferSize { get; set; }
        public int CountClients { get; set; }
        public List<ClientsFile> ClientsList {get;set;}

    }
    class ClientsFile
    {
        public string Username { get; set; }
        public int port { get; set; }
        public long ip { get; set; }

    }
}
