using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKMail.Data.Entities;

namespace MailNotificationAPI.Abstract
{
    public interface iNotificationRepository
    {
        Response SendMail(TKMailNotificationAPI.Models.MailData mailData);
    }
}
