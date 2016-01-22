using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSA.Views.SAProcess
{
    public class NonUse
    {
        //[Check_Authen]
        //public ActionResult NewSA_1()
        //{
        //    //if (Session["SA_Auth"] == null)
        //    //{
        //    //    return RedirectToAction("Index", "Home");
        //    //}
        //    ViewBag.Menu = 2;
        //    ViewBag.Title = "Special Acceptance Requisition Form";
        //    //var plant = from p in dbSA.TM_SysPlant
        //    //            where p.del_flag == false
        //    //            select p;
        //    var plant = from p in dbPCR.PCR_Impact
        //                select p;
        //    ViewBag.SysPlant = plant;
        //    if (Session["SA_Org"] != null)
        //    {
        //        ViewBag.ControlNo = InitialControlNo(Session["SA_Org"].ToString());
        //    }
        //    return View();
        //}
        //private string GetQAEmail(int id)
        //{
        //    var get_qa = from a in dbSA.TD_Transaction
        //                 where a.id == id && a.status_id == 2 && a.lv_id == 1
        //                 select a.actor;
        //    string email = "";
        //    foreach (var item in get_qa)
        //    {
        //        var get_email = (from b in dbTNC.tnc_user
        //                         where b.emp_code == item
        //                         select b.email).FirstOrDefault();
        //        if (get_email != null)
        //        {
        //            if (!string.IsNullOrEmpty(get_email))
        //                email += "," + get_email;
        //        }
        //    }
        //    email = email.Substring(1);
        //    return email;
        //}
        //[HttpPost]
        //public JsonResult CreateIssueSA(int sid)//, HttpPostedFileBase file_evidence)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
        //        }

        //        //var file = file_evidence;
        //        //var x = file_evidence.FileName;
        //        //var file = Request.Files["file_evidence"];

        //        DateTime dt = DateTime.Now;
        //        string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/" + sid + "/";
        //        if (!Directory.Exists(Server.MapPath(subPath)))
        //        {
        //            Directory.CreateDirectory(Server.MapPath(subPath));
        //        }

        //        TD_SA sa = new TD_SA();
        //        sa.id = sid;
        //        sa.sa_no = Request.Form["sa_no"];
        //        sa.cust_name = Request.Form["cust_name"];
        //        sa.result = byte.Parse(Request.Form["result"]);
        //        sa.issue_date = DateTime.Parse(Request.Form["issue_date"]);
        //        sa.reason = Request.Form["reason"];
        //        sa.update_by = Session["SA_Auth"].ToString();
        //        //sa.file_evidence = file.FileName;

        //        //if (file != null && file.ContentLength > 0)
        //        //{
        //        //    if (file.ContentType == "application/pdf")//Check accept file type.
        //        //    {
        //        //        var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(file.FileName);
        //        //        var path = Path.Combine(Server.MapPath(subPath), fileName);
        //        //        file.SaveAs(path);
        //        //        sa.file_evidence = subPath + fileName;
        //        //    }
        //        //}

        //        dbSA.TD_SA.Add(sa);
        //        dbSA.SaveChanges();

        //        return Json(new { Result = "OK", Record = dbSA.TD_SA.Where(w => w.id == sid).OrderByDescending(o => o.sa_id).FirstOrDefault() });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}
        //private bool AddConcernQC(int id, string concern)
        //{
        //    if (!string.IsNullOrEmpty(concern))
        //    {
        //        byte[] plant = concern.Split(',').Select(s => Convert.ToByte(s, 16)).ToArray();

        //        foreach (var item in plant)
        //        {
        //            var check = from a in dbSA.TD_ConcernQC
        //                        where a.id == id && a.Sys_Plant_id == item
        //                        select a;
        //            if (!check.Any())
        //            {
        //                var qc = new TD_ConcernQC();
        //                qc.id = id;
        //                qc.Sys_Plant_id = item;
        //                dbSA.TD_ConcernQC.Add(qc);
        //                dbSA.SaveChanges();
        //            }
        //        }
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //public ActionResult AddComment(IEnumerable<HttpPostedFileBase> files)
        //{
        //    int id = Request.Form["hdID"] != null ? int.Parse(Request.Form["hdID"].ToString()) : 0;
        //    int org = int.Parse(Session["SA_Org"].ToString());
        //    byte lv_id = byte.Parse(Session["SA_UserLv"].ToString());
        //    var comment = Request.Form["txaComment"] != null ? Request.Form["txaComment"].ToString() : null;
        //    if (id != 0)
        //    {
        //        Update_Transaction(id, lv_id, org, false, Session["SA_Auth"].ToString(), 1, comment);
        //        //NextStep_Transaction(id,,lv_id,org,act_id);
        //        //var query = (from a in dbSA.TD_Transaction
        //        //             where a.id == id && a.active == true && a.lv_id == lv_id && a.org_id == org
        //        //             select a).FirstOrDefault();
        //        //if (query != null)
        //        //{
        //        //    query.act_id = 1;//1=Comment
        //        //    query.act_dt = DateTime.Now;
        //        //    query.actor = Session["SA_Auth"].ToString();
        //        //    query.active = false;
        //        //    query.comment = Request.Form["txaComment"] != null ? Request.Form["txaComment"].ToString() : null;
        //        //    dbSA.SaveChanges();
        //        //}
        //        return RedirectToAction("Index", "SAProcess");
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}
        //public ActionResult TestUpload()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Upload(IEnumerable<HttpPostedFileBase> files)
        //{
        //    var dt = DateTime.Now;
        //    string subPath = "~/TestUpload/" + dt.Year + "/" + dt.Month + "/";
        //    if(!Directory.Exists(Server.MapPath(subPath))){
        //        Directory.CreateDirectory(Server.MapPath(subPath));
        //    }
        //    foreach (var file in files)
        //    {
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            if (file.ContentType == "application/pdf")
        //            {
        //                var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(file.FileName);//DateTime.Now.Ticks
        //                var path = Path.Combine(Server.MapPath(subPath), fileName);
        //                file.SaveAs(path);
        //                //SaveFiletoDB(1, subPath + fileName, "111080");
        //            }
        //        }

        //    }
        //    return RedirectToAction("TestUpload", "SAProcess");
        //}

        //public ActionResult ApproveReject(IEnumerable<HttpPostedFileBase> files)
        //{
        //    int id = Request.Form["hdID"] != null ? int.Parse(Request.Form["hdID"].ToString()) : 0;
        //    int org = int.Parse(Session["SA_Org"].ToString());
        //    byte lv_id = byte.Parse(Session["SA_UserLv"].ToString());
        //    if (id != 0)
        //    {
        //        var query = (from a in dbSA.TD_Transaction
        //                     where a.id == id && a.active == true && a.lv_id == lv_id && a.org_id == org
        //                     select a).FirstOrDefault();
        //        if (query != null)
        //        {
        //            query.act_id = byte.Parse(Request.Form["slAction"].ToString());
        //            query.act_dt = DateTime.Now;
        //            query.actor = Session["SA_Auth"].ToString();
        //            query.active = false;
        //            query.comment = Request.Form["txaComment"] != null ? Request.Form["txaComment"].ToString() : null;
        //            dbSA.SaveChanges();
        //        }
        //        return RedirectToAction("Index", "SAProcess");
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        //public ActionResult GetComment(int id, byte statusid)
        //{
        //    string comment = "";
        //    using (var db = new SAEntities())
        //    {
        //        var query = from a in dbSA.TD_Transaction
        //                    where a.id == id && a.status_id == statusid && a.actor != null
        //                    orderby a.lv_id, a.act_dt
        //                    select a;

        //        if (query != null)
        //        {
        //            comment += "<table class='table table-striped table-condensed'><thead><tr><th>Name</th><th>Action</th><th>Level</th><th>Date-Time</th><th>Comment</th></tr></thead></tbody>";
        //            foreach (var item in query)
        //            {
        //                comment += "<tr><td>" + item.actor + 
        //                    "</td><td>" + item.TM_Action.act_name +
        //                    "</td><td>" + item.TM_Level.lv_name + 
        //                    "</td><td>" + item.act_dt + 
        //                    "</td><td>" + item.comment + "</td></tr>";
        //            }
        //            comment += "</tbody></table>";
        //        }
        //    }
        //    return Json(comment, JsonRequestBehavior.AllowGet);
        //}

        //public List<byte> GetPosition(byte lv)
        //{
        //    List<byte> list_postion = new List<byte>();
        //    var get_minmax = (from m in dbSA.TM_Level
        //                       where m.lv_id == lv
        //                       select new { m.position_min, m.position_max }).FirstOrDefault();
        //    if (get_minmax != null)
        //    {
        //        var get_position = from p in dbTNC.tnc_position_master
        //                           where p.position_level >= get_minmax.position_min
        //                           && p.position_level <= get_minmax.position_max
        //                           select p.id;
        //        foreach (var item in get_position)
        //        {
        //            list_postion.Add((byte)item);
        //        }

        //    }
        //    return list_postion;
        //}

        //[OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        //public ActionResult RenderStatus(int id)
        //{
        //    string status_menu = "";
        //    int status = GetStatusId(id);
        //    using(var db = new SAEntities()){
        //        var query = from a in db.TM_Status
        //                    where a.status_id <= status
        //                    orderby a.status_id
        //                    select a;
        //        foreach (var item in query)
        //        {
        //            if (item.status_id != 0)
        //            {
        //                status_menu += "<div class='accordion-group'>"
        //                    + "<div class='accordion-heading'>"
        //                    + "<a class='accordion-toggle collapsed' data-toggle='collapse' href='#collapse" // data-parent='#divSADetail'
        //                    + item.status_id + "' data-id='" + id + "'>" + item.status_name + "</a></div>"
        //                    + "<div id='collapse" + item.status_id + "' class='accordion-body collapse'>"
        //                        + "<div class='accordion-inner'>"
        //                        + "</div>"
        //                    + "</div>"
        //                    + "</div>";
        //            }
        //            else
        //            {
        //                status_menu += "<div class='accordion-group'>"
        //                    + "<div class='accordion-heading'>"
        //                    + "<a class='accordion-toggle' data-toggle='collapse' href='#main" // data-parent='#divSADetail'
        //                    + item.status_id + "'>" + item.status_name + "</a></div>"
        //                    + "<div id='main" + item.status_id + "' class='accordion-body collapse in'>"
        //                        + "<div class='accordion-inner'>"
        //                        + GetMainData(id)
        //                        //+ "<div class='row-fluid'>"
        //                        //    + "<div class='span6'></div>"
        //                        //    + "<div class='span6'></div>"
        //                        //+ "</div>"
        //                        //+ "<button type='button' class='btn btn-danger' id='btnDialogFiles' name='btnDialogFiles' data-id='" + id + "'>Open Attach Files Dialog</button>"
        //                        + "</div>"
        //                    + "</div>"
        //                    + "</div>";
        //            }
        //        }

        //    }
        //    return Json(status_menu, JsonRequestBehavior.AllowGet);
        //}
        //[OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        //public ActionResult GetAttachFiles(int id)
        //{
        //    string attach_files = "";
        //    using (var db = new SAEntities())
        //    {
        //        var query = from a in db.TD_File
        //                    where a.id == id
        //                    orderby a.upload_dt descending
        //                    select a;
        //        if (query != null)
        //        {
        //            attach_files = "<table class='table'><thead><tr><th>File Name</th><th>Upload Date</th></tr></thead><tbody>";
        //            foreach (var item in query)
        //            {
        //                attach_files += "<tr><td>";
        //                var x = item.file_path.Split('/');
        //                var filename = x[x.Length - 1];
        //                attach_files += "<a href='" + Url.Content(item.file_path) + "' target='_blank' >" + filename + "</a></td>";
        //                attach_files += "<td>" + item.upload_dt + "</td></tr>";
        //            }
        //        }
        //    }
        //    return Json(attach_files, JsonRequestBehavior.AllowGet);
        //}

        //[OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        //public ActionResult CreateDetailPartial(int id)
        //{
        //    return PartialView("_Detail", id);
        //}

        //public ActionResult GridSAOnProcess(string sidx, string sord, int page, int rows, string search, string control_no, string jobno, string startdate, string enddate)
        //{
        //    CultureInfo enUS = new CultureInfo("en-US");

        //    DateTime startValue, endValue;
        //    var dbSel = from d in dbSA.TD_Main
        //                //where d.status_id > 0 && d.status_id < 10
        //                select d;
        //    if (!string.IsNullOrEmpty(control_no))
        //        dbSel = dbSel.Where(q => q.control_no.Contains(control_no));

        //    if (!string.IsNullOrEmpty(jobno))
        //        dbSel = dbSel.Where(q => q.job_no.Contains(jobno));

        //    if (DateTime.TryParseExact(startdate, "dd-MM-yyyy", enUS, DateTimeStyles.None, out startValue))
        //        dbSel = dbSel.Where(q => q.issue_dt >= startValue);

        //    if (DateTime.TryParseExact(enddate, "dd-MM-yyyy", enUS, DateTimeStyles.None, out endValue))
        //        dbSel = dbSel.Where(q => q.issue_dt <= endValue);

        //    var pageIndex = Convert.ToInt32(page) - 1;
        //    var pageSize = rows;
        //    var totalRecords = dbSel.Count();
        //    var totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);

        //    // This is possible because I'm using the LINQ Dynamic Query Library
        //    var on_process = dbSel.OrderBy(sidx + " " + sord)
        //            .Skip(pageIndex * pageSize)
        //            .Take(pageSize).AsEnumerable();

        //    var jsonData = new
        //    {
        //        total = totalPages,
        //        page = page,
        //        records = totalRecords,
        //        rows = on_process.ToList().Select(m => new
        //        {
        //            i = m.id,
        //            cell = new object[] {
        //                m.control_no,
        //                m.title,
        //                m.job_no,
        //                m.batch_no,
        //                m.reason,
        //                m.TM_Status.status_name
        //                //m.picture != null ? "<a class='fancybox' href='" + Url.Content("~/" + m.picture) + "'><img src='" + Url.Content("~/" + m.picture) + "' class='gridImage' /></a>" : "<img src='" + Url.Content("~/" + "Images/noimage.jpg") + "' class='gridImage' />",
        //                //m.issue_car == 1 ? "<a href='#' class='btCar' data-rejId=" + m.qc_reject_id + "><img src='" + Url.Content("~/" + "Images/Document-icon.png") + "' /></a>" : ""
        //            }
        //        }).ToArray()
        //    };

        //    return Json(jsonData);
        //}

        //[HttpPost]
        //public JsonResult JTableTest(string control_no, string title, string job_no, string batch_no, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        //{
        //    try
        //    {
        //        var query = from m in dbSA.TD_Main
        //                    select m;
        //        if (!string.IsNullOrEmpty(control_no))
        //        {
        //            query = query.Where(w => w.control_no.Contains(control_no));
        //        }
        //        if (!string.IsNullOrEmpty(title))
        //        {
        //            query = query.Where(w => w.title.Contains(title));
        //        }
        //        if (!string.IsNullOrEmpty(job_no))
        //        {
        //            query = query.Where(w => w.job_no.Contains(job_no));
        //        }
        //        if (!string.IsNullOrEmpty(batch_no))
        //        {
        //            query = query.Where(w => w.batch_no.Contains(batch_no));
        //        }
        //        //Get data from database
        //        int TotalRecord = query.Count();

        //        // Paging
        //        var output = query
        //            .Select(s => new
        //            {
        //                s.control_no,
        //                s.title,
        //                s.job_no,
        //                s.batch_no,
        //                s.reason,
        //                status_name = s.TM_Status.status_name
        //            }).AsQueryable().OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);
        //        //.Select(s => new
        //        //{
        //        //    s.Detail,
        //        //    s.Textbox,
        //        //    EntryDate = s.EntryDate.HasValue ? s.EntryDate : null,
        //        //    UpdateDate = s.UpdateDate.HasValue ? s.UpdateDate : null
        //        //});

        //        //Return result to jTable
        //        return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}
    }
}