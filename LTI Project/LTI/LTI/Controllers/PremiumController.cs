using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTI.Models;
using LTI.Models.ViewModels;

namespace LTI.Controllers
{
    public class PremiumController : Controller
    {
        private readonly InsuranceDbContext _context = new InsuranceDbContext();

        // GET: User/Calculate (show available plans)
        public ActionResult Calculate()
        {
            var plans = _context.Policies
                      .Where(p => p.Status == "Approved")
                      .Join(_context.PolicyTypes,
                            policy => policy.PolicyTypeID,
                            type => type.PolicyTypeID,
                            (policy, type) => new { policy, type })
                      .ToList();

            var model = new PremiumCalculationVM
            {
                AvailablePlans = plans.Select(x => new Policy
                {
                    PolicyID = x.policy.PolicyID,
                    PolicyName = x.policy.PolicyName + " (" + x.type.TypeName + ")",
                    PolicyTypeID = x.type.PolicyTypeID
                }).ToList()
            };

            ViewBag.Count = plans.Count;
            return View(model);
        }

        // POST: User/CalculatePremium
        [HttpPost]
        public ActionResult CalculatePremium(PremiumCalculationVM model)
        {
            var plan = _context.Policies.FirstOrDefault(p => p.PolicyID == model.PlanID);
            if (plan == null)
            {
                ModelState.AddModelError("", "Plan not found.");
                return RedirectToAction("Calculate");
            }

            var policyType = _context.PolicyTypes.FirstOrDefault(pt => pt.PolicyTypeID == plan.PolicyTypeID);

            decimal premium = plan.BasePremium;
            var breakdown = new List<string>();
            breakdown.Add($"Base premium: {plan.BasePremium}");

            // ---------------- MOTOR INSURANCE ----------------
            if (policyType != null && policyType.TypeName.Equals("Motor", StringComparison.OrdinalIgnoreCase))
            {
                if (model.VehicleAge.HasValue)
                {
                    if (model.VehicleAge.Value <= 3)
                    {
                        premium += 1000;
                        breakdown.Add("Vehicle age ≤ 3 years → +1000");
                    }
                    else if (model.VehicleAge.Value > 5)
                    {
                        premium -= 500;
                        breakdown.Add("Vehicle age > 5 years → -500");
                    }
                }

                if (plan.DurationMonths == 36)
                {
                    premium = premium * 2.5m;
                    breakdown.Add("3-year plan selected → premium × 2.5 (long-term benefit)");
                }
            }

            // ---------------- TRAVEL INSURANCE ----------------
            else if (policyType != null && policyType.TypeName.Equals("Travel", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(model.TravelDetails) &&
                    model.TravelDetails.ToLower().Contains("international"))
                {
                    premium += 2000;
                    breakdown.Add("International trip → +2000");
                }

                if (model.TripDurationDetails.HasValue && model.TripDurationDetails.Value > 15)
                {
                    premium += 2000;
                    breakdown.Add("Trip longer than 15 days → +2000");
                }

                if (model.TravellerAge.HasValue && model.TravellerAge.Value >= 60)
                {
                    premium += 3000;
                    breakdown.Add("Traveller age ≥ 60 → +3000");
                }
            }

            // ---------------- SAVE CALCULATION ----------------
            var calc = new PremiumCalculation
            {
                UserID = 1, // TODO: replace with Session["UserID"] when login is ready
                PolicyTypeID = plan.PolicyTypeID,
                VehicleModel = model.VehicleModel,
                VehicleAge = model.VehicleAge,
                TravelDetails = model.TravelDetails,
                EstimatedPremium = premium,
                CalculatedAt = DateTime.UtcNow
            };

            _context.PremiumCalculations.Add(calc);
            _context.SaveChanges();

            // ---------------- SEND RESULT TO VIEW ----------------
            ViewBag.PolicyName = plan.PolicyName;
            ViewBag.PolicyType = policyType?.TypeName;
            ViewBag.BasePremium = plan.BasePremium;
            ViewBag.FinalPremium = premium;
            ViewBag.Breakdown = breakdown;

            return View("PremiumResult");
        }

    }
}