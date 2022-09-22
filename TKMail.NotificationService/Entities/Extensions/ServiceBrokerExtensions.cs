using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKMail.NotificationService.Entities.Extensions
{
    public static class ServiceBrokerExtensions
    {
        public static byte[] GetMessage(this SqlConnection con, string queueName, TimeSpan timeout)
        {
            try
            {
                using (SqlDataReader r = con.GetMessageBatch(queueName, timeout, 1))
                {
                    if (r == null || !r.HasRows)
                        return null;
                    r.Read();
                    Guid conversation_handle = r.GetGuid(r.GetOrdinal("conversation_handle"));
                    string messageType = r.GetString(r.GetOrdinal("message_type_name"));
                    if (messageType == "http://schemas.microsoft.com/SQL/ServiceBroker/EndDialog")
                    {
                        con.EndConversation(conversation_handle);
                        return null;
                    }
                    var body = r.GetSqlBinary(r.GetOrdinal("message_body"));
                    con.EndConversationWithCleanUp(conversation_handle);
                    r.Close();
                    return body.Value;
                }
            }
            catch (Exception ex)
            {
                ex.Message.createLog("ServiceBrokerExtensions GetMessage", "30");
                return null;
            }
        }
        internal static void EndConversation(this SqlConnection con, Guid conversationHandle)
        {
            try
            {
                string SQL = "END CONVERSATION @ConversationHandle;";
                using (SqlCommand cmd = new SqlCommand(SQL, con))
                {
                    SqlParameter pConversation = cmd.Parameters.Add("@ConversationHandle", SqlDbType.UniqueIdentifier);
                    pConversation.Value = conversationHandle;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                ex.Message.createLog("ServiceBrokerExtensions EndConversation", "49");
            }
            catch (Exception ex2)
            {
                ex2.Message.createLog("ServiceBrokerExtensions EndConversation", "53");
            }
        }
        internal static void EndConversationWithCleanUp(this SqlConnection con, Guid conversationHandle)
        {
            try
            {
                string SQL = "END CONVERSATION @ConversationHandle WITH CLEANUP;";
                using (SqlCommand cmd = new SqlCommand(SQL, con))
                {
                    SqlParameter pConversation = cmd.Parameters.Add("@ConversationHandle", SqlDbType.UniqueIdentifier);
                    pConversation.Value = conversationHandle;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                ex.Message.createLog("ServiceBrokerExtensions EndConversationWithCleanUp", "70");
            }
            catch (Exception ex2)
            {
                ex2.Message.createLog("ServiceBrokerExtensions EndConversationWithCleanUp", "74");
            }
        }
        /// <summary>
        /// This is the method that actually receives Service Broker messages.
        /// </summary>
        /// <param name="timeout">Maximum time to wait for a message.  This is passed to the RECIEVE command, not used as a SqlCommand.CommandTimeout</param>
        /// <returns></returns>
        static SqlDataReader GetMessageBatch(this SqlConnection con, string queueName, TimeSpan timeout, int maxMessages)
        {
            try
            {
                string SQL = string.Format(@" waitfor(RECEIVE top (@count) conversation_handle,service_name,message_type_name,message_body,message_sequence_number FROM [{0}]), timeout @timeout", queueName);
                SqlCommand cmd = new SqlCommand(SQL, con);
                cmd = new SqlCommand(SQL, con);
                SqlParameter pCount = cmd.Parameters.Add("@count", SqlDbType.Int);
                pCount.Value = maxMessages;
                SqlParameter pTimeout = cmd.Parameters.Add("@timeout", SqlDbType.Int);
                if (timeout == TimeSpan.MaxValue)
                {
                    pTimeout.Value = -1;
                }
                else
                {
                    pTimeout.Value = (int)timeout.TotalMilliseconds;
                }
                cmd.CommandTimeout = 0; //honor the RECIEVE timeout, whatever it is.
                return cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                ex.Message.createLog("ServiceBrokerExtensions GetMessageBatch", "84");
                return null;
            }
        }

    }
}
