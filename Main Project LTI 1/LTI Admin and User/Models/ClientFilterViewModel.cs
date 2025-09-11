using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
    public class ClientFilterViewModel
    {
        public string Status { get; set; }
        public bool? IsVerified { get; set; }
        public DateTime? RegisteredAfter { get; set; }
        public List<ClientViewModel> FilteredClients { get; set; }
    }

}