using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
    public class EmailLog
    {
        public int EmailID { get; set; }
        public string RecipientType { get; set; } // "User" or "Client"
        public int RecipientID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string EmailType { get; set; }
        public DateTime SentAt { get; set; }
    }

}