using System;
using System.Collections.Generic;
using System.Linq;

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

        public static List<Clients> getClientsByUsernamePassword(string username, string password)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            return (from clients in db.Clients
                    where clients.Username == username
                    where clients.Password == password
                    select clients).ToList();
        }

        public static List<Clients> getClientsByLoginPackage(LoginPackage lp)
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
            return getClientsByLoginPackage(lp).Count > 0;
        }

        public static void activateClient(string username)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = getClientsByName(username).First();
            client.Active = true;

            db.SubmitChanges();
        }

        public static void deactivateClient(string username)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = getClientsByName(username).First();
            client.Active = false;

            db.SubmitChanges();
        }

        public static void setClientLogin(LoginPackage lp, int port)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            if (isLoginPackageExist(lp))
            {
                var client = getClientsByLoginPackage(lp).First();

                client.Active = true;
                client.IP = lp.IP;
                client.Port = port;

                db.SubmitChanges();
            }
        }

        public static void setClientLogout(string username)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            if (isUsernameExist(username))
            {
                var client = getClientsByName(username).First();

                client.Active = false;
                client.IP = null;
                client.Port = null;

                db.SubmitChanges();
            }
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

            var client = getClientsByName(username).First();

            if (client.Admin || client.Active)
                return false;

            db.Clients.DeleteOnSubmit(client);
            db.SubmitChanges();

            return true;
        }

        public static bool updateToAdmin(string username)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = getClientsByName(username).First();

            if (client.Admin)
                return false;

            client.Admin = true;
            db.SubmitChanges();

            return true;
        }

        public static bool updatePassword(string username, string password)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = getClientsByName(username).First();

            client.Password = password;
            db.SubmitChanges();

            return true;
        }

        public static bool updateUpPath(string username, string upPath)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = getClientsByName(username).First();

            client.UpPath = upPath;
            db.SubmitChanges();

            return true;
        }

        public static bool updateDownPath(string username, string downPath)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = getClientsByName(username).First();

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
        public static List<File> getAllFiles()
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            return (from files in db.Files
                    select files).ToList();
        }

        public static List<File> getFilesByName(string fileName)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            return (from files in db.Files
                    where files.Name == fileName
                    select files).ToList();
        }

        public static List<File> getAllDistinctFiles()
        {
            return new HashSet<File>(getAllFiles()).ToList();
        }

        public static List<File> getAllDistinctFilesByName(string fileName)
        {
            return new HashSet<File>(getFilesByName(fileName)).ToList();
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
        public static List<ClientFile> getClientFileByName(string fileName)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            return (from files in db.ClientFiles
                    where files.FileName == fileName
                    select files).ToList();
        }
    }
}
