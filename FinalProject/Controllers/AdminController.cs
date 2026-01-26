using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    public class AdminController : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-GKJO2DT\\SQLEXPRESS;Initial Catalog=finalproject;Integrated Security=True;"
);
        //"Server=sql.bsite.net\\MSSQL2016;Database=humanetics_;User Id=humanetics_; password=ankit2026"
        // GET: Admin
        //"Data Source=DESKTOP-GKJO2DT\\SQLEXPRESS;Initial Catalog=finalproject;Integrated Security=True;"
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Addopening()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Addopening(string title, string detail, string type, string city, int? minsalary, int? maxsalary, string gender, string shift, string exp, string education, int? vacancy, DateTime lastdate)
        {
            string command = $"INSERT INTO TBL_OPENING VALUES ('{title}','{detail}','{type}','{city}',{minsalary},{maxsalary},'{gender}','{shift}','{exp}','{education}',{vacancy},'{lastdate.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("yyyy-MM-dd")}',1)";
            SqlCommand cmd = new SqlCommand(command,con);
            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return Content("<script>alert('Opening added');location.href='/Admin/Addopening'</script>");
        }

        public ActionResult Applicationdetails()
        {
            SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM TBL_APPLICATION ORDER BY ID ASC ", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            ViewBag.application = dt;
            return View();
        }

        public ActionResult Leaveapplication()
        {
            String command = $"SELECT * FROM TBL_LEAVE ORDER BY ID DESC ";
            SqlDataAdapter adapter = new SqlDataAdapter(command, con);
            DataTable data = new DataTable();
            adapter.Fill(data);
            ViewBag.leave = data;
            return View();
        }

        public ActionResult Emplist()
        {
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM TBL_APPLICATION WHERE ISHIRED=1",con);
            DataTable data = new DataTable();
            adapter.Fill(data);
            ViewBag.employee = data;
            return View();
        }

        public ActionResult Salaryslip(String email, DateTime? fromdate, DateTime? todate)
        {
            //Select all hired employees 
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM TBL_APPLICATION WHERE ISHIRED=1", con);
            DataTable data = new DataTable();
            adapter.Fill(data);
            ViewBag.employee = data;


            if(email != null && fromdate.HasValue && todate.HasValue)
            {
                String command = $"SELECT * FROM TBL_ATTENDANCE WHERE EMPID='{email}' and ADATE BETWEEN '{fromdate.Value.ToString("yyyy-MM-dd")}' and '{todate.Value.ToString("yyyy-MM-dd")}'";

                SqlDataAdapter sda = new SqlDataAdapter(command, con);
                DataTable attend = new DataTable();
                sda.Fill(attend);
                ViewBag.attend = attend;
            }


            return View();
        }

        public ActionResult OpeningList()
        {
                SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM TBL_OPENING ", con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                ViewBag.opening = dt;
                return View();
        }

        public ActionResult Enquiry()
        {
            SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM TBL_ENQUIRY ", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            ViewBag.opening = dt;
            return View();
        }

        public ActionResult Hired(String email, int? appid)
        {
            if(email == null)
            {
                return Content("<script>alert('Please select a profile.');location.href='/Admin/Applicationdetails'</script>");

            }
            else
            {
                string command = $"UPDATE TBL_APPLICATION set ISHIRED=1 WHERE id={appid}";
                SqlCommand cmd = new SqlCommand(command, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                //send mail to the user to congratulate that u r hired
                MailMessage mail = new MailMessage("ankitbind237@gmail.com",email);mail.Subject = "Congratulations!! from Rock-hiring message";
                mail.Body = $"<b>Congratulations!!</b> you are hired in our company. Here is your login ID and Password to access employee fracility . <br/> <br/> Your login ID is :{email} <br/> Your Passwor is : rockers<br/> <br/>  Feel free to contact us on: 8858832963  any time if you have any query.";
                mail.IsBodyHtml= true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("ankitbind237@gmail.com", "kinc mukt vovo aple");
                smtp.EnableSsl = true;

                smtp.Send(mail);
                //save the login id and password of employee into TBL_LOGIN

                String sqlcmd = $"INSERT INTO TBL_EMPLOGIN VALUES ('{email}','rock',{appid})";
                SqlCommand cmd1 = new SqlCommand(sqlcmd,con);
                con.Open();
                cmd1.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("Applicationdetails");
            }

        }


        public ActionResult Rejected(int? appid)
        {
            string command = $"UPDATE TBL_APPLICATION set ISREJECTED=1 WHERE id={appid}";
            SqlCommand cmd = new SqlCommand(command, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("applicationdetails");
        }

        public ActionResult acceptleave(int? id)
        {
            string commad = $"UPDATE TBL_LEAVE SET ISACCEPTED=1 WHERE id={id}";
            SqlCommand cmd = new SqlCommand(commad,con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("Leaveapplication");

        }


        public ActionResult rejectleave(int? id)
        {
            string commad = $"UPDATE TBL_LEAVE SET ISACCEPTED=0 WHERE id={id}";
            SqlCommand cmd = new SqlCommand(commad, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("Leaveapplication");

        }


        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("login", "home");
        }


    }
}