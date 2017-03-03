<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="MiniTorrentPortal.Admin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

    <head>
        <title>Login</title>
    </head>

    <body style="background-image:url(BackGround.jpg);" >

        <p style = "font-size:40px; margin-left:50px;"><strong>Login</strong></p>

        <form id = "form1" runat = "server">
            <div style = "margin-left:50px; font-size:medium;">

                <p>Enter Username:</p>
                <asp:TextBox ID = "UsernameTB" runat = "server" />
                <asp:RequiredFieldValidator ErrorMessage = "Required Username!" ForeColor = "Red" 
                    ControlToValidate = "UsernameTB" runat = "server" />

                <p>Enter Password:</p>
                <asp:TextBox ID = "PasswordTB" runat = "server" TextMode = "Password" />
                <asp:RequiredFieldValidator ErrorMessage = "Required Password!" ForeColor = "Red" 
                    ControlToValidate = "PasswordTB" runat = "server" /><br /><br />

                <asp:Button ID = "LogInB" Text = "Login" runat = "server" OnClick = "LoginOnClick" />

            </div>
        </form>
    </body>
</html>
