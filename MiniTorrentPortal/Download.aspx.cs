using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace MiniTorrentPortal
{
    public partial class Download : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            

            HtmlTableRow row = new HtmlTableRow();

            HtmlTableCell cell = new HtmlTableCell();

            string connectString = System.Configuration.ConfigurationManager
               .ConnectionStrings["MiniTorrentDBConnectionString1"].ToString();
            MiniTorrentDataContext db = new MiniTorrentDataContext(connectString);

        
           int count = 0; 
           var DistinctItems = db.Files.GroupBy(x => x.Name).Select(y => y.First());
           foreach(var item in DistinctItems)
            {
                count++;   
                row = new HtmlTableRow();
                cell = new HtmlTableCell();
                cell.InnerText = (count).ToString();
                row.Cells.Add(cell);

                cell = new HtmlTableCell();
                cell.InnerText =item.Name;
                row.Cells.Add(cell);
                FielsTable.Rows.Add(row);
            }
            var c = (from clients in db.Clients
                     select clients).ToList();
            var activeClients = (from clients in db.Clients
                                  where clients.Active==true
                                  select clients).ToList();
            var filesList = (from files in db.Files
                         select files).ToList();

            SumLable.Text = "Total users: " + c.Count +
                            "\nTotal Active Users:" + activeClients.Count+
                            "\nTotal Files: " + filesList.Count;

        }

        protected void SearchB_Click(object sender, EventArgs e)
        {
            string connectString = System.Configuration.ConfigurationManager
              .ConnectionStrings["MiniTorrentDBConnectionString1"].ToString();
            MiniTorrentDataContext db = new MiniTorrentDataContext(connectString);

            HtmlTableRow row = new HtmlTableRow();

            HtmlTableCell cell = new HtmlTableCell();
            
            if (SearchTB.Text != "")
            {
                var DistinctItems = db.Files.GroupBy(x => x.Name).Select(y => y.First() ).Where(y => y.Name == SearchTB.Text);
                FielsTable.Rows.Clear();
                foreach (var item in DistinctItems)
                               
                {
                  
                    cell = new HtmlTableCell();
                    cell.InnerText = item.Name;
                    row.Cells.Add(cell);
                    FielsTable.Rows.Add(row);
                }

                    FilesLabel.Text = "";

                var c = (from clients in db.Clients
                         join clientsFile in db.ClientFiles
                         on clients.Username equals clientsFile.Username
                         join files in db.Files
                         on clientsFile.FileID equals files.ID
                         where files.Name== SearchTB.Text
                         where clients.Active==true
                         select clients).ToList();
               
                var filesList = (from files in db.Files
                                 where files.Name==SearchTB.Text
                                 select files).ToList();
            
                SumLable.Text = "Total active users have the file : " + c.Count +
                                "\nTotal Files: " + filesList.Count;

            }
        }
    }
}