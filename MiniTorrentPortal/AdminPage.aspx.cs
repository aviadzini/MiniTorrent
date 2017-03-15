using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MiniTorrentLibrary;
using System.Data;

namespace MiniTorrentPortal
{
    public partial class AdminPage : Page
    {
        private string username = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            username = Request.QueryString["Name"];

            if (!IsPostBack)
                BindGrid();
        }

        private void BindGrid()
        {
            DataTable dt = Client.getAllClients();

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        protected void OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = GridView1.Rows[e.RowIndex];

            string user = (row.Cells[1]).Text;

            string message = "The user is already admin!";

            if (Client.updateClientToAdmin(user))
                message = "The user become an admin! ";

            ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
                "alert('" + message + "'); window.location='" +
                Request.ApplicationPath + "AdminPage.aspx?Name=" + username + "';", true);

            GridView1.EditIndex = -1;
            BindGrid();
        }

        protected void OnRowCancelingEdit(object sender, EventArgs e)
        {
            GridView1.EditIndex = -1;
            BindGrid();
        }

        protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridViewRow row = GridView1.Rows[e.RowIndex];
            string user = (row.Cells[1]).Text;

            string message = "You cannot delete an admin user!";

            if (Client.deleteClient(user))
                message = "The user have been deleted!";

            ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
                "alert('" + message + "'); window.location='" +
                Request.ApplicationPath + "AdminPage.aspx?Name=" + username + "';", true);

            BindGrid();
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowIndex != GridView1.EditIndex)
            {
                (e.Row.Cells[0].Controls[2] as LinkButton).Attributes["onclick"] = "return confirm('Do you want to delete this client?');";
            }
        }

        protected void EditProfileOnClick(object sender, EventArgs e)
        {
            Response.Redirect("ClientPage.aspx?Name=" + username);
        }

        protected void LogoutOnClick(object sender, EventArgs e)
        {
            Response.Redirect("HomePage.html");
        }
    }
}