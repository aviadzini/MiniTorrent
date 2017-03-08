using System;
using System.Collections.Generic;

namespace MiniTorrentLibrary
{
    public class ServerConstants
    {
        public const int ServerPort = 8005;
        public const int BufferSize = 1024;
        public const string EOF = "<EOF>";
        public const string ServerIP = "192.168.1.15";
    }

    public class PackageWrapper
    {
        public Type PackageType { get; set; }
        public object Package { get; set; }
    }

    public class LoginPackage
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
    }

    public class FileSearch
    {
        public string FileName { get; set; }
    }

    public class FilePackage
    {
        public bool Exist { get; set; }
        public string FileName { get; set; }
        public int CountClients { get; set; }
        public List<FileDetails> FilesList { get; set; }
    }

    public class FileDetails
    {
        public string Username { get; set; }
        public int FileSize { get; set; }
        public int Port { get; set; }
        public string Ip { get; set; }
    }
}
