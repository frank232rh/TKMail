using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKMail.Data.Entities;

namespace TKMailNotificationAPI.Abstract
{
    public interface iNotificationRepository
    {
        int SendMail(eMailMessage eMailMessage, int idApp);
    }
}
