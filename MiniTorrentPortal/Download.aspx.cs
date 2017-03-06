using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MiniTorrentLibrary;

namespace MiniTorrentPortal
{
    public partial class Download : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell = new HtmlTableCell();

            int count = 0;

            var DistinctItems = FilesDBO.getAllDistinctFiles();

            foreach (var item in DistinctItems)
            {
                count++;

                row = new HtmlTableRow();
                cell = new HtmlTableCell();

                cell.InnerText = (count).ToString();
                row.Cells.Add(cell);

                cell = new HtmlTableCell();

                cell.InnerText = item.Name;
                row.Cells.Add(cell);

                FilesTable.Rows.Add(row);
            }

            var listOfClients = ClientsDBO.getAllClients();
            var activeClients = ClientsDBO.getAllActiveClients();
            var filesList = FilesDBO.getAllFiles();

            SumLable.Text = "Total users: " + listOfClients.Count +
                            "\r\nTotal Active Users:" + activeClients.Count +
                            "\r\nTotal Files: " + filesList.Count;
        }

        protected void SearchB_Click(object sender, EventArgs e)
        {
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell = new HtmlTableCell();

            if (SearchTB.Text != "")
            {
                var DistinctItems = FilesDBO.getAllDistinctFilesByName(SearchTB.Text);

                FilesTable.Rows.Clear();

                foreach (var item in DistinctItems)
                {
                    cell = new HtmlTableCell();

                    cell.InnerText = item.Name;
                    row.Cells.Add(cell);

                    FilesTable.Rows.Add(row);
                }

                FilesLabel.Text = "";

                var c = ClientFileDBO.getClientFileByName(SearchTB.Text);

                var filesList = FilesDBO.getFilesByName(SearchTB.Text);

                SumLable.Text = "Total active users have the file : " + c.Count +
                                "\nTotal Files: " + filesList.Count;
            }
        }
    }
}