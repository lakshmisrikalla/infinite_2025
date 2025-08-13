using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Assingment_ASP_1
{
  
        public partial class ProductViewer : System.Web.UI.Page
        {
            // Product data: name, image path, price
            private Dictionary<string, (string ImageUrl, string Price)> products = new Dictionary<string, (string, string)>
        {
            { "Dress", ("Dress.jpg", "Rs.5,500") },
            { "Furniture", ("Furniture.jpg", "Rs.1,50,000") },
            { "TeddyBear", ("TeddyBear.jpg", "Rs.1,100") },
            { "Handbag", ("HandBag.jpg", "Rs.3,500") }
        };

            protected void Page_Load(object sender, EventArgs e)
            {
                if (!IsPostBack)
                {
                    ddlProducts.Items.Clear();
                    ddlProducts.Items.Add("-- Select Product --");
                    foreach (var item in products.Keys)
                    {
                        ddlProducts.Items.Add(item);
                    }

                    imgProduct.ImageUrl = "";
                    lblPrice.Text = "";
                }
            }

            protected void ddlProducts_SelectedIndexChanged(object sender, EventArgs e)
            {
                string selected = ddlProducts.SelectedItem.Text;
                if (products.ContainsKey(selected))
                {
                    imgProduct.ImageUrl = products[selected].ImageUrl;
                    lblPrice.Text = ""; // Clear price until button is clicked
                }
                else
                {
                    imgProduct.ImageUrl = "";
                    lblPrice.Text = "";
                }
            }

            protected void btnGetPrice_Click(object sender, EventArgs e)
            {
                string selected = ddlProducts.SelectedItem.Text;
                if (products.ContainsKey(selected))
                {
                    lblPrice.Text = "Price: " + products[selected].Price;
                }
                else
                {
                    lblPrice.Text = "Please select a valid product.";
                }
            }
        }
    }

