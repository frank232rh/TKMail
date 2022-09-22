using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TKMail.Data.Entities;

namespace TKMailNotificationAPI.Models
{
    public class MailData
    {
        public MailConfig config { get; set; }
        public TKMailNotificationAPI.Models.eMailMessage message { get; set; }
    }
}