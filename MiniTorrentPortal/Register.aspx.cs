using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;

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
            {
                UpPathTB.Text = fbd.SelectedPath;
            }
           
        }
        protected void DownledClick(object sender, ImageClickEventArgs e)
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
            if (UpPathTB.Text == null)
            {

                //DownPathRFV.ErrorMessage="Required Upload Path!";
            }
            else

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
}