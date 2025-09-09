using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
        public class Message
        {
            public int MessageID { get; set; }
            public int ConversationID { get; set; }
            public string SenderRole { get; set; } // User or Client
            public int SenderID { get; set; }
            public string Body { get; set; }
            public DateTime SentAt { get; set; } = DateTime.UtcNow;
        }
}