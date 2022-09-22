using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKMail.Data.Entities
{
    public class MailData
    {
        public MailConfig config { get; set; }
        public eMailMessage message { get; set; }
    }
}
