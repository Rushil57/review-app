using Dapper;
using ItsReviewApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ItsReviewApp.Controllers
{
    public class UserController : Controller
    {

        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;

        public UserController()
        {
            con = new SqlConnection(connectionString);
        }

        // GET: Role
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Create()
        {
           
            List<RegisterViewModel> userList = new List<RegisterViewModel>();
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            userList = con.Query<RegisterViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).ToList();
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            foreach (var user in userList)
            {
                var item = new SelectListItem { Text = user.Name, Value = user.Id.ToString() };
                selectListItems.Add(item);
            }
            ViewBag.Name = selectListItems;

            return View();
        }

        [HttpPost]
        public ActionResult Create(UserViewModel userViewModel)
        {
            List<string> EmailList = new List<string>();
            foreach (var item in userViewModel.Users)
            {
                var parameter = new DynamicParameters();
                parameter.Add("@RegisterId", userViewModel.RegisterId, DbType.String, ParameterDirection.Input);
                parameter.Add("@Name", userViewModel.Name, DbType.String, ParameterDirection.Input);
                parameter.Add("@FirstName", item.FirstName, DbType.String, ParameterDirection.Input);
                parameter.Add("@LastName", item.LastName, DbType.String, ParameterDirection.Input);
                parameter.Add("@EmailId", item.EmailId, DbType.String, ParameterDirection.Input);
                //parameter.Add("@Category", roleViewModel.Category, DbType.String, ParameterDirection.Input);
                parameter.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
                using (var conn = new SqlConnection(connectionString))
                {
                    var userSave = conn.ExecuteScalar("sp_User", parameter, commandType: CommandType.StoredProcedure);
                    if (userSave == null)
                    {
                        EmailList.Add(item.EmailId);
                    }
                    else
                    {
                        foreach (var category in item.UserCategoryViewModel)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("@CategoryId", category, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@UserId", userSave, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
                            using (IDbConnection connection = new SqlConnection(connectionString))
                            {
                                var categorysave = connection.ExecuteScalar("sp_UserCategory", parameters, commandType: CommandType.StoredProcedure);
                                con.Close();
                            }
                        }
                    }
                    conn.Close();
                }

                
            }
            //return RedirectToAction("Role", "Role");
            return Json(EmailList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Review()
        {
            return View();
        }


        public JsonResult category()
        {
            List<CategoryViewModel> CategoryList = new List<CategoryViewModel>();
            var parameter = new DynamicParameters();
            parameter.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
            CategoryList = con.Query<CategoryViewModel>("sp_Salesdetails", parameter, commandType: CommandType.StoredProcedure).ToList();
            List<SelectListItem> ListItems = new List<SelectListItem>();
            foreach (var emp in CategoryList)
            {
                var item = new SelectListItem { Text = emp.CategoryName, Value = emp.Id.ToString() };
                ListItems.Add(item);
            }
            ViewBag.Category = ListItems;
            return Json(ListItems, JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        public JsonResult GetList()
        {
            List<UserViewModel> RoleList = new List<UserViewModel>();
            try
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
                RoleList = con.Query<UserViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).ToList();
                
            }
            catch (Exception)
            {
                con.Close();
                throw;
            }
            finally
            {
                con.Close();
            }
            return Json(RoleList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaltform()
        {
            List<SelectListItem> palform = new List<SelectListItem>() {
          
                new SelectListItem {Text = "Google", Value = "1"},
                new SelectListItem {Text = "TripAdvisor", Value = "2"},
                new SelectListItem {Text = "FaceBook", Value = "3"},};
                return Json(palform, JsonRequestBehavior.AllowGet);
        }

    }
}