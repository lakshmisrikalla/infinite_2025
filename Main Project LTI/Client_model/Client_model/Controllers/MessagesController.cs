using System.Linq;
using System.Web.Mvc;
using Client_model.Models;
using Client_model.ViewModels;

namespace Client_model.Controllers
{
    public class MessagesController : Controller
    {
        private readonly InsuranceDB1Entities _db = new InsuranceDB1Entities();

        public ActionResult Index()
        {
            // Replace with your actual method of retrieving the current client ID
            var cid = (int)Session["ClientID"]; // Example: get client ID from session

            var userMessages = (from m in _db.Messages
                                join u in _db.Users on m.SenderID equals u.UserID
                                where m.Conversation.ClientID == cid && m.SenderRole == "User"
                                orderby m.SentAt descending
                                select new MessageVm
                                {
                                    MessageID = m.MessageID,
                                    SenderName = u.FullName,
                                    SenderPhoneNumber = u.PhoneNumber,  // Phone number instead of email
                                    Body = m.Body,
                                    SentAt = m.SentAt
                                }).ToList();

            return View(userMessages);
        }
    }
}
