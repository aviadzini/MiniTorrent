<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="MiniTorrentPortal.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registeration</title>
</head>
<body>
    <p>Registeration</p>
    <form id="form1" runat="server">
    <div>
        <p>Enter Username: </p>
        <asp:TextBox ID = "UsernameTB" Text = "Enter Username here" runat = "server" />

        <p>Enter Password: </p>
        <asp:TextBox ID = "PasswordTB" Text = "Enter Password here" runat = "server" />

        <p>Enter Upload Path: </p>
        <asp:TextBox ID = "UpPathTB" Text = "Enter Upload Path here" runat = "server" />

        <p>Enter Download Path: </p>
        <asp:TextBox ID = "DownPathTB" Text = "Enter Download Path here" runat = "server" />

        <br />
        <asp:Button ID = "RegisterB" Text = "Register" runat = "server" OnClick = "RegisterOnClick" />
    </div>
    </form>
</body>
</html>
