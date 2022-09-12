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
        int companyId = 0;
        int trackingCompanyId = 0;
        bool companyResult = false;
        object emailresult;
        int emailCount = 0;
        int userId = 0;
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
        public ActionResult GetList()
        {
            SalesDetailsViewModel companylist = new SalesDetailsViewModel();
            UserViewModel userlist = new UserViewModel();
            WriterViewModel review = new WriterViewModel();
            UserTrackingViewModel userTrackingViewModel = new UserTrackingViewModel();
            var registerId = 0;

            if (Session["RegisterId"] != null)
            {
                registerId = Convert.ToInt32(Session["RegisterId"]);
            }
            try
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@RegisterId", registerId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
                userTrackingViewModel = con.Query<UserTrackingViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                if (companyId != 0)
                {
                    userTrackingViewModel.CompanyId = companyId.ToString();
                    companyId = 0;
                }
                var defaultCompanyId = 0;
                if (userTrackingViewModel != null && userTrackingViewModel.CompanyId != null)
                {
                    defaultCompanyId = Convert.ToInt32(userTrackingViewModel.CompanyId);
                }
                else
                {
                    userTrackingViewModel = new UserTrackingViewModel();
                }
                if (trackingCompanyId != 0)
                {
                    defaultCompanyId = 0;
                }
                if (userId == 1)
                {
                    userTrackingViewModel.UserId = "0";
                    userId = 0;
                }
                parameters = new DynamicParameters();
                parameters.Add("@CompanyId", defaultCompanyId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Mode", 7, DbType.Int32, ParameterDirection.Input);
                companylist = con.Query<SalesDetailsViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();

                parameters = new DynamicParameters();
                parameters.Add("@CompanyId", companylist.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Mode", 10, DbType.Int32, ParameterDirection.Input);
                var reviewcompanylist = con.Query<int>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                if(reviewcompanylist == 0)
                {
                    emailCount++;
                    if (emailCount > 170)
                    {
                        emailresult = new { user = "", reviews = "", company = "" };
                        return Json(emailresult, JsonRequestBehavior.AllowGet);
                    }
                    companyId = companylist.Id;
                    con.Close();
                    if (companyId != 0)
                    {
                        GetList();
                    }
                }
                if (companylist != null)
                {
                    trackingCompanyId = 0;
                    parameters = new DynamicParameters();
                    parameters.Add("@CompanyId", companylist.Id, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@RegisterId", registerId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Mode", 9, DbType.Int32, ParameterDirection.Input);
                    var list = con.Query<UserTrackingViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (userTrackingViewModel.UserId == null)
                    {
                        userTrackingViewModel.UserId = "0";
                    }
                    parameters = new DynamicParameters();
                    parameters.Add("@CompanyId", companylist.Id, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@RegisterId", registerId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@id", userTrackingViewModel.UserId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
                    userlist = con.Query<UserViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (userlist != null)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("@Mode", 6, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@CompanyId", companylist.Id, DbType.Int32, ParameterDirection.Input);
                        review = con.Query<WriterViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        if (review == null)
                        {
                            userlist = new UserViewModel();
                            review = new WriterViewModel();
                            review.ReviewName = "reviewnotfound";
                            emailCount++;
                            if (emailCount > 170)
                            {
                                emailresult = new { user = userlist, reviews = "reviewnotound", company = companylist };
                                return Json(emailresult, JsonRequestBehavior.AllowGet);
                            }
                            companyId = companylist.Id;
                            con.Close();
                            if (companyId != 0)
                            {
                                GetList();
                            }
                        }
                        else
                        {
                            emailresult = new { user = userlist, reviews = review, company = companylist };
                        }
                    }
                    else
                    {
                        userlist = new UserViewModel();
                        review = new WriterViewModel();
                        emailCount++;
                        if (emailCount > 170)
                        {
                            emailresult = new { user = "usernotound", reviews = review, company = companylist };
                            return Json(emailresult, JsonRequestBehavior.AllowGet);
                        }
                        con.Close();
                        if (userId == 0)
                        {
                            userId = 1;
                            GetList();
                        }
                    }

                }
                else
                {
                    emailresult = new { user = "nocontent", reviews = "", company = "" };
                }
            }
            catch (Exception)
            {
                con.Close();
            }
            finally
            {
                con.Close();
            }

            return Json(emailresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
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

    }
}