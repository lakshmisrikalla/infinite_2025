using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
    public class ManageUsersFilterVm
    {
        public string Search { get; set; }
        public string Status { get; set; } = "All";
        public string Blacklist { get; set; } = "All";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int Total { get; set; }
        public List<UserListItemViewModel> Items { get; set; } = new List<UserListItemViewModel>();
    }

    public class UserListItemViewModel
    {
        public int LoginID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public bool Blacklisted { get; set; }
    }
}


