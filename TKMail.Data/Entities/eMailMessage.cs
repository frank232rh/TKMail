using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TKMail.Data.Helpers;

namespace TKMail.Data.Entities
{
    public class eMailMessage
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string MailFrom { get; set; }
        public string MailTo { get; set; }
        public string TemplateHTML { get; set; }
        public DateTime TSInserted { get; set; }
        public DateTime TSSent { get; set; }
        public bool Sent { get; set; }
        public int IdApp { get; set; }
        public List<attach> Attachments { get; set; }

        private static string MainWebServiceUrl = ConfigurationManager.AppSettings["BaseAddress"].ToString(); // Put your main host url here
        private string WebServiceUrl = MainWebServiceUrl + "api/Email"; // put your api extension url/uri here
        HttpClient client;
        public eMailMessage()
        {
            client = new HttpClient();
        }

        public async Task<int> sendMail(eMailMessage model, MailConfig mailConfig)
        {
            int response = 0;
            int respuesta = 0;
            
            try
            {
                //if (lContacts.Count > 0)
                //{
                //    foreach (var user in lContacts)
                //    {
                //model.to = user.Email;

                MailData mailData = new MailData { config = mailConfig, message = model };

                string json = JsonConvert.SerializeObject(mailData);

                HttpContent s = new StringContent(json, Encoding.UTF8, "application/json");


                client.DefaultRequestHeaders.Clear();

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage regreso = await client.PostAsync(WebServiceUrl + "/SendMail", s).ConfigureAwait(false);

                if (regreso.IsSuccessStatusCode)
                {
                    var Resp = await regreso.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var rspt = JsonConvert.DeserializeObject<Response>(Resp);
                    response = rspt.Resp ? 1 : 0;
                    if (rspt.Resp == false)
                    {
                        LogMethods.writeException(new Exception(), "TKMail.Data.Entities.eMailMessage.sendMail\n " + rspt.Message);
                    }
                }
                else
                {
                    LogMethods.writeException(new Exception(), "TKMail.Data.Entities.eMailMessage.sendMail\n" + regreso.RequestMessage);
                }

                //    }
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se pudo enviar el email\nError: {ex.Message}");
                LogMethods.writeException(ex, "TKMail.Data.Entities.eMailMessage.sendMail");
                //(ex.Message).createLog("SupplierMService sendEmail", "60");
                response = -1;
            }
            //response = 1;
            return response;
        }

    }


    public class attach
    {
        public int IdNotificationMail { get; set; }
        public byte[] File { get; set; }
        public string FileName { get; set; }
    }

}
