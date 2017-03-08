using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;

namespace MiniTorrentLibrary
{
    public class ClientsDBO
    {
        public static List<Clients> getAllClients()
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            return (from clients in db.Clients
                    select clients).ToList();
        }

        public static List<Clients> getAllActiveClients()
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            return (from clients in db.Clients
                    where clients.Active == true
                    select clients).ToList();
        }

        public static List<Clients> getClientsByName(string username)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            return (from clients in db.Clients
                    where clients.Username == username
                    select clients).ToList();
        }

        public static List<Clients> getClientsByPort(int port)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            return (from clients in db.Clients
                    where clients.Port == port
                    select clients).ToList();
        }

        public static Clients getClientsByUsernamePassword(string username, string password)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            return (from clients in db.Clients
                    where clients.Username == username
                    where clients.Password == password
                    select clients).First();
        }

        public static Clients getClientsByLoginPackage(LoginPackage lp)
        {
            return getClientsByUsernamePassword(lp.Username, lp.Password);
        }

        public static bool isUsernameExist(string username)
        {
            return getClientsByName(username).Count > 0;
        }

        public static bool isPortExist(int port)
        {
            return getClientsByPort(port).Count > 0;
        }

        public static bool isLoginPackageExist(LoginPackage lp)
        {
            return getClientsByLoginPackage(lp) != null;
        }

        public static void activateClient(string username)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = (from clients in db.Clients
                    where clients.Username == username
                    select clients).First();

            client.Active = true;

            db.SubmitChanges();
        }

        public static void deactivateClient(string username)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = (from clients in db.Clients
                          where clients.Username == username
                          select clients).First();

            client.Active = false;

            db.SubmitChanges();
        }

        public static void setClientLogin(LoginPackage lp)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = (from clients in db.Clients
                    where clients.Username == lp.Username
                    where clients.Password == lp.Password
                    select clients).First();

            client.Active = true;
            client.IP = lp.IP;
            client.Port = lp.Port;

            db.SubmitChanges();
        }

        public static void setClientLogout(string username)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = (from clients in db.Clients
                          where clients.Username == username
                          select clients).First();

            client.Active = false;

            db.SubmitChanges();
        }

        public static Tuple<string, int> getIpPortByName(string username)
        {
            if (isUsernameExist(username))
            {
                Clients client = getClientsByName(username).First();
                return new Tuple<string, int>(client.IP, (int)client.Port);
            }

            return null;
        }

        public static bool deleteClient(string username)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = (from clients in db.Clients
                          where clients.Username == username
                          select clients).First();

            if (client.Admin) 
                return false;

            db.Clients.DeleteOnSubmit(client);
            db.SubmitChanges();

            return true;
        }

        public static bool updateToAdmin(string username)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = (from clients in db.Clients
                          where clients.Username == username
                          select clients).First();

            if (client.Admin)
                return false;

            client.Admin = true;
            db.SubmitChanges();

            return true;
        }

        public static bool updatePassword(string username, string password)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = (from clients in db.Clients
                          where clients.Username == username
                          select clients).First();

            client.Password = password;
            db.SubmitChanges();

            return true;
        }

        public static bool updateUpPath(string username, string upPath)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = (from clients in db.Clients
                          where clients.Username == username
                          select clients).First();

            client.UpPath = upPath;
            db.SubmitChanges();

            return true;
        }

        public static bool updateDownPath(string username, string downPath)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = (from clients in db.Clients
                          where clients.Username == username
                          select clients).First();

            client.DownPath = downPath;
            db.SubmitChanges();

            return true;
        }

        public static void insertClient(Clients client)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            db.Clients.InsertOnSubmit(client);
            db.SubmitChanges();
        }
    }

    public class FilesDBO
    {
        public static List<Files> getAllFiles()
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            return (from files in db.Files
                    select files).ToList();
        }

        public static List<Files> getFilesByName(string fileName)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            return (from files in db.Files
                    where files.Name == fileName
                    select files).ToList();
        }

        public static List<Files> getAllDistinctFiles()
        {
            return new HashSet<Files>(getAllFiles()).ToList();
        }

        public static List<Files> getAllDistinctFilesByName(string fileName)
        {
            return new HashSet<Files>(getFilesByName(fileName)).ToList();
        }

        public static int getFileSize(int id)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            return (from files in db.Files
                     where files.ID == id
                     select files.Size).Single();
        }
    }

    public class ClientFileDBO
    {
        public static List<ClientFiles> getClientFileByName(string fileName)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            return (from files in db.ClientFiles
                    where files.FileName == fileName
                    select files).ToList();
        }
    }
}

public class User
{
    public string Username { get; private set; }
    public string Password { get; private set; }
    public string UpPath { get; private set; }
    public string DownPath { get; private set; }
    public bool Active { get; private set; }
    public bool Admin { get; private set; }
    public string IP { get; private set; }
    public int Port { get; private set; }
}
