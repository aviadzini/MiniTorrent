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

            string message = "";

            var c = (from clients in db.Clients
                    where clients.Username == UsernameTB.Text.Trim()
                    select clients).ToList();

            if (c.Count == 0)
            {
                var u = new Clients
                {
                    Username = UsernameTB.Text.Trim(),
                    Password = PasswordTB.Text.Trim(),
                    UpPath = UpPathTB.Text.Trim(),
                    DownPath = DownPathTB.Text.Trim(),
                    Active = false,
                    Admin = false
                };

                db.Clients.InsertOnSubmit(u);
                db.SubmitChanges();

                message = "Registration successful.";
            }

            else
                message = "Username already exists.\\nPlease choose a different username.";

            ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
                "alert('" + message + "'); window.location='" +
                Request.ApplicationPath + "HomePage.html';", true);
        }
    }
}