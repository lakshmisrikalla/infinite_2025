<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="Electricity_Project.Main" %>


<!DOCTYPE html>
<html>
<head runat="server">
    <title>Electricity Billing Automation</title>
    <style>
        body { font-family: Arial; padding: 30px; background-color: #f9f9f9; }
        .section { margin-bottom: 30px; padding: 20px; background-color: #fff; border: 1px solid #ccc; border-radius: 8px; width: 500px; }
        .label { font-weight: bold; display: block; margin-top: 10px; }
        input, button, .aspNetTextBox, .aspNetButton { margin: 5px 0; padding: 8px; width: 100%; box-sizing: border-box; }
        .grid-section { margin-top: 40px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <!-- 🔘 Main Menu Panel -->
        <asp:Panel ID="pnlMainMenu" runat="server" CssClass="section">
            <h3>Welcome, Admin</h3>
            <asp:Button ID="btnGoToEntry" runat="server" Text="Enter Bills" OnClick="btnGoToEntry_Click" CssClass="aspNetButton" />
            <asp:Button ID="btnGoToRetrieve" runat="server" Text="Retrieve Bills" OnClick="btnGoToRetrieve_Click" CssClass="aspNetButton" />
            <asp:Button ID="btnGoToSearchConsumer" runat="server" Text="Search by Consumer Number" OnClick="btnGoToSearchConsumer_Click" CssClass="aspNetButton" />

            <asp:Button ID="btnLogout" runat="server" Text="Logout" OnClick="btnLogout_Click" CssClass="aspNetButton" />
        </asp:Panel>

        <!-- 🧾 Bill Entry Panel -->
        <asp:Panel ID="pnlEntry" runat="server" Visible="false" CssClass="section">
            <h3>Enter Electricity Bill Details</h3>
            <span class="label">Enter Number of Bills to Add:</span>
            <asp:TextBox ID="txtBillCount" runat="server" CssClass="aspNetTextBox" />
            <asp:Button ID="btnStartEntry" runat="server" Text="Start Entry" OnClick="btnStartEntry_Click" CssClass="aspNetButton" />

            <hr />

            <span class="label">Enter Consumer Number:</span>
            <asp:TextBox ID="txtConsumerNumber" runat="server" CssClass="aspNetTextBox" />

            <span class="label">Enter Consumer Name:</span>
            <asp:TextBox ID="txtConsumerName" runat="server" CssClass="aspNetTextBox" />

            <span class="label">Enter Units Consumed:</span>
            <asp:TextBox ID="txtUnitsConsumed" runat="server" CssClass="aspNetTextBox" />

            <asp:Button ID="btnSubmitBill" runat="server" Text="Submit Bill" OnClick="btnSubmitBill_Click" CssClass="aspNetButton" />
        </asp:Panel>

         <asp:Panel ID="pnlSearchConsumer" runat="server" Visible="false" CssClass="section">
    <h3>Search Bills by Consumer Number</h3>
    <asp:TextBox ID="txtSearchConsumerNumber" runat="server" Placeholder="Enter Consumer Number" CssClass="aspNetTextBox" />
    <asp:Button ID="btnSearchConsumer" runat="server" Text="Search" OnClick="btnSearchConsumer_Click" CssClass="aspNetButton" />
    <asp:Button ID="btnBackFromSearch" runat="server" Text="Back to Menu" OnClick="btnBack_Click" CssClass="aspNetButton" />

    <asp:Label ID="lblConsumerName" runat="server" CssClass="label" />
    <asp:GridView ID="gvConsumerBills" runat="server" AutoGenerateColumns="true" CssClass="aspNetGrid" />
</asp:Panel>


        <!-- ✅ Summary Panel -->
        <asp:Panel ID="pnlSummary" runat="server" Visible="false" CssClass="section">
            <h3>Bill Entry Summary</h3>
            <asp:Literal ID="litSummary" runat="server" />
            <asp:Button ID="btnBackFromSummary" runat="server" Text="Back to Menu" OnClick="btnBack_Click" CssClass="aspNetButton" />
        </asp:Panel>

        <!-- 📊 Retrieve Panel -->
        <asp:Panel ID="pnlRetrieve" runat="server" Visible="false" CssClass="section">
            <h3>Retrieve Last N Bills</h3>
            <span class="label">Enter Number of Bills to Retrieve:</span>
            <asp:TextBox ID="txtLastN" runat="server" CssClass="aspNetTextBox" />
            <asp:Button ID="btnRetrieve" runat="server" Text="Show Bills" OnClick="btnRetrieve_Click" CssClass="aspNetButton" />
            <asp:Button ID="btnBackFromRetrieve" runat="server" Text="Back to Menu" OnClick="btnBack_Click" CssClass="aspNetButton" />

            <div class="grid-section">
                <asp:GridView ID="gvBills" runat="server" AutoGenerateColumns="true" CssClass="aspNetGrid" />
            </div>
        </asp:Panel>

    </form>
</body>
</html>

