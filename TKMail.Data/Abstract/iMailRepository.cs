using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKMail.Data.Entities;

namespace TKMail.Data.Abstract
{
    public interface iMailRepository
    {
        MailConfig GetMailConfig(int idApp);
        Response AddMailNotification(eMailMessage eMail, int IdApp);
        eMailMessage GetMailNotification(int idMail);
        List<attach> GetAttach(int idMail);
        Response UpdateMail(int idMail, bool sent);
        List<eMailMessage> GetNotSentNotification();
        string GetGeneralConfiguration(string nameVariable);
    }
}
