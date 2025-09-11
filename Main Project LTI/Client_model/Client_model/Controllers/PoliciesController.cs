using System;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Client_model.Models;

namespace Client_model.Controllers
{
    public class PoliciesController : Controller
    {
        private readonly InsuranceDB1Entities _db = new InsuranceDB1Entities();

        private int? GetCurrentClientId()
        {
            try
            {
                var raw = Helpers.ClientContext.CurrentClientId;
                if (raw == null) return null;
                if (raw is int) return (int)raw;
                int parsed;
                if (int.TryParse(raw.ToString(), out parsed)) return parsed;
                return null;
            }
            catch
            {
                return null;
            }
        }

        // GET: Index
        public ActionResult Index()
        {
            var clientId = GetCurrentClientId();
            if (clientId == null) return RedirectToAction("Login", "Account");

            var policies = _db.Policies.Include(p => p.PolicyType)
                                      .Where(p => p.ClientID == clientId.Value)
                                      .OrderByDescending(p => p.PolicyID)
                                      .ToList();

            var vm = policies.Select(p => new PolicyListVm
            {
                PolicyID = p.PolicyID,
                PolicyName = p.PolicyName,
                PolicyTypeName = p.PolicyType != null ? p.PolicyType.TypeName : "—",
                PlanKind = p.PlanKind,
                BasePremium = p.BasePremium,
                Status = p.Status,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                CoverageDetails = p.CoverageDetails,
                Description = p.Description
            }).ToList();

            ViewBag.PolicyTypes = new SelectList(_db.PolicyTypes.OrderBy(t => t.TypeName), "PolicyTypeID", "TypeName");

            return View(vm);
        }

        // GET: Create
        public ActionResult Create()
        {
            ViewBag.PolicyTypes = new SelectList(_db.PolicyTypes.OrderBy(t => t.TypeName), "PolicyTypeID", "TypeName");
            return View(new PolicyCreateVm { DurationMonths = 12, PlanKind = "ThirdParty", IsActive = true });
        }

        // POST: Create
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(PolicyCreateVm vm, HttpPostedFileBase termsFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PolicyTypes = new SelectList(_db.PolicyTypes.OrderBy(t => t.TypeName), "PolicyTypeID", "TypeName", vm.PolicyTypeID);
                return View(vm);
            }

            var clientId = GetCurrentClientId();
            if (clientId == null) return RedirectToAction("Login", "Account");

            var policy = new Policy
            {
                PolicyName = vm.PolicyName,
                PolicyTypeID = vm.PolicyTypeID,
                PlanKind = string.IsNullOrWhiteSpace(vm.PlanKind) ? "ThirdParty" : vm.PlanKind,
                DurationMonths = vm.DurationMonths,
                BasePremium = vm.BasePremium,
                Status = "Approved",
                IsActive = vm.IsActive,
                ClientID = clientId.Value,
                CreatedAt = DateTime.UtcNow,
                CoverageDetails = vm.CoverageDetails,
                Description = vm.Description
            };

            _db.Policies.Add(policy);

            try
            {
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = "Validation failed: " + string.Join("; ", dbEx.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors).Select(e => e.PropertyName + ": " + e.ErrorMessage));
                ModelState.AddModelError("", msg);
                ViewBag.PolicyTypes = new SelectList(_db.PolicyTypes.OrderBy(t => t.TypeName), "PolicyTypeID", "TypeName", vm.PolicyTypeID);
                return View(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating policy: " + ex.Message);
                ViewBag.PolicyTypes = new SelectList(_db.PolicyTypes.OrderBy(t => t.TypeName), "PolicyTypeID", "TypeName", vm.PolicyTypeID);
                return View(vm);
            }

            // ************* IMPORTANT CHANGE: use OwnerType = "UserPolicy" (allowed by DB CHECK) *************
            // Save terms file (ownerType "UserPolicy" so CHECK constraint passes). DocumentType stays "Terms".
            var uploadRes = SaveDocumentFile(termsFile, "UserPolicy", policy.PolicyID, "Terms");
            if (!uploadRes.ok) TempData["DocError"] = uploadRes.error;

            TempData["Success"] = "Policy created successfully.";
            return RedirectToAction("Index");
        }

        // GET: Edit
        public ActionResult Edit(int id)
        {
            var p = _db.Policies.Find(id);
            if (p == null) return HttpNotFound();

            var clientId = GetCurrentClientId();
            if (clientId == null || p.ClientID != clientId.Value) return new HttpStatusCodeResult(403);

            var vm = new PolicyCreateVm
            {
                PolicyID = p.PolicyID,
                PolicyName = p.PolicyName,
                PolicyTypeID = p.PolicyTypeID,
                PlanKind = p.PlanKind,
                DurationMonths = p.DurationMonths,
                BasePremium = p.BasePremium,
                IsActive = p.IsActive,
                CoverageDetails = p.CoverageDetails,
                Description = p.Description
            };

            ViewBag.PolicyTypes = new SelectList(_db.PolicyTypes.OrderBy(t => t.TypeName), "PolicyTypeID", "TypeName", vm.PolicyTypeID);
            return View(vm);
        }

        // POST: Edit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(PolicyCreateVm vm, HttpPostedFileBase termsFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PolicyTypes = new SelectList(_db.PolicyTypes.OrderBy(t => t.TypeName), "PolicyTypeID", "TypeName", vm.PolicyTypeID);
                return View(vm);
            }

            var policy = _db.Policies.Find(vm.PolicyID);
            if (policy == null) return HttpNotFound();

            var clientId = GetCurrentClientId();
            if (clientId == null || policy.ClientID != clientId.Value) return new HttpStatusCodeResult(403);

            policy.PolicyName = vm.PolicyName;
            policy.PolicyTypeID = vm.PolicyTypeID;
            policy.PlanKind = string.IsNullOrWhiteSpace(vm.PlanKind) ? "ThirdParty" : vm.PlanKind;
            policy.DurationMonths = vm.DurationMonths;
            policy.BasePremium = vm.BasePremium;
            policy.IsActive = vm.IsActive;
            policy.CoverageDetails = vm.CoverageDetails;
            policy.Description = vm.Description;
            policy.UpdatedAt = DateTime.UtcNow;

            try
            {
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = "Validation failed: " + string.Join("; ", dbEx.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors).Select(e => e.PropertyName + ": " + e.ErrorMessage));
                ModelState.AddModelError("", msg);
                ViewBag.PolicyTypes = new SelectList(_db.PolicyTypes.OrderBy(t => t.TypeName), "PolicyTypeID", "TypeName", vm.PolicyTypeID);
                return View(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating policy: " + ex.Message);
                ViewBag.PolicyTypes = new SelectList(_db.PolicyTypes.OrderBy(t => t.TypeName), "PolicyTypeID", "TypeName", vm.PolicyTypeID);
                return View(vm);
            }

            // ************* IMPORTANT CHANGE: use OwnerType = "UserPolicy" here too *************
            var upRes = SaveDocumentFile(termsFile, "UserPolicy", policy.PolicyID, "Terms");
            if (!upRes.ok) TempData["DocError"] = upRes.error;

            TempData["Success"] = "Policy updated successfully.";
            return RedirectToAction("Index");
        }

        // GET: Details
        public ActionResult Details(int id)
        {
            var clientId = GetCurrentClientId();
            if (clientId == null) return RedirectToAction("Login", "Account");

            var policy = _db.Policies.Include(p => p.PolicyType).FirstOrDefault(p => p.PolicyID == id && p.ClientID == clientId.Value);
            if (policy == null) return HttpNotFound();

            ViewBag.TermsDoc = _db.Documents.FirstOrDefault(d => d.OwnerType == "UserPolicy" && d.OwnerID == id && d.DocumentType == "Terms");
            return View(policy);
        }

        // Toggle Active - AJAX button
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ToggleActive(int id, bool isActive)
        {
            try
            {
                var clientId = GetCurrentClientId();
                if (clientId == null) return Json(new { ok = false, error = "not_logged_in" });

                var policy = _db.Policies.Find(id);
                if (policy == null) return Json(new { ok = false, error = "not_found" });
                if (policy.ClientID != clientId.Value) return Json(new { ok = false, error = "forbidden" });

                policy.IsActive = isActive;
                policy.UpdatedAt = DateTime.UtcNow;
                _db.SaveChanges();

                return Json(new { ok = true, isActive = policy.IsActive });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ToggleActive exception: " + ex);
                return Json(new { ok = false, error = "exception", message = ex.Message });
            }
        }

        // Download a document file (streams file to browser)
        // Download a document file (streams file to browser) - defensive version
        public ActionResult DownloadDocument(int id)
        {
            try
            {
                var doc = _db.Documents.Find(id);
                if (doc == null)
                {
                    System.Diagnostics.Debug.WriteLine($"DownloadDocument: Document id={id} not found in DB.");
                    return HttpNotFound("Document record not found.");
                }

                // Determine physical path from stored FilePath (we stored virtual path like "~/App_Data/Uploads/xxx")
                var fileVirtual = doc.FilePath;
                if (string.IsNullOrWhiteSpace(fileVirtual))
                {
                    System.Diagnostics.Debug.WriteLine($"DownloadDocument: Document id={id} has empty FilePath.");
                    return HttpNotFound("Document has no stored file path.");
                }

                string filePhysical;
                try
                {
                    filePhysical = Server.MapPath(fileVirtual);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DownloadDocument: Server.MapPath failed for '{fileVirtual}' - {ex}");
                    return HttpNotFound("Server cannot map file path.");
                }

                if (!System.IO.File.Exists(filePhysical))
                {
                    System.Diagnostics.Debug.WriteLine($"DownloadDocument: file not found on disk: {filePhysical}");
                    return HttpNotFound("File not found on disk.");
                }

                // Determine a safe filename to send to client
                string fileNameToSend = null;
                try
                {
                    if (!string.IsNullOrWhiteSpace(doc.FileName))
                    {
                        fileNameToSend = doc.FileName;
                    }
                    else
                    {
                        // get name from stored path (virtual or physical)
                        fileNameToSend = Path.GetFileName(filePhysical) ?? $"document_{id}";
                    }

                    // Ensure filename is not empty
                    if (string.IsNullOrWhiteSpace(fileNameToSend))
                        fileNameToSend = $"document_{id}";
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DownloadDocument: error deriving filename for id={id} - {ex}");
                    fileNameToSend = $"document_{id}";
                }

                // Determine mime type. Guard against null by using file extension if needed
                string contentType = "application/octet-stream";
                try
                {
                    // MimeMapping requires a non-null filename
                    contentType = MimeMapping.GetMimeMapping(fileNameToSend);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DownloadDocument: MimeMapping failed for '{fileNameToSend}' - {ex}");
                    contentType = "application/octet-stream";
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePhysical);
                return File(fileBytes, contentType, fileNameToSend);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DownloadDocument unhandled exception: " + ex);
                return new HttpStatusCodeResult(500, "Unexpected server error while downloading file.");
            }
        }


        // Save uploaded file and register Document
        private (bool ok, string error) SaveDocumentFile(HttpPostedFileBase file, string ownerType, int ownerId, string docType)
        {
            if (file == null || file.ContentLength == 0) return (true, null);

            const int maxBytes = 5 * 1024 * 1024; // 5MB
            if (file.ContentLength > maxBytes) return (false, "File too large (max 5MB).");

            var allowed = new[] { ".pdf", ".doc", ".docx" };
            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !allowed.Contains(ext)) return (false, "Invalid file type. Allowed: PDF, DOC, DOCX.");

            // Ensure uploads folder exists
            var uploadsPhysical = Server.MapPath("~/App_Data/Uploads");
            if (string.IsNullOrEmpty(uploadsPhysical)) return (false, "Server map path failed.");

            try
            {
                if (!Directory.Exists(uploadsPhysical)) Directory.CreateDirectory(uploadsPhysical);
            }
            catch (Exception ex)
            {
                var msg = "Failed to create uploads folder: " + ex.Message;
                System.Diagnostics.Debug.WriteLine(msg);
                return (false, msg);
            }

            // test writability
            try
            {
                var tempPath = Path.Combine(uploadsPhysical, "._writetest_" + Guid.NewGuid().ToString("N") + ".tmp");
                System.IO.File.WriteAllText(tempPath, "test");
                System.IO.File.Delete(tempPath);
            }
            catch (Exception ex)
            {
                var msg = "Uploads folder is not writable by the app. Ensure IIS user has write permissions. (" + ex.Message + ")";
                System.Diagnostics.Debug.WriteLine(msg);
                return (false, msg);
            }

            // create safe filename and relative DB path
            var origName = Path.GetFileName(file.FileName);
            var safeName = MakeSafeFileName(origName);
            if (safeName.Length > 200) safeName = safeName.Substring(0, 200);

            var unique = Guid.NewGuid().ToString("N") + "_" + safeName;
            var savedPhysical = Path.Combine(uploadsPhysical, unique);

            try
            {
                file.SaveAs(savedPhysical);
            }
            catch (Exception ex)
            {
                var msg = "Failed to save file to disk: " + ex.Message;
                System.Diagnostics.Debug.WriteLine(msg);
                return (false, msg);
            }

            var virtualPath = "~/App_Data/Uploads/" + unique;
            if (virtualPath.Length > 500) virtualPath = virtualPath.Substring(0, 500);

            // create Document record using ownerType parameter that must match DB CHECK
            var doc = new Document
            {
                OwnerType = ownerType,   // MUST be one of ('Client','User','UserPolicy') per DB constraint
                OwnerID = ownerId,
                DocumentType = docType,
                FilePath = virtualPath,
                FileName = safeName,
                UploadedAt = DateTime.UtcNow,
                Visibility = "ClientOnly",
                IsVerified = false
            };

            _db.Documents.Add(doc);

            try
            {
                _db.SaveChanges();
                return (true, null);
            }
            catch (DbEntityValidationException dbValEx)
            {
                var validationMsg = string.Join("; ", dbValEx.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => e.PropertyName + ": " + e.ErrorMessage));
                System.Diagnostics.Debug.WriteLine("EF validation error saving Document: " + validationMsg);
                TryDeleteFile(savedPhysical);
                return (false, "Database validation error: " + validationMsg);
            }
            catch (DbUpdateException dbUpdEx)
            {
                var inner = dbUpdEx.InnerException;
                while (inner != null && inner.InnerException != null) inner = inner.InnerException;
                var innerMsg = inner != null ? inner.Message : dbUpdEx.Message;
                System.Diagnostics.Debug.WriteLine("DbUpdateException saving Document: " + innerMsg);
                TryDeleteFile(savedPhysical);
                return (false, "Database error saving document: " + innerMsg);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unknown error saving Document: " + ex);
                TryDeleteFile(savedPhysical);
                return (false, "Unexpected error saving document: " + ex.Message);
            }
        }

        private static string MakeSafeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
            return name;
        }

        private void TryDeleteFile(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to delete file: " + ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
