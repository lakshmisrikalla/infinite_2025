using System;
using System.Linq;
using System.Web.Mvc;
using Client_model.Helpers;
using Client_model.Models;

namespace Client_model.Controllers
{
    public class ApprovalsController : Controller
    {
        private readonly InsuranceDB1Entities _db = new InsuranceDB1Entities();

        public ActionResult Index()
        {
            var clientId = ClientContext.CurrentClientId;
            if (clientId == null) return RedirectToAction("Login", "Account");

            var q = _db.UserPolicies
                .Where(up => up.Policy.ClientID == clientId && up.Status == "Pending")
                .OrderByDescending(up => up.CreatedAt)
                .Select(up => new
                {
                    up.UserPolicyID,
                    Policy = up.Policy.PolicyName,
                    Holder = up.User.FullName,
                    up.Status,
                    up.PaymentStatus,
                    up.StartDate,
                    up.EndDate,
                    ApprovalID = up.UserPolicyApprovals.Select(a => a.UserPolicyApprovalID).FirstOrDefault(),
                    Decision = up.UserPolicyApprovals.Select(a => a.Decision).FirstOrDefault(),
                    Notes = up.UserPolicyApprovals.Select(a => a.Notes).FirstOrDefault()
                })
                .ToList();

            var vmList = q.Select(x => new PolicyApprovalVm
            {
                UserPolicyID = x.UserPolicyID,
                Policy = x.Policy,
                Holder = x.Holder,
                Status = x.Status,
                PaymentStatus = x.PaymentStatus,
                StartDate = (DateTime?)x.StartDate,
                EndDate = (DateTime?)x.EndDate,
                ApprovalID = x.ApprovalID == 0 ? (int?)null : x.ApprovalID,
                Decision = string.IsNullOrEmpty(x.Decision) ? "NotSet" : x.Decision,
                Notes = x.Notes ?? ""
            }).ToList();

            return View(vmList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(int userPolicyId, string decision = "Approved", string notes = null)
        {
            var clientId = ClientContext.CurrentClientId;
            if (clientId == null)
            {
                TempData["Error"] = "You must be logged in as a client to approve.";
                return RedirectToAction("Login", "Account");
            }
            if (userPolicyId <= 0)
            {
                TempData["Error"] = "Invalid request.";
                return RedirectToAction("Index");
            }

            var up = _db.UserPolicies
                .Where(u => u.UserPolicyID == userPolicyId)
                .Select(u => new { u.UserPolicyID, PolicyClientID = u.Policy.ClientID })
                .FirstOrDefault();

            if (up == null)
            {
                TempData["Error"] = "User policy not found.";
                return RedirectToAction("Index");
            }
            if (up.PolicyClientID != clientId.Value)
            {
                TempData["Error"] = "You are not authorized to approve this policy.";
                return RedirectToAction("Index");
            }

            try
            {
                var approval = _db.UserPolicyApprovals.FirstOrDefault(a => a.UserPolicyID == userPolicyId);
                if (approval == null)
                {
                    approval = new UserPolicyApproval
                    {
                        UserPolicyID = userPolicyId,
                        ClientID = clientId.Value,
                        Decision = decision,
                        DecisionAt = DateTime.UtcNow,
                        Notes = notes ?? $"Decision '{decision}' by client {clientId.Value} on {DateTime.UtcNow:u}"
                    };
                    _db.UserPolicyApprovals.Add(approval);
                }
                else
                {
                    approval.Decision = decision;
                    approval.DecisionAt = DateTime.UtcNow;
                    approval.Notes = string.IsNullOrWhiteSpace(approval.Notes)
                        ? (notes ?? $"Decision '{decision}' by client {clientId.Value} on {DateTime.UtcNow:u}")
                        : approval.Notes + Environment.NewLine + (notes ?? $"Decision '{decision}' by client {clientId.Value} on {DateTime.UtcNow:u}");
                    _db.Entry(approval).State = System.Data.Entity.EntityState.Modified;
                }

                _db.SaveChanges();
                TempData["Success"] = $"Policy {decision.ToLower()} successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to save decision. " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
