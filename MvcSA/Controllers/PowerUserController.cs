using MvcSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace MvcSA.Controllers
{
    public class PowerUserController : Controller
    {
        //
        // GET: /PowerUser/

        SAEntities dbSA = new SAEntities();
        TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();

        //----------------------------------------------//

        [Check_Authen]
        [Check_Authen_Admin]
        public ActionResult UserMaster()
        {
            ViewBag.Title = "User Master Management";
            return View();
        }

        [HttpPost]
        public JsonResult UserList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbSA.TM_SysUser.Where(w => w.user_active == true);

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.Sys_Plant_id,
                        s.Sys_GroupType_id,
                        s.Sys_User_id
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreateUser()
        {
            try
            {
                var formData = Request.Form["Sys_User_id"];
                var plant = byte.Parse(Request.Form["Sys_Plant_id"]);
                var dbData = dbSA.TM_SysUser.Where(w => w.Sys_User_id == formData && w.Sys_Plant_id == plant).FirstOrDefault();
                if (dbData == null)
                {
                    TM_SysUser data = new TM_SysUser();
                    data.Sys_User_id = formData;
                    data.Sys_Plant_id = plant;
                    data.Sys_GroupType_id = byte.Parse(Request.Form["Sys_GroupType_id"]);
                    data.lv_id = 1;
                    data.user_active = true;

                    dbSA.TM_SysUser.Add(data);
                }
                else
                {
                    dbData.user_active = true;
                }

                dbSA.SaveChanges();

                return Json(new { Result = "OK", Record = dbSA.TM_SysUser.FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult DeleteUser(string user, byte gtype, byte plant)
        {
            try
            {
                var data = (from g in dbSA.TM_SysUser
                            where g.Sys_Plant_id == plant && g.Sys_GroupType_id == gtype && g.Sys_User_id == user
                            select g).FirstOrDefault();
                if (data != null)
                {
                    //data.user_active = false;
                    dbSA.TM_SysUser.Remove(data);

                    dbSA.SaveChanges();
                }

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //----------------------------------------------//

        //[Check_Authen]
        //public ActionResult UserMaster1()
        //{
        //    ViewBag.Title = "User Master Management";
        //    return View();
        //}

        //[HttpPost]
        //public JsonResult UserList1(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        //{
        //    try
        //    {
        //        var query = dbSA.TM_SysUser.Where(w => w.user_active == true);

        //        //Get data from database
        //        int TotalRecord = query.Count();

        //        // Paging
        //        var output = query
        //            .Select(s => new
        //            {
        //                s.Sys_User_id,
        //                //gcode = (s.Sys_GroupType_id*100)+(s.Sys_Plant_id)
        //                s.Sys_Plant_id
        //            }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

        //        //Return result to jTable
        //        return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public JsonResult CreateUser1()
        //{
        //    try
        //    {
        //        var user = Request.Form["Sys_User_id"];
        //        var plant = byte.Parse(Request.Form["Sys_Plant_id"]);
        //        var dbData = dbSA.TM_SysUser.Where(w => w.Sys_User_id == user && w.Sys_Plant_id == plant).FirstOrDefault();
        //        if (dbData == null)
        //        {
        //            TM_SysUser data = new TM_SysUser();
        //            data.Sys_User_id = user;
        //            data.Sys_Plant_id = plant;
        //            data.Sys_GroupType_id = 2;
        //            data.lv_id = 1;
        //            data.user_active = true;

        //            dbSA.TM_SysUser.Add(data);
        //        }
        //        else
        //        {
        //            dbData.user_active = true;
        //        }
        //        dbSA.SaveChanges();

        //        return Json(new { Result = "OK", Record = dbSA.TM_SysUser.OrderByDescending(o => o.Sys_Plant_id).FirstOrDefault() });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public JsonResult UpdateUser1()
        //{
        //    try
        //    {
        //        var user = Request.Form["Sys_User_id"];
        //        var plant = byte.Parse(Request.Form["Sys_Plant_id"]);

        //        var data = dbSA.TM_SysUser.Find(plant, 2, user);

        //        data.Sys_Plant_id = plant;

        //        dbSA.SaveChanges();

        //        return Json(new { Result = "OK" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public JsonResult DeleteUser1()
        //{
        //    try
        //    {
        //        var user = Request.Form["Sys_User_id"];
        //        var plant = byte.Parse(Request.Form["Sys_Plant_id"]);

        //        var data = dbSA.TM_SysUser.Find(plant, (byte)2, user);
        //        dbSA.TM_SysUser.Remove(data);
        //        //data.user_active = false;

        //        dbSA.SaveChanges();

        //        return Json(new { Result = "OK" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        //----------------------------------------------//

        [Check_Authen]
        [Check_Authen_Admin]
        public ActionResult PlantMaster()
        {
            ViewBag.Title = "Plant Master Management";
            return View();
        }

        [HttpPost]
        public JsonResult PlantList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbSA.TM_SysPlant.Where(w => w.Sys_Plant_id != 0 && w.del_flag == false);

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.Sys_Plant_id,
                        s.Sys_Plant_name,
                        s.update_dt,
                        s.update_by,
                        s.del_flag
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreatePlant()
        {
            try
            {
                var formData = Request.Form["Sys_Plant_name"];
                var dbData = dbSA.TM_SysPlant.Where(w => w.Sys_Plant_name == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_SysPlant data = new TM_SysPlant();
                    data.Sys_Plant_name = Request.Form["Sys_Plant_name"];
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["SA_Auth"].ToString();
                    data.del_flag = false;

                    dbSA.TM_SysPlant.Add(data);
                }
                else
                {
                    dbData.del_flag = false;
                }
                dbSA.SaveChanges();

                return Json(new { Result = "OK", Record = dbSA.TM_SysPlant.OrderByDescending(o => o.Sys_Plant_id).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdatePlant()
        {
            try
            {
                var data = dbSA.TM_SysPlant.Find(byte.Parse(Request.Form["Sys_Plant_id"]));
                data.Sys_Plant_name = Request.Form["Sys_Plant_name"];
                data.update_dt = DateTime.Now;
                data.update_by = Session["SA_Auth"].ToString();

                dbSA.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeletePlant()
        {
            try
            {
                var data = dbSA.TM_SysPlant.Find(byte.Parse(Request.Form["Sys_Plant_id"]));
                data.del_flag = true;
                data.update_dt = DateTime.Now;
                data.update_by = Session["SA_Auth"].ToString();

                dbSA.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //----------------------------------------------//

        [Check_Authen]
        [Check_Authen_Admin]
        public ActionResult GroupMaster()
        {
            ViewBag.Title = "Group Master Management";
            return View();
        }

        [HttpPost]
        public JsonResult GroupList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbSA.TM_SysGroup.Where(w => w.del_flag == false);

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.Sys_GroupType_id,
                        hdGroupType = s.Sys_GroupType_id,
                        s.Sys_Plant_id,
                        s.group_id,
                        s.Sys_Group_name,
                        s.update_dt,
                        s.update_by,
                        s.del_flag
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreateGroup()
        {
            try
            {
                var formData = Request.Form["Sys_Group_name"];
                var dbData = dbSA.TM_SysGroup.Where(w => w.Sys_Group_name == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_SysGroup data = new TM_SysGroup();
                    data.Sys_Plant_id = byte.Parse(Request.Form["Sys_Plant_id"]);
                    data.Sys_GroupType_id = byte.Parse(Request.Form["Sys_GroupType_id"]);
                    data.Sys_Group_name = formData;
                    data.group_id = int.Parse(Request.Form["group_id"]);
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["SA_Auth"].ToString();
                    data.del_flag = false;

                    dbSA.TM_SysGroup.Add(data);
                }
                else
                {
                    dbData.del_flag = false;
                }
                dbSA.SaveChanges();

                return Json(new { Result = "OK", Record = dbSA.TM_SysGroup.OrderByDescending(o => o.Sys_Group_name).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateGroup()
        {
            try
            {
                byte gtype = byte.Parse(Request.Form["Sys_GroupType_id"]);
                byte plant = byte.Parse(Request.Form["Sys_Plant_id"]);
                var data = dbSA.TM_SysGroup.Find(byte.Parse(Request.Form["Sys_Plant_id"]), byte.Parse(Request.Form["Sys_GroupType_id"]));
                data.Sys_Group_name = Request.Form["Sys_Group_name"];
                data.group_id = int.Parse(Request.Form["group_id"]);
                data.update_dt = DateTime.Now;
                data.update_by = Session["SA_Auth"].ToString();

                dbSA.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult DeleteGroup(byte group_type, byte plant)
        {
            try
            {
                var data = (from g in dbSA.TM_SysGroup
                            where g.Sys_Plant_id == plant && g.Sys_GroupType_id == group_type
                            select g).FirstOrDefault();
                if (data != null)
                {
                    data.del_flag = true;
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["SA_Auth"].ToString();

                    dbSA.SaveChanges();
                }

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //----------------------------------------------//

        [Check_Authen]
        [Check_Authen_Admin]
        public ActionResult GroupTypeMaster()
        {
            ViewBag.Title = "Group Type Master Management";
            return View();
        }

        [HttpPost]
        public JsonResult GroupTypeList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbSA.TM_SysGroupType.Where(w => w.del_flag == false);

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.Sys_GroupType_id,
                        s.Sys_GroupType_name,
                        s.update_dt,
                        s.update_by,
                        s.del_flag
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreateGroupType()
        {
            try
            {
                var formData = Request.Form["Sys_GroupType_name"];
                var dbData = dbSA.TM_SysGroupType.Where(w => w.Sys_GroupType_name == formData).FirstOrDefault();
                if (dbData == null)
                {
                    TM_SysGroupType data = new TM_SysGroupType();
                    data.Sys_GroupType_name = Request.Form["Sys_GroupType_name"];
                    data.update_dt = DateTime.Now;
                    data.update_by = Session["SA_Auth"].ToString();
                    data.del_flag = false;

                    dbSA.TM_SysGroupType.Add(data);
                }
                else
                {
                    dbData.del_flag = false;
                }
                dbSA.SaveChanges();

                return Json(new { Result = "OK", Record = dbSA.TM_SysGroupType.OrderByDescending(o => o.Sys_GroupType_id).FirstOrDefault() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateGroupType()
        {
            try
            {
                var data = dbSA.TM_SysGroupType.Find(int.Parse(Request.Form["Sys_GroupType_id"]));
                data.Sys_GroupType_name = Request.Form["Sys_GroupType_name"];
                data.update_dt = DateTime.Now;
                data.update_by = Session["SA_Auth"].ToString();

                dbSA.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteGroupType()
        {
            try
            {
                var data = dbSA.TM_SysGroupType.Find(int.Parse(Request.Form["Sys_GroupType_id"]));
                data.del_flag = true;
                data.update_dt = DateTime.Now;
                data.update_by = Session["SA_Auth"].ToString();

                dbSA.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //----------------------------------------------//

        [Check_Authen]
        [Check_Authen_Admin]
        public ActionResult EditComment()
        {
            ViewBag.Title = "Edit Comment";
            return View();
        }

        [HttpPost]
        public JsonResult CommentList(int id, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = dbSA.TD_Transaction.Where(w => w.id == id && w.active == false);

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.id,
                        s.actor,
                        s.status_id,
                        s.lv_id,
                        s.org_id,
                        s.Sys_Plant_id,
                        s.act_id,
                        s.act_dt,
                        s.comment
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult SelectSA(string searchTerm)
        {
            var qa = dbSA.TD_Main
                .Where(w => w.control_no.Contains(searchTerm))
                .OrderBy(o => o.control_no)
                .Select(a => new { id = a.id, text = a.control_no })
                .Take(16).ToList();
            return Json(qa, JsonRequestBehavior.AllowGet);
        }

        //----------------------------------------------//

        [HttpPost]
        public JsonResult GetStatusList()
        {
            try
            {
                var result = dbSA.TM_Status
                    .Select(r => new { DisplayText = r.status_name, Value = r.status_id });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetLevelList()
        {
            try
            {
                var result = dbSA.TM_Level
                    .Select(r => new { DisplayText = r.lv_name, Value = r.lv_id });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetActionList()
        {
            try
            {
                var result = dbSA.TM_Action
                    .Select(r => new { DisplayText = r.act_name, Value = r.act_id });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetPlantList()
        {
            try
            {
                var result = dbSA.TM_SysPlant.Where(w => w.del_flag == false)
                    .Select(r => new { DisplayText = r.Sys_Plant_name, Value = r.Sys_Plant_id });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetGroupTypeList()
        {
            try
            {
                var result = dbSA.TM_SysGroupType.Where(w => w.del_flag == false)
                    .Select(r => new { DisplayText = r.Sys_GroupType_name, Value = r.Sys_GroupType_id });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetGroupList()
        {
            try
            {
                var result = dbSA.TM_SysGroup.Where(w => w.del_flag == false)
                    .Select(r => new { DisplayText = r.Sys_Group_name, Value = r.Sys_Plant_id });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetTNCGroupList()
        {
            try
            {
                var result = dbTNC.tnc_group_master.Select(r => new { DisplayText = r.group_name, Value = r.id });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetTNCUserList()
        {
            try
            {
                var result = dbTNC.tnc_user.OrderBy(o => o.emp_fname).ThenBy(o => o.emp_lname).Select(r => new { DisplayText = r.emp_fname + " " + r.emp_lname, Value = r.emp_code });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}
