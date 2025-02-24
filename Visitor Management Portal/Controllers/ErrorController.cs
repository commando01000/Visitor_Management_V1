using CrmEarlyBound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Visitor_Management_Portal.DAL.Repository.BuildingRepository;

namespace Visitor_Management_Portal.Controllers
{
    public class ErrorController : Controller
    {
       
        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Forbidden()
        {
            return View();
        }

    }
}