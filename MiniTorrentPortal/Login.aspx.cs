using System;
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
            if (!Client.isClientExist(UsernameTB.Text, PasswordTB.Text))
                ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
                   "alert(' Username or password is incorrect '); window.location='" +
                   Request.ApplicationPath + "Login.aspx';", true);

            else
            {
                Client client = new Client();
                client.getClient(UsernameTB.Text);

                if (client.Admin)
                    Response.Redirect("AdminPage.aspx?Name=" + UsernameTB.Text);

                else
                    Response.Redirect("ClientPage.aspx?Name=" + UsernameTB.Text);
            }
        }
    }
}