using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniTorrentPortal
{
    public partial class Admin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void LoginOnClick(object sender, EventArgs e)
        {
            string connectString = System.Configuration.ConfigurationManager.ConnectionStrings["MiniTorrentDBConnectionString1"].ToString();
            MiniTorrentDataContext db = new MiniTorrentDataContext(connectString);

            string message = "";

            var c = (from clients in db.Clients
                     where clients.Username == UsernameTB.Text.Trim()
                     where clients.Password==PasswordTB.Text.Trim()
                     select clients).ToList();
            /*
             * if((UsernameTB.Text.Trim()=="admin")&&(PasswordTB.Text.Trim()=="admin"))
             */

            if (c.Count == 0)
            {
                message = "Username or password is incorrect";
                ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
                   "alert('" + message + "'); window.location='" +
                   Request.ApplicationPath + "Login.aspx';", true);
            }

            else
            {
                //message = "Login successful.";
                //ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
                //    "alert('" + message + "'); window.location='" +
                //    Request.ApplicationPath + "AdminPage.aspx';", true);
                if (c.ElementAtOrDefault(0).Admin)
                    Response.Redirect("AdminPage.aspx");

                else
                    Response.Redirect("ClientPage.aspx");
            }
        }
    }

}