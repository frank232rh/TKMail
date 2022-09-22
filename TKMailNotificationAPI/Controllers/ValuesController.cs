using MailNotificationAPI.Concrete;
using MailNotificationAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TKMail.Data.Entities;
using TKMailNotificationAPI.Models;

namespace MailNotificationAPI.Controllers
{
    [RoutePrefix("api/Email")]
    public class ValuesController : ApiController
    {
        private NotificationRepository _notificationRepository;

        public ValuesController()
        {
            _notificationRepository = new NotificationRepository();
        }

        [Route("SendMail")]
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult SendMail([FromBody] TKMailNotificationAPI.Models.MailData mailData)
        {

            Response regreso = new Response();
            try
            {
                regreso = _notificationRepository.SendMail(mailData);
            }
            catch (Exception ex)
            {
                LogMethods.writeException(ex, "TKMail.MailNotificationAPI.Controllers.ValuesController.SendMail");
            }
            finally
            {
                GC.Collect();
            }
            return Ok(regreso);
        }

        [Route("Ping")]
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult Ping()
        {
            bool status = true;

            return Ok(status);
        }
    }
}
