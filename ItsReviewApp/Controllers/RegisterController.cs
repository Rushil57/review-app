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
    public class RegisterController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;

        public RegisterController()
        {
            con = new SqlConnection(connectionString);
        }

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPaltform()
        {
            List<SelectListItem> palform = new List<SelectListItem>() {
            new SelectListItem {Text = "Select Paltform", Value = "0"},
            new SelectListItem {Text = "Google", Value = "1"},
            new SelectListItem {Text = "TripAdvisor", Value = "2"},
            new SelectListItem {Text = "FaceBook", Value = "3"},};
            return Json(palform, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetList()
        {
            SalesDetailsViewModel companylist = new SalesDetailsViewModel();
            UserViewModel userlist = new UserViewModel();
            WriterViewModel review = new WriterViewModel();
            UserTrackingViewModel userTrackingViewModel = new UserTrackingViewModel();

            try
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
                userTrackingViewModel = con.Query<UserTrackingViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                if (userTrackingViewModel != null)
                {
                    parameters = new DynamicParameters();
                    //parameters.Add("@CompanyId", companylist.Id, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@CompanyId", userTrackingViewModel.CompanyId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Mode", 7, DbType.Int32, ParameterDirection.Input);
                    companylist = con.Query<SalesDetailsViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (companylist != null)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("@CompanyId", companylist.Id, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@Mode", 9, DbType.Int32, ParameterDirection.Input);
                        var list = con.Query<UserTrackingViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();

                        parameters = new DynamicParameters();
                        parameters.Add("@CompanyId", companylist.Id, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
                        userlist = con.Query<UserViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        if (userlist != null)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("@Mode", 6, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@CompanyId", companylist.Id, DbType.Int32, ParameterDirection.Input);
                            if(list != null)
                            {
                                parameters.Add("@WriterId", list.WriterId, DbType.String, ParameterDirection.Input);
                            }
                            review = con.Query<WriterViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        }

                    }
                }
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
            var result = new { user = userlist, reviews = review, company = companylist };
            //return Json(result, JsonRequestBehavior.AllowGet);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            //List<SelectListItem> Role = new List<SelectListItem>() {
            //new SelectListItem {Text = "Admin", Value = "1"},
            //new SelectListItem {Text = "Writer", Value = "2"},
            //new SelectListItem {Text = "Back Office", Value = "3"},
            //new SelectListItem {Text = "Check Up", Value = "4"},
            //new SelectListItem {Text = " Accounts", Value = "5"},
            //new SelectListItem {Text = " User", Value = "6"},};
            //ViewBag.Role = Role;
            //return View();

            List<RoleViewModel> roles = new List<RoleViewModel>();
            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
            roles = con.Query<RoleViewModel>("sp_Role", parameters, commandType: CommandType.StoredProcedure).ToList();
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            foreach (var role in roles)
            {
                var item = new SelectListItem { Text = role.RoleName, Value = role.Id.ToString() };
                selectListItems.Add(item);
            }
            ViewBag.Role = selectListItems;
            return View();
        }

        [HttpPost]
        public ActionResult Create(RegisterViewModel userViewModel)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Name", userViewModel.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@Address", userViewModel.Address, DbType.String, ParameterDirection.Input);
            parameters.Add("@PhoneNumber", userViewModel.PhoneNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("@ReferencePhoneNumber", userViewModel.ReferencePhoneNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("@EmailId", userViewModel.EmailId, DbType.String, ParameterDirection.Input);
            parameters.Add("@Password", userViewModel.Password, DbType.String, ParameterDirection.Input);
            parameters.Add("@Role", userViewModel.Role, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var register = connection.ExecuteScalar("sp_Register", parameters, commandType: CommandType.StoredProcedure);
                connection.Close();
            }
            return RedirectToAction("Create", "Register");
        }


        [HttpPost]
        public ActionResult SaveTrackingData(UserTrackingViewModel userTrackingViewModel)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", userTrackingViewModel.CompanyId, DbType.String, ParameterDirection.Input);
            parameters.Add("@WriterId", userTrackingViewModel.WriterId, DbType.String, ParameterDirection.Input);
            parameters.Add("@UserId", userTrackingViewModel.UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("@Status", userTrackingViewModel.Status, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var trackdata = connection.ExecuteScalar("sp_UserTracking", parameters, commandType: CommandType.StoredProcedure);
                connection.Close();
            }

            //return View();
            return RedirectToAction("Create", "Register");
        }

    }
}