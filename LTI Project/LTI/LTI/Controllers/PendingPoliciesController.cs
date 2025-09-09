using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTI.Models;
using LTI.Models.ViewModels;

namespace LTI.Controllers
{
    public class PendingPoliciesController : Controller
    {
        private InsuranceDbContext _context = new InsuranceDbContext();
        // GET: PendingPolicies
        public ActionResult Index()
        {
            int userId = 1; // TODO: replace with session UserID later

            var pendingPolicies = (from up in _context.UserPolicies

                                   join p in _context.Policies on up.PolicyID equals p.PolicyID

                                   where up.UserID == userId &&

                                   (up.Status == "Pending" || up.PaymentStatus == "Unpaid")

                                   select new PendingPolicyVM

                                   {

                                       UserPolicyID = up.UserPolicyID,

                                       PolicyName = p.PolicyName,

                                       InsuranceType = up.InsuranceType,

                                       Status = up.Status,

                                       PaymentStatus = up.PaymentStatus,

                                       StartDate = up.StartDate,

                                       EndDate = up.EndDate,

                                       BasePremium = p.BasePremium

                                   }).ToList();
            return View(pendingPolicies);
        }
    }
}