<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientPage.aspx.cs" Inherits="MiniTorrentPortal.ClientPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Client Page</title>
</head>
    <body style="background-image:url(BackGround.jpg);" >
        <h1>Client Page</h1>
    <form id="form1" runat="server">
    <div>
        <asp:GridView ID = "GridView1" runat = "server" DataKeyNames = "Username" 
            OnRowEditing = "OnRowEditing" OnRowCancelingEdit = "OnRowCancelingEdit"
            OnRowUpdating = "OnRowUpdating" AutoGenerateEditButton = "true" />
    </div> 
    </form>
</body>
</html>