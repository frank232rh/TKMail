using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKMail.Data.Abstract;
using TKMail.Data.Entities;
using TKMail.Data.Helpers;

namespace TKMail.Data.Concrete
{
    public class MailRepository : iMailRepository
    {
        public Response AddMailNotification(eMailMessage eMail, int IdApp)
        {
            Response regreso = new Response();
            try
            {
                Response regresoAttach = new Response();
                using (TKMailContext ctx = new TKMailContext())
                {
                    ctx.Database.Connection.Open();

                    var command = ctx.Database.Connection.CreateCommand();
                    command.CommandText = "[Notification].[SaveMail]";
                    command.CommandType = CommandType.StoredProcedure;
                    
                    command.Parameters.Add(new SqlParameter("@Subject", eMail.Subject));
                    command.Parameters.Add(new SqlParameter("@MailFrom", eMail.MailFrom));
                    command.Parameters.Add(new SqlParameter("@MailTo", eMail.MailTo));
                    command.Parameters.Add(new SqlParameter("@TemplateHTML", eMail.TemplateHTML));
                    command.Parameters.Add(new SqlParameter("@Sent", false));
                    command.Parameters.Add(new SqlParameter("@IdApp", IdApp));

                    var read = command.ExecuteReader();

                    while (read.Read())
                    {
                        regreso.Resp = read.GetBoolean(read.GetOrdinal("Response"));
                        regreso.Message = read.GetString(read.GetOrdinal("Message"));
                        regreso.IdReturn = read.GetInt32(read.GetOrdinal("Id"));

                        if (regreso.Resp) //Insert attachments
                        {
                            foreach (var item in eMail.Attachments)
                            {
                                var commandAttach = ctx.Database.Connection.CreateCommand();
                                commandAttach.CommandText = "[Notification].[SaveAttach]";
                                commandAttach.CommandType = CommandType.StoredProcedure;

                                commandAttach.Parameters.Add(new SqlParameter("@IdNotificationMail", regreso.IdReturn));
                                commandAttach.Parameters.Add(new SqlParameter("@Attach", item.File));
                                commandAttach.Parameters.Add(new SqlParameter("@FileName", item.FileName));

                                var readAttach = commandAttach.ExecuteReader();

                                while (readAttach.Read())
                                {
                                    regresoAttach.Resp = readAttach.GetBoolean(readAttach.GetOrdinal("Response"));
                                    regresoAttach.Message = readAttach.GetString(readAttach.GetOrdinal("Message"));
                                    regresoAttach.IdReturn = readAttach.GetInt32(readAttach.GetOrdinal("Id"));
                                }
                            }
                        }
                    }

                    ctx.Database.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                regreso = new Response { Resp = false, Message = "Ocurrió un error."};
                LogMethods.writeException(ex, "TKMail.Data.Concrete.MailRepository.AddMailNotification");
            }
            finally
            {
                GC.Collect();
            }
            return regreso;
        }

        public List<attach> GetAttach(int idMail)
        {
            
            List<attach> regreso = new List<attach>();
            byte[] byteResult = new byte[0];
            
            try
            {
                using (TKMailContext context = new TKMailContext())
                {
                    string query = "[Notification].[GetMailAttach] ";
                    query += " @IdMail = " + idMail + ";";
                    regreso = context.Database.SqlQuery<attach>(query).ToList();
                    context.Database.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                regreso = new List<attach>();
                LogMethods.writeException(ex, "TKMail.Data.Concrete.MailRepository.GetMailConfig");
            }
            finally
            {
                GC.Collect();
            }
            return regreso;
        }

        public MailConfig GetMailConfig(int idApp)
        {
            MailConfig regreso = new MailConfig();
            try
            {
                using (TKMailContext ctx = new TKMailContext())
                {
                    ctx.Database.Connection.Open();

                    var command = ctx.Database.Connection.CreateCommand();
                    command.CommandText = "[Configuration].[GetAppConfigurationMail]";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@IdApp", idApp));

                    var read = command.ExecuteReader();

                    while (read.Read())
                    {
                        regreso.Id = read.GetInt32(read.GetOrdinal("Id"));
                        regreso.Host = read.GetString(read.GetOrdinal("Host"));
                        regreso.Port = read.GetInt32(read.GetOrdinal("Port"));
                        regreso.IsEnableSSL = read.GetBoolean(read.GetOrdinal("IsEnableSSL"));
                        regreso.EmailAddress = read.GetString(read.GetOrdinal("EmailAddress"));
                        regreso.Password = read.GetString(read.GetOrdinal("Password"));
                        regreso.Subject = read.GetString(read.GetOrdinal("Subject"));
                        regreso.IdApp = read.GetInt32(read.GetOrdinal("IdApp"));
                    }

                    ctx.Database.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                regreso = new MailConfig();
                LogMethods.writeException(ex, "TKMail.Data.Concrete.MailRepository.GetMailConfig");
            }
            finally
            {
                GC.Collect();
            }
            return regreso;
        }

        public eMailMessage GetMailNotification(int idMail)
        {
            eMailMessage regreso = new eMailMessage();
            try
            {
                using (TKMailContext ctx = new TKMailContext())
                {
                    ctx.Database.Connection.Open();

                    var command = ctx.Database.Connection.CreateCommand();
                    command.CommandText = "[Notification].[GetMail]";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@IdMail", idMail));

                    var read = command.ExecuteReader();

                    while (read.Read())
                    {
                        regreso.Id = read.GetInt32(read.GetOrdinal("Id"));
                        regreso.Subject = read.GetString(read.GetOrdinal("Subject"));
                        regreso.MailFrom = read.GetString(read.GetOrdinal("MailFrom"));
                        regreso.MailTo = read.GetString(read.GetOrdinal("MailTo"));
                        regreso.TemplateHTML = read.GetString(read.GetOrdinal("TemplateHTML"));
                        regreso.IdApp = read.GetInt32(read.GetOrdinal("IdApp"));
                        if (regreso.Id > 0)
                        {
                            regreso.Attachments = GetAttach(regreso.Id);
                        }
                    }

                    ctx.Database.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                regreso = new eMailMessage();
                LogMethods.writeException(ex, "TKMail.Data.Concrete.MailRepository.GetMailConfig");
            }
            finally
            {
                GC.Collect();
            }
            return regreso;
        }

        public Response UpdateMail(int idMail, bool sent)
        {
            Response regreso = new Response();
            try
            {
                using (TKMailContext ctx = new TKMailContext())
                {
                    ctx.Database.Connection.Open();

                    var command = ctx.Database.Connection.CreateCommand();
                    command.CommandText = "[Notification].[UpdateEmail]";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@IdNotification", idMail));
                    command.Parameters.Add(new SqlParameter("@Sent", sent));

                    var read = command.ExecuteReader();

                    while (read.Read())
                    {
                        regreso.Resp = read.GetBoolean(read.GetOrdinal("Response"));
                        regreso.Message = read.GetString(read.GetOrdinal("Message"));
                        regreso.IdReturn = read.GetInt32(read.GetOrdinal("Id"));
                    }

                    ctx.Database.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                regreso = new Response{ Resp = false, Message = "Ocurrió un error.", IdReturn = 0};
                LogMethods.writeException(ex, "TKMail.Data.Concrete.MailRepository.GetMailConfig");
            }
            finally
            {
                GC.Collect();
            }
            return regreso;
        }

        public List<eMailMessage> GetNotSentNotification()
        {
            List<eMailMessage> regreso = new List<eMailMessage>();
            try
            {
                using (TKMailContext ctx = new TKMailContext())
                {
                    ctx.Database.Connection.Open();

                    var command = ctx.Database.Connection.CreateCommand();
                    command.CommandText = "[Notification].[GetNotSentNotifications]";
                    command.CommandType = CommandType.StoredProcedure;

                    var read = command.ExecuteReader();

                    while (read.Read())
                    {
                        regreso.Add(new eMailMessage
                        {
                            Id = read.GetInt32(read.GetOrdinal("Id")),
                            Subject = read.GetString(read.GetOrdinal("Subject")),
                            MailFrom = read.GetString(read.GetOrdinal("MailFrom")),
                            MailTo = read.GetString(read.GetOrdinal("MailTo")),
                            TemplateHTML = read.GetString(read.GetOrdinal("TemplateHTML")),
                            IdApp = read.GetInt32(read.GetOrdinal("IdApp"))
                        });
                    }
                    foreach (var item in regreso)
                    {
                        item.Attachments = GetAttach(item.Id);
                    }
                    ctx.Database.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                regreso = new List<eMailMessage>();
                LogMethods.writeException(ex, "TKMail.Data.Concrete.MailRepository.GetMailConfig");
            }
            finally
            {
                GC.Collect();
            }
            return regreso;
        }
        
        public string GetGeneralConfiguration(string nameVariable)
        {
            string regreso = "";
            try
            {
                using (TKMailContext ctx = new TKMailContext())
                {
                    ctx.Database.Connection.Open();

                    var command = ctx.Database.Connection.CreateCommand();
                    command.CommandText = "[Configuration].[GetGeneralConfigurationByKey]";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@NameVariable", nameVariable));

                    var read = command.ExecuteReader();

                    while (read.Read())
                    {
                        regreso = read.GetString(read.GetOrdinal("VariableValue"));
                    }
                    ctx.Database.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                regreso = "";
                LogMethods.writeException(ex, "TKMail.Data.Concrete.MailRepository.GetGeneralConfiguration");
            }
            finally
            {
                GC.Collect();
            }
            return regreso;
        }

    }
}
