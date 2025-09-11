using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
    public class AdminEmailModel
    {
        public string RecipientType { get; set; } // "User" or "Client"
        public string EmailType { get; set; }     // e.g., "Welcome Mail"
        public string ToEmail { get; set; }
        public int RecipientID { get; set; }
        public string CustomBody { get; set; }

    }

}