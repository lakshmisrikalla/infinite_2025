using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
    public class AuditLogVm
    {
        public int AuditID { get; set; }
        public string AdminName { get; set; }
        public string ActionType { get; set; }
        public string TargetType { get; set; }
        public int? TargetID { get; set; }
        public string Details { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

