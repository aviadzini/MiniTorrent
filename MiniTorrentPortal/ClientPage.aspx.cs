using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MiniTorrentLibrary;

namespace MiniTorrentPortal
{
    public partial class ClientPage : Page
    {
        private string username = "";
        private Client client = new Client();

        protected void Page_Load(object sender, EventArgs e)
        {
            username = Request.QueryString["Name"];

            client.getClient(username);

            if (!IsPostBack)
                BindGrid();
        }

        protected void LogoutOnClick(object sender, EventArgs e)
        {
            Response.Redirect("HomePage.html");
        }

        private void BindGrid()
        {
            DataSet ds = Client.getClientDataSet(username);

            GridView1.DataSource = ds;
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

            client.updateClient(password, upPath, downPath);

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