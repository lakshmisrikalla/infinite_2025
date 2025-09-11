using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LTI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    namespace LTI.Models
    {
        public class UserPolicy
        {
            [Key]
            public int UserPolicyID { get; set; }

            public int UserID { get; set; }
            public int PolicyID { get; set; }

            public string InsuranceType { get; set; }
            public string Status { get; set; }
            public string PaymentStatus { get; set; }

            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }

            // ✅ Add these two properties to resolve your errors
            public decimal BasePremium { get; set; }
            public int DurationMonths { get; set; }

            // Optional navigation properties
            public virtual Policy Policy { get; set; }
          
        }
    }

}