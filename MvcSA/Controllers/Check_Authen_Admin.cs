using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSA.Controllers
{
    public class Check_Authen_Admin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["SA_UType"].ToString() == "0")
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
            }
        }
    }
}