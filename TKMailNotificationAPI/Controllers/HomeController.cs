using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TKMail.Data.Abstract;
using TKMail.Data.Concrete;
using TKMail.Data.Entities;
using TKMail.Data.Helpers;

namespace MailNotificationAPI.Controllers
{
    public class HomeController : Controller
    {
        private iMailRepository _repositoryMail;

        public HomeController()
        {
            _repositoryMail = new MailRepository();
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [HttpPost]
        public JsonResult AddMailNotification(eMailMessage eMail, int IdApp)
        {

            Response regreso = new Response();
            try
            {
                regreso = _repositoryMail.AddMailNotification(eMail, IdApp);
            }
            catch (Exception ex)
            {
                LogMethods.writeException(ex, "TKMail.MailNotificationAPI.Controllers.HomeController.AddMailNotification");
            }
            finally
            {
                GC.Collect();
            }
            return Json(regreso, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAttach(int idMail)
        {

            List<attach> regreso = new List<attach>();
            try
            {
                regreso = _repositoryMail.GetAttach(idMail);
            }
            catch (Exception ex)
            {
                LogMethods.writeException(ex, "TKMail.MailNotificationAPI.Controllers.HomeController.GetAttach");
            }
            finally
            {
                GC.Collect();
            }
            return Json(regreso, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMailConfig(int idApp)
        {

            MailConfig regreso = new MailConfig();
            try
            {
                regreso = _repositoryMail.GetMailConfig(idApp);
            }
            catch (Exception ex)
            {
                LogMethods.writeException(ex, "TKMail.MailNotificationAPI.Controllers.HomeController.GetMailConfig");
            }
            finally
            {
                GC.Collect();
            }
            return Json(regreso, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMailNotification(int idMail)
        {

            eMailMessage regreso = new eMailMessage();
            try
            {
                regreso = _repositoryMail.GetMailNotification(idMail);
            }
            catch (Exception ex)
            {
                LogMethods.writeException(ex, "TKMail.MailNotificationAPI.Controllers.HomeController.GetMailNotification");
            }
            finally
            {
                GC.Collect();
            }
            return Json(regreso, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateMail(int idMail, bool sent)
        {

            Response regreso = new Response();
            try
            {
                regreso = _repositoryMail.UpdateMail(idMail, sent);
            }
            catch (Exception ex)
            {
                LogMethods.writeException(ex, "TKMail.MailNotificationAPI.Controllers.HomeController.UpdateMail");
            }
            finally
            {
                GC.Collect();
            }
            return Json(regreso, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetNotSentNotification()
        {

            List<eMailMessage> regreso = new List<eMailMessage>();
            try
            {
                regreso = _repositoryMail.GetNotSentNotification();
            }
            catch (Exception ex)
            {
                LogMethods.writeException(ex, "TKMail.MailNotificationAPI.Controllers.HomeController.GetNotSentNotification");
            }
            finally
            {
                GC.Collect();
            }
            return Json(regreso, JsonRequestBehavior.AllowGet);
        }

    }
}
