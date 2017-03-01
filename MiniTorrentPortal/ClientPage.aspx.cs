using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
namespace MiniTorrentPortal
{
    public partial class ClientPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindGrid();
            LinkButton logoutBT = new LinkButton();
            logoutBT.Text = "Logout";
            logoutBT.Attributes.Add("style", "right: 0px;");
            logoutBT.Click += new EventHandler(LogoutOnClick);
            form1.Controls.Add(logoutBT);
        }

        private void LogoutOnClick(object sender, EventArgs e)
        {
          
           

        }

        private void BindGrid()
        {
            string username = Request.QueryString["Name"];

            string constr = ConfigurationManager.ConnectionStrings["MiniTorrentDBConnectionString1"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("SELECT Username, Password, UpPath, DownPath FROM Clients WHERE Username = @username");
                cmd.Parameters.AddWithValue("@username", username);

                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                    }
                }
            }
        }

        protected void OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = GridView1.Rows[e.RowIndex];

            string username = GridView1.DataKeys[e.RowIndex].Values[0].ToString();
            string password = (row.Cells[2].Controls[0] as TextBox).Text;
            string upPath = (row.Cells[3].Controls[0] as TextBox).Text;
            string downPath = (row.Cells[4].Controls[0] as TextBox).Text;

            string constr = ConfigurationManager.ConnectionStrings["MiniTorrentDBConnectionString1"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE Clients SET Password = @password, UpPath = @upPath, DownPath = @downPath WHERE Username = @username"))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@upPath", upPath);
                    cmd.Parameters.AddWithValue("@downPath", downPath);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
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