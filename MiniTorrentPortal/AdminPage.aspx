<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="MiniTorrentPortal.AdminPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

    <head runat = "server">
        <title>Admin Page</title>
    </head>

    <body style = "background-image:url(BackGround.jpg);" >
        <p style = "font-size:40px; margin-left:50px;"><strong>Admin Page</strong></p>
        <form id = "form1" runat = "server">
            <div style = "margin-left:50px; ">
                <p style = "font-size:medium;">All Users</p>
                <table id = "ClientTable" runat = "server" cellpadding="10"/> 
            </div> <br />

            <asp:LinkButton ID = "editLB" runat = "server" Text = "EditProfile" 
                        style = "margin-left:50px; font-size:medium;"
                        OnClick = "EditProfileOnClick">Edit Profile</asp:LinkButton>

            <asp:LinkButton ID = "logoutB" runat = "server" Text = "Logout" 
                style = "font-size:medium;"
                OnClick = "LogoutOnClick">Logout</asp:LinkButton>
          
        </form>
    </body>
</html>
