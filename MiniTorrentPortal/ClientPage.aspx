<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientPage.aspx.cs" Inherits="MiniTorrentPortal.ClientPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Client Page</title>
</head>
    <body style="background-image:url(BackGround.jpg);" >
        <p style="font-size:40px; margin-left:50px;"><strong>Client Page</strong></p>
    <form id="form1" runat="server" >
    <div style="margin-left:50px; font-size:medium;">
        <asp:GridView ID = "GridView1" runat = "server" DataKeyNames = "Username" 
            OnRowEditing = "OnRowEditing" OnRowCancelingEdit = "OnRowCancelingEdit"
            OnRowUpdating = "OnRowUpdating" AutoGenerateEditButton = "true"
            CellPadding="10" CellSpacing="10" />
    </div> 
    </form>
</body>
</html>