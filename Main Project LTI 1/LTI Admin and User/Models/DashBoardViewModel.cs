using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
    public class AdminDashboardViewModel
    {
        public string Name { get; set; }
        public string LoginID { get; set; }
        public DateTime LoginTime { get; set; }
        public string Role { get; set; } = "Admin";
        public bool IsActive { get; set; } = true;
    }
}