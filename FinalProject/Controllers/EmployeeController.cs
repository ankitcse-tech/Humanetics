using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    public class EmployeeController : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-GKJO2DT\\SQLEXPRESS;Initial Catalog=finalproject;Integrated Security=True;");

        // GET: Employee
        public ActionResult Dashboard()
        {

            return View();
        }

        public ActionResult MyProfile()
        {
            string email = "";
            if (Session["emp"] != null)
            {
                email = Session["emp"].ToString();
            }
            else
            {
                return Content("<script>alert('Login First');location.href='/home/emplogin'</script>");

            }

            String command = $"SELECT * FROM TBL_APPLICATION WHERE EmailID='{email}' and ISHIRED=1";
            SqlDataAdapter adapter = new SqlDataAdapter(command,con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            ViewBag.user = dt;
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string opass, string npass, string cpass)
        {
            if(npass.Equals(cpass))
            {
                if(npass.Equals(opass))
                {
                    return Content("<script>alert('New Password and Old Password should be different');location.href='/employee/ChangePassword'</script>");

                }
                else
                {
                    //Change the password
                    String email = Session["emp"].ToString();
                    string command = $"UPDATE TBL_EMPLOGIN SET PASSWORD='{npass}' WHERE EMAIL='{email}' AND PASSWORD ='{opass}'";
                    SqlCommand cmd = new SqlCommand(command, con);
                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    con.Close();
                    if(result>0)
                    {
                        Session.RemoveAll();
                        return Content("<script>alert('Password Updated ');location.href='/Home/emplogin'</script>");

                    }
                    else
                    {
                        return Content("<script>alert('Password could not changed. Old password is incorrect');location.href='/employee/ChangePassword'</script>");

                    }
                }

            }
            else
            {
                return Content("<script>alert('Password updated');location.href='/employee/ChangePassword'</script>");

            }
        }
        public ActionResult LeaveApplication()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LeaveApplication(String subject, String detail, DateTime fromdate, DateTime todate, HttpPostedFileBase file)
        {
            String filename = "";
            if(file!=null)
            {
                filename = file.FileName;
                file.SaveAs(Server.MapPath("/Content/leavefile/"+ file.FileName));
            }
            int totaldays = 0;
            TimeSpan time= todate - fromdate;
            totaldays = time.Days;

            string userid = Session["emp"].ToString();

            string command = $"INSERT INTO TBL_LEAVE VALUES ('{userid}','{subject}','{detail}','{fromdate}','{todate}','{DateTime.Now.ToString("yyyy-MM-dd")}',{totaldays},null,'{filename}')";

            SqlCommand cmd = new SqlCommand(command, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return Content("<script>alert('Application send successfully. Please wait for admin approval.');location.href='/employee/LeaveApplicationStatus'</script>");
        }

        public ActionResult Attendance()
        {
            string email = "";
            if (Session["emp"] != null)
            {
                email = Session["emp"].ToString();
            }
            else
            {
                return Content("<script>alert('Login First');location.href='/home/emplogin'</script>");

            }

            String command =$"SELECT * FROM TBL_ATTENDANCE WHERE empid='{email}' and adate='{DateTime.Now.ToString("yyyy-MM-dd")}'";

            SqlDataAdapter adapter = new SqlDataAdapter(command,con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            ViewBag.data = dt;

            return View();
        }

        [HttpPost]
        public ActionResult Attendance(int? id)
        {
            String command = $"INSERT INTO TBL_ATTENDANCE VALUES ('{Session["emp"]}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToShortTimeString()}','{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}')";
            SqlCommand cmd = new SqlCommand(command,con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return Content("<script>alert('Your attendance is marked for today');location.href='/employee/Attendance'</script>");
        }

        public ActionResult LeaveApplicationStatus()
        {
            string email = "";
            if(Session["emp"] != null)
            {
                email = Session["emp"].ToString();
            }
            else 
            {
                return Content("<script>alert('Login First');location.href='/home/emplogin'</script>");

            }
            //String email = Session["emp"].ToString();
            String command = $"SELECT * FROM TBL_LEAVE WHERE empid ='{email}' ORDER BY ID DESC ";
            SqlDataAdapter adapter = new SqlDataAdapter(command, con);
            DataTable data = new DataTable();
            adapter.Fill(data);
            ViewBag.leave = data;
            return View();
        }

        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("emplogin", "home");
        }
    }
}