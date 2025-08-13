<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductViewer.aspx.cs" Inherits="Assingment_ASP_1.ProductViewer" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Product Viewer</title>
    <style>
        .product-image { width: 200px; height: 200px; border: 1px solid #ccc; margin-top: 10px; }
        .price-label { font-weight: bold; color: green; margin-top: 10px; display: block; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h3>Select a Product:</h3>
        <asp:DropDownList ID="ddlProducts" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlProducts_SelectedIndexChanged" />
        <br /><br />

        <asp:Image ID="imgProduct" runat="server" CssClass="product-image" />
        <br /><br />

        <asp:Button ID="btnGetPrice" runat="server" Text="Get Price" OnClick="btnGetPrice_Click" />
        <br />

        <asp:Label ID="lblPrice" runat="server" CssClass="price-label" />
    </form>
</body>
</html>

