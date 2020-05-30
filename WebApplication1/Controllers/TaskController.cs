using BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class TaskController : Controller
    {
        // GET: Task
        [HttpGet]
        public ActionResult Create(String ProjectId)
        {
            ViewBag.ProjectId = ProjectId;
            return View();
        }

        [HttpPost]
        public ActionResult Create(String ProjectId, FormCollection Form)
        {
            System.Diagnostics.Debug.WriteLine(Convert.ToString(Form.AllKeys.ToList().ElementAt(2)+ Form.AllKeys.ToList().ElementAt(3)));
            if (Form["Name"] == "")
            {
                return RedirectToAction("Error", "Task", new { ProjectId = ProjectId});
            }
            UserBissnessLayer userBissnessLayer = new UserBissnessLayer();
            Task task = mapTask(ProjectId, Form);
            userBissnessLayer.createTask(task);
            return RedirectToAction("Task", "Project", new { id = ProjectId });
        }

        private Task mapTask(string projectId, FormCollection form)
        {
            Task task = new Task();
            task.Id = Guid.NewGuid().ToString();
            task.Hour = form["Hour"] == "" ? 0 : Convert.ToInt32(form["Hour"].ToString());
            task.Name = form["Name"];
            task.Description = form["Description"];
            task.Assigne = form["Assigne"];
            task.ProjectId = projectId;
            return task;
        }

        public ActionResult Error(String ProjectId)
        {
            ViewBag.ProjectId = ProjectId;
            return View();
        }
        [HttpGet]
        public ActionResult Edit(String Id)
        {
            ViewBag.ProjectId = Id;
            UserBissnessLayer userBissnessLayer = new UserBissnessLayer();
            Task task = userBissnessLayer.getTaskFromId(Id);
            return View(task);
        }
        [HttpGet]
        public ActionResult Delete(String Id)
        {
            ViewBag.ProjectId = Id;
            UserBissnessLayer userBissnessLayer = new UserBissnessLayer();
            Task task = userBissnessLayer.getTaskFromId(Id);
            return View(task);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            UserBissnessLayer userBissnessLayer = new UserBissnessLayer();
            Task task = new Task();
            userBissnessLayer.DeleteTask(form["Id"]);
            task.Id = form["Id"];
            task.Hour = form["Hour"] == "" ? 0 : Convert.ToInt32(form["Hour"].ToString());
            task.Name = form["Name"];
            task.Description = form["Description"];
            task.Assigne = form["Assigne"];
            task.ProjectId = form["ProjectId"];
            userBissnessLayer.createTask(task);
            return RedirectToAction("Task", "Project", new { id = task.ProjectId });
        }
    }
}