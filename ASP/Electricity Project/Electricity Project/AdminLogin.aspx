<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminLogin.aspx.cs" Inherits="Electricity_Project.AdminLogin" %>


<!DOCTYPE html>
<html>
<head runat="server">
    <title>Admin Login</title>
    <style>
        body { font-family: Arial; padding: 40px; }
        .login-box { width: 300px; margin: auto; border: 1px solid #ccc; padding: 20px; border-radius: 5px; }
        .login-box h2 { text-align: center; }
        .login-box input { width: 100%; margin-bottom: 10px; padding: 8px; }
        .login-box .error { color: red; text-align: center; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-box">
            <h2>Admin Login</h2>
            <asp:TextBox ID="txtUsername" runat="server" Placeholder="Username" />
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Placeholder="Password" />
            <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
            <asp:Label ID="lblError" runat="server" CssClass="error" />
        </div>
    </form>
</body>
</html>

