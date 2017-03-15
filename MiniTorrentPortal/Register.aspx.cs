using System;
using System.Web.UI;
using System.Windows.Forms;
using MiniTorrentLibrary;

namespace MiniTorrentPortal
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void UploadClick(object sender, ImageClickEventArgs e)
        {   
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.ShowNewFolderButton = false;
            if (fbd.ShowDialog() == DialogResult.OK)
                UpPathTB.Text = fbd.SelectedPath;
        }

        protected void DownloadClick(object sender, ImageClickEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.ShowDialog();
            if (fbd.ShowDialog()==DialogResult.OK)
                    UpPathTB.Text = fbd.SelectedPath;
          
            fbd.Description = "Select Folder";
        }

        protected void RegisterOnClick(object sender, EventArgs e)
        {
            if (UpPathTB.Text == "")
            {
                //DownPathRFV.ErrorMessage="Required Upload Path!";
            }

            else
            {
                string message = "";
                Client client = new Client(UsernameTB.Text, PasswordTB.Text, UpPathTB.Text, DownPathTB.Text);

                if (client.insertClient())
                    message = "Registration successful.";

                else
                    message = "Username already exists.\\nPlease choose a different username.";

                ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
                    "alert('" + message + "'); window.location='" +
                    Request.ApplicationPath + "HomePage.html';", true);
            }
        }
    }
}