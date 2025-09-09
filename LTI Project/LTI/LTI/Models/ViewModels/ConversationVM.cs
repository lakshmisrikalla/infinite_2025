using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models.ViewModels
{
    public class ConversationVM
    {
        public int ConversationID { get; set; }
        public int ClientID { get; set; }
        public string CompanyName { get; set; }
        public List<MessageItemVM> Messages { get; set; } = new List<MessageItemVM>();
    }

    public class MessageItemVM
    {
        public string SenderRole { get; set; }
        public string Body { get; set; }
        public DateTime SentAt { get; set; }
    }
}