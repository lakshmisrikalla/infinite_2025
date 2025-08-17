using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



namespace Electricity_Project
{

        public partial class Main : System.Web.UI.Page
    { 
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminLoggedIn"] == null || !(bool)Session["AdminLoggedIn"])
            {
                Response.Redirect("AdminLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ShowPanel("menu");
                
            }
        }

        private int TotalBillsToAdd
            {
                get => Session["TotalBillsToAdd"] != null ? (int)Session["TotalBillsToAdd"] : 0;
                set => Session["TotalBillsToAdd"] = value;
            }

            private int CurrentBillIndex
            {
                get => Session["CurrentBillIndex"] != null ? (int)Session["CurrentBillIndex"] : 0;
                set => Session["CurrentBillIndex"] = value;
            }

            private List<ElectricityBill> BillsList
            {
                get
                {
                    if (Session["BillsList"] == null)
                        Session["BillsList"] = new List<ElectricityBill>();
                    return (List<ElectricityBill>)Session["BillsList"];
                }
                set => Session["BillsList"] = value;
            }



        private void ShowPanel(string panel)
        {
            pnlMainMenu.Visible = panel == "menu";
            pnlEntry.Visible = panel == "entry";
            pnlRetrieve.Visible = panel == "retrieve";
            pnlSummary.Visible = panel == "summary";
            pnlSearchConsumer.Visible = panel == "search";
        }


        protected void btnGoToEntry_Click(object sender, EventArgs e)
            {
                ShowPanel("entry");
            }

            protected void btnGoToRetrieve_Click(object sender, EventArgs e)
            {
                ShowPanel("retrieve");
            }
        protected void btnGoToSearchConsumer_Click(object sender, EventArgs e)
        {
            ShowPanel("search");
        }

        protected void btnBack_Click(object sender, EventArgs e)
            {
                ShowPanel("menu");
            }

            protected void btnStartEntry_Click(object sender, EventArgs e)
            {
                if (int.TryParse(txtBillCount.Text.Trim(), out int count) && count > 0)
                {
                    TotalBillsToAdd = count;
                    CurrentBillIndex = 0;
                    BillsList = new List<ElectricityBill>();
                    ShowPanel("entry");
                    Response.Write($"<br/>Enter details for bill {CurrentBillIndex + 1} of {TotalBillsToAdd}");
                }
                else
                {
                    Response.Write("Please enter a valid number of bills.");
                }
            }

            protected void btnSubmitBill_Click(object sender, EventArgs e)
            {
                try
                {
                    ElectricityBill eb = new ElectricityBill();
                    eb.ConsumerNumber = txtConsumerNumber.Text.Trim();
                    eb.ConsumerName = txtConsumerName.Text.Trim();

                    if (!int.TryParse(txtUnitsConsumed.Text.Trim(), out int units))
                    {
                        Response.Write("Units must be a valid number.");
                        return;
                    }

                    try
                    {
                        eb.UnitsConsumed = units;
                    }
                catch (InvalidOperationException ex)
                {
                    Response.Write("<br/> " + ex.Message);
                }



                ElectricityValuation valuation = new ElectricityValuation();
                    valuation.CalculateBill(eb);
                    valuation.AddBill(eb);

                    BillsList.Add(eb);

                    Response.Write($"<br/>Bill Added: {eb.ConsumerNumber} {eb.ConsumerName} {eb.UnitsConsumed} ₹{eb.BillAmount}");

                    CurrentBillIndex++;
                    if (CurrentBillIndex >= TotalBillsToAdd)
                    {
                        ShowPanel("summary");

                        string summaryHtml = "<h3> All bills entered successfully!</h3><ul>";
                        foreach (var bill in BillsList)
                        {
                            summaryHtml += $"<li>{bill.ConsumerNumber} - {bill.ConsumerName} - Units: {bill.UnitsConsumed} - ₹{bill.BillAmount}</li>";
                        }
                        summaryHtml += "</ul>";
                        litSummary.Text = summaryHtml;
                    }
                    else
                    {
                        Response.Write($"<br/>Enter details for bill {CurrentBillIndex + 1} of {TotalBillsToAdd}");
                        txtConsumerNumber.Text = "";
                        txtConsumerName.Text = "";
                        txtUnitsConsumed.Text = "";
                    }
                }
                catch (FormatException ex)
                {
                    Response.Write("<br/>" + ex.Message);
                }
                catch (Exception ex)
                {
                    Response.Write("<br/>Unexpected error: " + ex.GetType().Name + " - " + ex.Message);
                }
            }

            protected void btnRetrieve_Click(object sender, EventArgs e)
            {
                try
                {
                    if (!int.TryParse(txtLastN.Text.Trim(), out int n) || n <= 0)
                    {
                        Response.Write("Please enter a valid number for N.");
                        return;
                    }

                    ElectricityValuation valuation = new ElectricityValuation();
                    List<ElectricityBill> bills = valuation.Generate_N_BillDetails(n);

                    gvBills.DataSource = bills;
                    gvBills.DataBind();
                }
                catch (Exception ex)
                {
                    Response.Write("<br/>Error retrieving bills: " + ex.Message);
                }
            }

        protected void btnSearchConsumer_Click(object sender, EventArgs e)
        {
            string consumerNumber = txtSearchConsumerNumber.Text.Trim();
            if (string.IsNullOrEmpty(consumerNumber))
            {
                Response.Write("Please enter a consumer number.");
                return;
            }

            ElectricityValuation valuation = new ElectricityValuation();
            List<ElectricityBill> bills = valuation.GetBillsByConsumerNumber(consumerNumber, out string consumerName);

            if (bills.Count == 0)
            {
                lblConsumerName.Text = "No bills found for this consumer number.";
                gvConsumerBills.DataSource = null;
                gvConsumerBills.DataBind();
            }
            else
            {
                lblConsumerName.Text = $"Consumer Name: {consumerName}";
                gvConsumerBills.DataSource = bills;
                gvConsumerBills.DataBind();
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
            {
                Session.Abandon();
                Response.Redirect("AdminLogin.aspx");
            }
        }
    }