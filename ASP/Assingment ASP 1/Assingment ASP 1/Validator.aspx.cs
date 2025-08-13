using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Text;

namespace Assingment_ASP_1
{
    public partial class Validator : System.Web.UI.Page
        {
            protected void Page_Load(object sender, EventArgs e)
            {
                Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
                txtmsg.Text = "";
            }

            protected void btncheck_Click(object sender, EventArgs e)
            {
                // Manually trigger validation
                Page.Validate("M");

                List<string> errorFields = new List<string>();

                if (!RequiredFieldValidator1.IsValid) errorFields.Add("Name");
                if (!RequiredFieldValidator2.IsValid || !CustomValidator1.IsValid) errorFields.Add("Family Name");
                if (!RequiredFieldValidator3.IsValid || !CustomValidator_Adr.IsValid) errorFields.Add("Address");
                if (!RequiredFieldValidator4.IsValid || !CustomValidate_City.IsValid) errorFields.Add("City");
                if (!RequiredFieldValidator5.IsValid || !CustomValidator_Zip.IsValid) errorFields.Add("Zip Code");
                if (!RequiredFieldValidator6.IsValid || !CustomValidator_Phone.IsValid) errorFields.Add("Phone");
                if (!RequiredFieldValidator7.IsValid || !CustomValidator_email.IsValid) errorFields.Add("Email");

                if (errorFields.Count == 0)
                {
                    txtmsg.Text = "All inputs are valid.";
                }
                else
                {
                    string alertMessage = "Please correct the following fields:\\n";
                    foreach (string field in errorFields)
                    {
                        alertMessage += "• " + field + "\\n";
                    }

                    // Show alert box from localhost
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{alertMessage}');", true);

                    // Also show in label below the button
                    txtmsg.Text = "Validation failed for: " + string.Join(", ", errorFields);
                }
            }

            protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
            {
                string name = Txtname.Text.Trim();
                string family = Txtfname.Text.Trim();
                args.IsValid = !string.IsNullOrEmpty(name) &&
                               !string.IsNullOrEmpty(family) &&
                               !name.Equals(family, StringComparison.OrdinalIgnoreCase);
            }

            protected void CustomValidator_Adr_ServerValidate(object source, ServerValidateEventArgs args)
            {
                args.IsValid = args.Value.Trim().Length >= 2;
            }

            protected void CustomValidate_City_ServerValidate(object source, ServerValidateEventArgs args)
            {
                args.IsValid = args.Value.Trim().Length >= 2;
            }

            protected void CustomValidator_Zip_ServerValidate(object source, ServerValidateEventArgs args)
            {
                args.IsValid = Regex.IsMatch(args.Value.Trim(), @"^\d{5}$");
            }

            protected void CustomValidator_Phone_ServerValidate(object source, ServerValidateEventArgs args)
            {
                args.IsValid = Regex.IsMatch(args.Value.Trim(), @"^\d{2}-\d{7}$|^\d{3}-\d{7}$");
            }

            protected void CustomValidator_email_ServerValidate(object source, ServerValidateEventArgs args)
            {
                args.IsValid = Regex.IsMatch(args.Value.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            }
        }
    }
