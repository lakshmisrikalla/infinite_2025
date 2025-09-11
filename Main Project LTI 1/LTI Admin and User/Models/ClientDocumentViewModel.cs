using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
    public class ClientDocumentViewModel
    {
        public int OwnerID { get; set; }
        public string CompanyName { get; set; }
        public int? DocumentID { get; set; }
        public string DocumentType { get; set; }
        public string FilePath { get; set; }
    }
}