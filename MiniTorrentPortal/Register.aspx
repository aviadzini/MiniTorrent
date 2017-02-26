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
        <asp:TextBox ID = "UsernameTB" runat = "server" />
        <asp:RequiredFieldValidator ErrorMessage="Required Username!" ForeColor="Red" 
            ControlToValidate="UsernameTB" runat="server" />

        <p>Enter Password: </p>
        <asp:TextBox ID = "PasswordTB" runat = "server" TextMode="Password" />
        <asp:RequiredFieldValidator ErrorMessage="Required Password!" ForeColor="Red" 
            ControlToValidate="PasswordTB" runat="server" />

        <p>Confirm Password: </p>
        <asp:TextBox ID = "ConfirmPasswordTB" runat = "server" TextMode="Password" />
            <asp:CompareValidator ErrorMessage="Passwords do not match!" ForeColor="Red" 
                ControlToCompare="PasswordTB" ControlToValidate="ConfirmPasswordTB" 
                runat="server" />

        <p>Enter Upload Path: </p>
        <asp:TextBox ID = "UpPathTB" runat = "server" />
        <asp:RequiredFieldValidator ErrorMessage="Required Upload Path!" ForeColor="Red" 
            ControlToValidate="UpPathTB" runat="server" />

        <p>Enter Download Path: </p>
        <asp:TextBox ID = "DownPathTB" runat = "server" />
        <asp:RequiredFieldValidator ErrorMessage="Required Download Path!" ForeColor="Red" 
            ControlToValidate="DownPathTB" runat="server" />

        <br />
        <asp:Button ID = "RegisterB" Text = "Register" runat = "server" OnClick = "RegisterOnClick" />
    </div>
    </form>
</body>
</html>
