<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Download.aspx.cs" Inherits="MiniTorrentPortal.Download" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Download</title>
</head>

    <body style = "background-image:url(BackGround.jpg);">
         
    <form id="form1" runat="server">
    <p style = "font-size:40px; margin-left:50px;"><strong>Download Page</strong></p>
    
    <div style="margin-left:50px">
        <p>
            <asp:TextBox ID = "SearchTB" runat = "server" Width="250px" Height="25px" placeholder="Enter here the file you want to search" />
             <asp:Button ID = "SearchB" Text = "Search" Height="30px" runat = "server" OnClick="SearchB_Click"/>
             
        </p>
        <asp:Label ID =  "FilesLabel" runat="server" Text="Files in application"/> 
        <table id = "FilesTable" runat = "server" cellpadding="5" />
        <p>
         <asp:Label ID =  "SumLable" runat="server" Text=""/> 
        </p>
    </div>
    </form>
</body>
</html>
