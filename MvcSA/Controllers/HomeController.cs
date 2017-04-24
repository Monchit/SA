using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcSA.Models;
using TNCWebClassLibrary;

namespace MvcSA.Controllers
{
    public class HomeController : Controller
    {
        TNC_ADMINEntities dbTNCAdmin = new TNC_ADMINEntities();
        SAEntities dbSA = new SAEntities();

        public ActionResult Index()
        {
            ViewBag.Title = "Home";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Title = "About";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Title = "Contact";
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            string username = Request.Form["Username"].ToString();
            Util ut = new Util();
            var pass = ut.CalculateMD5Hash(Request.Form["Password"].ToString());
            if (ModelState.IsValid)
            {
                var user = (from u in dbTNCAdmin.tnc_user
                            where u.username == username && u.password == pass//Open when real
                            select u).FirstOrDefault();

                if (user != null)
                {
                    Session["SA_Auth"] = user.emp_code;
                    Session["SA_Name"] = user.emp_fname + " " + user.emp_lname;

                    var sa_user = (from u in dbSA.TM_Admin
                                   where u.emp_code == user.emp_code
                                   select u).FirstOrDefault();

                    Session["SA_UType"] = sa_user != null ? sa_user.utype_id : 0;

                    var position_lv = (from a in dbTNCAdmin.tnc_position_master
                                       where a.id == user.emp_position
                                       select a.position_level).FirstOrDefault();
                    if (position_lv != null)
                    {
                        var sa_lv = (from b in dbSA.TM_Level
                                     where b.position_min <= position_lv && b.position_max >= position_lv
                                     select b.lv_id).FirstOrDefault();
                        Session["SA_UserLv"] = sa_lv;
                        if (sa_lv <= 2)
                        {
                            Session["SA_Org"] = user.emp_group;
                        }
                        else if (sa_lv == 3)
                        {
                            Session["SA_Org"] = user.emp_depart;
                        }
                        else if (sa_lv == 4)
                        {
                            Session["SA_Org"] = user.emp_plant;
                        }
                    }

                    if (Session["Redirect"] != null)
                    {
                        string url = Session["Redirect"].ToString();
                        Session.Remove("Redirect");
                        return Redirect(url);
                    }
                    else
                    {
                        return RedirectToAction("Index", "SAProcess");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string key)
        {
            getUserInfo user = new getUserInfo();
            if (!string.IsNullOrEmpty(key))
            {
                string empcode = TNCCrypto.Decode(key);
                if (user.setUser(empcode))
                {
                    Session["SA_Auth"] = empcode;
                    Session["SA_Name"] = user.Emp_fname + " " + user.Emp_lname;
                    
                    var sa_user = (from u in dbSA.TM_Admin
                                   where u.emp_code == empcode
                                   select u).FirstOrDefault();
                    Session["SA_UType"] = sa_user != null ? sa_user.utype_id : 0;

                    var position_lv = (from a in dbTNCAdmin.tnc_position_master
                                       where a.id == user.Emp_position
                                       select a.position_level).FirstOrDefault();
                    if (position_lv != null)
                    {
                        var sa_lv = (from b in dbSA.TM_Level
                                     where b.position_min <= position_lv && b.position_max >= position_lv
                                     select b.lv_id).FirstOrDefault();
                        Session["SA_UserLv"] = sa_lv;
                        if (sa_lv <= 2)
                        {
                            Session["SA_Org"] = user.GroupID;
                        }
                        else if (sa_lv == 3)
                        {
                            Session["SA_Org"] = user.DeptID;
                        }
                        else if (sa_lv == 4)
                        {
                            Session["SA_Org"] = user.PlantID;
                        }
                    }

                    if (Session["Redirect"] != null)
                    {
                        string url = Session["Redirect"].ToString();
                        Session.Remove("Redirect");
                        return Redirect(url);
                    }
                    else
                    {
                        return RedirectToAction("Index", "SAProcess");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Logout()
        {
            Session.Remove("SA_Auth");
            Session.Remove("SA_Name");
            Session.Remove("SA_UType");
            Session.Remove("SA_UserLv");
            Session.Remove("SA_Org");
            return RedirectToAction("Index", "Home");
        }

        public ActionResult GetLoginPlant(string user)
        {
            var query = from a in dbSA.TM_SysUser.Where(w => w.Sys_User_id == user)
                        join b in dbSA.TM_SysPlant on a.Sys_Plant_id equals b.Sys_Plant_id into g
                        from b in g.DefaultIfEmpty()
                        select new
                        {
                            pid = a.Sys_Plant_id,
                            pname = b.Sys_Plant_name
                        };

            if (query != null)
                return Json(query, JsonRequestBehavior.AllowGet);
            else
                return Json(0, JsonRequestBehavior.AllowGet);
        }

        //Update Date 2014-09-10 by Monchit W. - Add Method
        protected override void Dispose(bool disposing)
        {
            dbSA.Dispose();
            base.Dispose(disposing);
        }
    }
}
