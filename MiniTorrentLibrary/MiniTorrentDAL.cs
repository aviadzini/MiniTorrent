using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniTorrentLibrary
{
    public class ClientsClass
    {
        public static List<Clients> getClientsByLogin(LoginPackage lp)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = (from clients in db.Clients
                     where clients.Username == lp.Username
                     where clients.Password == lp.Password
                     select clients).ToList();

            return client;
        }

        public static List<Clients> getClientsByPort(int randomPort)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var ports = (from clients in db.Clients
                     where clients.Port == randomPort
                     select clients).ToList();

            return ports;
        }

        public static void setClientActive(LoginPackage lp, int port)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var client = (from clients in db.Clients
                          where clients.Username == lp.Username
                          where clients.Password == lp.Password
                          select clients).Single();

            client.Active = true;
            client.IP = lp.IP;
            client.Port = port;

            db.SubmitChanges();
        }

        public static List<ClientFile> getClientFiles(string fileName)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            //var f = (from files in db.Files
            //         join clintsFile in db.ClientFiles
            //         on files.ID equals clintsFile.FileID
            //         join clients in db.Clients
            //         on clintsFile.Username equals clients.Username
            //         where files.Name == fileName
            //         select new { clients.Username, files.Size, clients.Port, clients.IP }).ToList();

            var f = (from files in db.ClientFiles
                    where files.FileName == fileName
                    select files).ToList();

            return f;
        }

        public static Tuple<string, int> getIpPort(string username)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var u = (from clients in db.Clients
                     where clients.Username == username
                     /////////////////////////////////////////only if the user is active we can download his files.
                     where clients.Active==true
                     select clients).Single();

            return new Tuple<string, int>(u.IP, (int)u.Port);
        }

        public static int getFileSize(int id)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();

            var z = (from files in db.Files
                     where files.ID == id
                     select files.Size).Single();

            return z;
        }
      
        /// /////////////////////////////////////////////////
  
        public static void logout(string username)
        {
            MiniTorrentDBDataContext db = new MiniTorrentDBDataContext();
            var client = (from clients in db.Clients
                          where clients.Username == username
                          select clients).Single();

            client.Active = false;
            client.IP = null;
            client.Port = null;

            db.SubmitChanges();
        }
    }

    public class FilesClass
    {
    }

    public class ClientFileClass
    {
    }


}
