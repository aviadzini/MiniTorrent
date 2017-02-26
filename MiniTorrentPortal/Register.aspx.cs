using System;
using System.Linq;
using System.Web.UI;

namespace MiniTorrentPortal
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RegisterOnClick(object sender, EventArgs e)
        {
            string connectString = System.Configuration.ConfigurationManager.ConnectionStrings["MiniTorrentDBConnectionString1"].ToString();
            MiniTorrentDataContext db = new MiniTorrentDataContext(connectString);

            var c = (from clients in db.Clients
                    where clients.Username == UsernameTB.Text
                    select clients).ToList();

            if (c.Count == 0)
            {
                var u = new Clients
                {
                    Username = UsernameTB.Text,
                    Password = PasswordTB.Text,
                    UpPath = UpPathTB.Text,
                    DownPath = DownPathTB.Text,
                    Active = false
                };

                db.Clients.InsertOnSubmit(u);
                db.SubmitChanges();


            }
        }
    }
}