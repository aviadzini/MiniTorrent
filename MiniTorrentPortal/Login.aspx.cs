
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniTorrentPortal
{
    public partial class Admin : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LoginOnClick(object sender, EventArgs e)
        {
            string connectString = System.Configuration.ConfigurationManager
                .ConnectionStrings["MiniTorrentDBConnectionString1"].ToString();

            MiniTorrentDataContext db = new MiniTorrentDataContext(connectString);

            var c = (from clients in db.Clients
                     where clients.Username == UsernameTB.Text.Trim()
                     where clients.Password == PasswordTB.Text.Trim()
                     select clients).ToList();

            if (c.Count == 0)
                ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
                   "alert(' Username or password is incorrect '); window.location='" +
                   Request.ApplicationPath + "Login.aspx';", true);

            else if (c.ElementAt(0).Active)
                ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
                   "alert(' Username is already loggedin '); window.location='" +
                   Request.ApplicationPath + "Login.aspx';", true);

            else
            {

                c.ElementAt(0).Active = true;
                db.SubmitChanges();
                if (c.ElementAtOrDefault(0).Admin)
                {
                    Response.Redirect("AdminPage.aspx?Name="+ UsernameTB.Text);
                }
                else
                    Response.Redirect("ClientPage.aspx?Name=" + UsernameTB.Text);
            }
        }
    }
}