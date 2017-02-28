using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI;

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

            cell.InnerText = "Username";
            row.Cells.Add(cell);

            cell = new HtmlTableCell();
            cell.InnerText = "Active";
            row.Cells.Add(cell);

            cell = new HtmlTableCell();
            cell.InnerText = "Admin";
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

                ClientTable.Rows.Add(row);
            }
        }
    }
}