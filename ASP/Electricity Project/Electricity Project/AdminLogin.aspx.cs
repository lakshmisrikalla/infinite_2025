using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Electricity_Project
{
    public partial class AdminLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = "";
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (username == "lucky" && password == "lucky@123")
            {
                Session["AdminLoggedIn"] = true;
                Response.Redirect("Main.aspx");
            }
            else
            {
                lblError.Text = "Invalid login credentials.";
            }
        }
    }
}