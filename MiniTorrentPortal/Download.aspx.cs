using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MiniTorrentLibrary;
using System.Data;

namespace MiniTorrentPortal
{
    public partial class Download : Page
    {
        private DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dt = File.getAllFiles();
                BindGrid();
            }
        }

        private void BindGrid()
        {
            GridView1.DataSource = dt;
            GridView1.DataBind();

            SumLable.Text = "Total users: " + Client.getNumOfClients() +
                "\r\nTotal Active Users:" + Client.getNumOfActiveClients() +
                "\r\nTotal Files: " + dt.Rows.Count;
        }

        protected void SearchB_Click(object sender, EventArgs e)
        {
            if (SearchTB.Text != "")
            {
                dt = File.getAllFilesByName(SearchTB.Text);
                BindGrid();

                FilesLabel.Text = "";
            }

            else
            {
                dt = File.getAllFiles();
                BindGrid();
            }
        }
    }
}