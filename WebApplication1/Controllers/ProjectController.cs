using BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        [HttpGet]
        public ActionResult AddNewProject(String Id)
        {
            ViewBag.Id = Id;
            return View();
        }

        [HttpPost]
        public ActionResult AddNewProject(String id, FormCollection formCollection)
        {
            UserBissnessLayer bissnessLayer = new UserBissnessLayer();
            Project project = MapProject(formCollection);
            bissnessLayer.AddProject(project);
            bissnessLayer.AddPermissionToProject(id, project.Id, "1");
            return RedirectToAction("Project", "Home");
        }
        public ActionResult RemoveProject(String id)
        {
            StringObject str = new StringObject();
            str.Object = id;
            return View(str);
        }

        public ActionResult UsersInProject(String id, String projectId)
        {
            UserBissnessLayer bissnessLayer = new UserBissnessLayer();
            List<UserWithPerm> user = bissnessLayer.UserInProject(id).ToList();
            ViewBag.Id = id;
            ViewBag.ProjectId = projectId;
            return View(user);
        }

        public ActionResult SubmitRemoveProject(String id)
        {
            UserBissnessLayer bissnessLayer = new UserBissnessLayer();
            bissnessLayer.DeleteProject(id);
            bissnessLayer.DeletePermissions(id);
            bissnessLayer.DeleteTaskFromProject(id);
            return RedirectToAction("Project", "Home");

        }

        public ActionResult PermError(String id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult SubmitRemoveUserFromProject(String User, String Project)
        {
            UserBissnessLayer bissnessLayer = new UserBissnessLayer();
            string userId = bissnessLayer.GetUserIdFromName(User);
            bissnessLayer.RemoveProjectFromUser(userId, Project);
            return RedirectToAction("UsersInProject", new { id = Project });
        }

        public ActionResult Task(String id)
        {
            UserBissnessLayer bissnessLayer = new UserBissnessLayer();
            List<Task> TaskList = bissnessLayer.GetTaskForProject(id);

            ViewBag.ProjectId = id;
            return View(TaskList);
        }

        public ActionResult Csv(String id)
        {
            UserBissnessLayer layer = new UserBissnessLayer();
            Project project = layer.GetAllProject().Find(x => x.Id == id);
            List<Task> tasks = layer.GetTaskForProject(id);
            List<UserWithPerm> users = layer.UserInProject(id).ToList();
            Response.AppendHeader("Content-Disposition", "attachment;filename=project.csv");
            byte[] file = Encoding.ASCII.GetBytes(layer.createCSV(project, tasks, users));
            return File(file, "text/csv", "project.csv");
        }

        public ActionResult Xml(String id)
        {
            UserBissnessLayer layer = new UserBissnessLayer();
            Project project = layer.GetAllProject().Find(x => x.Id == id);
            List<Task> tasks = layer.GetTaskForProject(id);
            List<UserWithPerm> users = layer.UserInProject(id).ToList();
            Response.AppendHeader("Content-Disposition", "attachment;filename=project.xml");
            byte[] file = Encoding.ASCII.GetBytes(layer.createXML(project, tasks, users));
            return File(file, "text/xml", "project.xml");
        }

        private Project MapProject(FormCollection formCollection)
        {
            Project project = new Project();
            project.Id = Guid.NewGuid().ToString();
            project.Language = formCollection["Language"];
            project.Name = formCollection["Name"];
            return project;
        }

    }
}