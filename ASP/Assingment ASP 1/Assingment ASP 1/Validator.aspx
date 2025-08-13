<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Validator.aspx.cs" Inherits="Assingment_ASP_1.Validator" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Validation Page</title>
    <style>
        .error { color: red; font-size: small; margin-left: 10px; }
        .summary-heading { font-weight: bold; color: red; margin-top: 20px; }
    </style>
    <script type="text/javascript">
        function validateZipCode(source, args) {
            var zip = args.Value;
            var regex = /^\d{5}$/;
            args.IsValid = zip.match(regex) !== null;
            if (!args.IsValid) {
                alert("Zip Code must be exactly 5 digits.");
            }
        }

        function validatePhone(source, args) {
            var phone = args.Value;
            var regex = /^\d{2}-\d{7}$|^\d{3}-\d{7}$/;
            args.IsValid = regex.test(phone);
            if (!args.IsValid) {
                alert("Phone must be in format XX-XXXXXXX or XXX-XXXXXXX.");
            }
        }

        function validateEmail(source, args) {
            var email = args.Value;
            var regex = /^[^@\s]+@[^@\s]+\.[^@\s]+$/;
            args.IsValid = email.match(regex) !== null;
            if (!args.IsValid) {
                alert("Please enter a valid email address.");
            }
        }

        function validateAddress(source, args) {
            var address = args.Value;
            args.IsValid = address.length >= 2;
            if (!args.IsValid) {
                alert("Address must be at least 2 characters.");
            }
        }

        function validateCity(source, args) {
            var city = args.Value;
            args.IsValid = city.length >= 2;
            if (!args.IsValid) {
                alert("City must be at least 2 characters.");
            }
        }

        function validateNameAndFamily(source, args) {
            var name = document.getElementById("Txtname").value.trim();
            var familyName = args.Value.trim();
            if (name === "" || familyName === "") {
                args.IsValid = false;
                alert("Name and Family Name cannot be blank.");
            } else if (name.toLowerCase() === familyName.toLowerCase()) {
                args.IsValid = false;
                alert("Name and Family Name must be different.");
            } else {
                args.IsValid = true;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="Label1" runat="server" Text="Insert your Details"></asp:Label><br /><br />

        Name:
        <asp:TextBox ID="Txtname" runat="server" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Txtname" ErrorMessage="* Must not be empty" CssClass="error" ValidationGroup="M" /><br />

        Family Name:
        <asp:TextBox ID="Txtfname" runat="server" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Txtfname" ErrorMessage="* Must not be empty" CssClass="error" ValidationGroup="M" />
        <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="Txtfname" ErrorMessage="* Must be different from Name" CssClass="error" OnServerValidate="CustomValidator1_ServerValidate" ValidationGroup="M" ClientValidationFunction="validateNameAndFamily" /><br />

        Address:
        <asp:TextBox ID="Txtaddr" runat="server" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="Txtaddr" ErrorMessage="* Must not be empty" CssClass="error" ValidationGroup="M" />
        <asp:CustomValidator ID="CustomValidator_Adr" runat="server" ControlToValidate="Txtaddr" ErrorMessage="* Must be at least 2 characters" CssClass="error" OnServerValidate="CustomValidator_Adr_ServerValidate" ValidationGroup="M" ClientValidationFunction="validateAddress" /><br />

        City:
        <asp:TextBox ID="txtcity" runat="server" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtcity" ErrorMessage="* Must not be empty" CssClass="error" ValidationGroup="M" />
        <asp:CustomValidator ID="CustomValidate_City" runat="server" ControlToValidate="txtcity" ErrorMessage="* Must be at least 2 characters" CssClass="error" OnServerValidate="CustomValidate_City_ServerValidate" ValidationGroup="M" ClientValidationFunction="validateCity" /><br />

        Zip Code:
        <asp:TextBox ID="txtzip" runat="server" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtzip" ErrorMessage="* Must not be empty" CssClass="error" ValidationGroup="M" />
        <asp:CustomValidator ID="CustomValidator_Zip" runat="server" ControlToValidate="txtzip" ErrorMessage="* Must be exactly 5 digits" CssClass="error" OnServerValidate="CustomValidator_Zip_ServerValidate" ValidationGroup="M" ClientValidationFunction="validateZipCode" /><br />

        Phone:
        <asp:TextBox ID="Txtphone" runat="server" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="Txtphone" ErrorMessage="* Must not be empty" CssClass="error" ValidationGroup="M" />
        <asp:CustomValidator ID="CustomValidator_Phone" runat="server" ControlToValidate="Txtphone" ErrorMessage="* Format: XX-XXXXXXX or XXX-XXXXXXX" CssClass="error" OnServerValidate="CustomValidator_Phone_ServerValidate" ValidationGroup="M" ClientValidationFunction="validatePhone" /><br />

        Email:
        <asp:TextBox ID="txtemail" runat="server" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtemail" ErrorMessage="* Must not be empty" CssClass="error" ValidationGroup="M" />
        <asp:CustomValidator ID="CustomValidator_email" runat="server" ControlToValidate="txtemail" ErrorMessage="* Must be a valid email format" CssClass="error" OnServerValidate="CustomValidator_email_ServerValidate" ValidationGroup="M" ClientValidationFunction="validateEmail" /><br /><br />

        <asp:Button ID="btncheck" runat="server" Text="Check" OnClick="btncheck_Click" ValidationGroup="M" /><br /><br />

        <asp:Label ID="SummaryHeading" runat="server" CssClass="summary-heading" Text="Validation Errors:" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ForeColor="Red" ValidationGroup="M" /><br />

        <asp:Label ID="txtmsg" runat="server" ForeColor="Green" />
    </form>
</body>
</html>

