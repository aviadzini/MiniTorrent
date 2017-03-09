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
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

            return (from clients in db.Clients
                    select clients).ToList();
        }

        public static List<Clients> getAllActiveClients()
        {
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

            return (from clients in db.Clients
                    where clients.Active == true
                    select clients).ToList();
        }

        public static List<Clients> getClientsByName(string username)
        {
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

            return (from clients in db.Clients
                    where clients.Username == username
                    select clients).ToList();
        }

        public static List<Clients> getClientsByPort(int port)
        {
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

            return (from clients in db.Clients
                    where clients.Port == port
                    select clients).ToList();
        }

        public static Clients getClientsByUsernamePassword(string username, string password)
        {
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

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
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

            var client = (from clients in db.Clients
                    where clients.Username == username
                    select clients).First();

            client.Active = true;

            db.SubmitChanges();
        }

        public static void deactivateClient(string username)
        {
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

            var client = (from clients in db.Clients
                          where clients.Username == username
                          select clients).First();

            client.Active = false;

            db.SubmitChanges();
        }

        public static void setClientLogin(LoginPackage lp)
        {
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

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
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

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
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

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
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

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
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

            var client = (from clients in db.Clients
                          where clients.Username == username
                          select clients).First();

            client.Password = password;
            db.SubmitChanges();

            return true;
        }

        public static bool updateUpPath(string username, string upPath)
        {
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

            var client = (from clients in db.Clients
                          where clients.Username == username
                          select clients).First();

            client.UpPath = upPath;
            db.SubmitChanges();

            return true;
        }

        public static bool updateDownPath(string username, string downPath)
        {
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

            var client = (from clients in db.Clients
                          where clients.Username == username
                          select clients).First();

            client.DownPath = downPath;
            db.SubmitChanges();

            return true;
        }

        public static void insertClient(Clients client)
        {
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

            db.Clients.InsertOnSubmit(client);
            db.SubmitChanges();
        }
    }

    public class FilesDBO
    {
        public static List<Files> getAllFiles()
        {
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

            return (from files in db.Files
                    select files).ToList();
        }

        public static List<Files> getFilesByName(string fileName)
        {
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

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
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

            return (from files in db.Files
                     where files.ID == id
                     select files.Size).Single();
        }
    }

    public class ClientFileDBO
    {
        public static List<ClientFiles> getClientFileByName(string fileName)
        {
            MiniTorrentDatabaseDataContext db = new MiniTorrentDatabaseDataContext();

            return (from files in db.ClientFiles
                    where files.FileName == fileName
                    select files).ToList();
        }
    }
}
