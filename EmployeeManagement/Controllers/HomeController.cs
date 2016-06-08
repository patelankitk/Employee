using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using EmployeeManagement.Models;
using System.IO;
using System.Runtime.InteropServices.ComTypes;



namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private EMPSYSEntities db = new EMPSYSEntities();

        public ActionResult Index()
        {
            if (Session["LogedUserID"] != null)
            {
                var Dept = from d in db.tblDept1
                           select d;
                return View(Dept);
            }
            
            else
            {
                return RedirectToAction("Login");
            }

            
        }
        public ActionResult ADD()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ADD([Bind(Include = "DeptID,DeptName")] tblDept1 dept)
        {
            db.tblDept1.Add(dept);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteDept(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDept1 dept = db.tblDept1.Find(id);

            if (dept == null)
            {
                return HttpNotFound();
            }
            return View(dept);
        }

        [HttpPost, ActionName("DeleteDept")]

        public ActionResult DeleteConfirmDept(int? id)
        {
            tblDept1 dept = db.tblDept1.Find(id);
            db.tblDept1.Remove(dept);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(string name, string searchString)
        {
            if (name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dept1 = from d in db.tblEmp1 where d.EmpDeptName.ToLower() ==name.ToLower() select d;
                       
            if (dept1 == null)
            {
                return HttpNotFound();
            }
            return View(dept1);
        }
        public ActionResult AddEmpDetails()
        {
            var DeptList = new List<string>();
            var DeptQuery = from d in db.tblDept1
                             orderby d.DeptName
                             select d.DeptName;
            DeptList.AddRange(DeptQuery.Distinct());
            ViewBag.DepartmentNames = new SelectList(DeptList);

            List<string> GenderList = new List<string>();
            GenderList.Add("Male");
            GenderList.Add("Female");
            ViewBag.Gender = new SelectList(GenderList);
            return View();
        }

        [HttpPost]
        public ActionResult AddEmpDetails([Bind(Include = "EmpID,EmpName,EmpDOB,EmpEmail,EmpContactNo,EmpSalary")] tblEmp1 emp, HttpPostedFileBase file, FormCollection form)
        {
            if (file != null)
            {
                string pic = Path.GetFileName(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/"), pic);
                // file is uploaded
                file.SaveAs(path);
                emp.EmpPhoto = "~/images/" + pic;
            }
            emp.EmpGender = form["Gender"];
            emp.EmpDeptName= form["DepartmentNames"];
            db.tblEmp1.Add(emp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult EmpDetails(int? Empid)
        {
            
            if (Empid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dept1 = from d in db.tblEmp1 where d.EmpID==Empid select d;

            if (dept1 == null)
            {
                return HttpNotFound();
            }
            return View(dept1);
        }


        public ActionResult DeleteEmp(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblEmp1 emp = db.tblEmp1.Find(id);

            if (emp == null)
            {
                return HttpNotFound();
            }
            return View(emp);
        }

        [HttpPost, ActionName("DeleteEmp")]

        public ActionResult DeleteConfirmed(int? id)
        {
            tblEmp1 emp = db.tblEmp1.Find(id);
            db.tblEmp1.Remove(emp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult EditEmpDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblEmp1 emp = db.tblEmp1.Find(id);

            if (emp == null)
            {
                return HttpNotFound();
            }

            var DeptList = new List<string>();
            var DeptQuery = from d in db.tblDept1
                            orderby d.DeptName
                            select d.DeptName;
            DeptList.AddRange(DeptQuery.Distinct());
            ViewBag.DepartmentNames = new SelectList(DeptList);

            //List<string> GenderList = new List<string>();
            //GenderList.Add("Male");
            //GenderList.Add("Female");
            //ViewBag.Gender = new SelectList(GenderList);
            return View(emp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmpDetails([Bind(Include = "EmpID,EmpName,EmpDOB,EmpGender,EmpEmail,EmpContactNo,EmpDeptName,EmpSalary,EmpPhoto")] tblEmp1 emp, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string pic = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/images/"), pic);
                    // file is uploaded
                    file.SaveAs(path);
                    emp.EmpPhoto = "~/images/" + pic;
                }
                
                db.Entry(emp).State = EntityState.Modified;
                db.SaveChanges();
                // return RedirectToAction("Index");
                return RedirectToAction("Details", new { name = emp.EmpDeptName });
            }
            return View(emp);

        }

        public ActionResult EditDept(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDept1 dept = db.tblDept1.Find(id);

            if (dept == null)
            {
                return HttpNotFound();
            }
            return View(dept);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDept([Bind(Include = "DeptID,DeptName")] tblDept1 dept)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dept).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
                //return RedirectToAction("EmpDetails", new { id = emp.DeptID});
            }
            return View(dept);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(tblUser1 u)
        {
            // this action is to handle post (login)
            if (ModelState.IsValid) // this checks validity 
            {
                
                  var v = db.tblUser1.Where(a => a.UserName.Equals(u.UserName) && a.Password.Equals(u.Password)).FirstOrDefault();
                    if (v != null)
                    {
                        Session["LogedUserID"] = v.UserID.ToString();
                        Session["LogedUserName"] = v.UserName.ToString();
                        return RedirectToAction("Index");
                    }
                
            }
            return View(u);
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login","Home");
        }

    }
}