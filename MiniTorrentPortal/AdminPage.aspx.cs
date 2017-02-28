using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiniTorrentPortal
{
    public partial class AdminPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string connectString = System.Configuration.ConfigurationManager.ConnectionStrings["MiniTorrentDBConnectionString1"].ToString();
            MiniTorrentDataContext db = new MiniTorrentDataContext(connectString);

            var c = (from clients in db.Clients
                     select clients).ToList();

            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell = new HtmlTableCell();
            LinkButton deleteButton = new LinkButton();
            LinkButton updateButton = new LinkButton();
            
           
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

            cell = new HtmlTableCell();
            ClientTable.CellPadding = 10;
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

        private void UpdateToAdmin(object sender, EventArgs e)
        {
            string message = "";
            LinkButton button = (LinkButton)sender;
            string user = button.CommandArgument;
            string connectString = System.Configuration.ConfigurationManager.ConnectionStrings["MiniTorrentDBConnectionString1"].ToString();
            MiniTorrentDataContext db = new MiniTorrentDataContext(connectString);
            var c = (from clients in db.Clients
                     where clients.Username == user
                     select clients).First();

            if (c.Admin == true) { 
                c.Admin = false;
                message = "The user is no longer admin! ";
                db.SubmitChanges();
        }
            else
            {

                c.Admin = true;
                message = "The user is now admin! ";
                db.SubmitChanges();
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
              "alert('" + message + "'); window.location='" +
              Request.ApplicationPath + "AdminPage.aspx';", true);

        }

        private void DeleteUser(object sender, EventArgs e)
        {
            string message = "";
            LinkButton button = (LinkButton)sender;
            string user = button.CommandArgument;
            string connectString = System.Configuration.ConfigurationManager.ConnectionStrings["MiniTorrentDBConnectionString1"].ToString();
            MiniTorrentDataContext db = new MiniTorrentDataContext(connectString);
            var c = (from clients in db.Clients
                         where clients.Username == user
                         select clients).First();

            if (string.Compare(c.Active.ToString(), "true") == 0)
                message = "The user active now,/nPlease try later! ";
            else
            {
                db.Clients.DeleteOnSubmit(c);
                message = "The user deleted! ";
                db.SubmitChanges();


            }
            ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
              "alert('" + message + "'); window.location='" +
              Request.ApplicationPath + "AdminPage.aspx';", true);

        }
    }
 
}