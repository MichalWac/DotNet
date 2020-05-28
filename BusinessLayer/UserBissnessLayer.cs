using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

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
        public static int GetPermission(String id)
        {
            return 2;
        }
    }
}
