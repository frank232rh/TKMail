using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TKMail.Data.Abstract;
using TKMail.Data.Concrete;
using TKMail.Data.Entities;
using TKMail.NotificationService.Entities.Extensions;

namespace TKMail.NotificationService.MessageProcessors
{
    public class QueueProcessor : IDisposable
    {
        private EventWaitHandle eventWaitHandle = new AutoResetEvent(false);
        private Thread worker;
        private readonly object locker = new object();
        private Queue<string> OrdIds = new Queue<string>();
        private static iMailRepository repository;
        public QueueProcessor()
        {
            repository = new MailRepository();
            worker = new Thread(Work);
            worker.Start();
        }
        private async void Work()
        {
            string Id = "";
            int res = 0;
            while (true)
            {
                try
                {
                    Id = null;
                    // Dequeue the Id
                    lock (locker)
                        if (OrdIds.Count > 0)
                        {
                            Id = OrdIds.Dequeue().RemoveSpecialCharacters();
                        }
                    if (Id != null)
                    {
                        try
                        {
                            ("Dequeue - " + Id + " - " + worker.ManagedThreadId.ToString()).createLog("QueueProcessor - Work ", "46");
                            eMailMessage model = repository.GetMailNotification(int.Parse(Id));
                            MailConfig mailConfig = repository.GetMailConfig(model.IdApp);
                            if (model != null && !model.Sent)
                            {
                                res = model.sendMail(model, mailConfig).Result;
                                if (res != 1)
                                {
                                    //("Result was distinct to 1 " + res.ToString() + " | The result was reenqueue").createLog("QueueProcessor - Work ", "53");
                                    //EnqueueOrdId(Id);
                                    var x = repository.UpdateMail(int.Parse(Id), false);
                                }
                                else
                                {
                                    var x = repository.UpdateMail(int.Parse(Id), true);
                                    //int x = repository.updateMessage(Id);
                                }
                            }
                            else
                            {
                                ("Result was sended " + res.ToString() + " | The result was sended before").createLog("QueueProcessor - Work ", "63");
                            }
                        }
                        catch (Exception ex)
                        {
                            ("Work thread failed - " + Id + " - " + worker.ManagedThreadId.ToString() + " | Error" + ex.Message).createLog("QueueProcessor - Work ", "68");
                        }
                    }
                    else
                    {
                        // No more ORDIDs - wait for a signal
                        eventWaitHandle.WaitOne();
                    }
                }
                catch (Exception ex)
                {
                    ("Dequeue failed - " + Id + " - " + worker.ManagedThreadId.ToString() + " | Error" + ex.Message).createLog("QueueProcessor - Work ", "79");
                }
            }
        }
        /// <summary>
        /// This function Enqueue all OrdIds like FIFO and avoids the duplicates
        /// </summary>
        /// <param name="OrdId">string</param>
        public void EnqueueOrdId(string OrdId)
        {
            try
            {
                // Enqueue the ORDID
                // This statement is secured by lock to prevent other thread to mess with queue while enqueuing OrdId
                if (!OrdIds.Contains(OrdId))
                {
                    lock (locker) OrdIds.Enqueue(OrdId);
                    ("Added to the queue - " + OrdId).createLog("QueueProcessor - EnqueueOrdId ", "96");
                }
                else
                {
                    ("Already in the queue - " + OrdId).createLog("QueueProcessor - EnqueueOrdId ", "100");
                }

            }
            catch (Exception ex)
            {
                ("Enqueue failed - " + OrdId + " | Error" + ex.Message).createLog("QueueProcessor - EnqueueOrdId", "106");
            }

            // Signal worker that OrdId is enqueued and that it can be processed
            eventWaitHandle.Set();
        }
        #region IDisposable Members
        public void Dispose()
        {
            // Signal the FileProcessor to exit
            EnqueueOrdId(null);
            // Wait for the FileProcessor's thread to finish
            worker.Join();
            // Release any OS resources
            eventWaitHandle.Close();
        }
        #endregion
    }
}
