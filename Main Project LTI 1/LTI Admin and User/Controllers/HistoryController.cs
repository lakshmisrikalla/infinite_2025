using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTI.Models;
using LTI.Models.ViewModels;

namespace LTI.Controllers
{
    public class HistoryController : Controller
    {
        private readonly InsuranceDbContext _context;

        public HistoryController()
        {
            _context = new InsuranceDbContext();
        }

        // GET: History
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userId = Convert.ToInt32(Session["UserID"]);

            var user = _context.Users.FirstOrDefault(u => u.UserID == userId);

            // Premium Calculations
            var calculations = (from c in _context.PremiumCalculations
                                join u in _context.Users on c.UserID equals u.UserID
                                join pt in _context.PolicyTypes on c.PolicyTypeID equals pt.PolicyTypeID
                                where c.UserID == userId
                                orderby c.CalculatedAt descending
                                select new CalculationHistoryVM
                                {
                                    CalculationID = c.CalculationID,
                                    PolicyType = pt.TypeName,
                                    VehicleModel = c.VehicleModel,
                                    VehicleAge = c.VehicleAge,
                                    TravelDetails = c.TravelDetails,
                                    UserID = u.UserID,
                                    UserName = u.FullName,
                                    PhoneNumber = u.PhoneNumber,
                                    Email = u.LoginCredentials.Email,
                                    EstimatedPremium = c.EstimatedPremium,
                                    CalculatedAt = c.CalculatedAt
                                }).ToList();

            // Policies bought
            var policies = (from up in _context.UserPolicies
                            join p in _context.Policies on up.PolicyID equals p.PolicyID
                            where up.UserID == userId
                            orderby up.CreatedAt descending
                            select new PolicyHistoryVM
                            {
                                UserPolicyID = up.UserPolicyID,
                                PolicyName = p.PolicyName,
                                InsuranceType = up.InsuranceType,
                                Status = up.Status,
                                PaymentStatus = up.PaymentStatus,
                                StartDate = up.StartDate,
                                EndDate = up.EndDate
                            }).ToList();

            // Renewals
            var renewals = (from r in _context.Renewals
                            join up in _context.UserPolicies on r.UserPolicyID equals up.UserPolicyID
                            where up.UserID == userId
                            orderby r.RequestedAt descending
                            select new RenewalHistoryVM
                            {
                                RenewalID = r.RenewalID,
                                UserPolicyID = r.UserPolicyID,
                                Amount = r.Amount,
                                Status = r.Status,
                                RequestedAt = r.RequestedAt,
                                ApprovedAt = r.ApprovedAt
                            }).ToList();

            // Claims
            var claims = (from cl in _context.Claims
                          join up in _context.UserPolicies on cl.UserPolicyID equals up.UserPolicyID
                          where up.UserID == userId
                          orderby cl.ClaimDate descending
                          select new ClaimHistoryVM
                          {
                              ClaimID = cl.ClaimID,
                              UserPolicyID = cl.UserPolicyID,
                              ClaimType = cl.ClaimType,
                              Reason = cl.Reason,
                              ClaimedAmount = cl.ClaimedAmount,
                              ApprovedAmount = cl.ApprovedAmount,
                              Status = cl.Status,
                              ClaimDate = cl.ClaimDate
                          }).ToList();

            // Combine into one VM
            var viewModel = new UserHistoryVM
            {
                Calculations = calculations,
                Policies = policies,
                Renewals = renewals,
                Claims = claims
            };

            return View(viewModel);
        }
    }
}