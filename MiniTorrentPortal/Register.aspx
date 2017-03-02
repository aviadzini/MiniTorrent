<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="MiniTorrentPortal.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registeration</title>
</head>
<body style="background-image:url(BackGround.jpg);">
    <p style="font-size:40px; margin-left:50px;"><strong>Registeration</strong></p>
    <form id="form1" runat="server">
    <div style="margin-left:50px; font-size:medium;">
        <p>Enter Username: </p>
        <asp:TextBox ID = "UsernameTB" runat = "server" />
       <%-- <asp:RequiredFieldValidator ErrorMessage="Required Username!" ForeColor="Red" 
            ControlToValidate="UsernameTB" runat="server" />--%>

        <p >Enter Password: </p>
        <asp:TextBox ID = "PasswordTB" runat = "server" TextMode="Password" />
     <%--   <asp:RequiredFieldValidator ErrorMessage="Required Password!" ForeColor="Red" 
            ControlToValidate="PasswordTB" runat="server" />--%>

        <p>Confirm Password: </p>
        <asp:TextBox ID = "ConfirmPasswordTB" runat = "server" TextMode="Password" />
            <asp:CompareValidator ErrorMessage="Passwords do not match!" ForeColor="Red" 
                ControlToCompare="PasswordTB" ControlToValidate="ConfirmPasswordTB" 
                runat="server" />

        <p>Enter Upload Path: </p>
        <asp:TextBox ID = "UpPathTB" runat = "server" /> 
        <asp:ImageButton ID="UploadPathIB" runat = "server" ImageUrl="BrowserImage.png" style="width:20px;height:20px;" OnClick="UploadClick"></asp:ImageButton>
     <%--   <asp:RequiredFieldValidator ErrorMessage="Required Upload Path!" ForeColor="Red" 
            ControlToValidate="UpPathTB" runat="server" />--%>
      

        <p>Enter Download Path: </p>
        <asp:TextBox ID = "DownPathTB" runat = "server" />
        <asp:ImageButton ID="DownPathIB" runat = "server" ImageUrl="BrowserImage.png" style="width:20px;height:20px;" OnClick="DownledClick" ></asp:ImageButton>
       <%--<asp:RequiredFieldValidator ID="DownPathRFV" ErrorMessage="" ForeColor="Red" 
            ControlToValidate="DownPathTB" runat="server"  />--%>

       <p>
        <asp:Button ID = "RegisterB" Text = "Register" runat = "server" OnClick = "DownledClick" />
       </p>
    </div>
    </form>
</body>
</html>
