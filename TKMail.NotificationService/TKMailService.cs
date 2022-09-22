using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using TKMail.NotificationService.Entities;
using TKMail.NotificationService.Entities.Extensions;
using TKMail.NotificationService.MessageProcessors;
using TKMail.NotificationService.Quartz;

namespace TKMail.NotificationService
{
    public partial class TKMailService : ServiceBase
    {
        private static List<QueueListenerConfig> QueueSettings = new List<QueueListenerConfig>();
        private static List<Thread> Listeners = new List<Thread>();
        private static QueueProcessor processor = new QueueProcessor();
        private static bool stopping = false;
        public TKMailService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }
        public void Start()
        {
            try
            {

                JobScheduler jobScheduler = new JobScheduler();

                //Producción
                jobScheduler.Start();//Incia servicios de notificación recurrente de acuerdo a la frecuencia indicada y envio de reporte de SRC

                //Debug reporte RMA
                //ReportRepository report = new ReportRepository();
                //DataTable dataTrasa = report.GetReportRMA("");
                //report.CrearReportRMA(dataTrasa);


                string stQueueName = ConfigurationManager.AppSettings["QueueName"].ToString();
                int ThreadsNumber = int.Parse(ConfigurationManager.AppSettings["ThreadsNumber"].ToString());
                string stConnectionString = ConfigurationManager.ConnectionStrings["MailConnectionString"].ConnectionString;

                var l = new QueueListenerConfig();
                l.QueueName = stQueueName;//The name of the service broker queue
                l.Threads = ThreadsNumber;//We can add more threads <- may be for the future this variable can be on the appconfig
                l.EnlistMessageProcessor = false;//Don't call the message processor in the context of the RECEIVE transaction
                l.MessageProcessor = InboundMessageProcessor.ProcessMessage;//Wire up the message processors
                l.FailedMessageProcessor = InboundMessageProcessor.SaveFailedMessage;
                l.ConnectionString = stConnectionString;
                QueueSettings.Add(l);
                foreach (var q in QueueSettings)
                {
                    for (int i = 0; i < q.Threads; i++)
                    {
                        Thread listenerThread = new Thread(ListenerThreadProc);
                        listenerThread.Name = "Listener Thread " + i.ToString() + " for " + q.QueueName;
                        listenerThread.IsBackground = false;
                        Listeners.Add(listenerThread);
                        listenerThread.Start(q);
                        ("Started thread " + listenerThread.Name).createLog(listenerThread.Name, "55");
                        ("Started thread").createLog(listenerThread.Name, "56");
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Message.createLog("HalMailService Start", "62");
            }
        }
        protected override void OnStop()
        {
        }
        
        public void thisStop()
        {
            try
            {
                stopping = true;
                processor.Dispose();
                Listeners.Clear();
                foreach (var t in Listeners)
                {
                    t.Abort();
                }
                QueueSettings.Clear();
            }
            catch (Exception ex)
            {
                (ex.Message).createLog("HalService OnStop", "83");
            }
        }
        public static void ListenerThreadProc(object queueListenerConfig)
        {
            QueueListenerConfig config = (QueueListenerConfig)queueListenerConfig;
            while (!stopping)
            {
                TransactionOptions to = new TransactionOptions();
                to.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                to.Timeout = TimeSpan.MaxValue;
                CommittableTransaction tran = new CommittableTransaction(to);
                try
                {
                    using (var con = new SqlConnection(config.ConnectionString))
                    {
                        con.Open();
                        con.EnlistTransaction(tran);
                        byte[] message = con.GetMessage(config.QueueName, TimeSpan.FromSeconds(10));
                        if (message == null) //no message available
                        {
                            tran.Commit();
                            con.Close();
                            continue;
                        }
                        else
                        {
                            string text = Encoding.UTF8.GetString(message);
                            ("InboundMessageProcessor Recieved Message " + text).createLog("ListenerThreadProc", "59");
                            processor.EnqueueOrdId(text);
                        }
                        try
                        {
                            if (config.EnlistMessageProcessor)
                            {
                                using (var ts = new TransactionScope(tran))
                                {
                                    config.MessageProcessor(message);
                                    ts.Complete();
                                }
                            }
                            else
                            {
                                config.MessageProcessor(message);
                            }

                        }
                        catch (SqlException ex)
                        {
                            config.FailedMessageProcessor(message, con, ex);
                        }
                        tran.Commit();
                        con.Close();
                    }
                }
                catch (SqlException ex)
                {
                    ("Processing message from " + config.QueueName + " : " + ex.Message).createLog("ListenerThreadProc", "89");
                    tran.Rollback();
                    tran.Dispose();
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    ("Unexpected Exception in Thread Proc for " + config.QueueName + ".Thread Proc is exiting: " + ex.Message).createLog("ListenerThreadProc", "96");
                    tran.Rollback();
                    tran.Dispose();
                    return;
                }
            }
        }
    }
}
