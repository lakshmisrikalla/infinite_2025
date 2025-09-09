using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTI.Models
{
    public class Conversation
    {
        public int ConversationID { get; set; }
        public int UserID { get; set; }
        public int ClientID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}