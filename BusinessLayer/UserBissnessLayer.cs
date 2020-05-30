using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace BusinessLayer
{
    public class UserBissnessLayer
    {
        public IEnumerable<User> Users
        {
            get
            {
                string connectionString =
                    ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                List<User> users = new List<User>();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetAllUsers", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        User user = new User();
                        user.ID = rdr["Id"].ToString();
                        user.Projects = GetProjectForUser(user.ID);
                        user.UserName = rdr["UserName"].ToString();

                        users.Add(user);
                    }
                }

                return users;
            }
        }

        public void createTask(Task task)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spAddTask", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramId = new SqlParameter();
                paramId.ParameterName = "@Id";
                paramId.Value = task.Id;
                cmd.Parameters.Add(paramId);

                SqlParameter ParamProjectId = new SqlParameter();
                ParamProjectId.ParameterName = "@ProjectId";
                ParamProjectId.Value = task.ProjectId;
                cmd.Parameters.Add(ParamProjectId);

                SqlParameter paramAssigne = new SqlParameter();
                paramAssigne.ParameterName = "@Assigne";
                paramAssigne.Value = task.Assigne;
                cmd.Parameters.Add(paramAssigne);

                SqlParameter paramHour = new SqlParameter();
                paramHour.ParameterName = "@Hour";
                paramHour.Value = task.Hour;
                cmd.Parameters.Add(paramHour);

                SqlParameter paramDescription = new SqlParameter();
                paramDescription.ParameterName = "@Description";
                paramDescription.Value = task.Description;
                cmd.Parameters.Add(paramDescription);

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@Name";
                paramName.Value = task.Name;
                cmd.Parameters.Add(paramName);


                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public String createXML(Project project, List<Task> tasks, List<UserWithPerm> users)
        {

            XElement xml = new XElement("Projekt",
                new XElement("Nazwa", project.Name),
                new XElement("Jezyk", project.Language),
                new XElement("Uzytkownicy",
                from u in users
                select new XElement("Uzytkownik",
                       new XElement("Nazwa", u.UserName),
                       new XElement("Rola", u.Perm == "1" ? "Zarzadca" : "Pracownik")

                       )
                ),
                new XElement("Zadania",
                from t in tasks
                select new XElement("Zadanie",
                       new XElement("Nazwa", t.Name),
                       new XElement("Opis", t.Description),
                       new XElement("Wykonawca", t.Assigne),
                       new XElement("Czas", t.Hour)
                       )
                )
            );
            return xml.ToString();
        }

        public String createCSV(Project project, List<Task> tasks, List<UserWithPerm> users)
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("\"Nazwa Projektu\",\"Jezyk/Technologia\"");
            sw.WriteLine(string.Format("{0},{1}", project.Name, project.Language));
            sw.WriteLine("");
            sw.WriteLine(string.Format("{0},{1}", "Pracownicy", "Stanowisko"));
            foreach(UserWithPerm x in users)
            {
                sw.WriteLine(string.Format("{0},{1}", x.UserName, x.Perm == "1" ? "Zarzadca": "Pracownik"));
            }
            sw.WriteLine("");
            sw.WriteLine(string.Format("{0},{1},{2},{3}", "Nazwa zadania", "Opis", "Przydzielono do", "Oczekiwany czas"));
            foreach (Task x in tasks)
            {
                sw.WriteLine(string.Format("{0},{1},{2},{3}", x.Name ,x.Description ,x.Assigne , x.Hour));
            }
            return sw.ToString();
        }

        public void DeleteTaskFromProject(string id)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmdPerm = new SqlCommand("spDeleteAllTaskForProjectId", con);
                cmdPerm.CommandType = CommandType.StoredProcedure;

                SqlParameter paramIdToPerm = new SqlParameter();
                paramIdToPerm.ParameterName = "@Id";
                paramIdToPerm.Value = id;
                cmdPerm.Parameters.Add(paramIdToPerm);
                con.Open();
                cmdPerm.ExecuteNonQuery();
            }
        }

        public void DeleteTask(string id)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmdPerm = new SqlCommand("spDeleteTask", con);
                cmdPerm.CommandType = CommandType.StoredProcedure;

                SqlParameter paramIdToPerm = new SqlParameter();
                paramIdToPerm.ParameterName = "@Id";
                paramIdToPerm.Value = id;
                cmdPerm.Parameters.Add(paramIdToPerm);
                con.Open();
                cmdPerm.ExecuteNonQuery();
            }
        }

        public void ClearAssigneForUser(string user, string project)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmdPerm = new SqlCommand("spClearAssigneUP", con);
                cmdPerm.CommandType = CommandType.StoredProcedure;

                SqlParameter paramId = new SqlParameter();
                paramId.ParameterName = "@Id";
                paramId.Value = user;
                cmdPerm.Parameters.Add(paramId);
                SqlParameter paramIdP = new SqlParameter();
                paramIdP.ParameterName = "@ProjectId";
                paramIdP.Value = project;
                cmdPerm.Parameters.Add(paramIdP);
                con.Open();
                cmdPerm.ExecuteNonQuery();
            }
        }

        public void DeleteAllAssigneTask(string user)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmdPerm = new SqlCommand("spDeleteAllTaskForUserId", con);
                cmdPerm.CommandType = CommandType.StoredProcedure;

                SqlParameter paramIdToPerm = new SqlParameter();
                paramIdToPerm.ParameterName = "@Id";
                paramIdToPerm.Value = user;
                cmdPerm.Parameters.Add(paramIdToPerm);
                con.Open();
                cmdPerm.ExecuteNonQuery();
            }
        }

        public Task getTaskFromId(string id)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            List<Project> project = new List<Project>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetTask", con);
                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@Id";
                paramName.Value = id;
                cmd.Parameters.Add(paramName);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Task t = new Task();
                    t.Id = rdr["Id"].ToString();
                    t.Description = rdr["Description"].ToString();
                    t.Hour = Convert.ToInt32(rdr["Hour"].ToString());
                    t.ProjectId = rdr["ProjectId"].ToString();
                    t.Assigne = rdr["Assigne"].ToString();
                    t.Name = rdr["Name"].ToString();
                    return t;
                }
            }

            return null;
        }

        public IEnumerable<UserWithPerm> UserInProject(String id)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            List<UserWithPerm> users = new List<UserWithPerm>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserInProject", con);
                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@Id";
                paramName.Value = id;
                cmd.Parameters.Add(paramName);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserWithPerm user = new UserWithPerm();
                    user.ID = rdr["Id"].ToString();
                    user.Perm = rdr["Perm"].ToString() == "0" ? "Pracownik" : "Zarządca";
                    user.UserName = rdr["UserName"].ToString();

                    users.Add(user);
                }
            }
            return users;
        }

        public void DeleteAllPermissionsForUser(string user)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmdPerm = new SqlCommand("spDeleteAllPermissionForUser", con);
                cmdPerm.CommandType = CommandType.StoredProcedure;

                SqlParameter paramIdToPerm = new SqlParameter();
                paramIdToPerm.ParameterName = "@Id";
                paramIdToPerm.Value = user;
                cmdPerm.Parameters.Add(paramIdToPerm);
                con.Open();
                cmdPerm.ExecuteNonQuery();
            }
        }

        public void DeleteUser(string user)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmdPerm = new SqlCommand("spDeleteUser", con);
                cmdPerm.CommandType = CommandType.StoredProcedure;

                SqlParameter paramIdToPerm = new SqlParameter();
                paramIdToPerm.ParameterName = "@Id";
                paramIdToPerm.Value = user;
                cmdPerm.Parameters.Add(paramIdToPerm);
                con.Open();
                cmdPerm.ExecuteNonQuery();
            }
        }

        public List<Task> GetTaskForProject(string id)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            List<Task> tasks = new List<Task>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spTaskInProject", con);
                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@Id";
                paramName.Value = id;
                cmd.Parameters.Add(paramName);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Task task = new Task();
                    task.Id = rdr["Id"].ToString();
                    task.Assigne = rdr["Assigne"].ToString();
                    task.ProjectId = rdr["ProjectId"].ToString();
                    task.Hour = Convert.ToInt32(rdr["Hour"].ToString());
                    task.Description = rdr["Description"].ToString();
                    task.Name = rdr["Name"].ToString();

                    tasks.Add(task);
                }
            }
            return tasks;
        }

        public string GetUserIdFromName(string userName)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            List<Project> project = new List<Project>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetIdFromUserName", con);
                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@Name";
                paramName.Value = userName;
                cmd.Parameters.Add(paramName);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    return (String)rdr["Id"];
                }
            }

            return "";
        }

        List<Project> GetProjectForUser(String id)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            List<Project> project = new List<Project>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetProject", con);
                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@Id";
                paramName.Value = id;
                cmd.Parameters.Add(paramName);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Project p = new Project();
                    p.Id = rdr["Id"].ToString();
                    p.Name = rdr["Name"].ToString();
                    p.Language = rdr["Language"].ToString();
                    project.Add(p);
                }
            }

            return project.ToList();

        }

        public void AddPermissionToProject(string id, String project, String perm)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spAddPermission", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramId = new SqlParameter();
                paramId.ParameterName = "@Id";
                paramId.Value = id;
                cmd.Parameters.Add(paramId);

                SqlParameter ParamProjectId = new SqlParameter();
                ParamProjectId.ParameterName = "@ProjectId";
                ParamProjectId.Value = project;
                cmd.Parameters.Add(ParamProjectId);

                SqlParameter paramPerm = new SqlParameter();
                paramPerm.ParameterName = "@Perm";
                paramPerm.Value = perm;
                cmd.Parameters.Add(paramPerm);


                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void RemoveProjectFromUser(string user, string project)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteProjectFromUser", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramUserId = new SqlParameter();
                paramUserId.ParameterName = "@UserId";
                paramUserId.Value = user;
                cmd.Parameters.Add(paramUserId);
                SqlParameter paramProjectId = new SqlParameter();
                paramProjectId.ParameterName = "@ProjectId";
                paramProjectId.Value = project;
                cmd.Parameters.Add(paramProjectId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Project> GetAllProject()
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            List<Project> project = new List<Project>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllProject", con);

                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Project p = new Project();
                    p.Id = rdr["Id"].ToString();
                    p.Name = rdr["Name"].ToString();
                    p.Language = rdr["Language"].ToString();
                    project.Add(p);
                }
            }

            return project.ToList();
        }

        public void DeleteProject(String id)
        {
            string connectionString =
        ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteProject", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramId = new SqlParameter();
                paramId.ParameterName = "@Id";
                paramId.Value = id;
                cmd.Parameters.Add(paramId);


                con.Open();
                cmd.ExecuteNonQuery();
            }

        }

        public void DeletePermissions(String id)
        {
            string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmdPerm = new SqlCommand("spDeleteAllPermission", con);
                cmdPerm.CommandType = CommandType.StoredProcedure;

                SqlParameter paramIdToPerm = new SqlParameter();
                paramIdToPerm.ParameterName = "@Id";
                paramIdToPerm.Value = id;
                cmdPerm.Parameters.Add(paramIdToPerm);
                con.Open();
                cmdPerm.ExecuteNonQuery();
            }

        }
        public void AddProject(Project project)
        {
            string connectionString =
                    ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spAddProject", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@Name";
                paramName.Value = project.Name;
                cmd.Parameters.Add(paramName);

                SqlParameter paramId = new SqlParameter();
                paramId.ParameterName = "@Id";
                paramId.Value = project.Id;
                cmd.Parameters.Add(paramId);

                SqlParameter paramLanguage = new SqlParameter();
                paramLanguage.ParameterName = "@Language";
                paramLanguage.Value = project.Language;
                cmd.Parameters.Add(paramLanguage);


                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<SelectListItem> getAllProjectForUserWithPerm(String userId, String id, String perm)
        {

            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            List<Project> project = new List<Project>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllProjectWithPerm", con);
                SqlParameter paramId = new SqlParameter();
                paramId.ParameterName = "@Id";
                paramId.Value = id;
                cmd.Parameters.Add(paramId);
                SqlParameter paramPerm = new SqlParameter();
                paramPerm.ParameterName = "@Perm";
                paramPerm.Value = perm;
                cmd.Parameters.Add(paramPerm);
                SqlParameter paramUserId = new SqlParameter();
                paramUserId.ParameterName = "@UserId";
                paramUserId.Value = userId;
                cmd.Parameters.Add(paramUserId);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Project p = new Project();
                    p.Id = rdr["Id"].ToString();
                    p.Name = rdr["Name"].ToString();
                    project.Add(p);
                }
            }
            List<SelectListItem> list = new List<SelectListItem>();
            project.ToList().ForEach(x => list.Add(new SelectListItem { Text = x.Name, Value = x.Id }));
            return list;
        }

        public static IEnumerable<SelectListItem> getUserFromProject(String ProjectId)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            List<User> project = new List<User>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllUserFromProject", con);
                SqlParameter paramId = new SqlParameter();
                paramId.ParameterName = "@Id";
                paramId.Value = ProjectId;
                cmd.Parameters.Add(paramId);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    User p = new User();
                    p.ID = rdr["Id"].ToString();
                    p.UserName = rdr["UserName"].ToString();
                    project.Add(p);
                }
            }
            List<SelectListItem> list = new List<SelectListItem>();
            project.ToList().ForEach(x => list.Add(new SelectListItem { Text = x.UserName, Value = x.ID }));
            return list;
        }

        public static IEnumerable<SelectListItem> getNotHavingProject(String UserId, String CurrentUserId)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            List<Project> project = new List<Project>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetNotHavingProject", con);
                SqlParameter paramId = new SqlParameter();
                paramId.ParameterName = "@Id";
                paramId.Value = UserId;
                cmd.Parameters.Add(paramId);

                SqlParameter userId = new SqlParameter();
                userId.ParameterName = "@UId";
                userId.Value = CurrentUserId;
                cmd.Parameters.Add(userId);

                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Project p = new Project();
                    p.Id = rdr["Id"].ToString();
                    p.Name = rdr["UserName"].ToString();
                    project.Add(p);
                }
            }
            List<SelectListItem> list = new List<SelectListItem>();
            project.ToList().ForEach(x => list.Add(new SelectListItem { Text = x.Name, Value = x.Id }));
            return list;
        }

        public static int getIfPermIsCorrect(String userId, String id)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            List<Project> project = new List<Project>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetProjectIfPermIsEnought", con);
                SqlParameter paramId = new SqlParameter();
                paramId.ParameterName = "@Id";
                paramId.Value = id;
                cmd.Parameters.Add(paramId);
                SqlParameter paramUserId = new SqlParameter();
                paramUserId.ParameterName = "@UserId";
                paramUserId.Value = userId;
                cmd.Parameters.Add(paramUserId);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string count = rdr["Perm"].ToString();
                    return Convert.ToInt32(count);
                }
                return -1;
            }
        }

        public static bool GetPermission(String id)
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            List<Project> project = new List<Project>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("CheckSpecialUser", con);
                SqlParameter paramId = new SqlParameter();
                paramId.ParameterName = "@Id";
                paramId.Value = id;
                cmd.Parameters.Add(paramId);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string Id = rdr["Id"].ToString();
                    return Id != "" ? true : false;
                }
                return false;
            }
        }
    }
}
