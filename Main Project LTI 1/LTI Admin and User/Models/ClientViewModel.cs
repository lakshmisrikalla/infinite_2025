using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
    public class ClientViewModel
    {
        public int ClientID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Status { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string DocumentType { get; set; }
        public string FilePath { get; set; }
        public int DocumentID { get; set; }

    }
}