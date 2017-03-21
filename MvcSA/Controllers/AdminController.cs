using MvcSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using MvcJqGrid;
using System.Data.OleDb;
//using System.Data.SqlClient;
using System.IO;
using System.Data;

namespace MvcSA.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        SAEntities dbSA = new SAEntities();
        TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();
        PCREntities dbPCR = new PCREntities();

        public ActionResult Index()
        {
            ViewBag.Title = "Master";
            return View();
        }

        [Check_Authen]
        [Check_Authen_Admin]
        public ActionResult Master_User()
        {
            ViewBag.Title = "Master User";
            return View();
        }

        [Check_Authen]
        [Check_Authen_Admin]
        public ActionResult Master_Group()
        {
            ViewBag.Title = "Master Group";
            return View();
        }

        [Check_Authen]
        [Check_Authen_Admin]
        public ActionResult Master_GroupType()
        {
            ViewBag.Title = "Master Group Type";
            return View();
        }

        [Check_Authen]
        [Check_Authen_Admin]
        public ActionResult Master_Plant()
        {
            ViewBag.Title = "Master Plant";
            return View();
        }

        [Check_Authen]
        [Check_Authen_Admin]
        public ActionResult Master_GroupCode()
        {
            ViewBag.Title = "Master Group Code";
            return View();
        }

        public ActionResult GridPlantMaster(string sidx, string sord, int page, int rows, string search)
        {
            //var dbSel = dbSA.TM_SysPlant.Where(w => w.del_flag != true);
            var dbSel = dbPCR.PCR_Impact;
            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;
            var totalRecords = dbSel.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);

            var plant = dbSel
                            .OrderBy(sidx + " " + sord)
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize).AsEnumerable();
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = plant.Select(p => new
                {
                    //i = p.Sys_Plant_id,
                    //cell = new object[] {
                    //    p.Sys_Plant_id,
                    //    p.Sys_Plant_name
                    //}
                    i = p.imp_id,
                    cell = new object[] {
                        p.imp_id,
                        p.imp_name
                    }
                }).ToArray()
            };
            return Json(jsonData);
        }
        public ActionResult AddPlant(string Sys_Plant_name)
        {
            try
            {
                using (var db = new SAEntities())
                {
                    TM_SysPlant plant = new TM_SysPlant();
                    plant.Sys_Plant_name = Sys_Plant_name;
                    plant.del_flag = false;
                    plant.update_dt = DateTime.Now;
                    plant.update_by = Session["SA_Auth"].ToString();
                    db.TM_SysPlant.Add(plant);
                    db.SaveChanges();
                }

                //var db = new PCREntities();
                //PCR_Impact plant = new PCR_Impact();
                //plant.imp_name = Sys_Plant_name;
                //db.PCR_Impact.Add(plant);
                
                return Json(true);
            }
            catch (Exception)
            {
                // Do some error logging stuff, handle exception, etc.
                return Json(false);
            }
        }
        public ActionResult EditPlant(byte Sys_Plant_id, string Sys_Plant_name)
        {
            try
            {
                //var db = new SAEntities();

                //var query = from p in db.TM_SysPlant
                //            where p.Sys_Plant_id == Sys_Plant_id
                //            select p;

                //var plant = query.First();
                //plant.Sys_Plant_name = Sys_Plant_name;
                //plant.update_dt = DateTime.Now;
                //plant.update_by = Session["SA_Auth"].ToString();

                var db = new PCREntities();

                var query = from p in db.PCR_Impact
                            where p.imp_id == Sys_Plant_id
                            select p;

                var plant = query.First();
                plant.imp_name = Sys_Plant_name;

                db.SaveChanges();

                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
        public ActionResult DelPlant(byte id)
        {
            try
            {
                //var db = new SAEntities();

                //var query = from p in db.TM_SysPlant
                //            where p.Sys_Plant_id == id
                //            select p;

                //var plant = query.First();
                //plant.del_flag = true;
                //plant.update_dt = DateTime.Now;
                //plant.update_by = Session["SA_Auth"].ToString();

                //var db = new PCREntities();

                //var query = db.PCR_Impact.Find(id);
                //db.PCR_Impact.Remove(query);

                //db.SaveChanges();

                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        //public ActionResult GridGroupTypeMaster(string sidx, string sord, int page, int rows, string search)
        //{
        //    var dbSel = dbSA.TM_SysGroupType.Where(w => w.del_flag != true);
        //    var pageIndex = Convert.ToInt32(page) - 1;
        //    var pageSize = rows;
        //    var totalRecords = dbSel.Count();
        //    var totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);

        //    var plant = dbSel
        //                    .OrderBy(sidx + " " + sord)
        //                    .Skip(pageIndex * pageSize)
        //                    .Take(pageSize).AsEnumerable();
        //    var jsonData = new
        //    {
        //        total = totalPages,
        //        page = page,
        //        records = totalRecords,
        //        rows = plant.Select(p => new
        //        {
        //            i = p.Sys_GroupType_id,
        //            cell = new object[] {
        //                p.Sys_GroupType_id,
        //                p.Sys_GroupType_name
        //            }
        //        }).ToArray()
        //    };
        //    return Json(jsonData);
        //}
        public ActionResult AddGroupType(string Sys_GroupType_name)
        {
            try
            {
                //var db = new SAEntities();
                //TM_SysGroupType gtype = new TM_SysGroupType();
                //gtype.Sys_GroupType_name = Sys_GroupType_name;
                //gtype.del_flag = false;
                //gtype.update_dt = DateTime.Now;
                //gtype.update_by = Session["SA_Auth"].ToString();
                //db.TM_SysGroupType.Add(gtype);
                //db.SaveChanges();

                return Json(true);
            }
            catch (Exception)
            {
                // Do some error logging stuff, handle exception, etc.
                return Json(false);
            }
        }
        public ActionResult EditGroupType(byte Sys_GroupType_id, string Sys_GroupType_name)
        {
            try
            {
                //var db = new SAEntities();

                //var query = from p in db.TM_SysGroupType
                //            where p.Sys_GroupType_id == Sys_GroupType_id
                //            select p;

                //var gtype = query.First();

                //gtype.Sys_GroupType_name = Sys_GroupType_name;
                //gtype.update_dt = DateTime.Now;
                //gtype.update_by = Session["SA_Auth"].ToString();
                //db.SaveChanges();

                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
        public ActionResult DelGroupType(byte id)
        {
            try
            {
                //var db = new SAEntities();

                //var query = from p in db.TM_SysGroupType
                //            where p.Sys_GroupType_id == id
                //            select p;

                //var gtype = query.First();

                //gtype.del_flag = true;
                //gtype.update_dt = DateTime.Now;
                //gtype.update_by = Session["SA_Auth"].ToString();
                //db.SaveChanges();

                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        //public ActionResult GridGroupMaster(string sidx, string sord, int page, int rows, string search)
        //{
        //    var dbSel = dbSA.TM_SysGroup.Where(w => w.del_flag != true);
        //    var pageIndex = Convert.ToInt32(page) - 1;
        //    var pageSize = rows;
        //    var totalRecords = dbSel.Count();
        //    var totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);

        //    var group = (from p in dbSA.TM_SysGroup.ToList()
        //                 join g in dbTNC.tnc_group_master.ToList()
        //                 on p.group_id equals g.id
        //                 select new
        //                 {
        //                     group_name = p.Sys_Group_name,
        //                     group_id = g.group_name,
        //                     group_type = p.TM_SysGroupType.Sys_GroupType_name,
        //                     plant = p.TM_SysPlant.Sys_Plant_name
        //                 }).AsQueryable()
        //                 .OrderBy(sidx + " " + sord)
        //                 .Skip(pageIndex * pageSize)
        //                 .Take(pageSize).AsEnumerable();
        //    var jsonData = new
        //    {
        //        total = totalPages,
        //        page = page,
        //        records = totalRecords,
        //        rows = group.Select(g => new
        //        {
        //            i = g.group_id,
        //            cell = new object[] {
        //                g.group_id,
        //                g.group_name,
        //                g.group_type,
        //                g.plant
        //            }
        //        }).ToArray()
        //    };
        //    return Json(jsonData);
        //}
        public ActionResult AddGroup(int group_id, string group_name, byte group_type, byte plant)
        {
            try
            {
                //var db = new SAEntities();
                //TM_SysGroup group = new TM_SysGroup();
                //group.group_id = group_id;
                //group.Sys_Group_name = group_name;
                //group.Sys_GroupType_id = group_type;
                //group.Sys_Plant_id = plant;
                //group.del_flag = false;
                //group.update_dt = DateTime.Now;
                //group.update_by = Session["SA_Auth"].ToString();
                //dbSA.TM_SysGroup.Add(group);
                //dbSA.SaveChanges();

                PCR_SubGroup group = new PCR_SubGroup();
                group.pcr_group_id = group_id;
                group.pcr_group_name = group_name;
                group.group_type_id = group_type;
                dbPCR.PCR_SubGroup.Add(group);
                dbPCR.SaveChanges();

                return Json(true);
            }
            catch (Exception)
            {
                // Do some error logging stuff, handle exception, etc.
                return Json(false);
            }
        }
        public ActionResult EditGroup(int group_id, string group_name, byte group_type, byte plant)
        {
            try
            {
                //var query = from g in dbSA.TM_SysGroup
                //            where g.group_id == group_id
                //            select g;

                //var group = query.First();

                //group.Sys_Group_name = group_name;
                //group.Sys_GroupType_id = group_type;
                //group.Sys_Plant_id = plant;
                //group.update_dt = DateTime.Now;
                //group.update_by = Session["SA_Auth"].ToString();
                //dbSA.SaveChanges();

                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
        public ActionResult DelGroup(byte id)
        {
            try
            {
                //var db = new SAEntities();

                //var query = from p in db.TM_SysGroupType
                //            where p.Sys_GroupType_id == id
                //            select p;

                //var gtype = query.First();

                //gtype.del_flag = true;
                //gtype.update_dt = DateTime.Now;
                //gtype.update_by = Session["SA_Auth"].ToString();
                //db.SaveChanges();

                //var db = new PCREntities();

                //var query = from p in db.PCR_GroupType
                //            select p;

                //var gtype = query.First();

                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        //public ActionResult GridUserMaster(string sidx, string sord, int page, int rows, string search)
        //{
        //    var dbSel = dbSA.TM_SysUser;
        //    var pageIndex = Convert.ToInt32(page) - 1;
        //    var pageSize = rows;
        //    var totalRecords = dbSel.Count();
        //    var totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);

        //    var user = (from u in dbSA.TM_SysUser.ToList()
        //                 join t in dbTNC.tnc_user.ToList()
        //                 on u.Sys_User_id equals t.emp_code
        //                 select new
        //                 {
        //                     emp_code = u.Sys_User_id,
        //                     emp_name = t.emp_fname + " " + t.emp_lname,
        //                     group_name = u.TM_SysGroup.Sys_Group_name,
        //                     plant_name = u.TM_SysGroup.TM_SysPlant.Sys_Plant_name,
        //                     group_type = u.TM_SysGroup.TM_SysGroupType.Sys_GroupType_name
        //                 }).AsQueryable()
        //                 .OrderBy(sidx + " " + sord)
        //                 .Skip(pageIndex * pageSize)
        //                 .Take(pageSize).AsEnumerable();
        //    var jsonData = new
        //    {
        //        total = totalPages,
        //        page = page,
        //        records = totalRecords,
        //        rows = user.Select(u => new
        //        {
        //            i = u.emp_code,
        //            cell = new object[] {
        //                u.emp_code,
        //                u.emp_name,
        //                u.group_name,
        //                u.plant_name,
        //                u.group_type
        //            }
        //        }).ToArray()
        //    };
        //    return Json(jsonData);
        //}
        //public ActionResult AddUser(string emp_code, int group_name)
        //{
        //    try
        //    {
        //        //var db = new SAEntities();
        //        //TM_SysUser user = new TM_SysUser();
        //        //user.group_id = group_name;
        //        //user.Sys_User_id = emp_code;

        //        //dbSA.TM_SysUser.Add(user);
        //        //dbSA.SaveChanges();

        //        PCR_OperateUser user = new PCR_OperateUser();
        //        user.pcr_group_id = group_name;
        //        user.emp_code = emp_code;

        //        dbPCR.PCR_OperateUser.Add(user);
        //        dbPCR.SaveChanges();
                

        //        return Json(true);
        //    }
        //    catch (Exception)
        //    {
        //        // Do some error logging stuff, handle exception, etc.
        //        return Json(false);
        //    }
        //}
        //public ActionResult EditUser(string emp_code, int group_name)
        //{
        //    try
        //    {
        //        //var query = from u in dbSA.TM_SysUser
        //        //            where u.Sys_User_id == emp_code
        //        //            select u;

        //        //var user = query.First();
        //        //user.group_id = group_name;
        //        //dbSA.SaveChanges();

        //        var query = from u in dbPCR.PCR_OperateUser
        //                    where u.emp_code == emp_code
        //                    select u;
        //        var user = query.First();
        //        user.pcr_group_id = group_name;

        //        dbPCR.SaveChanges();

        //        return Json(true);
        //    }
        //    catch (Exception)
        //    {
        //        return Json(false);
        //    }
        //}
        //public ActionResult DelUser(string id)
        //{
        //    try
        //    {
        //        //var user = dbSA.TM_SysUser.Find(id);
        //        //dbSA.TM_SysUser.Remove(user);
        //        //dbSA.SaveChanges();

        //        var user = dbPCR.PCR_OperateUser.Find(id);
        //        dbPCR.PCR_OperateUser.Remove(user);

        //        dbPCR.SaveChanges();

        //        return Json(true);
        //    }
        //    catch (Exception)
        //    {
        //        return Json(false);
        //    }
        //}

        //public ActionResult GridGroupCodeMaster(string sidx, string sord, int page, int rows, string search)
        //{
        //    var dbSel = dbPCR.PCR_GCode;
        //    var pageIndex = Convert.ToInt32(page) - 1;
        //    var pageSize = rows;
        //    var totalRecords = dbSel.Count();
        //    var totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);

        //    var plant = dbSel
        //                    .OrderBy(sidx + " " + sord)
        //                    .Skip(pageIndex * pageSize)
        //                    .Take(pageSize).AsEnumerable();
        //    var jsonData = new
        //    {
        //        total = totalPages,
        //        page = page,
        //        records = totalRecords,
        //        rows = plant.Select(p => new
        //        {
        //            i = p.g_id,
        //            cell = new object[] {
        //                p.g_id,
        //                p.g_code
        //            }
        //        }).ToArray()
        //    };
        //    return Json(jsonData);
        //}
        //public ActionResult AddGroupCode(string Sys_Plant_name)
        //{
        //    try
        //    {
        //        //var db = new SAEntities();
        //        //TM_SysPlant plant = new TM_SysPlant();
        //        //plant.Sys_Plant_name = Sys_Plant_name;
        //        //plant.del_flag = false;
        //        //plant.update_dt = DateTime.Now;
        //        //plant.update_by = Session["SA_Auth"].ToString();
        //        //db.TM_SysPlant.Add(plant);
        //        //db.SaveChanges();

        //        var db = new PCREntities();
        //        PCR_GCode gcode = new PCR_GCode();
        //        //gcode.g_id = ;
        //        gcode.g_code = Sys_Plant_name;
        //        db.PCR_GCode.Add(gcode);
        //        db.SaveChanges();

        //        return Json(true);
        //    }
        //    catch (Exception)
        //    {
        //        // Do some error logging stuff, handle exception, etc.
        //        return Json(false);
        //    }
        //}
        //public ActionResult EditGroupCode(int g_id, string g_name)
        //{
        //    try
        //    {
        //        var query = from p in dbPCR.PCR_GCode
        //                    where p.g_id == g_id
        //                    select p;

        //        var code = query.First();

        //        code.g_code = g_name;
        //        dbPCR.SaveChanges();

        //        return Json(true);
        //    }
        //    catch (Exception)
        //    {
        //        return Json(false);
        //    }
        //}
        //public ActionResult DelGroupCode(int id)
        //{
        //    try
        //    {
        //        var query = dbPCR.PCR_GCode.Find(id);
        //        dbPCR.PCR_GCode.Remove(query);
        //        dbPCR.SaveChanges();

        //        return Json(true);
        //    }
        //    catch (Exception)
        //    {
        //        return Json(false);
        //    }
        //}

        public string GroupSelectList()
        {
            //TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();

            string result = "<select>";

            foreach (var item in dbTNC.tnc_group_master.OrderBy(g => g.group_name))
            {
                result = result + "<option value = '" + item.id + "'>" + item.group_name + "</option>";
            }

            result = result + "</select>";
            return result;
        }
        public string GroupTypeSelectList()
        {
            //QISEntities dbQIS = new QISEntities();

            string result = "<select>";

            //foreach (var item in dbSA.TM_SysGroupType.Where(w => w.del_flag != true))
            //{
            //    result = result + "<option value = '" + item.Sys_GroupType_id + "'>" + item.Sys_GroupType_name + "</option>";
            //}
            foreach (var item in dbPCR.PCR_GroupType)
            {
                result = result + "<option value = '" + item.group_type_id + "'>" + item.group_type + "</option>";
            }

            result = result + "</select>";
            return result;
        }
        public string PlantSelectList()
        {
            //QISEntities dbQIS = new QISEntities();

            string result = "<select>";

            //foreach (var item in dbSA.TM_SysPlant.Where(w=>w.del_flag != true))
            //{
            //    result = result + "<option value = '" + item.Sys_Plant_id + "'>" + item.Sys_Plant_name + "</option>";
            //}
            foreach (var item in dbPCR.PCR_Impact)
            {
                result = result + "<option value = '" + item.imp_id + "'>" + item.imp_name + "</option>";
            }

            result = result + "</select>";
            return result;
        }
        public string GroupInSelectList()
        {
            //QISEntities dbQIS = new QISEntities();

            string result = "<select>";

            //foreach (var item in dbSA.TM_SysGroup.Where(w => w.del_flag != true))
            //{
            //    result = result + "<option value = '" + item.group_id + "'>" + item.Sys_Group_name + "</option>";
            //}
            foreach (var item in dbPCR.PCR_SubGroup)
            {
                result = result + "<option value = '" + item.pcr_group_id + "'>" + item.pcr_group_name + "</option>";
            }

            result = result + "</select>";
            return result;
        }
        public string UserSelectList()
        {
            //TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();

            string result = "<select>";

            foreach (var item in dbTNC.tnc_user.OrderBy(u => u.emp_fname))
            {
                result = result + "<option value = '" + item.emp_code + "'>" + item.emp_fname + " " + item.emp_lname + "</option>";
            }

            result = result + "</select>";
            return result;
        }

        //public ActionResult ImportExcel()
        //{
        //    if (Request.Files["FileUpload"].ContentLength > 0)
        //    {
        //        string extension = System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName);
        //        string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/UploadedFolder"), Request.Files["FileUpload"].FileName);
        //        if (System.IO.File.Exists(path1))
        //            System.IO.File.Delete(path1);

        //        Request.Files["FileUpload1"].SaveAs(path1);
        //        string sqlConnectionString = @"Data Source=LEEDHAR2-PC\SQLEXPRESS;Database=Leedhar_Import;Trusted_Connection=true;Persist Security Info=True";


        //        //Create connection string to Excel work book
        //        string excelConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=Excel 12.0;Persist Security Info=False";
        //        //Create Connection to Excel work book
        //        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
        //        //Create OleDbCommand to fetch data from Excel
        //        OleDbCommand cmd = new OleDbCommand("Select [id],[Name],[Marks],[Grade] from [Sheet1$]", excelConnection);

        //        excelConnection.Open();
        //        OleDbDataReader dReader;
        //        dReader = cmd.ExecuteReader();

        //        SqlBulkCopy sqlBulk = new SqlBulkCopy(sqlConnectionString);
        //        //Give your Destination table name
        //        sqlBulk.DestinationTableName = "StudentRecord";
        //        sqlBulk.WriteToServer(dReader);
        //        excelConnection.Close();

        //        // SQL Server Connection String
        
        //    }

        //    return RedirectToAction("Import");
        //}

        //[HttpPost]
        //public ActionResult ImportExcel(HttpPostedFileBase xlsfile)
        //{
        //    var fileName = Path.GetFileName(xlsfile.FileName);
        //    // store the file inside ~/App_Data/uploads folder
        //    var path = Path.Combine(Server.MapPath("~/App_Data/"), fileName);
        //    xlsfile.SaveAs(path);
        //    var connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Path.Combine(Server.MapPath("~/App_Data/"), fileName) + ";Extended Properties=Excel 12.0;");

        //    //Fill the dataset with information from the BV and REV worksheet.
        //    var adapterbvandrev = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
        //    var dsmaster = new DataSet();
        //    adapterbvandrev.Fill(dsmaster);
        //    DataTable dtmaster = dsmaster.Tables[0];
        //    List<TM_SysGroupCode> objmaster = new List<TM_SysGroupCode>();
        //    for (int i = 0; i < dtmaster.Rows.Count; i++)
        //    {
        //        objmaster.Add(new TM_SysGroupCode { g_id = int.Parse(dtmaster.Rows[i]["g_id"].ToString()), g_code = dtmaster.Rows[i]["g_code"].ToString() });
        //    }
        //    SaveData(objmaster);
        //    return View();
        //}
        //public bool SaveData(List<TM_SysGroupCode> objgcode)
        //{
        //    try
        //    {
        //        foreach (var item in objgcode)
        //        {
        //            TM_SysGroupCode obj = new TM_SysGroupCode();
        //            obj.g_id = item.g_id;
        //            obj.g_code = item.g_code;
        //            dbSA.TM_SysGroupCode.Add(obj);
        //            dbSA.SaveChanges();
        //        }
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //[HttpPost]
        //public ActionResult AdmissionUpload()
        //{
        //    string filePath = null;
        //    foreach (string inputTagName in Request.Files)
        //    {
        //        HttpPostedFileBase Infile = Request.Files[inputTagName];
        //        if (Infile.ContentLength > 0 && (Path.GetExtension(Infile.FileName) == ".xls" || Path.GetExtension(Infile.FileName) == ".xlsx" || Path.GetExtension(Infile.FileName) == ".xlsm"))
        //        {
        //            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
        //                          Path.GetFileName(Infile.FileName));
        //            if (System.IO.File.Exists(filePath))
        //            {
        //                System.IO.File.Delete(filePath);
        //            }
        //            Infile.SaveAs(filePath);
        //            //Infile.SaveAs(filePath); 
        //        }

        //        if (filePath != null)
        //        {
        //            System.Data.OleDb.OleDbConnection oconn = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath.ToString() + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES\";");
        //            oconn.Open();

        //            try
        //            {
        //                if (oconn.State == System.Data.ConnectionState.Closed)
        //                    oconn.Open();
        //            }
        //            catch (Exception)
        //            {
        //                // MessageBox.Show(ex.Message);
        //            }

        //            dynamic myTableName = oconn.GetSchema("Tables").Rows[0]["TABLE_NAME"];
        //            OleDbCommand ocmd = new OleDbCommand("select * from [" + myTableName + "]", oconn);
        //            OleDbDataReader odr = ocmd.ExecuteReader();
        //            if (odr.HasRows)
        //            {
        //                while (odr.Read())
        //                {
        //                    var model = new TM_SysGroupCode();
        //                    model.g_id = Convert.ToInt32(odr[0]);
        //                    model.g_code = odr[1].ToString().Trim();
        //                    dbSA.TM_SysGroupCode.Add(model);
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}
    }
}
