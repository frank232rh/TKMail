using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKMail.NotificationService.Entities.Extensions;

namespace TKMail.NotificationService.MessageProcessors
{
    public class InboundMessageProcessor
    {
        public static void ProcessMessage(byte[] message)
        {
            Trace.WriteLine("InboundMessageProcessor Recieved Message");
            ("ProcessMessage Recieved Message").createLog("InboundMessageProcessor", "13");
            return;
        }

        public static void SaveFailedMessage(byte[] message, SqlConnection con, Exception errorInfo)
        {
            Trace.WriteLine("InboundMessageProcessor Recieved Failed Message");
            ("SaveFailedMessage Recieved Failed Message").createLog("InboundMessageProcessor", "20");
            return;
        }
    }
}
