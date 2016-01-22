using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEmailReminder
{
    class Program
    {
        static void Main(string[] args)
        {
            Program pgm = new Program();

            try
            {
                MailCenterEntities dbMail = new MailCenterEntities();
                SpecialAcceptanceEntities dbSA = new SpecialAcceptanceEntities();
                TNC_ADMINEntities dbTNC = new TNC_ADMINEntities();

                var query = from a in dbSA.TD_Transaction
                            where a.active == true && a.comment != "Tell"
                            select a;

                foreach (var g in query)//each transaction
                {
                    string email = "";

                    if (g.Sys_Plant_id == 0)
                    {
                        var positionlv = (from p in dbSA.TM_Level
                                          where p.lv_id == g.lv_id
                                          select p).FirstOrDefault();

                        if (positionlv != null)
                        {
                            var get_email = from a in dbTNC.V_Employee_Info
                                            where (a.email != null && a.email != "") && a.emp_status == "A" &&
                                            (a.position_level >= positionlv.position_min && a.position_level <= positionlv.position_max)
                                            && (a.email != null && a.email != "") && a.LeafOrgGroupId == g.org_id
                                            && (a.emp_position != 19 && a.emp_position != 20 && a.emp_position != 21 
                                            && a.emp_position != 11 && a.emp_position != 12 && a.emp_position != 27)// Discard Assist, Deputy, Project
                                            select a.email;
                            if (get_email.Any())
                            {
                                foreach (var item in get_email)
                                {
                                    if (!string.IsNullOrEmpty(item))
                                        email += "," + item;
                                }
                                email = email.Substring(1);
                            }
                        }
                    }
                    else
                    {
                        var get_user = from u in dbSA.TM_SysUser
                                       where u.Sys_GroupType_id == 2
                                       && u.Sys_Plant_id == g.Sys_Plant_id && u.lv_id == g.lv_id
                                       select u.Sys_User_id;

                        foreach (var item in get_user)
                        {
                            var get_email = (from e in dbTNC.tnc_user
                                             where e.emp_code == item
                                             select e.email).FirstOrDefault();
                            if (!string.IsNullOrEmpty(get_email))
                                email += "," + get_email;
                        }
                        email = email.Substring(1);
                    }

                    if (!string.IsNullOrEmpty(email))
                    {
                        var get_sa = (from sa in dbSA.TD_Main
                                      where sa.id == g.id
                                      select sa).FirstOrDefault();

                        if (get_sa != null)
                        {
                            string int_link = "http://webExternal/SAOnline";
                            string ext_link = "http://webExternal.nok.co.th/SAOnline";

                            TT_MAIL_WIP ttMail = new TT_MAIL_WIP();
                            ttMail.ProgramID = 8;
                            ttMail.CreateDate = DateTime.Now;
                            ttMail.Sender = "TNCAutoMail-SA@nok.co.th";
                            ttMail.Receiver = email;
                            //ttMail.BCC = "patcharee_ji@nok.co.th,monchit@nok.co.th";
                            ttMail.Title = "[Remind] You have SA Online waiting for Operate.";
                            ttMail.HTMLBody =
                                "Dear. All Concern,<br /><br />" +
                                "<b>Control No. : </b>" + get_sa.control_no + "<br />" +
                                "<b>Type of Nonconformities. : </b>" + get_sa.title + "<br />" +
                                "<b>Item code : </b>" + get_sa.item_code + "<br />" +
                                "<b>Customer : </b>" + get_sa.customer + "<br />" +
                                "<b>Link : </b><a href='" + int_link + "/SAProcess/Detail/" + get_sa.id + "'>Internal</a>, <a href='" + ext_link + "/SAProcess/Detail/" + get_sa.id + "'>External</a><br />" +
                                "<br /><b>*This message is automatic sending from SA Online, please do not reply*</b><br />" +
                                "<br />Best Regard,<br />From SA Online";
                            //ttMail.Flag = 1;

                            dbMail.TT_MAIL_WIP.Add(ttMail);
                        }
                        Console.WriteLine(g.id + " -> OK org:" + g.org_id);
                    }
                    else
                    {
                        TT_MAIL_WIP ttMail = new TT_MAIL_WIP();
                        ttMail.ProgramID = 8;
                        ttMail.CreateDate = DateTime.Now;
                        ttMail.Sender = "TNCAutoMail-SA@nok.co.th";
                        ttMail.Receiver = "monchit@nok.co.th";
                        ttMail.Title = "Error Remind Send Email SA Online.";
                        ttMail.HTMLBody = "Error<br /> id = " + g.id + "<br />Org = " + g.org_id;
                        //ttMail.Flag = 1;

                        dbMail.TT_MAIL_WIP.Add(ttMail);
                        Console.WriteLine(g.id + " -> NG org:" + g.org_id);
                    }
                }
                dbMail.SaveChanges();
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
                Console.ReadLine();
            }
        }
    }
}
