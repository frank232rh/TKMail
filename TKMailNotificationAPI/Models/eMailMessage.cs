using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TKMailNotificationAPI.Models
{

    public class eMailMessage
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string MailFrom { get; set; }
        public string MailTo { get; set; }
        public string TemplateHTML { get; set; }
        public DateTime TSInserted { get; set; }
        public DateTime TSSent { get; set; }
        public bool Sent { get; set; }
        public int IdApp { get; set; }
        public List<attach> Attachments { get; set; }
    }
    public class attach
    {
        public int IdNotificationMail { get; set; }
        public byte[] File { get; set; }
        public string FileName { get; set; }
    }
}