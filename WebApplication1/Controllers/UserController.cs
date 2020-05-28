using BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            UserBissnessLayer userBissnessLayer = new UserBissnessLayer();
            List<User> users = userBissnessLayer.Users.ToList();
            return View(users);
        }

        public ActionResult UserManagementRemove(String Id, String Project)
        {
            RemoveFromProjectData model = new RemoveFromProjectData();
            model.UserId = Id;
            model.ProjectId = Project;
            return View(model);
        }


        [HttpGet]
        public ActionResult EditPerm(String User, String ProjectId)
        {
            ViewBag.ProjectId = ProjectId;
            StringObject str = new StringObject();
            str.Object = User;
            return View(str);
        }

        [HttpPost]
        public ActionResult EditPerm(String UserId, String ProjectId, FormCollection form)
        {
            UserBissnessLayer userBissnessLayer = new UserBissnessLayer();

            String user = userBissnessLayer.GetUserIdFromName(UserId);
            if (form["Permission"] == "" || user =="")
            {
                return RedirectToAction("PermError", "Project", new { id = ProjectId });
            }
            userBissnessLayer.RemoveProjectFromUser(user, ProjectId);
            userBissnessLayer.AddPermissionToProject(user, ProjectId, form["Permission"]);
            ViewBag.ProjectId = ProjectId;
            StringObject str = new StringObject();
            str.Object = UserId;
            return RedirectToAction("UsersInProject", "Project", new {id = ProjectId });
        }

        public ActionResult DeleteFromProject(String User, String ProjectId)
        {
            ViewBag.User = User;
            ViewBag.ProjectId = ProjectId;
            return View();
        }
        [HttpGet]
        public ActionResult UserManagementAdd(String Id, String AdmId)
        {
            UserBissnessLayer userBissnessLayer = new UserBissnessLayer();
            List<SelectListItem> project = userBissnessLayer.getAllProjectForUserWithPerm(Id, AdmId, "1");
            ListOfProjects list = new ListOfProjects();
            list.list = project;
            list.count = project.Count;
            list.userId = Id;
            list.id = AdmId;
            ViewBag.Id = Id;
            return View(list);
        }
        [HttpPost]
        public ActionResult UserManagementAdd(String Id, FormCollection form)
        {
            if(form["Project"] == "" || form["Permission"] == "")
                return RedirectToAction("Error", "User");
            UserBissnessLayer userBissnessLayer = new UserBissnessLayer();
            userBissnessLayer.AddPermissionToProject(Id, form["Project"], form["Permission"]);
            return RedirectToAction("Index", "User");
        }
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult UserManagementDelete(String id)
        {
            return View();
        }

        public ActionResult SubmitRemove(String User, String Project)
        {
            UserBissnessLayer userBissnessLayer = new UserBissnessLayer();
            userBissnessLayer.RemoveProjectFromUser(User, Project);
            return RedirectToAction("Index", "User");
        }
    }
}