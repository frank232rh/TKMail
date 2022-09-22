using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKMail.NotificationService.Entities
{
    public class QueueListenerConfig
    {
        public string QueueName { get; set; }
        public int Threads { get; set; }
        public bool EnlistMessageProcessor { get; set; }
        public Action<byte[]> MessageProcessor { get; set; }
        public Action<byte[], SqlConnection, Exception> FailedMessageProcessor { get; set; }
        public string ConnectionString { get; set; }
    }
}
