using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-GKJO2DT\\SQLEXPRESS;Initial Catalog=finalproject;Integrated Security=True;");
        public ActionResult Index()
        { 
            return View();
        }
        public ActionResult About() 
        {
            return View();
        }
        public ActionResult Contact() 
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(String name, String email, long? mob, String msg )
        {    
                string command = $"INSERT INTO TBL_ENQUIRY VALUES ('{name}',{mob},'{email}','{msg}','{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}')";
                SqlCommand cmd = new SqlCommand(command, con);
                con.Open();
                int result = cmd.ExecuteNonQuery();
                con.Close();
                return Content("<script>alert('Thank you for your enquiry. We will contact you back soon');location.href='/Home/Contact'</script>");   
        }

        public ActionResult Team() 
        {
            return View();
        }

        public ActionResult Opening()
        {
            SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM TBL_OPENING ORDER BY ID DESC", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            ViewBag.opening = dt;
            return View();
        }

        public ActionResult Apply(int? jobid)
        {
            if (jobid.HasValue)
            {
                return View();
            }

            else
            {
                return Content("<script>alert('Please Select a Job.');location.href='/Home/Opening'</script>");
            }
        }

        [HttpPost]

        public ActionResult Apply(int? jobid,String name, String email, long? mob, string address, String exp, int salary, String qualification, String gender, HttpPostedFileBase resume, HttpPostedFileBase profile)
        {
            string command = $"INSERT INTO TBL_APPLICATION VALUES ('{jobid}','{name}',{mob},'{email}','{address}','{qualification}','{exp}','{salary}','{gender}','{resume.FileName}','{profile.FileName}','{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}',1,0,0)";
            SqlCommand cmd = new SqlCommand (command, con);
            con.Open();
            int result = cmd.ExecuteNonQuery();
            con.Close();
            //move the uploaded file in the server folder
            resume.SaveAs(Server.MapPath("/Content/resume") + resume.FileName);
            profile.SaveAs(Server.MapPath("/Content/resume") + profile.FileName);
            return Content("<script>alert('Successfully Applied. Please wait for Admin response.');location.href='/Home/Opening'</script>");


        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Login(string userid, string password)
        {
            //return RedirectToAction("Dashboard", "admin");
            Session["admin"] = userid;
            if(userid.Equals("ankit") && password.Equals("ankit123"))
            {
                return Content("<script>alert('Welcome Admin');location.href='/Admin/Dashboard'</script>");
            }
            else
            {
                return Content("<script>alert('User ID or Password is Incorrect');location.href='/Home/login'</script>");

            }
        }


        public ActionResult Emplogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Emplogin(String userid, String password)
        {
            SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM TBL_EMPLOGIN WHERE EMAIL='{userid}' and password='{password}'", con);
            DataTable data = new DataTable();
            adapter.Fill(data);

            if (data.Rows.Count > 0)
            {
                Session["emp"] =userid;
                return RedirectToAction("dashboard","employee");
            }
            else
            {
                return Content("<script>alert('Invalid Id or Password');location.href='/Home/Emplogin'</script>");

            }
        }

        public ActionResult Application(string email)
        {
            if(email != null)
            {
                string command = $"SELECT * FROM TBL_APPLICATION WHERE EMAILID='{email}'";
                SqlDataAdapter sda = new SqlDataAdapter(command, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                ViewBag.app = dt;
            }
            return View();
        }

        public ActionResult Services()
        {
            return View();
        }

    }
}