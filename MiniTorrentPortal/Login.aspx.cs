using System;
using System.Linq;
using System.Web.UI;
using MiniTorrentLibrary;

namespace MiniTorrentPortal
{
    public partial class Admin : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void LoginOnClick(object sender, EventArgs e)
        {
            var client = ClientsDBO.getClientsByUsernamePassword(UsernameTB.Text, PasswordTB.Text);

            if (client.Count == 0)
                ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
                   "alert(' Username or password is incorrect '); window.location='" +
                   Request.ApplicationPath + "Login.aspx';", true);

            else
            {
                if (!client.First().Active)
                {
                    ClientsDBO.activateClient(client.First().Username);

                    if (client.First().Admin)
                        Response.Redirect("AdminPage.aspx?Name=" + UsernameTB.Text);

                    else
                        Response.Redirect("ClientPage.aspx?Name=" + UsernameTB.Text);
                }

                else
                    ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
                       "alert(' Username is already loggedin '); window.location='" +
                       Request.ApplicationPath + "Login.aspx';", true);
            }
        }
    }
}