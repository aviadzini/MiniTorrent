using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace MiniTorrentPortal
{
    public partial class ClientPage : Page
    {
        string username = "";
        string connectString;
        MiniTorrentDataContext db;

        protected void Page_Load(object sender, EventArgs e)
        {
            username = Request.QueryString["Name"];

            connectString = ConfigurationManager.ConnectionStrings["MiniTorrentDBConnectionString1"].ToString();
            db = new MiniTorrentDataContext(connectString);

            if (!IsPostBack)
                BindGrid();
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

        private void BindGrid()
        {
            var c = from clients in db.Clients
                    where clients.Username == username
                    select new { clients.Username, clients.Password, clients.UpPath, clients.DownPath };

            GridView1.DataSource = c;
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
            
            string password = (row.Cells[2].Controls[0] as TextBox).Text;
            string upPath = (row.Cells[3].Controls[0] as TextBox).Text;
            string downPath = (row.Cells[4].Controls[0] as TextBox).Text;

            var c = (from clients in db.Clients
                    where clients.Username == username
                    select clients).Single();

            c.Password = password;
            c.UpPath = upPath;
            c.DownPath = downPath;
            db.SubmitChanges();

            GridView1.EditIndex = -1;
            BindGrid();
        }

        protected void OnRowCancelingEdit(object sender, EventArgs e)
        {
            GridView1.EditIndex = -1;
            BindGrid();
        }
    }
}