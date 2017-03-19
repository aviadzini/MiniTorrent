using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MiniTorrentLibrary
{
    public class Client
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string UpPath { get; private set; }
        public string DownPath { get; private set; }
        public bool Active { get; private set; }
        public bool Admin { get; private set; }
        public string Ip { get; private set; }
        public int Port { get; private set; }

        public Client(string username, string password, string upPath, string downPath, bool active, bool admin, string ip, int port)
        {
            Username = username;
            Password = password;
            UpPath = upPath;
            DownPath = downPath;
            Active = active;
            Admin = admin;
            Ip = ip;
            Port = port;
        }

        public Client(string username, string password, string upPath, string downPath)
        {
            Username = username;
            Password = password;
            UpPath = upPath;
            DownPath = downPath;
            Active = false;
            Admin = false;
            Ip = "127.0.0.1";
            Port = 8006;
        }

        public Client()
        {
        }

        private bool isUsernameExist()
        {
            int count = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "SELECT COUNT(*) from Clients where Username = @username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@username", Username);

                count = (int)command.ExecuteScalar();
            }

            return count != 0;
        }

        public bool insertClient()
        {
            if (isUsernameExist())
                return false;

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "INSERT INTO Clients VALUES (@username, @password, @upPath, @downPath, @active, @admin, @ip, @port)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@username", Username);
                command.Parameters.AddWithValue("@password", Password);
                command.Parameters.AddWithValue("@upPath", UpPath);
                command.Parameters.AddWithValue("@downPath", DownPath);
                command.Parameters.AddWithValue("@active", Active);
                command.Parameters.AddWithValue("@admin", Admin);
                command.Parameters.AddWithValue("@ip", Ip);
                command.Parameters.AddWithValue("@port", Port);

                command.ExecuteScalar();
            }

            return true;
        }

        public static bool isClientExist(string username, string password)
        {
            int count = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "SELECT COUNT(*) from Clients where Username = @username AND Password = @password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                count = (int)command.ExecuteScalar();
            }

            return count != 0;
        }

        public void getClient(string username)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "SELECT * from Clients where Username = @username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@username", username);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Username = reader[0].ToString();
                    Password = reader[1].ToString();
                    UpPath = reader[2].ToString();
                    DownPath = reader[3].ToString();
                    Active = bool.Parse(reader[4].ToString());
                    Admin = bool.Parse(reader[5].ToString());
                    Ip = reader[6].ToString();
                    Port = int.Parse(reader[7].ToString());
                }
                reader.Close();
            }
        }

        public static DataSet getClientDataSet(string username)
        {
            DataSet ds = new DataSet();

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "SELECT Username, Password, UpPath, DownPath from Clients where Username = @username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@username", username);

                adapter.Fill(ds);
            }

            return ds;
        }

        public void updateClient(string password, string upPath, string downPath)
        {
            Password = password;
            UpPath = upPath;
            DownPath = downPath;

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "UPDATE Clients SET Password = @password, UpPath = @upPath, DownPath = @downPath where Username = @username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@username", Username);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@upPath", upPath);
                command.Parameters.AddWithValue("@downPath", downPath);

                command.ExecuteScalar();
            }
        }

        public static bool updateClientToAdmin(string username)
        {
            Client c = new Client();
            c.getClient(username);
            if (c.Admin)
                return false;

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "UPDATE Clients SET Admin = @admin where Username = @username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@admin", true);

                command.ExecuteScalar();
            }

            return true;
        }

        public static bool deleteClient(string username)
        {
            Client c = new Client();
            c.getClient(username);
            if (c.Admin)
                return false;

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "DELETE from Clients where Username = @username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@username", username);

                command.ExecuteScalar();
            }

            return true;
        }

        public static DataTable getAllClients()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "SELECT Username, Active, Admin from Clients";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        using (DataTable dt = new DataTable())
                        {
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
        }

        public static int getNumOfClients()
        {
            int num = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "SELECT * from Clients";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    num++;

                reader.Close();
            }

            return num;
        }

        public static int getNumOfActiveClients()
        {
            int num = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "SELECT * from Clients where Active = @active";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@active", true);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    num++;

                reader.Close();
            }

            return num;
        }

        public static ClientsDetailsPackage getClientsDetailsPackage(LoginPackage lp)
        {
            if (!isClientExist(lp.Username, lp.Password))
                return new ClientsDetailsPackage { Exist = false };

            setLogOn(lp);
            Client c = new Client();
            c.getClient(lp.Username);

            foreach (var item in lp.FileList)
            {
                int id = item.insertFile();
                ClientFile cf = new ClientFile(lp.Username, id);
                cf.insertClientFile();
            }

            return new ClientsDetailsPackage { Exist = true, Username = c.Username, UpPath = c.UpPath, DownPath = c.DownPath, IP = c.Ip, Port = c.Port };
        }

        private static void setLogOn(LoginPackage lp)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "UPDATE Clients SET Active = @active, Ip = @ip, Port = @port where Username = @username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@username", lp.Username);
                command.Parameters.AddWithValue("@active", true);
                command.Parameters.AddWithValue("@ip", lp.IP);
                command.Parameters.AddWithValue("@port", lp.Port);

                command.ExecuteScalar();
            }
        }

        public static void setLogOut(string username)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "UPDATE Clients SET Active = @active where Username = @username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@active", false);

                command.ExecuteScalar();
            }

            ClientFile.removeAllFilesByUsername(username);
        }
    }

    public class File
    {
        [JsonProperty("Id")]
        public int Id { get; private set; }

        [JsonProperty("Name")]
        public string Name { get; private set; }

        [JsonProperty("Size")]
        public int Size { get; private set; }

        public File(string name, int size)
        {
            Name = name;
            Size = size;
        }

        public File(int id, string name, int size)
        {
            Id = id;
            Name = name;
            Size = size;
        }

        public File()
        {
        }

        public int insertFile()
        {
            int id = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "INSERT INTO Files (Name, Size) output INSERTED.Id VALUES (@name, @size)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                
                command.Parameters.AddWithValue("@name", Name);
                command.Parameters.AddWithValue("@size", Size);

                id = (int)command.ExecuteScalar();
            }

            return id;
        }

        public static void removeFileById(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "DELETE from Files where Id = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@id", id);

                command.ExecuteScalar();
            }
        }

        public static DataTable getAllFiles()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "SELECT * from Files";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        using (DataTable dt = new DataTable())
                        {
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
        }

        public static DataTable getAllFilesByName(string name)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "SELECT * from Files where Name = @name";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        using (DataTable dt = new DataTable())
                        {
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
        }

        public static bool isFileExist(string name)
        {
            int count = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "SELECT COUNT(*) from Files where Name = @name";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@name", name);

                count = (int)command.ExecuteScalar();
            }

            return count != 0;
        }

        public static List<File> getAllFilesList()
        {
            List<File> list = new List<File>();

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "SELECT * from Files";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new File(int.Parse(reader[0].ToString()), reader[1].ToString(), int.Parse(reader[2].ToString())));
                }
                reader.Close();
            }

            return list;
        }

        public static List<File> getAllFilesListByName(string name)
        {
            List<File> list = new List<File>();

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "SELECT * from Files where Name = @name";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@name", name);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new File(int.Parse(reader[0].ToString()), reader[1].ToString(), int.Parse(reader[2].ToString())));
                }
                reader.Close();
            }

            return list;
        }
    }

    public class ClientFile
    {
        public string Username { get; private set; }
        public int FileID { get; private set; }

        public ClientFile(string username, int fileId)
        {
            Username = username;
            FileID = fileId;
        }

        public ClientFile()
        {
        }

        public void insertClientFile()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "INSERT INTO ClientFiles (Username, FileID) VALUES (@username, (Select Id From Files where Id = @fileId))";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@username", Username);
                command.Parameters.AddWithValue("@fileId", FileID);

                command.ExecuteScalar();
            }
        }

        public static void removeAllFilesByUsername(string username)
        {
            List<int> list = getAllIdByUsername(username);

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "DELETE from ClientFiles where Username = @username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@username", username);

                command.ExecuteScalar();
            }

            foreach(var item in list)
                File.removeFileById(item);
        }

        private static List<int> getAllIdByUsername(string username)
        {
            List<int> list = new List<int>();

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "SELECT FileID from ClientFiles where Username = @username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@username", username);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(int.Parse(reader[0].ToString()));
                }
                reader.Close();
            }

            return list;
        }

        public static List<ClientFile> getAllFilesById(int id)
        {
            List<ClientFile> list = new List<ClientFile>();

            string connectionString = ConfigurationManager.ConnectionStrings["MiniTorrentLibrary.Properties.Settings.MiniTorrentDBConnectionString"].ConnectionString;
            string query = "SELECT * from ClientFiles where FileID = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new ClientFile(reader[0].ToString(), int.Parse(reader[1].ToString())));
                }
                reader.Close();
            }

            return list;
        }
    }
}
