using System;

namespace Client_model.ViewModels
{
    public class MessageVm
    {
        public int MessageID { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string Body { get; set; }
        public DateTime SentAt { get; set; }
    }
}
