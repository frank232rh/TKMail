using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKMail.Data.Entities
{
    public class MailConfig
    {
        public int Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool IsEnableSSL { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string Subject { get; set; }
        public int IdApp { get; set; }
    }
}
