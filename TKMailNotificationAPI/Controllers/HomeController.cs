using MailNotificationAPI.Concrete;
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
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

    }
}
