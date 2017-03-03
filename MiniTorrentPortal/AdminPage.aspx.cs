using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace MiniTorrentPortal
{
    public partial class AdminPage : Page
    {
        string username = "";
        string connectString;
        MiniTorrentDataContext db;

        protected void Page_Load(object sender, EventArgs e)
        {
            username = Request.QueryString["Name"];

            connectString = ConfigurationManager.ConnectionStrings["MiniTorrentDBConnectionString1"].ToString();
            db = new MiniTorrentDataContext(connectString);

            var c = (from clients in db.Clients
                     where clients.Username != username
                     select clients).ToList();
            
            LinkButton deleteButton = new LinkButton();
            LinkButton updateButton = new LinkButton();

            HtmlTableRow row = new HtmlTableRow();

            HtmlTableCell cell = new HtmlTableCell();
            cell.InnerText = "Username";
            row.Cells.Add(cell);

            cell = new HtmlTableCell();
            cell.InnerText = "Active";
            row.Cells.Add(cell);

            cell = new HtmlTableCell();
            cell.InnerText = "Admin";
            row.Cells.Add(cell);

            cell = new HtmlTableCell();
            cell.InnerText = "Update To Admin";
            row.Cells.Add(cell);

            cell = new HtmlTableCell();
            cell.InnerText = "Delete";
            row.Cells.Add(cell);
            
            ClientTable.Rows.Add(row);
           
            foreach (var item in c)
            {
                row = new HtmlTableRow();

                cell = new HtmlTableCell();
                cell.InnerText = item.Username;
                row.Cells.Add(cell);

                cell = new HtmlTableCell();
                cell.InnerText = item.Active.ToString();
                row.Cells.Add(cell);

                cell = new HtmlTableCell();
                cell.InnerText = item.Admin.ToString();
                row.Cells.Add(cell);

                updateButton = new LinkButton();
                updateButton.Text = "Update To Admin";
                updateButton.CommandArgument = item.Username;
                updateButton.Click += new EventHandler(UpdateToAdmin);
                cell = new HtmlTableCell();
                cell.Controls.Add(updateButton);
                row.Cells.Add(cell);
 
                deleteButton = new LinkButton();
                deleteButton.Text = "Delete";
                deleteButton.CommandArgument = item.Username;
                deleteButton.Click += new EventHandler(DeleteUser);
                cell = new HtmlTableCell();
                cell.Controls.Add(deleteButton);
                row.Cells.Add(cell);

                ClientTable.Rows.Add(row);
            }
        }

        protected void EditProfileOnClick(object sender, EventArgs e)
        {
            Response.Redirect("ClientPage.aspx?Name=" + username);
        }
        protected void LogoutOnClick(object sender, EventArgs e)
        {
            var c = (from clients in db.Clients
                     where clients.Username == username
                     select clients).Single();

            c.Active = false;
            db.SubmitChanges();

            Response.Redirect("HomePage.html");
        }

        private void UpdateToAdmin(object sender, EventArgs e)
        {
            string message = "The user is already admin!";
            LinkButton button = (LinkButton)sender;
            string user = button.CommandArgument;

            var c = (from clients in db.Clients
                     where clients.Username == user
                     select clients).Single();

            if (!c.Admin)
            {
                c.Admin = true;
                db.SubmitChanges();

                message = "The user become an admin! ";
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
              "alert('" + message + "'); window.location='" +
              Request.ApplicationPath + "AdminPage.aspx?Name=" + username + "';", true);
        }

        private void DeleteUser(object sender, EventArgs e)
        {
            string message = "";
            LinkButton button = (LinkButton)sender;
            string user = button.CommandArgument;

            var c = (from clients in db.Clients
                         where clients.Username == user
                         select clients).Single();

            if (c.Active)
                message = "The user active now,//nPlease try later! ";

            else if (c.Admin)
                message = "You cannot delete an admin user! ";

            else
            {
                db.Clients.DeleteOnSubmit(c);
                db.SubmitChanges();
                message = "The user have been deleted! ";
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
              "alert('" + message + "'); window.location='" +
              Request.ApplicationPath + "AdminPage.aspx?Name=" + username + "';", true);
        }
    }
}