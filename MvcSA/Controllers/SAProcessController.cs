using MvcSA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using MvcSA.Models;
using System.Globalization;
using System.Transactions;
using System.IO;
using WebCommonFunction;
using Rotativa;

namespace MvcSA.Controllers
{
    public class SAProcessController : Controller
    {
        SAEntities dbSA = new SAEntities();
        PCREntities dbPCR = new PCREntities();
        TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();

        [Check_Authen]
        public ActionResult Index()
        {
            ViewBag.Menu = 1;
            ViewBag.Title = "Main";
            var status = from s in dbSA.TM_Status
                         select s;

            ViewBag.Status = status;
            
            ViewBag.IssueGroup = from a in dbTNC.tnc_group_master
                                 orderby a.group_name
                                 select a;
            return View();
        }

        [Check_Authen]
        public ActionResult Index2()
        {
            ViewBag.Menu = 1;
            ViewBag.Title = "Main";
            var status = from s in dbSA.TM_Status
                         select s;

            ViewBag.Status = status;

            ViewBag.IssueGroup = from a in dbTNC.tnc_group_master
                                 orderby a.group_name
                                 select a;
            return View();
        }

        [Check_Authen]
        public ActionResult SAClosed()
        {
            ViewBag.Title = "Closed";
            var status = from s in dbSA.TM_Status
                         select s;

            ViewBag.Status = status;

            ViewBag.IssueGroup = from a in dbTNC.tnc_group_master
                                 orderby a.group_name
                                 select a;
            return View();
        }

        [Check_Authen]
        public ActionResult SADetail(int id)
        {
            return View();
        }

        [Check_Authen]
        public ActionResult Detail(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        
        [Check_Authen]
        public ActionResult NewSA()
        {
            if (Session["SA_UserLv"].ToString() == "1" || Session["SA_UserLv"].ToString() == "2")
            {
                ViewBag.Title = "Special Acceptance Requisition Form";
                ViewBag.Menu = 2;
                
                var plant = from a in dbSA.TM_SysGroup
                            where a.Sys_GroupType_id == 2
                            select a;
                ViewBag.SysPlant = plant;

                if (Session["SA_Org"] != null)
                {
                    ViewBag.ControlNo = InitialControlNo(Session["SA_Org"].ToString());
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "SAProcess");
            }
        }

        [Check_Authen]
        public ActionResult NewSA1()
        {
            if (Session["SA_UserLv"].ToString() == "1" || Session["SA_UserLv"].ToString() == "2")
            {
                ViewBag.Title = "Special Acceptance Requisition Form";
                ViewBag.Menu = 2;

                var plant = from a in dbSA.TM_SysGroup
                            where a.Sys_GroupType_id == 2
                            select a;
                ViewBag.SysPlant = plant;

                if (Session["SA_Org"] != null)
                {
                    ViewBag.ControlNo = InitialControlNo(Session["SA_Org"].ToString());
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "SAProcess");
            }
        }

        [Check_Authen]
        public ActionResult NewSA2()
        {
            if (Session["SA_UserLv"].ToString() == "1" || Session["SA_UserLv"].ToString() == "2")
            {
                ViewBag.Title = "Special Acceptance Requisition Form";
                ViewBag.Menu = 2;

                var plant = from a in dbSA.TM_SysGroup
                            where a.Sys_GroupType_id == 2
                            select a;
                ViewBag.SysPlant = plant;

                if (Session["SA_Org"] != null)
                {
                    ViewBag.ControlNo = InitialControlNo(Session["SA_Org"].ToString());
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "SAProcess");
            }

        }

        [Check_Authen]
        public ActionResult EditSA(int id)
        {
            ViewBag.Title = "Edit SA";
            var query = (from a in dbSA.TD_Main
                         where a.id == id
                         select a).FirstOrDefault();

            var plant = from a in dbSA.TM_SysGroup
                        where a.Sys_GroupType_id == 2
                        select a;
            ViewBag.SAID = id;
            ViewBag.SysPlant = plant;
            return View(query);
        }
        
        [Check_Authen_Admin]
        public ActionResult SpecialEdit(int id)
        {
            ViewBag.Title = "Edit SA";
            var query = (from a in dbSA.TD_Main
                         where a.id == id
                         select a).FirstOrDefault();

            var plant = from a in dbSA.TM_SysGroup
                        where a.Sys_GroupType_id == 2
                        select a;
            ViewBag.SAID = id;
            ViewBag.SysPlant = plant;
            return View(query);
        }


        public ActionResult SpecialReject(int id)
        {
            var actor = Session["SA_Auth"].ToString();
            int org = int.Parse(Session["SA_Org"].ToString());
            byte lv = byte.Parse(Session["SA_UserLv"].ToString());

            var tran = from a in dbSA.TD_Transaction
                       where a.id == id && a.active == true
                       select a;

            foreach (var item in tran)
            {
                if (item.act_id > 1)
                {
                    item.act_id = 99;
                }
                item.act_dt = DateTime.Now;
                item.active = false;
            }
            dbSA.SaveChanges();

            Insert_Transaction(id, 101, lv, org, false);

            return RedirectToAction("Index","SAProcess");
        }

        [Check_Authen]
        public ActionResult IssueSA(int sid)
        {
            ViewBag.Title = "Issue SA";
            ViewBag.SAID = sid;
            ViewBag.QAOrg = Session["SA_Org"];
            ViewBag.Result = dbSA.TM_Result;
            ViewBag.Complete = CheckIssueSAComplete(sid);
            return View();
        }

        [Check_Authen]
        public ActionResult Report()
        {
            ViewBag.Title = "Print Report";
            ViewBag.Menu = 3;
            var sacomplete = from a in dbSA.TD_Transaction
                             select a;
            return View(sacomplete);
        }

        public ActionResult ReportPDF(int id)
        {
            ViewBag.Title = "SA Report";
            
            var report = (from a in dbSA.TD_Main
                          where a.id == id
                          select a).FirstOrDefault();
            
            ViewBag.Comment = from c in dbSA.TD_Transaction
                              where c.id == id && (c.act_id != null && c.act_id != 0) && c.act_dt != null
                              orderby c.status_id, c.lv_id
                              select c;
            return View(report);
        }

        public ActionResult PrintPDF1(int said)
        {
            var id = said;
            return new ActionAsPdf("ReportPDF", new { id = said });// { FileName = "Test.pdf" };
        }

        public ActionResult PrintPDF()
        {
            int id = int.Parse(Request.Form["slSA"]);

            return new ActionAsPdf("ReportPDF", new { id = id });// { FileName = "Test.pdf" };
        }

        [Check_Authen]
        public ActionResult ExcelFilter()
        {
            ViewBag.Title = "Report Filter";
            ViewBag.Menu = 3;
            return View();
        }

        [Check_Authen]
        public ActionResult _ShowDetail(int id)
        {
            var user = Session["SA_Auth"].ToString();
            byte user_type = byte.Parse(Session["SA_UType"].ToString());

            if (user_type == 2) ViewBag.SPEdit = true;

            var main = dbSA.TD_Main.Where(w => w.id == id).FirstOrDefault();
            
            byte current_status = GetStatusId(id);
            
            if (current_status == 7 && CheckIsQAComment(id,user))
                ViewBag.IssueSA = 1;
            else
                ViewBag.IssueSA = 0;

            if (current_status > 2) ViewBag.ShowConcern = true;

            ViewBag.ShowIssueSA = Check_IssueSA(id);

            ViewBag.SAID = id;
            ViewBag.ShowCarPar = Check_CarPar(id);

            return PartialView(main);
        }

        //private string GetConcernGroup(int id)
        //{
        //    string text = "";
        //    var getqc = from a in dbSA.TD_ConcernQC
        //                where a.id == id
        //                select a.group_id;
        //    var getother = from b in dbSA.TD_ConcernOther
        //                   where b.id == id
        //                   select b.group_id;
        //    var geten = from c in dbSA.TD_ConcernEN
        //                where c.id == id
        //                select c.group_id;
        //    var qc = "";
        //    foreach (var item in getqc)
        //    {
        //        qc += "," + item;
        //    }
        //    qc = qc.Substring(1);
        //    var other = "";
        //    foreach (var item in getother)
        //    {
        //        other += "," + item;
        //    }
        //    other = other.Substring(1);
        //    var en = "";
        //    foreach (var item in geten)
        //    {
        //        en += "," + item;
        //    }
        //    en = en.Substring(1);

        //    text = 

        //    return text;
        //}

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult _ShowComment(int id, byte status_id)
        {
            string user = Session["SA_Auth"].ToString();
            byte user_lv = byte.Parse(Session["SA_UserLv"].ToString());
            int user_org = int.Parse(Session["SA_Org"].ToString());
            byte user_type = byte.Parse(Session["SA_UType"].ToString());

            if (user_type == 2)
            {
                ViewBag.SPEdit = true;
            }

            List<VM_Comment> vobj = new List<VM_Comment>();
            VM_Comment vm;
            var comment = from t in dbSA.TD_Transaction
                          where t.id == id && t.status_id == status_id //&& t.act_id != null
                          orderby t.act_dt ascending
                          select t;

            foreach (var item in comment)
            {
                vm = new VM_Comment();
                vm.id = item.id;
                vm.actor = GetUserName(item.actor);
                vm.act_name = item.act_id != null ? item.TM_Action.act_name : "Wait";
                vm.act_dt = item.act_dt;
                vm.status_name = item.TM_Status.status_name;
                vm.lv_name = item.TM_Level.lv_name;
                vm.comment = item.comment;
                vm.plant = item.TM_SysPlant.Sys_Plant_name;
                vm.org_name = GetOrgName(item.lv_id, item.org_id);

                vobj.Add(vm);
            }
            
            var query = (from a in dbSA.TD_Transaction
                         where a.id == id && a.active == true && a.status_id == status_id 
                         && a.lv_id == user_lv && a.org_id == user_org
                         select a).FirstOrDefault();
            ViewBag.SAID = id;

            if (query != null)//Have Transaction <-> User
            {
                if (status_id == 1)//Issuer
                {
                    var issuer = (from a in dbSA.TD_Main
                                  where a.id == id
                                  select a.issue_by).FirstOrDefault();
                    if (issuer == user)
                    {
                        ViewBag.ShowForm = 11;//Edit SA
                    }
                    else
                    {
                        ViewBag.ShowForm = user_lv;
                    }
                }
                else if (status_id == 2)//QA
                {
                    if (user_lv == 1)//Eng.
                    {
                        if (Check_SysUser(user, status_id, user_lv, query.Sys_Plant_id))
                        {
                            ViewBag.ShowForm = user_lv;
                        }
                    }
                    else if (user_lv == 2)//Mgr.
                    {
                        ViewBag.ShowForm = 12;//ApproveS1 Form
                    }
                }
                else if (status_id == 3 && user_lv <= 2)//QC and Mgr. Down
                {
                    ViewBag.ShowForm = user_lv;
                    ViewBag.CarPar = Check_CarPar(id);
                }
                else
                {
                    ViewBag.ShowForm = user_lv;
                }
            }
            else
            {
                if (user_lv == 3)//Dept.
                {
                    var get_all_group = from a in dbTNC.View_Organization
                                        where a.dept_id == user_org && a.group_id != 0
                                        select a.group_id;
                    foreach (var item in get_all_group)
                    {
                        var check_group = (from c in dbSA.TD_Transaction
                                           where c.id == id && c.active == true && c.status_id == status_id
                                           && c.org_id == item
                                           select c).FirstOrDefault();
                        if (check_group != null)
                        {
                            ViewBag.ShowForm = status_id != 2 ? user_lv : 12;//Update 2014-08-25 by Monchit // 12 is Form S1
                            break;
                        }
                    }
                }
                else if (user_lv == 4)//Plant/Div.
                {
                    var get_all_dept = from a in dbTNC.View_Organization
                                       where a.plant_id == user_org && a.dept_id != 0
                                       select a.dept_id;
                    foreach (var item in get_all_dept)
                    {
                        var check_dept = (from c in dbSA.TD_Transaction
                                           where c.id == id && c.active == true && c.status_id == status_id
                                           && c.org_id == item
                                           select c).FirstOrDefault();
                        if (check_dept != null)
                        {
                            ViewBag.ShowForm = status_id != 2 ? user_lv : 12;//Update 2014-08-25 by Monchit // 12 is Form S1
                            break;
                        }
                    }
                }
            }

            //Special Approve root (QC Div.)
            if (status_id == 8 && user_lv == 4 && user_org == 11)//user_org : 11 is QC Div.
            {
                var final = (from a in dbSA.TD_Transaction
                             where a.id == id && a.active == true && a.status_id == status_id
                             select a).FirstOrDefault();
                if (final != null)
                {
                    ViewBag.ShowForm = 4;
                }
            }

            //Change Final Dicision Judgement
            //var chgFinal = (from a in dbSA.TD_Transaction
            //                where a.id == id && (a.status_id == 100 || a.status_id == 101)
            //                && a.lv_id == user_lv && a.org_id == user_org
            //                select a).FirstOrDefault();
            //if (chgFinal != null)
            //{
            //    ViewBag.ShowForm = user_lv;
            //}

            return PartialView(vobj);
        }

        private bool CheckIsQAComment(int id, string user)
        {
            var query = from a in dbSA.TD_Transaction
                        where a.id == id && a.status_id == 2 && a.lv_id == 1 && a.actor == user
                        select a;
            if (query.Any()) return true;
            else return false;
        }

        //[OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult _ShowFiles(int id)
        {
            List<VM_AttachFiles> vobj = new List<VM_AttachFiles>();
            VM_AttachFiles vm;
            var files = from t in dbSA.TD_File
                             where t.id == id
                             select t;
            
            foreach (var item in files)
            {
                vm = new VM_AttachFiles();
                vm.id = item.id;
                vm.file_id = item.file_id;
                vm.file_path = item.file_path;
                vm.upload_by = GetUserName(item.upload_by);
                vm.upload_dt = item.upload_dt;

                vobj.Add(vm);
            }
            ViewBag.IsIssuer = CheckIsIssuer(id);
            ViewBag.SAID = id;
            return PartialView(vobj);
        }
        
        public ActionResult _ShowTellIssuer(int id)
        {
            List<VM_TellIssuer> vobj = new List<VM_TellIssuer>();
            VM_TellIssuer vm;
            var tellissuer = from t in dbSA.TD_Tell
                             where t.id == id
                             select t;

            foreach (var item in tellissuer)
            {
                vm = new VM_TellIssuer();
                vm.id = item.id;
                vm.tell_user = GetUserName(item.tell_user);
                vm.tell_content = item.tell_content;
                vm.tell_id = item.tell_id;
                vm.tell_dt = item.tell_dt;
                vm.reply_dt = item.reply_dt;

                vobj.Add(vm);
            }

            ViewBag.IsIssuer = CheckIsIssuer(id);

            return PartialView(vobj);
        }

        public ActionResult _ShowCarPar(int id)
        {
            var get_carpar = (from pc in dbSA.TD_CarPar
                              where pc.id == id
                              select pc).FirstOrDefault();
            return PartialView(get_carpar);
        }

        public ActionResult _ShowSA(int id)
        {
            var get_sa = from a in dbSA.TD_SA
                         where a.id == id
                         select a;
            return PartialView(get_sa);
        }

        public ActionResult _ShowConcern(int id)
        {
            ViewBag.ConcernID = id;
            return PartialView();
        }

        public ActionResult _FormNewSA(int id = 0)
        {
            var plant = from p in dbSA.TM_SysPlant
                        where p.del_flag == false
                        select p;
            ViewBag.SysPlant = plant;

            if (Session["SA_Org"] != null)
                ViewBag.ControlNo = InitialControlNo(Session["SA_Org"].ToString());
            
            return PartialView();
        }

        public ActionResult _FormApproveS1(int id)
        {
            byte lv = byte.Parse(Session["SA_UserLv"].ToString());
            byte status = GetStatusId(id);

            string[] list;
            var query = (from a in dbSA.TM_ActionMapping
                         where a.status_id == status && a.lv_id == lv
                         select a.action_list).FirstOrDefault();
            if (query != null)
            {
                char[] delimiter = new char[] { ',' };
                list = query.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var action = from a in dbSA.TM_Action.ToList()
                             where list.Contains(a.act_id.ToString())
                             select a;
                ViewBag.Action = action;
            }
            ViewBag.SAID = id;
            ViewBag.STID = status;

            var get_plant = (from p in dbSA.TD_Main
                             where p.id == id
                             select p.Sys_Plant_id).FirstOrDefault();

            var get_qa = from q in dbSA.TM_SysGroup
                         where q.del_flag == false && q.Sys_GroupType_id == 2 && q.Sys_Plant_id != get_plant
                         select q;

            ViewBag.QAGroup = get_qa;
            
            return PartialView();
        }

        public ActionResult _FormApprove(int id)
        {
            byte utype = byte.Parse(Session["SA_UType"].ToString());
            byte lv = byte.Parse(Session["SA_UserLv"].ToString());
            byte status = GetStatusId(id);
            if (status == 8 && utype == 3)
            {
                string[] list;
                var query = (from a in dbSA.TM_ActionMapping
                             where a.status_id == status && a.lv_id == 3
                             select a.action_list).FirstOrDefault();
                if (query != null)
                {
                    char[] delimiter = new char[] { ',' };
                    list = query.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    var action = from a in dbSA.TM_Action.ToList()
                                 where list.Contains(a.act_id.ToString())
                                 select a;
                    ViewBag.Action = action;
                }
            }
            else
            {
                string[] list;
                var query = (from a in dbSA.TM_ActionMapping
                             where a.status_id == status && a.lv_id == lv
                             select a.action_list).FirstOrDefault();
                if (query != null)
                {
                    char[] delimiter = new char[] { ',' };
                    list = query.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    var action = from a in dbSA.TM_Action.ToList()
                                 where list.Contains(a.act_id.ToString())
                                 select a;
                    ViewBag.Action = action;
                }
            }

            ViewBag.SAID = id;
            ViewBag.STID = status;
            return PartialView();
        }

        public ActionResult _FormCarPar(int id)
        {
            ViewBag.SAID = id;
            return PartialView();
        }

        public ActionResult _FormTellIssuer(int id)
        {
            ViewBag.SAID = id;
            byte status = GetStatusId(id);
            ViewBag.STID = status;
            return PartialView();
        }

        public ActionResult _FormIssuerUpload(int id)
        {
            ViewBag.SAID = id;
            return PartialView();
        }

        public ActionResult _SectionUploadFiles()
        {
            return PartialView();
        }

        public ActionResult _SectionSelQA()
        {
            var get_group = from g in dbTNC.View_Organization
                            where g.dept_id == 13 && g.group_id != 0 && g.active == true && g.IsRealCostCode == 1
                            group g by new { g.group_id, g.group_name } into v
                            select v;
            return PartialView(get_group);
        }

        [Check_Authen]
        [HttpPost]
        public ActionResult AddSA(IEnumerable<HttpPostedFileBase> files)
        {
            // define our transaction scope
            var scope = new TransactionScope(
                // a new transaction will always be created
                TransactionScopeOption.RequiresNew,
                // we will allow volatile data to be read during transaction
                new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
                    Timeout = TransactionManager.MaximumTimeout
                }
            );
            DateTime dt = DateTime.Now;
            int main_id;
            var controlno = Request.Form["hdControlNo"] != null ? Request.Form["hdControlNo"].ToString() : string.Empty;

            try
            {
                using (scope)
                {
                    // Add data to DB TD_Main //
                    TD_Main main = new TD_Main();
                    main.control_no = controlno + GetRunNo(controlno).ToString("000");
                    main.title = Request.Form["txtNonconformities"] != null ? Request.Form["txtNonconformities"].ToString() : null;
                    main.item_code = Request.Form["txtItemcode"] != null ? Request.Form["txtItemcode"].ToString() : null;
                    main.customer = Request.Form["txtCustomer"] != null ? Request.Form["txtCustomer"].ToString() : null;
                    main.issue_dt = dt;
                    main.material = Request.Form["txtMaterial"] != null ? Request.Form["txtMaterial"].ToString() : null;
                    main.batch_no = Request.Form["txtBatchNo"] != null ? Request.Form["txtBatchNo"].ToString() : null;
                    main.job_no = Request.Form["txtJobNo"] != null ? Request.Form["txtJobNo"].ToString() : null;
                    main.defective_qty = Request.Form["txtDefectiveQty"] != null ? Request.Form["txtDefectiveQty"].ToString() : "0";
                    main.expect_date = Request.Form["dtpExpectDate"] != null ? ParseToDate(Request.Form["dtpExpectDate"].ToString()) : dt;
                    main.reason = Request.Form["txaReason"] != null ? Request.Form["txaReason"].ToString() : "";
                    main.preventive = Request.Form["txaPreventive"] != null ? Request.Form["txaPreventive"].ToString() : "";
                    main.effective_dt = Request.Form["dtpEffectiveDate"] != null ? ParseToDate(Request.Form["dtpEffectiveDate"].ToString()) : dt;
                    main.issue_by = Session["SA_Auth"].ToString();
                    main.Sys_Plant_id = byte.Parse(Request.Form["slPlant"].ToString());

                    dbSA.TD_Main.Add(main);
                    dbSA.SaveChanges();
                    // End Add data DB to TD_Main //
                    main_id = main.id;
                    // Add data to DB TD_Problem //
                    if (Request.Form["cbxProblem"] != null)
                    {
                        var problem = Request.Form["cbxProblem"].ToString();
                        var listProb = problem.Split(',');
                        foreach (var item in listProb)
                        {
                            var problem_id = byte.Parse(item);
                            var query = (from a in dbSA.TM_Problem
                                         where a.prob_id == problem_id
                                         select a).First();
                            TD_Problem prob = new TD_Problem();
                            prob.id = main_id;
                            prob.prob_id = problem_id;
                            if (query.text_flag)
                            {
                                prob.prob_text = Request.Form["txtProb" + item].ToString();
                            }
                            dbSA.TD_Problem.Add(prob);
                        }
                    }
                    // End Add data DB to TD_Problem //

                    // Add data to DB TD_Disposition //
                    if (Request.Form["rdoDispos"] != null)
                    {
                        var dispos_id = byte.Parse(Request.Form["rdoDispos"].ToString());
                        TD_Disposition disp = new TD_Disposition();
                        disp.id = main_id;
                        disp.dispos_id = dispos_id;
                        var query = (from a in dbSA.TM_Disposition
                                     where a.dispos_id == dispos_id
                                     select a).First();
                        if (query.text_flag)
                        {
                            disp.dispos_text = Request.Form["txtDisp" + dispos_id].ToString();
                        }
                        dbSA.TD_Disposition.Add(disp);
                    }
                    // End Add data to DB TD_Disposition //

                    // Add data to DB TD_File //
                    string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/" + main_id + "/";
                    if (!Directory.Exists(Server.MapPath(subPath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(subPath));
                    }
                    foreach (var file in files)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            if (file.ContentType == "application/pdf")//Check accept file type.
                            {
                                var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(file.FileName);
                                var path = Path.Combine(Server.MapPath(subPath), fileName);
                                file.SaveAs(path);
                                SaveFiletoDB(main_id, subPath + fileName, Session["SA_Auth"].ToString());
                            }
                        }
                    }

                    // End Add data to DB TD_File //
                    dbSA.SaveChanges();

                    //var concern_qc = Request.Form["selectQC"] != null ? Request.Form["selectQC"].ToString() : null;
                    //var concern_en = Request.Form["selectEN"] != null ? Request.Form["selectEN"].ToString() : null;
                    //var concern_other = Request.Form["selectOther"] != null ? Request.Form["selectOther"].ToString() : null;
                    //var inform = Request.Form["selectInform"] != null ? Request.Form["selectInform"].ToString() : null;
                    //AddConcernQC(main_id, concern_qc);
                    //AddConcernEN(main_id, concern_en);
                    //AddConcernOther(main_id, concern_other);
                    //AddInform(main_id, inform);

                    scope.Complete();
                }

                // Add data to DB TD_Transaction //
                int org_id = int.Parse(Session["SA_Org"].ToString());
                byte lv = byte.Parse(Session["SA_UserLv"].ToString());

                NextStep_Transaction(main_id, 1, lv, org_id, 0);
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index", "SAProcess");
        }

        [Check_Authen]
        [HttpPost]
        public ActionResult AddSA1(IEnumerable<HttpPostedFileBase> files)
        {
            // define our transaction scope
            var scope = new TransactionScope(
                // a new transaction will always be created
                TransactionScopeOption.RequiresNew,
                // we will allow volatile data to be read during transaction
                new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
                    Timeout = TransactionManager.MaximumTimeout
                }
            );
            DateTime dt = DateTime.Now;
            int main_id;
            var controlno = Request.Form["hdControlNo"] != null ? Request.Form["hdControlNo"].ToString() : string.Empty;

            try
            {
                using (scope)
                {
                    // Add data to DB TD_Main //
                    TD_Main main = new TD_Main();
                    main.control_no = controlno + GetRunNo(controlno).ToString("000");
                    main.title = Request.Form["txtNonconformities"] != null ? Request.Form["txtNonconformities"].ToString() : null;
                    main.item_code = Request.Form["txtItemcode"] != null ? Request.Form["txtItemcode"].ToString() : null;
                    main.customer = Request.Form["txtCustomer"] != null ? Request.Form["txtCustomer"].ToString() : null;
                    main.issue_dt = dt;
                    main.material = Request.Form["txtMaterial"] != null ? Request.Form["txtMaterial"].ToString() : null;
                    main.batch_no = Request.Form["txtBatchNo"] != null ? Request.Form["txtBatchNo"].ToString() : null;
                    main.job_no = Request.Form["txtJobNo"] != null ? Request.Form["txtJobNo"].ToString() : null;
                    main.defective_qty = Request.Form["txtDefectiveQty"] != null ? Request.Form["txtDefectiveQty"].ToString() : "0";
                    main.expect_date = Request.Form["dtpExpectDate"] != null ? ParseToDate(Request.Form["dtpExpectDate"].ToString()) : dt;
                    main.reason = Request.Form["txaReason"] != null ? Request.Form["txaReason"].ToString() : "";
                    main.preventive = Request.Form["txaPreventive"] != null ? Request.Form["txaPreventive"].ToString() : "";
                    main.effective_dt = Request.Form["dtpEffectiveDate"] != null ? ParseToDate(Request.Form["dtpEffectiveDate"].ToString()) : dt;
                    main.issue_by = Session["SA_Auth"].ToString();
                    main.Sys_Plant_id = byte.Parse(Request.Form["slPlant"].ToString());

                    dbSA.TD_Main.Add(main);
                    dbSA.SaveChanges();
                    // End Add data DB to TD_Main //
                    main_id = main.id;
                    // Add data to DB TD_Problem //
                    if (Request.Form["cbxProblem"] != null)
                    {
                        var problem = Request.Form["cbxProblem"].ToString();
                        var listProb = problem.Split(',');
                        foreach (var item in listProb)
                        {
                            var problem_id = byte.Parse(item);
                            var query = (from a in dbSA.TM_Problem
                                         where a.prob_id == problem_id
                                         select a).First();
                            TD_Problem prob = new TD_Problem();
                            prob.id = main_id;
                            prob.prob_id = problem_id;
                            if (query.text_flag)
                            {
                                prob.prob_text = Request.Form["txtProb" + item].ToString();
                            }
                            dbSA.TD_Problem.Add(prob);
                        }
                    }
                    // End Add data DB to TD_Problem //

                    // Add data to DB TD_Disposition //
                    if (Request.Form["rdoDispos"] != null)
                    {
                        var dispos_id = byte.Parse(Request.Form["rdoDispos"].ToString());
                        TD_Disposition disp = new TD_Disposition();
                        disp.id = main_id;
                        disp.dispos_id = dispos_id;
                        var query = (from a in dbSA.TM_Disposition
                                     where a.dispos_id == dispos_id
                                     select a).First();
                        if (query.text_flag)
                        {
                            disp.dispos_text = Request.Form["txtDisp" + dispos_id].ToString();
                        }
                        dbSA.TD_Disposition.Add(disp);
                    }
                    // End Add data to DB TD_Disposition //

                    // Add data to DB TD_File //
                    string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/" + main_id + "/";
                    if (!Directory.Exists(Server.MapPath(subPath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(subPath));
                    }
                    foreach (var file in files)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            if (file.ContentType == "application/pdf")//Check accept file type.
                            {
                                var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(file.FileName);
                                var path = Path.Combine(Server.MapPath(subPath), fileName);
                                file.SaveAs(path);
                                SaveFiletoDB(main_id, subPath + fileName, Session["SA_Auth"].ToString());
                            }
                        }
                    }

                    // End Add data to DB TD_File //
                    dbSA.SaveChanges();

                    var concern_qc = Request.Form["selectQC"] != null ? Request.Form["selectQC"].ToString() : null;
                    AddConcernQC(main_id, concern_qc);

                    scope.Complete();
                }

                // Add data to DB TD_Transaction //
                int org_id = int.Parse(Session["SA_Org"].ToString());
                byte lv = byte.Parse(Session["SA_UserLv"].ToString());

                NextStep_Transaction(main_id, 1, lv, org_id, 0);
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index", "SAProcess");
        }

        [Check_Authen]
        [HttpPost]
        public ActionResult AddSA2(IEnumerable<HttpPostedFileBase> files)
        {
            // define our transaction scope
            var scope = new TransactionScope(
                // a new transaction will always be created
                TransactionScopeOption.RequiresNew,
                // we will allow volatile data to be read during transaction
                new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
                    Timeout = TransactionManager.MaximumTimeout
                }
            );
            DateTime dt = DateTime.Now;
            int main_id;
            var controlno = Request.Form["hdControlNo"] != null ? Request.Form["hdControlNo"].ToString() : string.Empty;

            try
            {
                using (scope)
                {
                    // Add data to DB TD_Main //
                    TD_Main main = new TD_Main();
                    main.control_no = controlno + GetRunNo(controlno).ToString("000");
                    main.title = Request.Form["txtNonconformities"] != null ? Request.Form["txtNonconformities"].ToString() : null;
                    main.item_code = Request.Form["txtItemcode"] != null ? Request.Form["txtItemcode"].ToString() : null;
                    main.customer = Request.Form["txtCustomer"] != null ? Request.Form["txtCustomer"].ToString() : null;
                    main.issue_dt = dt;
                    main.material = Request.Form["txtMaterial"] != null ? Request.Form["txtMaterial"].ToString() : null;
                    main.batch_no = Request.Form["txtBatchNo"] != null ? Request.Form["txtBatchNo"].ToString() : null;
                    main.job_no = Request.Form["txtJobNo"] != null ? Request.Form["txtJobNo"].ToString() : null;
                    main.defective_qty = Request.Form["txtDefectiveQty"] != null ? Request.Form["txtDefectiveQty"].ToString() : "0";
                    main.expect_date = Request.Form["dtpExpectDate"] != null ? ParseToDate(Request.Form["dtpExpectDate"].ToString()) : dt;
                    main.reason = Request.Form["txaReason"] != null ? Request.Form["txaReason"].ToString() : "";
                    main.preventive = Request.Form["txaPreventive"] != null ? Request.Form["txaPreventive"].ToString() : "";
                    main.effective_dt = Request.Form["dtpEffectiveDate"] != null ? ParseToDate(Request.Form["dtpEffectiveDate"].ToString()) : dt;
                    main.issue_by = Session["SA_Auth"].ToString();
                    main.Sys_Plant_id = byte.Parse(Request.Form["slPlant"].ToString());

                    dbSA.TD_Main.Add(main);
                    dbSA.SaveChanges();
                    // End Add data DB to TD_Main //
                    main_id = main.id;
                    // Add data to DB TD_Problem //
                    if (Request.Form["cbxProblem"] != null)
                    {
                        var problem = Request.Form["cbxProblem"].ToString();
                        var listProb = problem.Split(',');
                        foreach (var item in listProb)
                        {
                            var problem_id = byte.Parse(item);
                            var query = (from a in dbSA.TM_Problem
                                         where a.prob_id == problem_id
                                         select a).First();
                            TD_Problem prob = new TD_Problem();
                            prob.id = main_id;
                            prob.prob_id = problem_id;
                            if (query.text_flag)
                            {
                                prob.prob_text = Request.Form["txtProb" + item].ToString();
                            }
                            dbSA.TD_Problem.Add(prob);
                        }
                    }
                    // End Add data DB to TD_Problem //

                    // Add data to DB TD_Disposition //
                    if (Request.Form["rdoDispos"] != null)
                    {
                        var dispos_id = byte.Parse(Request.Form["rdoDispos"].ToString());
                        TD_Disposition disp = new TD_Disposition();
                        disp.id = main_id;
                        disp.dispos_id = dispos_id;
                        var query = (from a in dbSA.TM_Disposition
                                     where a.dispos_id == dispos_id
                                     select a).First();
                        if (query.text_flag)
                        {
                            disp.dispos_text = Request.Form["txtDisp" + dispos_id].ToString();
                        }
                        dbSA.TD_Disposition.Add(disp);
                    }
                    // End Add data to DB TD_Disposition //

                    // Add data to DB TD_File //
                    string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/" + main_id + "/";
                    if (!Directory.Exists(Server.MapPath(subPath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(subPath));
                    }
                    foreach (var file in files)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            if (file.ContentType == "application/pdf")//Check accept file type.
                            {
                                var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(file.FileName);
                                var path = Path.Combine(Server.MapPath(subPath), fileName);
                                file.SaveAs(path);
                                SaveFiletoDB(main_id, subPath + fileName, Session["SA_Auth"].ToString());
                            }
                        }
                    }

                    // End Add data to DB TD_File //
                    dbSA.SaveChanges();

                    var concern_qc = Request.Form["selectQC"] != null ? Request.Form["selectQC"].ToString() : null;
                    var concern_en = Request.Form["selectEN"] != null ? Request.Form["selectEN"].ToString() : null;
                    var concern_other = Request.Form["selectOther"] != null ? Request.Form["selectOther"].ToString() : null;
                    var inform = Request.Form["selectInform"] != null ? Request.Form["selectInform"].ToString() : null;
                    AddConcernQC(main_id, concern_qc);
                    AddConcernEN(main_id, concern_en);
                    AddConcernOther(main_id, concern_other);
                    AddInform(main_id, inform);

                    scope.Complete();
                }

                // Add data to DB TD_Transaction //
                int org_id = int.Parse(Session["SA_Org"].ToString());
                byte lv = byte.Parse(Session["SA_UserLv"].ToString());

                NextStep_Transaction(main_id, 1, lv, org_id, 0);
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index", "SAProcess");
        }

        public ActionResult UpdateSA(IEnumerable<HttpPostedFileBase> files)
        {
            // define our transaction scope
            var scope = new TransactionScope(
                // a new transaction will always be created
                TransactionScopeOption.RequiresNew,
                // we will allow volatile data to be read during transaction
                new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
                    Timeout = TransactionManager.MaximumTimeout
                }
            );
            DateTime dt = DateTime.Now;
            int main_id = int.Parse(Request.Form["hdID"]);

            try
            {
                using (scope)
                {
                    // Add data to DB TD_Main //
                    var main = (from a in dbSA.TD_Main
                                where a.id == main_id
                                select a).FirstOrDefault();

                    if (main != null)
                    {
                        main.title = Request.Form["txtNonconformities"] != null ? Request.Form["txtNonconformities"].ToString() : null;
                        main.item_code = Request.Form["txtItemcode"] != null ? Request.Form["txtItemcode"].ToString() : null;
                        main.customer = Request.Form["txtCustomer"] != null ? Request.Form["txtCustomer"].ToString() : null;
                        main.issue_dt = dt;
                        main.material = Request.Form["txtMaterial"] != null ? Request.Form["txtMaterial"].ToString() : null;
                        main.batch_no = Request.Form["txtBatchNo"] != null ? Request.Form["txtBatchNo"].ToString() : null;
                        main.job_no = Request.Form["txtJobNo"] != null ? Request.Form["txtJobNo"].ToString() : null;
                        main.defective_qty = Request.Form["txtDefectiveQty"] != null ? Request.Form["txtDefectiveQty"].ToString() : "0";
                        main.expect_date = Request.Form["dtpExpectDate"] != null ? ParseToDate(Request.Form["dtpExpectDate"].ToString()) : dt;
                        main.reason = Request.Form["txaReason"] != null ? Request.Form["txaReason"].ToString() : "";
                        main.preventive = Request.Form["txaPreventive"] != null ? Request.Form["txaPreventive"].ToString() : "";
                        main.effective_dt = Request.Form["dtpEffectiveDate"] != null ? ParseToDate(Request.Form["dtpEffectiveDate"].ToString()) : dt;
                        main.Sys_Plant_id = byte.Parse(Request.Form["slPlant"].ToString());

                        dbSA.SaveChanges();
                    }
                    // End Add data DB to TD_Main //

                    // Add data to DB TD_Problem //
                    DelProblem(main_id);
                    if (Request.Form["cbxProblem"] != null)
                    {
                        var problem = Request.Form["cbxProblem"].ToString();
                        var listProb = problem.Split(',');
                        foreach (var item in listProb)
                        {
                            var problem_id = byte.Parse(item);
                            var query = (from a in dbSA.TM_Problem
                                         where a.prob_id == problem_id
                                         select a).First();
                            TD_Problem prob = new TD_Problem();
                            prob.id = main_id;
                            prob.prob_id = problem_id;
                            if (query.text_flag)
                            {
                                prob.prob_text = Request.Form["txtProb" + item].ToString();
                            }
                            dbSA.TD_Problem.Add(prob);
                        }
                    }
                    // End Add data DB to TD_Problem //

                    // Add data to DB TD_Disposition //
                    DelDisposition(main_id);
                    if (Request.Form["rdoDispos"] != null)
                    {
                        var dispos_id = byte.Parse(Request.Form["rdoDispos"].ToString());
                        TD_Disposition disp = new TD_Disposition();
                        disp.id = main_id;
                        disp.dispos_id = dispos_id;
                        var query = (from a in dbSA.TM_Disposition
                                     where a.dispos_id == dispos_id
                                     select a).First();
                        if (query.text_flag)
                        {
                            disp.dispos_text = Request.Form["txtDisp" + dispos_id].ToString();
                        }
                        dbSA.TD_Disposition.Add(disp);
                    }
                    // End Add data to DB TD_Disposition //

                    // Add data to DB TD_File //
                    string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/" + main_id + "/";
                    if (!Directory.Exists(Server.MapPath(subPath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(subPath));
                    }
                    foreach (var file in files)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            if (file.ContentType == "application/pdf")//Check accept file type.
                            {
                                var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(file.FileName);
                                var path = Path.Combine(Server.MapPath(subPath), fileName);
                                file.SaveAs(path);
                                SaveFiletoDB(main_id, subPath + fileName, Session["SA_Auth"].ToString());
                            }
                        }
                    }

                    // End Add data to DB TD_File //
                    dbSA.SaveChanges();

                    scope.Complete();
                }

                // Add data to DB TD_Transaction //
                int org_id = int.Parse(Session["SA_Org"].ToString());
                byte lv = byte.Parse(Session["SA_UserLv"].ToString());

                NextStep_Transaction(main_id, 1, lv, org_id, 0);
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index", "SAProcess");
        }

        public ActionResult UpdateSAS(IEnumerable<HttpPostedFileBase> files)
        {
            // define our transaction scope
            var scope = new TransactionScope(
                // a new transaction will always be created
                TransactionScopeOption.RequiresNew,
                // we will allow volatile data to be read during transaction
                new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
                    Timeout = TransactionManager.MaximumTimeout
                }
            );
            DateTime dt = DateTime.Now;
            int main_id = int.Parse(Request.Form["hdID"]);

            try
            {
                using (scope)
                {
                    // Add data to DB TD_Main //
                    var main = (from a in dbSA.TD_Main
                                where a.id == main_id
                                select a).FirstOrDefault();

                    if (main != null)
                    {
                        main.title = Request.Form["txtNonconformities"] != null ? Request.Form["txtNonconformities"].ToString() : null;
                        main.item_code = Request.Form["txtItemcode"] != null ? Request.Form["txtItemcode"].ToString() : null;
                        main.customer = Request.Form["txtCustomer"] != null ? Request.Form["txtCustomer"].ToString() : null;
                        //main.issue_dt = dt;
                        main.material = Request.Form["txtMaterial"] != null ? Request.Form["txtMaterial"].ToString() : null;
                        main.batch_no = Request.Form["txtBatchNo"] != null ? Request.Form["txtBatchNo"].ToString() : null;
                        main.job_no = Request.Form["txtJobNo"] != null ? Request.Form["txtJobNo"].ToString() : null;
                        main.defective_qty = Request.Form["txtDefectiveQty"] != null ? Request.Form["txtDefectiveQty"].ToString() : "0";
                        main.expect_date = Request.Form["dtpExpectDate"] != null ? ParseToDate(Request.Form["dtpExpectDate"].ToString()) : dt;
                        main.reason = Request.Form["txaReason"] != null ? Request.Form["txaReason"].ToString() : "";
                        main.preventive = Request.Form["txaPreventive"] != null ? Request.Form["txaPreventive"].ToString() : "";
                        main.effective_dt = Request.Form["dtpEffectiveDate"] != null ? ParseToDate(Request.Form["dtpEffectiveDate"].ToString()) : dt;
                        main.Sys_Plant_id = byte.Parse(Request.Form["slPlant"].ToString());
                        
                        dbSA.SaveChanges();
                    }
                    // End Add data DB to TD_Main //

                    // Add data to DB TD_Problem //
                    DelProblem(main_id);
                    if (Request.Form["cbxProblem"] != null)
                    {
                        var problem = Request.Form["cbxProblem"].ToString();
                        var listProb = problem.Split(',');
                        foreach (var item in listProb)
                        {
                            var problem_id = byte.Parse(item);
                            var query = (from a in dbSA.TM_Problem
                                         where a.prob_id == problem_id
                                         select a).First();
                            TD_Problem prob = new TD_Problem();
                            prob.id = main_id;
                            prob.prob_id = problem_id;
                            if (query.text_flag)
                            {
                                prob.prob_text = Request.Form["txtProb" + item].ToString();
                            }
                            dbSA.TD_Problem.Add(prob);
                        }
                    }
                    // End Add data DB to TD_Problem //

                    // Add data to DB TD_Disposition //
                    DelDisposition(main_id);
                    if (Request.Form["rdoDispos"] != null)
                    {
                        var dispos_id = byte.Parse(Request.Form["rdoDispos"].ToString());
                        TD_Disposition disp = new TD_Disposition();
                        disp.id = main_id;
                        disp.dispos_id = dispos_id;
                        var query = (from a in dbSA.TM_Disposition
                                     where a.dispos_id == dispos_id
                                     select a).First();
                        if (query.text_flag)
                        {
                            disp.dispos_text = Request.Form["txtDisp" + dispos_id].ToString();
                        }
                        dbSA.TD_Disposition.Add(disp);
                    }
                    // End Add data to DB TD_Disposition //

                    // Add data to DB TD_File //
                    string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/" + main_id + "/";
                    if (!Directory.Exists(Server.MapPath(subPath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(subPath));
                    }
                    foreach (var file in files)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            if (file.ContentType == "application/pdf")//Check accept file type.
                            {
                                var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(file.FileName);
                                var path = Path.Combine(Server.MapPath(subPath), fileName);
                                file.SaveAs(path);
                                SaveFiletoDB(main_id, subPath + fileName, Session["SA_Auth"].ToString());
                            }
                        }
                    }

                    // End Add data to DB TD_File //
                    dbSA.SaveChanges();

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index", "SAProcess");
        }

        public void DelDisposition(int id)
        {
            var list = from a in dbSA.TD_Disposition
                      where a.id == id
                      select a.dispos_id;
            foreach (var item in list)
            {
                var del = dbSA.TD_Disposition.Find(id, item);
                dbSA.TD_Disposition.Remove(del);
            }
            dbSA.SaveChanges();
        }

        public void DelProblem(int id)
        {
            var list = from a in dbSA.TD_Problem
                       where a.id == id
                       select a.prob_id;
            foreach (var item in list)
            {
                var del = dbSA.TD_Problem.Find(id, item);
                dbSA.TD_Problem.Remove(del);
            }
            dbSA.SaveChanges();
        }

        public ActionResult AddApprove(IEnumerable<HttpPostedFileBase> files)
        {
            int id = Request.Form["hdID"] != null ? int.Parse(Request.Form["hdID"].ToString()) : 0;
            int org = int.Parse(Session["SA_Org"].ToString());
            byte lv_id = byte.Parse(Session["SA_UserLv"].ToString());
            byte utype = byte.Parse(Session["SA_UType"].ToString());
            byte act_id = byte.Parse(Request.Form["slAction"].ToString());
            byte status = byte.Parse(Request.Form["hdSTID"].ToString());
            string approver = Session["SA_Auth"].ToString();
            byte plant = GetUserSysPlant(approver);
            var comment = Request.Form["txaComment"];
            if (id != 0)
            {
                UploadFiles(files, id, approver);

                byte temp_act = (act_id == 15 || act_id == 16) ? (byte)11 : act_id;

                if (status == 8 && org == 11 && lv_id == 4)
                {
                    UpdateTransaction(id, status, 3, 49, true, approver, temp_act, comment);
                }
                else
                {
                    UpdateTransaction(id, status, lv_id, org, true, approver, temp_act, comment, plant);
                }

                if (act_id == 15)
                {
                    InsertEndTransaction(id, 8, lv_id, org, 5, approver, comment);
                }
                else if (act_id == 16)
                {
                    InsertEndTransaction(id, 8, lv_id, org, 6, approver, comment);
                }

                NextStep_Transaction(id, status, lv_id, org, act_id, comment);

                return RedirectToAction("Index", "SAProcess");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AddApproveS1(IEnumerable<HttpPostedFileBase> files)
        {
            int id = Request.Form["hdID"] != null ? int.Parse(Request.Form["hdID"].ToString()) : 0;
            int org = int.Parse(Session["SA_Org"].ToString());
            byte lv_id = byte.Parse(Session["SA_UserLv"].ToString());
            byte act_id = byte.Parse(Request.Form["slAction"].ToString());
            byte status = byte.Parse(Request.Form["hdSTID"].ToString());
            string approver = Session["SA_Auth"].ToString();
            byte plant = GetUserSysPlant(approver);
            var comment = Request.Form["txaComment"];
            if (id != 0)
            {
                UploadFiles(files, id, approver);

                if (act_id == 8)//Select Other QA
                {
                    var other_qa = Request.Form["selectQA"] != null ? Request.Form["selectQA"].ToString() : null;

                    if (!string.IsNullOrEmpty(other_qa))
                    {
                        byte[] allplant = other_qa.Split(',').Select(s => Convert.ToByte(s, 16)).ToArray();
                        byte[] selplant = GetSysPlant(id);
                        var newplant = allplant.Where(w => !selplant.Contains(w));
                        int i = 0;
                        foreach (var item in newplant)
                        {
                            var get_qa_group = (from q in dbSA.TM_SysGroup
                                                where q.Sys_GroupType_id == 2 && q.Sys_Plant_id == item
                                                select q).FirstOrDefault();
                            if (get_qa_group != null)
                            {
                                Insert_Transaction(id, 2, 1, get_qa_group.group_id, true, item);
                                //Send Email to other QA Eng.
                                SendEmailCenter(GetSysEmail(2, item, 1), id);
                            }
                            if (get_qa_group.group_id == org) i++;
                        }

                        if (i == 0)//not group that same mgr.
                        {
                            Update_Transaction(id, lv_id, org, true, approver, null, comment, status, plant, true);
                        }
                        else
                        {
                            Update_Transaction(id, lv_id, org, true, approver, act_id, comment, status, plant);
                        }
                        NextStep_Transaction(id, status, lv_id, org, act_id, comment);
                    }
                }
                else//Accept or Not Accept
                {
                    //var concern_qc = Request.Form["selectQC"] != null ? Request.Form["selectQC"].ToString() : null;
                    //var concern_en = Request.Form["selectEN"] != null ? Request.Form["selectEN"].ToString() : null;
                    //var concern_other = Request.Form["selectOther"] != null ? Request.Form["selectOther"].ToString() : null;
                    //var inform = Request.Form["selectInform"] != null ? Request.Form["selectInform"].ToString() : null;
                    //AddConcernQC(id, concern_qc);
                    //AddConcernEN(id, concern_en);
                    //AddConcernOther(id, concern_other);
                    //AddInform(id, inform);

                    Update_Transaction(id, lv_id, org, true, approver, act_id, comment, status, plant);
                    //if (act_id == 4)// 4 = Not Accept
                    //{
                    //    SendEmailCenter(GetIssuerEmail(id), id, 5, "");
                    //}
                    var concern_qc = Request.Form["selectQC"] != null ? Request.Form["selectQC"].ToString() : null;
                    var concern_en = Request.Form["selectEN"] != null ? Request.Form["selectEN"].ToString() : null;
                    var concern_other = Request.Form["selectOther"] != null ? Request.Form["selectOther"].ToString() : null;
                    var inform = Request.Form["selectInform"] != null ? Request.Form["selectInform"].ToString() : null;
                    AddConcernQC(id, concern_qc);
                    AddConcernEN(id, concern_en);
                    AddConcernOther(id, concern_other);
                    AddInform(id, inform);
                    NextStep_Transaction(id, status, lv_id, org, act_id, comment);
                }
                

                return RedirectToAction("Index", "SAProcess");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private byte GetUserSysPlant(string approver)
        {
            byte default_plant = 0;
            var query = (from a in dbSA.TM_SysUser
                         where a.Sys_User_id == approver && a.user_active == true
                         select a).FirstOrDefault();
            if (query != null)
                default_plant = query.Sys_Plant_id;

            return default_plant;
        }

        [Check_Authen]
        [HttpPost]
        public ActionResult AddCarPar()
        {
            int id = Request.Form["hdID"] != null ? int.Parse(Request.Form["hdID"].ToString()) : 0;
            if (id != 0)
            {
                using (var db = new SAEntities())
                {
                    TD_CarPar pc = new TD_CarPar();
                    pc.id = id;
                    pc.car_no = Request.Form["txtCAR"] != null ? Request.Form["txtCAR"].ToString() : null;
                    pc.par_no = Request.Form["txtPAR"] != null ? Request.Form["txtPAR"].ToString() : null;
                    pc.root_cause_section = Request.Form["txaRoot"] != null ? Request.Form["txaRoot"].ToString() : null;
                    pc.issue_by = Session["SA_Auth"].ToString();
                    pc.create_dt = DateTime.Now;
                    db.TD_CarPar.Add(pc);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "SAProcess");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Check_Authen]
        [HttpPost]
        public ActionResult AddTellIssuer()
        {
            int id = Request.Form["hdID"] != null ? int.Parse(Request.Form["hdID"].ToString()) : 0;
            if (id != 0)
            {
                using (var db = new SAEntities())
                {
                    
                    byte status = byte.Parse(Request.Form["hdSTID"].ToString());
                    byte lv_id = byte.Parse(Session["SA_UserLv"].ToString());
                    int org = int.Parse(Session["SA_Org"].ToString());
                    string approver = Session["SA_Auth"].ToString();
                    byte plant = GetUserSysPlant(approver);

                    string tellisuer = Request.Form["txaComment"];
                    TD_Tell tell = new TD_Tell();
                    tell.id = id;
                    tell.tell_content = tellisuer;
                    tell.tell_dt = DateTime.Now;
                    tell.tell_user = approver;
                    db.TD_Tell.Add(tell);

                    //var query = db.TD_Transaction.Find(id, status, lv_id, org, plant);
                    var query = db.TD_Transaction.Where(w => w.id == id && w.status_id == status && w.lv_id == lv_id && w.org_id == org).FirstOrDefault();
                    if (query != null)
                    {
                        query.comment = "Tell";
                        query.actor = approver;
                    }
                    db.SaveChanges();
                    //Send Email to Issuer
                    SendEmailCenter(GetTellEmail(id), id, 2, tellisuer);
                }
                return RedirectToAction("Index", "SAProcess");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Check_Authen]
        public ActionResult ReplyTellIssuer(int tid = 0)
        {
            var issuer = Session["SA_Auth"].ToString();
            if (tid != 0)
            {
                using (var db = new SAEntities())
                {
                    var query = (from t in db.TD_Tell
                                 where t.tell_id == tid
                                 select t).FirstOrDefault();
                    if (query != null)
                    {
                        query.reply_dt = DateTime.Now;
                        db.SaveChanges();
                        //Send Email to Tell Issuer
                        string email = GetEmailByEmpcode(query.tell_user);
                        if(email != "")
                            SendEmailCenter(email, query.id, 6, query.tell_content);
                    }
                }
                return RedirectToAction("Index", "SAProcess");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private string GetEmailByEmpcode(string p)
        {
            var get_email = (from a in dbTNC.V_Employee_Info
                             where a.emp_code == p
                             && (a.email != null && a.email != "")
                             select a.email).FirstOrDefault();
            if (get_email != null)
            {
                if (!string.IsNullOrEmpty(get_email))
                    return get_email;
            }
            return "";
        }

        public void SaveFiletoDB(int id, string path, string uploadby)
        {
            using (var db = new SAEntities())
            {
                TD_File file = new TD_File();
                file.id = id;
                file.file_path = path;
                file.upload_by = uploadby;
                file.upload_dt = DateTime.Now;
                db.TD_File.Add(file);
                db.SaveChanges();
            }
        }

        public ActionResult IssuerUpload(IEnumerable<HttpPostedFileBase> files)
        {
            var uploader = Session["SA_Auth"].ToString();
            int id = Request.Form["hdID"] != null ? int.Parse(Request.Form["hdID"].ToString()) : 0;
            
            if (id != 0)
            {
                UploadFiles(files, id, uploader);
                return RedirectToAction("Index", "SAProcess");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public bool UploadFiles(IEnumerable<HttpPostedFileBase> files, int id, string uploader)
        {
            try
            {
                DateTime dt = DateTime.Now;
                string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/" + id + "/";
                if (!Directory.Exists(Server.MapPath(subPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(subPath));
                }
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        if (file.ContentType == "application/pdf")//Check accept file type.
                        {
                            var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath(subPath), fileName);
                            file.SaveAs(path);
                            SaveFiletoDB(id, subPath + fileName, uploader);
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public DateTime ParseToDate(string inputDT)
        {
            char[] delimiters = new char[] { '-', '/', ' ' };
            var temp = inputDT.Split(delimiters);
            DateTime outputDT = DateTime.Parse(temp[2] + "-" + temp[1] + "-" + temp[0]);
            return outputDT;
        }

        public string ProblemCheckBoxList(int said = 0)
        {
            string result = "";

            if (said != 0)
            {
                foreach (var item in dbSA.TM_Problem)
                {
                    var query = (from a in dbSA.TD_Problem
                                 where a.id == said && a.prob_id == item.prob_id
                                 select a).FirstOrDefault();
                    if (query != null)
                    {
                        result += "<input type='checkbox' name='cbxProblem' value='" + item.prob_id + "' class='prob' checked='checked'> " + item.prob_name; 
                        
                        if (item.text_flag)
                        {
                            if (string.IsNullOrEmpty(query.prob_text))
                            {
                                result += " <input type='text' id='txtProb" + item.prob_id + "' name='txtProb" + item.prob_id + "' class='input-medium' />";
                            }
                            else
                            {
                                result += " <input type='text' id='txtProb" + item.prob_id + "' name='txtProb" + item.prob_id + "' class='input-medium' value='" + query.prob_text + "' />";
                            }
                        }
                    }
                    else
                    {
                        result += "<input type='checkbox' name='cbxProblem' value='" + item.prob_id + "' class='prob'> " + item.prob_name;
                        if (item.text_flag)
                        {
                            result += " <input type='text' id='txtProb" + item.prob_id + "' name='txtProb" + item.prob_id + "' class='input-medium' />";
                        }
                    }
                    result += "<br />";
                }
            }
            else
            {
                foreach (var item in dbSA.TM_Problem)
                {
                    result += "<input type='checkbox' name='cbxProblem' value='" + item.prob_id + "' class='prob'> " + item.prob_name;
                    // data-validation='checkbox_group' data-validation-qty='min1'
                    if (item.text_flag)
                    {
                        result += " <input type='text' id='txtProb" + item.prob_id + "' name='txtProb" + item.prob_id + "' class='input-medium' />";
                    }
                    result += "<br />";
                }
            }
            return result;
        }

        public string DispositionRadioList(int said = 0)
        {
            string result = "";

            if (said != 0)
            {
                var query = dbSA.TD_Disposition.Where(w => w.id == said).FirstOrDefault();

                foreach (var item in dbSA.TM_Disposition)
                {
                    if (query.dispos_id == item.dispos_id)
                    {
                        result += "<input type='radio' name='rdoDispos' value='" + item.dispos_id + "' class='dispos' checked='checked'> " + item.dispos_name;
                    }
                    else
                    {
                        result += "<input type='radio' name='rdoDispos' value='" + item.dispos_id + "' class='dispos'> " + item.dispos_name;
                    }
                    
                    if (item.text_flag)
                    {
                        if (string.IsNullOrEmpty(query.dispos_text))
                        {
                            result += " <input type='text' id='txtDisp" + item.dispos_id + "' name='txtDisp" + item.dispos_id + "' class='input-medium dispostxt' />";
                        }
                        else
                        {
                            result += " <input type='text' id='txtDisp" + item.dispos_id + "' name='txtDisp" + item.dispos_id + "' class='input-medium dispostxt' value='" + query.dispos_text + "' />";
                        }
                    }
                    result += "<br />";
                }
            }
            else
            {
                foreach (var item in dbSA.TM_Disposition)
                {
                    result += "<input type='radio' name='rdoDispos' value='" + item.dispos_id + "' class='dispos'> " + item.dispos_name;
                    if (item.text_flag)
                    {
                        result += " <input type='text' id='txtDisp" + item.dispos_id + "' name='txtDisp" + item.dispos_id + "' class='input-medium dispostxt' />";
                    }
                    result += "<br />";
                }
            }
            return result;
        }

        public string InitialControlNo(string usergroup)
        {
            PCREntities dbPCR = new PCREntities();
            var prefix = "SA " + DateTime.Now.ToString("yy", new CultureInfo("en-US")) + "-";
            var mm = "-" + DateTime.Now.ToString("MM", new CultureInfo("en-US")) + "-";

            var group_id = int.Parse(usergroup);
            var get_code = (from g in dbPCR.PCR_GCode
                         where g.g_id == group_id
                         select g).FirstOrDefault();
            if (get_code != null)
            {
                return prefix + get_code.g_code + mm;
            }
            else
            {
                return "";
            }
        }

        public int GetRunNo(string prefix)
        {
            var runno = 1;
            if (!string.IsNullOrEmpty(prefix))
            {
                var get_runno = (from m in dbSA.TD_Main
                                 where m.control_no.Contains(prefix)
                                 orderby m.control_no descending
                                 select m.control_no).FirstOrDefault();
                if (get_runno != null)
                {
                    runno = int.Parse(get_runno.Substring(get_runno.Length - 3, 3)) + 1;
                }
            }
            return runno;
        }

        public string GetUserName(string emp_code)
        {
            string name = "";
            var query = (from u in dbTNC.tnc_user
                         where u.emp_code == emp_code
                         select u).FirstOrDefault();
            if (query != null)
            {
                name = query.emp_fname + " " + query.emp_lname;
            }
            return name;
        }

        public string GetSysEmail(byte group_type, byte plant_id, byte lv_id)
        {
            string email = "";
            var get_user = from u in dbSA.TM_SysUser
                           where u.Sys_GroupType_id == group_type
                           && u.Sys_Plant_id == plant_id && u.lv_id == lv_id
                           select u;

            foreach (var item in get_user)
            {
                var get_email = (from e in dbTNC.tnc_user
                                 where e.emp_code == item.Sys_User_id
                                 select e.email).FirstOrDefault();
                if (!string.IsNullOrEmpty(get_email))
                    email += "," + get_email;
            }
            email = email.Substring(1);
            return email;
        }

        public string GetEmailinRoot(int id)
        {
            string email_root = "";
            string email_inform = "";
            var get_all = from a in dbSA.TD_Transaction
                          where a.id == id
                          group a by a.actor into g
                          select new { actor = g.Key };
            if (get_all.Any())
            {
                foreach (var item in get_all)
                {
                    var get_email = (from a in dbTNC.V_Employee_Info
                                     where a.emp_code == item.actor
                                     && (a.email != null && a.email != "")
                                     select a.email).FirstOrDefault();
                    if (get_email != null)
                    {
                        if (!string.IsNullOrEmpty(get_email))
                            email_root += "," + get_email;
                    }
                }
                email_root = email_root.Substring(1);
            }

            //Update Date 2014-09-12 by Monchit W. - Add get Email Inform
            var get_inform = from a in dbSA.TD_Inform
                             where a.id == id
                             select a.group_id;
            if (get_inform.Any())
            {
                foreach (var item in get_inform)
                {
                    var get_email = (from a in dbTNC.tnc_user
                                     where a.emp_group == item && a.emp_position == 2
                                     && (a.email != null && a.email != "")
                                     select a.email).FirstOrDefault();
                    if (get_email != null)
                    {
                        if (!string.IsNullOrEmpty(get_email))
                            email_inform += "," + get_email;
                    }
                }
            }

            return email_root + email_inform;
        }

        public string GetTNCEmailbyGroup(int group_id, byte lv)
        {
            string email = "";

            var positionlv = (from p in dbSA.TM_Level
                              where p.lv_id == lv
                              select p).FirstOrDefault();

            if (positionlv != null)
            {
                var get_email = from a in dbTNC.V_Employee_Info
                                where a.group_id == group_id && 
                                (a.position_level >= positionlv.position_min && a.position_level <= positionlv.position_max)
                                && (a.email != null && a.email != "")
                                select a.email;
                if (get_email.Any())
                {
                    foreach (var item in get_email)
                    {
                        if (!string.IsNullOrEmpty(item))
                            email += "," + item;
                    }
                }

                email = email.Substring(1);
            }
            return email;
        }

        public string GetTNCEmailbyEmpCode(string emp_code)
        {
            var get_email = (from a in dbTNC.V_Employee_Info
                             where a.emp_code == emp_code
                             && (a.email != null && a.email != "")
                             select a.email).FirstOrDefault();
            if (get_email != null)
            {
                if (!string.IsNullOrEmpty(get_email))
                    return get_email;
            }
            return "";
        }

        public string GetStatusName(int id)
        {
            string status;
            var query = (from a in dbSA.TD_Transaction
                         where a.id == id && a.active == true
                         orderby a.status_id descending, a.lv_id descending
                         select a).FirstOrDefault();
            if (query != null)
            {
                status = "Wait " + query.TM_Level.lv_name + " (" + query.TM_Status.status_name + ")";
            }
            else
            {
                var query1 = (from b in dbSA.TD_Transaction
                              where b.id == id && b.status_id > 50
                              orderby b.status_id descending, b.lv_id descending
                              select b).FirstOrDefault();
                status = query1 != null ? query1.TM_Status.status_name + "." : "Error";
            }
            return status;
        }

        public byte GetStatusId(int id)
        {
            var query = (from a in dbSA.TD_Transaction
                         where a.id == id && a.active == true
                         orderby a.status_id descending, a.lv_id descending
                         select a).FirstOrDefault();
            if (query != null)
                return query.status_id;
            else
            {
                var query1 = (from b in dbSA.TD_Transaction
                              where b.id == id && b.status_id < 100
                              orderby b.status_id descending, b.lv_id descending
                              select b).FirstOrDefault();
                return query1.status_id;
            }
        }

        private string GetOrgName(byte user_lv, int p)
        {
            string org_name = "";

            if (user_lv < 3)
            {
                org_name = (from o in dbTNC.View_Organization
                            where o.group_id == p
                            select o.group_name).FirstOrDefault();
            }
            else if (user_lv < 4)
            {
                org_name = (from o in dbTNC.View_Organization
                            where o.dept_id == p
                            select o.dept_name).FirstOrDefault();
            }
            else if (user_lv < 5)
            {
                org_name = (from o in dbTNC.View_Organization
                            where o.plant_id == p
                            select o.plant_name).FirstOrDefault();
            }

            return org_name;
        }
        
        public string GetMainData(int id)
        {
            string main_data = "";
            using (var db = new SAEntities())
            {
                var query = (from a in db.TD_Main
                             where a.id == id
                             select a).FirstOrDefault();
                main_data = "<table class='table table-condensed table-hover table-bordered'><tr>" 
                    + "<td class='td_title'>Control No.:</td><td>" + query.control_no + "</td>"
                    + "<td class='td_title'>Item Code:</td><td>" + query.item_code + "</td>"
                    + "<td class='td_title'>Material:</td><td>" + query.material + "</td>"
                    + "<td class='td_title'>Job No.:</td><td>" + query.job_no + "</td>"
                    + "</tr><tr>"
                    + "<td class='td_title'>Type of Nonconformities:</td><td>" + query.title + "</td>"
                    + "<td class='td_title'>Customer:</td><td>" + query.customer + "</td>"
                    + "<td class='td_title'>Batch No.:</td><td>" + query.batch_no + "</td>"
                    + "<td class='td_title'>Defective Qty:</td><td>" + query.defective_qty + "</td>"
                    + "</tr><tr>"
                    + "<td class='td_title'>Problem:</td><td colspan='3'>" + GetProblem(id) + "</td>"
                    + "<td class='td_title'>Expect Date:</td><td>" + query.expect_date.ToString("dd/MM/yyyy") + "</td>"
                    + "<td class='td_title'>Plant:</td><td>" + query.TM_SysPlant.Sys_Plant_name + "</td>"
                    + "</tr><tr>"
                    + "<td class='td_title'>Reason:</td><td colspan='3'>" + query.reason + "</td>"
                    + "<td class='td_title'>Effective Date:</td><td>" + query.effective_dt.ToString("dd/MM/yyyy") + "</td>"
                    + "<td class='td_title'>Issued Date:</td><td>" + query.issue_dt + "</td>"
                    + "</tr><tr>"
                    + "<td class='td_title'>Disposition of Defectives:</td><td colspan='3'>" + GetDisposition(id) + "</td>"
                    + "<td class='td_title'>Preventive action:</td><td colspan='3'>" + query.preventive + "</td>"
                    + "</tr></table>";
            }
            return main_data;
        }
        
        public string GetProblem(int id)
        {
            string prob = "";
            using (var db = new SAEntities())
            {
                var query = from a in db.TD_Problem
                            where a.id == id
                            select a;
                foreach (var item in query)
                {
                    prob += ", " + item.TM_Problem.prob_name;
                    prob += item.prob_text != null ? " : " + item.prob_text : "";
                }
                prob = prob.Substring(1);
            }
            return prob;
        }

        public string GetDisposition(int id)
        {
            string disp = "";
            using (var db = new SAEntities())
            {
                var query = from a in db.TD_Disposition
                            where a.id == id
                            select a;
                foreach (var item in query)
                {
                    disp += ", " + item.TM_Disposition.dispos_name;
                    disp += item.dispos_text != null ? " : " + item.dispos_text : "";
                }
                disp = disp.Substring(1);
            }
            return disp;
        }

        public byte[] GetSysPlant(int id)
        {
            var query = (from a in dbSA.TD_Transaction
                        where a.id == id && a.status_id == 2 && a.lv_id == 1
                        select a.Sys_Plant_id).ToArray();
            return query;
        }

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult GetSelectedQA(int id)
        {
            var get_plant = (from a in dbSA.TD_Transaction
                           where a.id == id && a.status_id == 2 && a.lv_id == 1
                           select a.Sys_Plant_id).ToList();
            var get_qa = dbSA.TM_SysGroup.Where(w => w.Sys_GroupType_id == 2 &&  get_plant.Contains(w.Sys_Plant_id))
                .Select(s => new { id = s.Sys_Plant_id, text = s.Sys_Group_name, locked = true });
            if (get_qa != null)
                return Json(get_qa, JsonRequestBehavior.AllowGet);
            else
                return Json(0, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult GetSelectedQC(int id)
        {
            var get_selected = (from a in dbSA.TD_ConcernQC
                                where a.id == id
                                select a.group_id).ToList();
            var get_qc = dbTNC.tnc_group_master.Where(w => get_selected.Contains(w.id))
                .Select(s => new { id = s.id, text = s.group_name, locked = true });
            if (get_qc != null)
                return Json(get_qc, JsonRequestBehavior.AllowGet);
            else
                return Json(0, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult GetSelectedEN(int id)
        {
            var get_selected = (from a in dbSA.TD_ConcernEN
                                where a.id == id
                                select a.group_id).ToList();
            var get_en = dbTNC.tnc_group_master.Where(w => get_selected.Contains(w.id))
                .Select(s => new { id = s.id, text = s.group_name, locked = true });
            if (get_en != null)
                return Json(get_en, JsonRequestBehavior.AllowGet);
            else
                return Json(0, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult GetSelectedOther(int id)
        {
            var get_selected = (from a in dbSA.TD_ConcernOther
                                where a.id == id
                                select a.group_id).ToList();
            var get_other = dbTNC.tnc_group_master.Where(w => get_selected.Contains(w.id))
                .Select(s => new { id = s.id, text = s.group_name, locked = true });
            if (get_other != null)
                return Json(get_other, JsonRequestBehavior.AllowGet);
            else
                return Json(0, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult GetSelectedInform(int id)
        {
            //Add Date 2014-10-13 by Monchit W.
            var get_selected = (from a in dbSA.TD_Inform
                                where a.id == id
                                select a.group_id).ToList();
            var get_inf = dbTNC.tnc_group_master.Where(w => get_selected.Contains(w.id))
                .Select(s => new { id = s.id, text = s.group_name, locked = true });
            if (get_inf != null)
                return Json(get_inf, JsonRequestBehavior.AllowGet);
            else
                return Json(0, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetResultList()
        {
            try
            {
                var result = dbSA.TM_Result.Select(r => new { DisplayText = r.result_name, Value = r.result_id });
                return Json(new { Result = "OK", Options = result });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private bool CheckIsIssuer(int id)
        {
            string auth = Session["SA_Auth"].ToString();
            var query = dbSA.TD_Main.Where(w => w.id == id && w.issue_by == auth).Count();
            return query == 1 ? true : false;
        }

        private bool Check_CarPar(int id)
        {
            var chk_carpar = (from pc in dbSA.TD_CarPar
                              where pc.id == id
                              select pc).FirstOrDefault();
            return chk_carpar == null ? false : true;
        }

        private bool Check_IssueSA(int id)
        {
            var chk_sa = from a in dbSA.TD_SA
                         where a.id == id
                         select a;
            if (chk_sa.Any()) return true;
            else return false;
        }

        //public void InsertTransaction(int id, byte status, byte lv, int org, byte plant = 0, bool active = true)
        //{
        //    if(!Check_Transaction(id,status,lv,plant,org)){
        //        var tran = new TD_Transaction();
        //        tran.id = id;
        //        tran.status_id = status;
        //        tran.lv_id = lv;
        //        tran.org_id = org;
        //        tran.Sys_Plant_id = plant;
        //        tran.active = active;
        //        tran.act_dt = DateTime.Now;
        //        dbSA.TD_Transaction.Add(tran);
        //        dbSA.SaveChanges();
        //    }
        //}

        public void UpdateTransaction(int id, byte status, byte lv, int org, bool chk_active, string actor, byte action, string comment, byte chk_plant = 0, bool set_active = false)
        {
            var query = (from t in dbSA.TD_Transaction
                         where t.id == id && t.status_id == status && t.lv_id == lv && t.org_id == org
                         && t.active == chk_active //&& t.Sys_Plant_id == chk_plant //Update Date 2014-12-19 By Monchit W.
                         select t).FirstOrDefault();
            if (query != null)
            {
                query.act_id = action;
                query.actor = actor;
                query.act_dt = DateTime.Now;
                query.comment = comment;
                query.active = set_active;
                dbSA.SaveChanges();
            }
            else
            {
                var chgFinal = (from c in dbSA.TD_Transaction
                                where c.id == id && c.status_id == 8 && c.lv_id == lv 
                                && c.org_id == org && c.Sys_Plant_id == chk_plant
                                select c).FirstOrDefault();
                if (chgFinal != null)
                {
                    chgFinal.act_id = action;
                    chgFinal.actor = actor;
                    chgFinal.act_dt = DateTime.Now;
                    chgFinal.comment = comment;
                    dbSA.SaveChanges();
                }
                else if (lv == 3)//Dept.
                {
                    var get_all_group = (from a in dbTNC.View_Organization
                                         where a.dept_id == org && a.group_id != 0
                                         select a.group_id).Distinct();
                    foreach (var item in get_all_group)
                    {
                        var check_group = (from c in dbSA.TD_Transaction
                                           where c.id == id && c.active == chk_active && c.status_id == status
                                           && c.org_id == item && c.Sys_Plant_id == chk_plant
                                           select c).FirstOrDefault();
                        if (check_group != null)
                        {
                            check_group.act_id = action;
                            check_group.actor = actor;
                            check_group.act_dt = DateTime.Now;
                            check_group.comment = comment;
                            check_group.active = set_active;
                            dbSA.SaveChanges();
                            break;
                        }
                    }
                }
                else if (lv == 4)//Plant/Div.
                {
                    var get_all_dept = from a in dbTNC.View_Organization
                                        where a.plant_id == org && a.dept_id != 0
                                        select a.dept_id;
                    foreach (var item in get_all_dept)
                    {
                        var check_dept = (from c in dbSA.TD_Transaction
                                           where c.id == id && c.active == chk_active && c.status_id == status
                                           && c.org_id == item && c.Sys_Plant_id == chk_plant
                                           select c).FirstOrDefault();
                        if (check_dept != null)
                        {
                            check_dept.act_id = action;
                            check_dept.actor = actor;
                            check_dept.act_dt = DateTime.Now;
                            check_dept.comment = comment;
                            check_dept.active = set_active;
                            dbSA.SaveChanges();
                            break;
                        }
                    }
                }
            }
        }

        public void Insert_Transaction(int id, byte status, byte lv, int org_id, bool active, byte plant = 0, string actor = null, byte act_id = 255)
        {
            using (var db = new SAEntities())
            {
                if (status >= 100)
                {
                    var del_tran = from d in db.TD_Transaction
                                   where d.id == id && d.lv_id == lv && d.org_id == org_id
                                   && d.Sys_Plant_id == plant && d.status_id >= 100
                                   select d;
                    if (del_tran.Any())
                    {
                        foreach (var item in del_tran)
                        {
                            db.TD_Transaction.Remove(item);
                        }
                    }
                }
                var query = (from a in db.TD_Transaction
                             where a.id == id && a.status_id == status && a.lv_id == lv 
                             && a.org_id == org_id && a.Sys_Plant_id == plant
                             select a).FirstOrDefault();
                var tran = new TD_Transaction();
                if (query != null)
                {
                    query.active = active;
                    if (act_id != 255)
                    {
                        query.act_id = act_id;
                    }
                    else
                    {
                        query.act_id = null;
                    }
                }
                else
                {
                    tran.id = id;
                    tran.status_id = status;
                    tran.lv_id = lv;
                    tran.org_id = org_id;
                    tran.Sys_Plant_id = plant;
                    tran.active = active;
                    tran.actor = actor;
                    tran.act_dt = DateTime.Now;
                    if (act_id != 255)
                    {
                        tran.act_id = act_id;
                    }
                    db.TD_Transaction.Add(tran);
                }
                
                db.SaveChanges();
            }
        }

        public void InsertEndTransaction(int id, byte status, byte lv, int org_id, byte action, string actor, string comment, bool active = false, byte plant = 0)
        {
            using (var db = new SAEntities())
            {
                var tran = new TD_Transaction();
                tran.id = id;
                tran.status_id = status;
                tran.lv_id = lv;
                tran.org_id = org_id;
                tran.Sys_Plant_id = plant;
                tran.act_id = action;
                tran.actor = actor;
                tran.act_dt = DateTime.Now;
                tran.comment = comment;
                db.TD_Transaction.Add(tran);
                db.SaveChanges();
            }
        }

        public void Update_Transaction(int id, byte chk_lv, int chk_org, bool chk_active, string set_actor, byte? set_act, string comment, byte chk_status = 0, byte chk_plant = 0, bool set_active = false)
        {
            var query = from t in dbSA.TD_Transaction
                        where t.id == id && t.lv_id == chk_lv && t.org_id == chk_org && t.active == chk_active && t.Sys_Plant_id == chk_plant
                        select t;
            if (chk_status != 0)
            {
                query = query.Where(w => w.status_id == chk_status).OrderByDescending(o => o.status_id).ThenByDescending(o => o.lv_id);
            }

            var update = query.FirstOrDefault();

            if (update != null)
            {
                update.act_id = set_act;
                update.actor = set_actor;
                update.act_dt = DateTime.Now;
                update.comment = string.IsNullOrEmpty(comment) ? null : comment;
                update.active = set_active;
                dbSA.SaveChanges();
            }
        }

        /// <summary>
        /// Update Transaction of Issuer for Revise Action
        /// </summary>
        /// <param name="id"></param>
        /// <param name="set_active"></param>
        /// <param name="set_action"></param>
        private void UpdateIssuer_Transaction(int id, bool set_active, byte set_action = 1)
        {
            var query = (from t in dbSA.TD_Transaction
                         where t.id == id && t.status_id == 1
                         orderby t.status_id, t.lv_id
                         select t).First();
            if (query != null)
            {
                query.act_id = set_action;
                query.active = set_active;
                dbSA.SaveChanges();
            }
        }

        private bool Check_SysUser(string emp_code, byte group_type, byte lv, byte plant = 0)
        {
            var query = (from a in dbSA.TM_SysUser
                         where a.Sys_GroupType_id == group_type //a.Sys_Plant_id == plant && //Update Date 2014-12-19 By Monchit W.
                         && a.Sys_User_id == emp_code && a.lv_id == lv
                         select a).Count();
            if (query > 0) return true;
            else return false;
        }

        private bool Check_Transaction(int id, byte status, byte lv, byte plant = 0, int org = 0)
        {
            var query = from a in dbSA.TD_Transaction
                        where a.id == id && a.status_id == status && a.lv_id == lv && a.Sys_Plant_id == plant
                        select a;
            if (org != 0)
            {
                query = query.Where(w => w.org_id == org);
            }
            if (query.Any()) return true;
            else return false;
        }

        private void NextStep_Transaction(int id, byte status, byte lv, int org_id, byte act_id, string other = null)
        {
            TNCOrganization tnc_org = new TNCOrganization();
            if (Session["SA_Auth"] != null)
            {
                string user = Session["SA_Auth"].ToString();

                tnc_org.GetApprover(user);

                if (act_id == 0)//Issue
                {
                    //Add Issue Transaction
                    Insert_Transaction(id, status, lv, org_id, false, 0, user, act_id);
                    if (tnc_org.OrgLevel == 1)
                    {
                        Insert_Transaction(id, status, (byte)(tnc_org.OrgLevel + 1), tnc_org.OrgId, true, 0, tnc_org.ManagerId);
                        SendEmailCenter(tnc_org.ManagerEMail, id);
                    }
                    else if (tnc_org.OrgLevel == 2)
                    {
                        Insert_Transaction(id, status, (byte)(tnc_org.OrgLevel + 1), tnc_org.OrgId, true, 0, tnc_org.ManagerId);
                        var email_div = GetEmailDivByDept(tnc_org.OrgId);
                        SendEmailCenter(tnc_org.ManagerEMail + email_div, id);
                    }
                    
                }
                else if (act_id < 6)//Go to next step
                {
                    var lv_max = (from n in dbSA.TM_Status
                                  where n.status_id == status
                                  select n.lv_max).FirstOrDefault();
                    
                    if (act_id == 4)// 4 = Not Accept
                    {
                        SendEmailCenter(GetIssuerEmail(id), id, 5, other);
                    }

                    if (lv < lv_max)
                    {
                        int user_org = int.Parse(Session["SA_Org"].ToString());

                        if (lv == 1)
                        {
                            if (Check_All_Approve(id, status, lv, user_org))
                            {
                                Insert_Transaction(id, status, (byte)(tnc_org.OrgLevel + 1), tnc_org.OrgId, true, 0, tnc_org.ManagerId);
                                SendEmailCenter(tnc_org.ManagerEMail, id);
                            }
                        }
                        else if (lv == 2 || lv == 3)
                        {
                            if (Check_All_Approve(id, status, lv))
                            {
                                //Update Date 2014-10-22 by Monchit W.
                                Insert_Transaction(id, status, (byte)(tnc_org.OrgLevel + 1), tnc_org.OrgId, true, 0, tnc_org.ManagerId);
                                SendEmailCenter(tnc_org.ManagerEMail, id);
                            }
                        }
                    }
                    else //lv >= lv_max
                    {
                        byte new_status = (byte)(status + 1);//Next Status

                        var get_min_lv = (from m in dbSA.TM_Status
                                          where m.status_id == new_status && m.lv_min != 0
                                          select m.lv_min).FirstOrDefault();

                        if (status == 1)//status issuer go to QC
                        {
                            if (Check_All_Approve(id, status))
                            {
                                AddQCTransaction(id, 3, 2);//3 = QC
                            }
                        }
                        //else if (status == 2)//status QA go to QC
                        //{
                        //    if (Check_All_Approve(id, status))
                        //    {
                        //        if (!AddQCTransaction(id, new_status, get_min_lv))
                        //        {
                        //            AddOtherTransaction(id, (byte)(new_status + 1), 2);
                        //        }
                        //    }
                        //}
                        //if (status == 1 || status == 2)//status Issue, QA go to QC
                        //{
                        //    if (Check_All_Approve(id, status))
                        //    {
                        //        //Update Date 2016-09-29 by Monchit W.
                        //        if (AddQCTransaction(id, 3, get_min_lv)){}
                        //        else if (AddOtherTransaction(id, 4, 2)){}
                        //        else if (AddENTransaction(id, 5, 1)){}
                        //        else
                        //        {
                        //            Insert_Transaction(id, 6, 3, 49, true);//49 = QS Department

                        //            var get_email = from a in dbTNC.tnc_user
                        //                            where (a.emp_position == 6 && a.emp_depart == 49) ||//QS Department
                        //                            (a.emp_position == 4 && a.emp_plant == 11)//QC Div.
                        //                            select a.email;
                        //            foreach (var item in get_email)
                        //            {
                        //                SendEmailCenter(item, id);
                        //            }
                        //        }
                        //    }
                        //}
                        else if (status == 3)//status QC go to
                        {
                            if (Check_All_Approve(id, status))
                            {
                                //Update Date 2016-09-29 by Monchit W.
                                if (AddOtherTransaction(id, new_status, get_min_lv)){}//Add Other Transaction
                                else if (AddENTransaction(id, (byte)(new_status + 1), 1)){}//Add Engineer Transaction
                                else
                                {
                                    var get_group_qc = (from a in dbSA.TD_Transaction
                                                        where a.id == id && a.status_id == 3 && a.lv_id < 3
                                                        select a.org_id).ToList();

                                    var get_dept_qc = (from a in dbTNC.View_Organization
                                                       where get_group_qc.Contains(a.group_id.Value)
                                                       select new { a.dept_id, a.DeptMgr_email, a.plant_id, a.PlantMgr_email }).Distinct();

                                    foreach (var item in get_dept_qc)
                                    {
                                        if (item.DeptMgr_email != null)
                                        {
                                            Insert_Transaction(id, 6, 3, item.dept_id.Value, true);
                                            SendEmailCenter(item.DeptMgr_email, id);
                                        }
                                        else
                                        {
                                            Insert_Transaction(id, 6, 4, item.plant_id.Value, true);
                                            SendEmailCenter(item.PlantMgr_email, id);
                                        }
                                    }

                                    //Insert_Transaction(id, 6, 3, 49, true);//49 = QS Department

                                    //var get_email = from a in dbTNC.tnc_user
                                    //                where (a.emp_position == 6 && a.emp_depart == 49) ||//QS Department
                                    //                (a.emp_position == 4 && a.emp_plant == 11)//QC Div.
                                    //                select a.email;
                                    //foreach (var item in get_email)
                                    //{
                                    //    SendEmailCenter(item, id);
                                    //}
                                }
                            }
                        }
                        else if (status == 4)//status Other go to EN
                        {
                            if (Check_All_Approve(id, status))
                            {
                                //Update Date 2016-09-29 by Monchit W.
                                if (AddENTransaction(id, new_status, get_min_lv)){}
                                else{
                                    var get_group_qc = (from a in dbSA.TD_Transaction
                                                        where a.id == id && a.status_id == 3 && a.lv_id < 3
                                                        select a.org_id).ToList();

                                    var get_dept_qc = (from a in dbTNC.View_Organization
                                                       where get_group_qc.Contains(a.group_id.Value)
                                                       select new { a.dept_id, a.DeptMgr_email, a.plant_id, a.PlantMgr_email }).Distinct();

                                    foreach (var item in get_dept_qc)
                                    {
                                        if (item.DeptMgr_email != null)
                                        {
                                            Insert_Transaction(id, 6, 3, item.dept_id.Value, true);
                                            SendEmailCenter(item.DeptMgr_email, id);
                                        }
                                        else
                                        {
                                            Insert_Transaction(id, 6, 4, item.plant_id.Value, true);
                                            SendEmailCenter(item.PlantMgr_email, id);
                                        }
                                    }
                                    //Insert_Transaction(id, 6, 3, 49, true);//49 = QS Department

                                    //var get_email = from a in dbTNC.tnc_user
                                    //                where (a.emp_position == 6 && a.emp_depart == 49) ||//QS Department
                                    //                (a.emp_position == 4 && a.emp_plant == 11)//QC Div.
                                    //                select a.email;
                                    //foreach (var item in get_email)
                                    //{
                                    //    SendEmailCenter(item, id);
                                    //}
                                }
                            }
                        }
                        else if (status == 5)//status EN go to QS Review
                        {
                            if (Check_All_Approve(id, status))
                            {
                                var get_group_qc = (from a in dbSA.TD_Transaction
                                                    where a.id == id && a.status_id == 3 && a.lv_id < 3
                                                    select a.org_id).ToList();

                                var get_dept_qc = (from a in dbTNC.View_Organization
                                                   where get_group_qc.Contains(a.group_id.Value)
                                                   select new { a.dept_id, a.DeptMgr_email, a.plant_id, a.PlantMgr_email }).Distinct();

                                foreach (var item in get_dept_qc)
                                {
                                    if (item.DeptMgr_email != null)
                                    {
                                        Insert_Transaction(id, 6, 3, item.dept_id.Value, true);
                                        SendEmailCenter(item.DeptMgr_email, id);
                                    }
                                    else
                                    {
                                        Insert_Transaction(id, 6, 4, item.plant_id.Value, true);
                                        SendEmailCenter(item.PlantMgr_email, id);
                                    }
                                }
                                //Insert_Transaction(id, 6, 3, 49, true);//49 = QS Department

                                //var get_email = from a in dbTNC.tnc_user
                                //                where (a.emp_position == 6 && a.emp_depart == 49) ||//QS Department
                                //                (a.emp_position == 4 && a.emp_plant == 11)//QC Div.
                                //                select a.email;
                                //foreach (var item in get_email)
                                //{
                                //    SendEmailCenter(item, id);
                                //}
                            }
                        }
                        else if (status == 8)//Final Approve
                        {
                            Insert_Transaction(id, 100, lv, org_id, false);
                            SendEmailCenter(GetEmailinRoot(id), id, 3, other);
                        }
                    }
                }
                else if (act_id == 6)//Reject -> Close SA
                {
                    if (status == 1)
                    {
                        Insert_Transaction(id, 102, lv, org_id, false);//Canceled
                    }
                    else if(status == 8)//Final Dicision
                    {
                        Insert_Transaction(id, 101, lv, org_id, false);//Rejected
                    }
                    SendEmailCenter(GetEmailinRoot(id), id, 4, other);
                }
                else if (act_id == 7)//Revise : Return to Issuer
                {
                    UpdateIssuer_Transaction(id, true);
                    SendEmailCenter(GetIssuerEmail(id), id, 1, other);
                }
                else if (act_id == 10 || act_id == 11)
                {
                    if (Check_All_Approve(id, status))
                    {
                        if (dbSA.TD_Transaction.Any(w => w.id == id && act_id == 10))//Issue SA
                        {
                            //GetQAResponse
                        }
                        else//No Issue SA
                        {
                            var get_qc_review = from a in dbSA.TD_Transaction
                                                where a.id == id && a.status_id == 6
                                                select a;

                            foreach (var tran in get_qc_review)
                            {
                                Insert_Transaction(id, 8, tran.lv_id, tran.org_id, true);
                                SendEmailCenter("", id);
                            }
                        }
                    }
                }
                //else if (act_id == 10)//Issue SA Cust/NOK
                //{
                //    if (Check_All_Approve(id, status))
                //    {
                //        //Modify Date 2015-04-21 by Monchit
                //        var get_qa_group = (from a in dbSA.TD_Transaction
                //                           where a.id == id && a.status_id == 2 && a.lv_id == 1
                //                           select a).FirstOrDefault();

                //        Insert_Transaction(id, 7, 1, get_qa_group.org_id, true);
                //        SendEmailCenter(GetQAEngEmail(id), id);
                //        //foreach (var item in get_qa_group)
                //        //{

                //        //}
                //    }
                //}
                //else if (act_id == 11)//No issue SA Cust/NOK
                //{
                //    if (Check_All_Approve(id, status))
                //    {
                //        Insert_Transaction(id, 8, 3, 49, true);//49 = QS Department

                //        var get_email = from a in dbTNC.tnc_user
                //                        where (a.emp_position == 6 && a.emp_depart == 49) ||//QS Department
                //                        (a.emp_position == 4 && a.emp_plant == 11)//QC Div.
                //                        select a.email;
                //        foreach (var item in get_email)
                //        {
                //            SendEmailCenter(item, id);
                //        }
                //    }
                //}
                else if (act_id == 15)//No issuer SA Cust/NOK & Approve
                {
                    Insert_Transaction(id, 100, lv, org_id, false);//Approve
                    SendEmailCenter(GetEmailinRoot(id), id, 3, other);
                }
                else if (act_id == 16)//No issuer SA Cust/NOK & Reject
                {
                    Insert_Transaction(id, 101, lv, org_id, false);//Rejected
                    SendEmailCenter(GetEmailinRoot(id), id, 4, other);
                }
            }
        }

        private string GetEmailDivByDept(int deptid)
        {
            var query = (from a in dbTNC.View_Organization
                         where a.dept_id == deptid && a.active == true
                         select a.PlantMgr_email).First();
            if (query != null)
            {
                return "," + query;
            }
            else
            {
                return "";
            }
        }

        private string GetIssuerEmail(int id)
        {
            var get_sa = (from a in dbSA.TD_Main
                          where a.id == id
                          select a.issue_by).FirstOrDefault();
            if (get_sa != null)
            {
                var get_email = (from b in dbTNC.tnc_user
                                 where b.emp_code == get_sa
                                 select b.email).FirstOrDefault();
                if (!string.IsNullOrEmpty(get_email))
                {
                    return get_email;
                }
            }
            return "";
        }

        private string GetTellEmail(int id)
        {
            var get_sa = from a in dbSA.TD_Transaction
                         where a.id == id && a.status_id == 1 && a.lv_id <= 3 && a.active == false
                         select a.actor;

            string email = "";
            if (get_sa != null)
            {
                foreach (var item in get_sa)
                {
                    var get_email = (from b in dbTNC.tnc_user
                                     where b.emp_code == item
                                     select b.email).FirstOrDefault();
                    if (get_email != null)
                    {
                        if (!string.IsNullOrEmpty(get_email))
                            email += "," + get_email;
                    }
                }
                email += ",thaworn@nok.co.th,thudthong@nok.co.th";
                email = email.Substring(1);
            }
            return email;
        }

        private string GetQAEngEmail(int id)
        {
            string email = "";
            var get_all = from a in dbSA.TD_Transaction
                          where a.id == id && a.actor != null && a.status_id == 2 && a.lv_id == 1
                          select a.actor;
            if (get_all.Any())
            {
                foreach (var item in get_all)
                {
                    var get_email = (from a in dbTNC.V_Employee_Info
                                     where a.emp_code == item
                                     && (a.email != null && a.email != "")
                                     select a.email).FirstOrDefault();
                    if (get_email != null)
                    {
                        if (!string.IsNullOrEmpty(get_email))
                            email += "," + get_email;
                    }
                }
                email = email.Substring(1);
            }
            return email;
        }
        
        private bool Check_All_Approve(int id, byte status, byte lv=0, int org_id=0)
        {
            var query = from a in dbSA.TD_Transaction
                        where a.id == id && a.status_id == status && 
                        (a.act_id == null || a.act_id == 1 || (a.act_id > 7 && a.act_id < 10)) 
                        select a;
            if (lv != 0)
            {
                query = query.Where(w => w.lv_id <= lv);
            }
            if (org_id != 0)
            {
                query = query.Where(w => w.org_id == org_id);
            }
            return query.Any() ? false : true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="id"></param>
        /// <param name="type">0:Default, 1:Revise, 2:Tell Issuer, 3:Complete, 4:Close</param>
        public void SendEmailCenter(string receiver, int id, int type=0, string content="")
        {
            if (!string.IsNullOrEmpty(receiver))
            {
                string mailto = "";
                char[] delimiter = new char[] { ',' };
                string[] email = receiver.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                int max_email = 47;
                if (email.Length > max_email)//Max send email = 50
                {
                    for (int i = 0; i < max_email; i++)
                    {
                        mailto += "," + email[i];
                    }
                    mailto = mailto.Substring(1);
                }
                else
                {
                    mailto = receiver;
                }
                
                var get_sa = (from a in dbSA.TD_Main
                              where a.id == id
                              select a).FirstOrDefault();

                if (get_sa != null)
                {
                    TNCUtility tnc_util = new TNCUtility();
                    string subject = "";
                    string body = mailto + "<br />";
                    string int_link = "http://webExternal";//web02,webExternal
                    string ext_link = "http://webExternal.nok.co.th";//web02,webExternal
                    short flag = 0;//0=Send, 1=Not Send
                    if (type == 0)//Default
                    {
                        subject = "You have SA Online waiting for Operate.";
                        body += "Dear. All Concern,<br /><br />" + 
                            "<b>Control No. : </b>" + get_sa.control_no + "<br />" +
                            "<b>Type of Nonconformities. : </b>" + get_sa.title + "<br />" +
                            "<b>Item code : </b>" + get_sa.item_code + "<br />" +
                            "<b>Customer : </b>" + get_sa.customer + "<br />" +
                            "<b>Link : </b><a href='" + int_link + Url.Action("Detail", "SAProcess", new { id = get_sa.id }) + "'>Internal</a>, <a href='" + ext_link + Url.Action("Detail", "SAProcess", new { id = get_sa.id }) + "'>External</a><br />" +
                            "<br /><b>*This message is automatic sending from SA Online, please do not reply*</b><br />" +
                            "<br />Best Regard,<br />From SA Online";
                    }
                    else if (type == 1)//Revise
                    {
                        subject = "You have SA Online waiting for Revise.";
                        body += "Dear. Issuer,<br /><br />" + 
                            "<b>Control No. : </b>" + get_sa.control_no + "<br />" +
                            "<b>Type of Nonconformities. : </b>" + get_sa.title + "<br />" +
                            "<b>Item code : </b>" + get_sa.item_code + "<br />" +
                            "<b>Customer : </b>" + get_sa.customer + "<br />" +
                            "<b>Comment : </b>" + content +
                            " (<b>From : </b>" + Session["SA_Name"].ToString() + ")<br />" +
                            "<b>Link : </b><a href='" + int_link + Url.Action("Detail", "SAProcess", new { id = get_sa.id }) + "'>Internal</a>, <a href='" + ext_link + Url.Action("Detail", "SAProcess", new { id = get_sa.id }) + "'>External</a><br />" +
                            "<br /><b>*This message is automatic sending from SA Online, please do not reply*</b><br />" +
                            "<br />Best Regard,<br />From SA Online";
                    }
                    else if (type == 2)//Tell Issuer
                    {
                        subject = "You have SA Online waiting for support";
                        body += "Dear. Issuer,<br /><br />" + 
                            "<b>Control No. : </b>" + get_sa.control_no + "<br />" +
                            "<b>Type of Nonconformities. : </b>" + get_sa.title + "<br />" +
                            "<b>Item code : </b>" + get_sa.item_code + "<br />" +
                            "<b>Customer : </b>" + get_sa.customer + "<br />" +
                            "<b>Tell Issuer : </b>" + content +
                            " (<b>From : </b>" + Session["SA_Name"].ToString() + ")<br />" +
                            "<b>*Must click reply after support complete*</b><br />"+
                            "<b>Link : </b><a href='" + int_link + Url.Action("Detail", "SAProcess", new { id = get_sa.id }) + "'>Internal</a>, <a href='" + ext_link + Url.Action("Detail", "SAProcess", new { id = get_sa.id }) + "'>External</a><br />" +
                            "<br /><b>*This message is automatic sending from SA Online, please do not reply*</b><br />" +
                            "<br />Best Regard,<br />From SA Online";
                    }
                    else if (type == 3)//Complete
                    {
                        subject = "SA Accepted (SA No. : " + get_sa.control_no + ")";
                        body += "Dear All,<br /><br />" + 
                            "<b>Control No. : </b>" + get_sa.control_no + "<br />" +
                            "<b>Type of Nonconformities. : </b>" + get_sa.title + "<br />" +
                            "<b>Item code : </b>" + get_sa.item_code + "<br />" +
                            "<b>Customer : </b>" + get_sa.customer + "<br />" +
                            "<b>Defective Q'ty : </b>" + get_sa.defective_qty + "<br />" +
                            "<b>Remark : </b>" + content + "<br />" +
                            "<b>Link : </b><a href='" + int_link + Url.Action("Detail", "SAProcess", new { id = get_sa.id }) + "'>Internal</a>, <a href='" + ext_link + Url.Action("Detail", "SAProcess", new { id = get_sa.id }) + "'>External</a><br />" +
                            "<br /><b>*This message is automatic sending from SA Online, please do not reply*</b><br />" +
                            "<br />Best Regard,<br />From SA Online";
                    }
                    else if (type == 4)//Close, Reject
                    {
                        subject = "SA Rejected (SA No. : " + get_sa.control_no + ")";
                        body += "Dear All,<br /><br />" + 
                            "<b>Control No. : </b>" + get_sa.control_no + "<br />" +
                            "<b>Type of Nonconformities. : </b>" + get_sa.title + "<br />" +
                            "<b>Item code : </b>" + get_sa.item_code + "<br />" +
                            "<b>Customer : </b>" + get_sa.customer + "<br />" +
                            "<b>Remark : </b>" + content + "<br />" +
                            "<b>Link : </b><a href='" + int_link + Url.Action("Detail", "SAProcess", new { id = get_sa.id }) + "'>Internal</a>, <a href='" + ext_link + Url.Action("Detail", "SAProcess", new { id = get_sa.id }) + "'>External</a><br />" +
                            "<br /><b>*This message is automatic sending from SA Online, please do not reply*</b><br />" +
                            "<br />Best Regard,<br />From SA Online";
                    }
                    else if (type == 5)//Not Accept
                    {
                        subject = "SA Not Accept";
                        body += "Dear Issuer,<br /><br />" + 
                            "<b>Control No. : </b>" + get_sa.control_no + "<br />" +
                            "<b>Type of Nonconformities. : </b>" + get_sa.title + "<br />" +
                            "<b>Item code : </b>" + get_sa.item_code + "<br />" +
                            "<b>Customer : </b>" + get_sa.customer + "<br />" +
                            "<b>Not Accepted because : </b>" + content + 
                            " (<b>From : </b>" + Session["SA_Name"].ToString() + ")<br />" +
                            "<b>Link : </b><a href='" + int_link + Url.Action("Detail", "SAProcess", new { id = get_sa.id }) + "'>Internal</a>, <a href='" + ext_link + Url.Action("Detail", "SAProcess", new { id = get_sa.id }) + "'>External</a><br />" +
                            "<br /><b>*This message is automatic sending from SA Online, please do not reply*</b><br />" +
                            "<br />Best Regard,<br />From SA Online";
                    }
                    else if (type == 6)//Reply Tell Issuer
                    {
                        subject = "Support completed, You have SA Online for Operate again.";
                        body += "Dear All,<br /><br />" + 
                            "<b>Control No. : </b>" + get_sa.control_no + "<br />" +
                            "<b>Type of Nonconformities. : </b>" + get_sa.title + "<br />" +
                            "<b>Item code : </b>" + get_sa.item_code + "<br />" +
                            "<b>Customer : </b>" + get_sa.customer + "<br />" +
                            "<b>Tell Issuer : </b>" + content + "<br />" +
                            "<b>Link : </b><a href='" + int_link + Url.Action("Detail", "SAProcess", new { id = get_sa.id }) + "'>Internal</a>, <a href='" + ext_link + Url.Action("Detail", "SAProcess", new { id = get_sa.id }) + "'>External</a><br />" +
                            "<br /><b>*This message is automatic sending from SA Online, please do not reply*</b><br />" +
                            "<br />Best Regard,<br />From SA Online";
                    }

                    //tnc_util.SendMail(8, "TNCAutoMail-SA@nok.co.th", mailto, subject, body, null, flag: flag);//Real
                    tnc_util.SendMail(8, "TNCAutoMail-SA@nok.co.th", "monchit@nok.co.th", subject, body);//Test
                }
            }
        }

        [HttpPost]
        public ActionResult GridSAOnProcess(string control_no, string title, string name, string group, string status, string item, int user = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = from a in dbSA.v_sa
                            select a;

                if (!string.IsNullOrEmpty(control_no))
                {
                    query = query.Where(w => w.control_no.Contains(control_no.ToUpper()));
                }
                if (!string.IsNullOrEmpty(title))
                {
                    query = query.Where(w => w.title.ToLower().Contains(title.ToLower()));
                }
                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(w => w.name.ToLower().Contains(name.ToLower()));
                }
                if (!string.IsNullOrEmpty(group))
                {
                    query = query.Where(w => w.group_name.ToLower().Contains(group.ToLower()));
                }
                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(w => w.status.ToLower().Contains(status.ToLower()));
                }
                if (!string.IsNullOrEmpty(item))
                {
                    query = query.Where(w => w.item_code.ToLower().Contains(item.ToLower()));
                }

                if (user == 1)
                {
                    if (Session["SA_Auth"] != null)
                    {
                        var auth = Session["SA_Auth"].ToString();
                        query = query.Where(w => w.issue_by == auth);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query.AsQueryable().OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize)
                    .Select(s => new
                    {
                        s.id,
                        s.control_no,
                        s.title,
                        s.item_code,
                        s.name,
                        s.group_name,
                        s.issue_dt,
                        s.status
                    });

                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        
        [HttpPost]
        public ActionResult GridSAClosed(string control_no, string title, string issuer, string group, string status, string item, int user = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = (from m in dbSA.TD_Main.ToList()
                             join u in dbTNC.tnc_user.ToList() on m.issue_by equals u.emp_code
                             join t in dbSA.TD_Transaction.Where(w => w.status_id >= 100).ToList() on m.id equals t.id
                             join g in dbTNC.tnc_group_master.ToList() on t.org_id equals g.id
                             select new
                             {
                                 m.id,
                                 m.control_no,
                                 m.title,
                                 name = u.emp_fname + " " + u.emp_lname,
                                 g.group_name,
                                 m.issue_dt,
                                 m.issue_by,
                                 m.item_code,
                                 status = GetStatusName(m.id)
                             });

                if (!string.IsNullOrEmpty(control_no))
                {
                    query = query.Where(w => w.control_no.IndexOf(control_no, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                if (!string.IsNullOrEmpty(title))
                {
                    query = query.Where(w => w.title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                if (!string.IsNullOrEmpty(issuer))
                {
                    query = query.Where(w => w.name.IndexOf(issuer, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                if (!string.IsNullOrEmpty(group))
                {
                    query = query.Where(w => w.group_name.IndexOf(group, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(w => w.status.IndexOf(status, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                if (!string.IsNullOrEmpty(item))
                {
                    query = query.Where(w => w.item_code.IndexOf(item, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (user == 1)
                {
                    if (Session["SA_Auth"] != null)
                        query = query.Where(w => w.issue_by == Session["SA_Auth"].ToString());
                    else
                        return RedirectToAction("Index", "Home");
                }

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query.AsQueryable().OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize)
                    .Select(s => new
                    {
                        s.id,
                        s.control_no,
                        s.title,
                        issuer = s.name,
                        s.group_name,
                        s.issue_dt,
                        s.status
                    });

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult IssueSAList(int said, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var query = from a in dbSA.TD_SA
                            where a.id == said
                            select a;

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query.ToList()
                    .Select(s => new
                    {
                        s.id,
                        s.sa_id,
                        s.sa_no,
                        cust_name = !string.IsNullOrEmpty(s.cust_name) ? s.cust_name : "",
                        s.result,
                        s.issue_date,
                        reason = !string.IsNullOrEmpty(s.reason) ? s.reason : "",
                        file_evidence = !string.IsNullOrEmpty(s.file_evidence) ? //Url.Content(s.file_evidence) : ""
                        "<a href='" + Url.Content(s.file_evidence) + "' target='_blank'><i class='icon-folder-open'></i></a>" : ""
                    }).AsQueryable().OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult AddIssueSA(int sid, HttpPostedFileBase files)
        {
            try
            {
                TD_SA sa = new TD_SA();
                sa.id = sid;
                sa.sa_no = Request.Form["txtSANo"];
                sa.cust_name = Request.Form["txtCust"];
                sa.result = byte.Parse(Request.Form["selResult"]);
                sa.issue_date = ParseToDate(Request.Form["dtpIssueDate"]);
                sa.reason = Request.Form["txaReason"];
                sa.update_by = Session["SA_Auth"].ToString();
                //sa.file_evidence = file.FileName;
                DateTime dt = DateTime.Now;
                string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/" + sid + "/";
                if (!Directory.Exists(Server.MapPath(subPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(subPath));
                }
                if (files != null && files.ContentLength > 0)
                {
                    if (files.ContentType == "application/pdf")//Check accept file type.
                    {
                        var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(files.FileName);
                        var path = Path.Combine(Server.MapPath(subPath), fileName);
                        files.SaveAs(path);
                        sa.file_evidence = subPath + fileName;
                    }
                }

                dbSA.TD_SA.Add(sa);
                dbSA.SaveChanges();
                //return Json(new { Result = "OK" });
                return RedirectToAction("IssueSA", "SAProcess", new { sid = sid });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult CompIssueSA()
        {
            var id = int.Parse(Request.Form["hdIDC"]);
            var org = int.Parse(Request.Form["hdOrg"]);//Add by Monchit 2015/07/27
            //TNCOrganization tnc_org = new TNCOrganization();
            //tnc_org.GetApprover(Session["SA_Auth"].ToString(),3);

            Update_Transaction(id, 1, org, true, Session["SA_Auth"].ToString(), 2, "", set_active:false);
            //Insert_Transaction(id, 8, 3, tnc_org.OrgId, true);//11 is Quality Control Div.
            Insert_Transaction(id, 8, 3, 49, true);//49 = QS Department
            //SendEmailCenter(tnc_org.ManagerEMail,id);
            //SendEmailCenter("duangnapa@nok.co.th", id);
            var get_email = from a in dbTNC.tnc_user
                            where (a.emp_position == 6 && a.emp_depart == 49) ||//QS Department
                            (a.emp_position == 4 && a.emp_plant == 11)//QC Div.
                            select a.email;
            foreach (var item in get_email)
            {
                SendEmailCenter(item, id);
            }
            //AddDeptDivTransaction(id, 7, 1, 1);
            
            return RedirectToAction("Index","SAProcess");
        }

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public JsonResult GetIssueSA(int sa_id)
        {
            var sa = (from a in dbSA.TD_SA.ToList()
                      where a.sa_id == sa_id
                      select new
                      {
                          id = a.id,
                          sa_no = a.sa_no,
                          cust_name = a.cust_name,
                          result = a.result,
                          reason = a.reason,
                          issue_date = a.issue_date.Day + "-" + a.issue_date.Month + "-" + a.issue_date.Year
                      }).FirstOrDefault();
            if (sa != null)
                return Json(sa, JsonRequestBehavior.AllowGet);
            else
                return Json(0, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateIssueSA(int sid, HttpPostedFileBase files_e)
        {
            try
            {
                //var sa = dbSA.TD_SA.Find(int.Parse(Request.Form["sa_id"]));
                int sa_id = int.Parse(Request.Form["hdId"]);
                var sa = (from a in dbSA.TD_SA
                          where a.id == sid && a.sa_id == sa_id
                          select a).FirstOrDefault();

                sa.sa_no = Request.Form["txtSANo_e"];
                sa.cust_name = Request.Form["txtCust_e"];
                sa.result = byte.Parse(Request.Form["selResult_e"]);
                sa.issue_date = ParseToDate(Request.Form["dtpIssueDate_e"]);
                sa.reason = Request.Form["txaReason_e"];
                sa.update_by = Session["SA_Auth"].ToString();
                //sa.file_evidence = file.FileName;

                DateTime dt = DateTime.Now;
                string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/" + sid + "/";
                if (!Directory.Exists(Server.MapPath(subPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(subPath));
                }
                if (files_e != null && files_e.ContentLength > 0)
                {
                    //DateTime dt = DateTime.Now;
                    //string subPath = "~/UploadFiles/" + dt.Year + "/" + dt.Month + "/" + sid + "/";
                    if (files_e.ContentType == "application/pdf")//Check accept file type.
                    {
                        var fileName = DateTime.Now.Millisecond + "_" + Path.GetFileName(files_e.FileName);
                        var path = Path.Combine(Server.MapPath(subPath), fileName);
                        files_e.SaveAs(path);
                        sa.file_evidence = subPath + fileName;
                    }
                }

                dbSA.SaveChanges();
                return RedirectToAction("IssueSA", "SAProcess", new { sid = sid });
                //return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public bool CheckIssueSAComplete(int id)
        {
            var query = from a in dbSA.TD_SA
                        where a.id == id && (a.result == 1)
                        select a;
            if (query.Any())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [OutputCache(Duration = 0, VaryByParam = "*", NoStore = true)]
        public ActionResult RenderStatus(int id)
        {
            string status_menu = "";
            var all_status = dbSA.TM_Status.Where(w => w.lv_min != 0).OrderBy(o => o.status_id);
            int current_status = GetStatusId(id);
            foreach (var item in all_status)
            {
                if (current_status < item.status_id)
                {
                    status_menu += "<button type='button' class='btn' disabled='disabled' data-stid='" + item.status_id + "' data-id='" + id + "' >" + item.status_name + "</button> ";
                }
                else if (current_status > item.status_id)
                {
                    status_menu += "<button type='button' class='btn btn-success' data-stid='" + item.status_id + "' data-id='" + id + "' >" + item.status_name + "</button> ";
                }
                else
                {
                    status_menu += "<button type='button' class='btn btn-primary' data-stid='" + item.status_id + "' data-id='" + id + "' >" + item.status_name + "</button> ";
                }
            }

            return Json(status_menu, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SelectQA(string searchTerm)
        {
            var qa = dbSA.TM_SysGroup
                .Where(w => w.Sys_GroupType_id == 2 && (w.Sys_Group_name.Contains(searchTerm)))
                .OrderBy(o => o.Sys_Plant_id)
                .Select(a => new { id = a.Sys_Plant_id, text = a.Sys_Group_name })
                .Take(10).ToList();
            return Json(qa, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SelectQC(string searchTerm)
        {
            var qc = dbTNC.View_Organization
                .Where(w => (w.dept_id == 3 || w.dept_id == 14 || w.dept_id == 50) && w.active == true && w.IsRealCostCode == 1 && w.group_id != 0 
                    && (w.group_name.Contains(searchTerm)))
                .GroupBy(g => new {g.group_id, g.group_name})
                .OrderBy(g => g.Key.group_name)
                .Select(g => new { id = g.Key.group_id, text = g.Key.group_name })
                .Take(20).ToList();
            return Json(qc, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SelectEN(string searchTerm)
        {
            var en = dbTNC.View_Organization
                .Where(w => w.plant_id == 10 && w.group_id != 0 && w.active == true && w.IsRealCostCode == 1
                    && (w.group_name.Contains(searchTerm)))
                .GroupBy(g => new { g.group_id, g.group_name })
                .OrderBy(g => g.Key.group_name)
                .Select(g => new { id = g.Key.group_id, text = g.Key.group_name })
                .Take(16).ToList();
            return Json(en, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SelectOther(string searchTerm)
        {
            var other = dbTNC.View_Organization
                .Where(w => (w.dept_id < 14 || w.dept_id > 18) && w.group_id != 0 && w.active == true && w.IsRealCostCode == 1
                    && (w.group_name.Contains(searchTerm)))
                .GroupBy(g => new { g.group_id, g.group_name })
                .OrderBy(g => g.Key.group_name)
                .Select(g => new { id = g.Key.group_id, text = g.Key.group_name })
                .Take(20).ToList();
            return Json(other, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SelectAll(string searchTerm)
        {
            var group = dbTNC.View_Organization
                .Where(w => w.group_id != 0 && w.active == true && w.IsRealCostCode == 1
                    && (w.group_name.Contains(searchTerm)))
                .GroupBy(g => new { g.group_id, g.group_name })
                .OrderBy(g => g.Key.group_name)
                .Select(g => new { id = g.Key.group_id, text = g.Key.group_name })
                .Take(20).ToList();
            return Json(group, JsonRequestBehavior.AllowGet);
        }

        private bool AddConcernQC(int id, string concern)
        {
            if (!string.IsNullOrEmpty(concern))
            {
                int[] group = concern.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
                foreach (var item in group)
                {
                    var check = from a in dbSA.TD_ConcernQC
                                where a.id == id && a.group_id == item
                                select a;
                    if (!check.Any())
                    {
                        var qc = new TD_ConcernQC();
                        qc.id = id;
                        qc.group_id = item;
                        dbSA.TD_ConcernQC.Add(qc);
                        dbSA.SaveChanges();
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool AddConcernEN(int id, string concern)
        {
            if (!string.IsNullOrEmpty(concern))
            {
                int[] group = concern.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
                foreach (var item in group)
                {
                    var check = from a in dbSA.TD_ConcernEN
                                where a.id == id && a.group_id == item
                                select a;
                    if (!check.Any())
                    {
                        var en = new TD_ConcernEN();
                        en.id = id;
                        en.group_id = item;
                        dbSA.TD_ConcernEN.Add(en);
                        dbSA.SaveChanges();
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool AddConcernOther(int id, string concern)
        {
            if (!string.IsNullOrEmpty(concern))
            {
                int[] group = concern.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
                foreach (var item in group)
                {
                    var check = from a in dbSA.TD_ConcernOther
                                where a.id == id && a.group_id == item
                                select a;
                    if (!check.Any())
                    {
                        var oth = new TD_ConcernOther();
                        oth.id = id;
                        oth.group_id = item;
                        dbSA.TD_ConcernOther.Add(oth);
                        dbSA.SaveChanges();
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool AddInform(int id, string inform)
        {
            if (!string.IsNullOrEmpty(inform))
            {
                int[] group = inform.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
                foreach (var item in group)
                {
                    var check = from a in dbSA.TD_Inform
                                where a.id == id && a.group_id == item
                                select a;
                    if (!check.Any())
                    {
                        var inf = new TD_Inform();
                        inf.id = id;
                        inf.group_id = item;
                        dbSA.TD_Inform.Add(inf);
                        dbSA.SaveChanges();
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddQATransaction(int id, byte new_status, byte min_lv)
        {
            //Add QA that Issuer select plant of QA
            var get_plant = (from p in dbSA.TD_Main
                             where p.id == id
                             select p.Sys_Plant_id).FirstOrDefault();

            var get_group = (from q in dbSA.TM_SysGroup
                             where q.del_flag == false && q.Sys_Plant_id == get_plant
                             && q.Sys_GroupType_id == new_status
                             select q.group_id).FirstOrDefault();

            Insert_Transaction(id, new_status, min_lv, get_group, true, get_plant);
            SendEmailCenter(GetSysEmail(new_status, get_plant, min_lv), id);
        }

        private bool AddQCTransaction(int id, byte new_status, byte min_lv)
        {
            var get_qc = from a in dbSA.TD_ConcernQC
                         where a.id == id
                         select a;

            if (get_qc.Any())
            {
                foreach (var item in get_qc)
                {
                    Insert_Transaction(id, new_status, min_lv, item.group_id, true);
                    SendEmailCenter(GetTNCEmailbyGroup(item.group_id, 1), id);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool AddENTransaction(int id, byte new_status, byte min_lv)
        {
            var get_en = from a in dbSA.TD_ConcernEN
                         where a.id == id
                         select a;

            if (get_en.Any())
            {
                foreach (var item in get_en)
                {
                    Insert_Transaction(id, new_status, min_lv, item.group_id, true);
                    SendEmailCenter(GetTNCEmailbyGroup(item.group_id, 1), id);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool AddOtherTransaction(int id, byte new_status, byte min_lv)
        {
            var get_oth = from a in dbSA.TD_ConcernOther
                         where a.id == id
                         select a;

            if (get_oth.Any())
            {
                foreach (var item in get_oth)
                {
                    TNCOrganization ot_org = new TNCOrganization();
                    ot_org.GetApprover(item.group_id, 1);

                    Insert_Transaction(id, new_status, (byte)(ot_org.OrgLevel + 1), ot_org.OrgId, true);
                    SendEmailCenter(ot_org.ManagerEMail, id);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddDeptDivTransaction(int id, byte status, byte lv, byte add_status = 0)
        {
            TNCOrganization tnc_org = new TNCOrganization();
            var query = from a in dbSA.TD_Transaction
                        where a.id == id && a.status_id == status && a.lv_id == lv
                        select a.actor;

            byte new_status = (byte)(status + add_status);

            foreach (var item in query)
            {
                tnc_org.GetApprover(item);
                if (status == 1)//Issuer
                {
                    Insert_Transaction(id, new_status, (byte)(tnc_org.OrgLevel + 1), tnc_org.OrgId, true);
                    SendEmailCenter(tnc_org.ManagerEMail, id);
                }
                else//Other
                {
                    if (!Check_Transaction(id, new_status, (byte)(tnc_org.OrgLevel + 1), 0, tnc_org.OrgId))
                    {
                        Insert_Transaction(id, new_status, (byte)(tnc_org.OrgLevel + 1), tnc_org.OrgId, true);
                        SendEmailCenter(tnc_org.ManagerEMail, id);
                    }
                }
            }
        }

        public void ExportLedger()
        {
            TNCUtility util = new TNCUtility();
            DateTime dateForm = ParseToDate(Request.Form["dtpDateFrom"]);
            DateTime dateTo = ParseToDate(Request.Form["dtpDateTo"]);

            var query = (from a in dbSA.TD_Main.ToList()
                         where a.issue_dt >= dateForm && a.issue_dt <= dateTo.AddDays(1)
                         select new
                         {
                             IssueDate = a.issue_dt.ToString("dd-MM-yyyy"),
                             Plant_Div = getUserPlant(a.issue_by),
                             IssueGroup = getIssueGroup(a.id),
                             IssueNo = a.control_no,
                             ItemCode = a.item_code,
                             MainProblem = GetProblem(a.id),
                             ProblemDetail = a.title,
                             Status = getStatusExcel(a.id),
                             Reason = getFinalReason(a.id)
                         }).ToList();
                         
            util.CreateExcel(query,"Ledger_" + DateTime.Now.ToString("dd-MM-yyyy"));
        }

        private string getFinalReason(int p)
        {
            var get_reason = (from a in dbSA.TD_Transaction
                              where a.id == p && a.status_id == 8 && a.act_id == 5
                              select a).FirstOrDefault();
            if (get_reason != null)
                return get_reason.comment;
            else
                return "";
        }

        private object getStatusExcel(int p)
        {
            var status = "";
            var get_status = (from a in dbSA.TD_Transaction
                              where a.id == p && a.active == false
                              orderby a.status_id descending
                              select a).FirstOrDefault();
            if (get_status != null)
            {
                if (get_status.status_id == 100)
                {
                    status = "Approved";
                }
                else if (get_status.status_id == 101)
                {
                    status = "Rejected";
                }
                else if (get_status.status_id == 102)
                {
                    status = "Canceled";
                }
                else
                {
                    status = "Pending";
                }
            }
            else
            {
                status = "Error";
            }

            return status;
        }

        private string getUserPlant(string p)
        {
            var plant_name = "";
            var get_plant = (from a in dbTNC.V_Employee_Info
                            where a.emp_code == p
                            select a.plant_name).FirstOrDefault();

            if(get_plant.Any()){
                plant_name = get_plant;
            }

            return plant_name;
        }

        private string getIssueGroup(int p)
        {
            var group_name = "";
            var query = (from a in dbSA.TD_Transaction
                         where a.act_id == 0 && a.id == p
                         select a).FirstOrDefault();
            if (query != null)
            {
                var getgroupname = (from b in dbTNC.tnc_group_master
                                    where b.id == query.org_id
                                    select b.group_name).FirstOrDefault();
                if (getgroupname != null)
                {
                    group_name = getgroupname;
                }
            }
            return group_name;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) dbSA.Dispose();
            base.Dispose(disposing);
        }
    }
}
