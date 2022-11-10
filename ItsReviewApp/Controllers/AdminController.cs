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
    public class AdminController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        public AdminController()
        {
            con = new SqlConnection(connectionString);
        }
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [UserRoleProvider]
        public ActionResult UserReport()
        {
            if (Session["RegisterId"] == null)
            {
                return RedirectToAction("Login", "Login", new { area = "" });
            }
            return View();
        }
        [HttpGet]
        public ActionResult GetUserReport(DateTime FromDate, DateTime ToDate)
        {
            List<RegisterViewModel> RoleList = new List<RegisterViewModel>();
            try
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", FromDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@ToDate", ToDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
                RoleList = con.Query<RegisterViewModel>("sp_UserReport", parameters, commandType: CommandType.StoredProcedure).ToList();

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
        [HttpGet]
        public ActionResult GetUserReviewDayReport(DateTime FromDate, DateTime ToDate)
        {
            List<UserTrackingViewModel> ExpectedReviewList = new List<UserTrackingViewModel>();
            try
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", FromDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@ToDate", ToDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
                //RoleList = con.Query<RegisterViewModel>("sp_UserReport", parameters, commandType: CommandType.StoredProcedure).ToList();
                var reader = con.QueryMultiple("sp_UserReport", parameters, commandType: CommandType.StoredProcedure);
                ExpectedReviewList = reader.Read<UserTrackingViewModel>().ToList();
                var UseReviewList = reader.Read<UserTrackingViewModel>().ToList();
                foreach (var item in ExpectedReviewList)
                {
                    var expected = UseReviewList.Where(x => x.CreatedDate == item.CreatedDate).FirstOrDefault();
                    if (expected != null)
                    {
                        item.UseReview = expected.UseReview;
                        item.TodayCount = expected.TodayCount;
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
            return Json(ExpectedReviewList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetUserList()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
            con.Open();
            var empList = con.Query<RegisterViewModel>("sp_UserReport", parameters, commandType: CommandType.StoredProcedure);
            con.Close();
            return Json(empList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetWriterUserList()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
            con.Open();
            var empList = con.Query<RegisterViewModel>("sp_UserReport", parameters, commandType: CommandType.StoredProcedure);
            con.Close();
            return Json(empList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetCompanyTrackReportList(DateTime FromDate, DateTime ToDate, string CompanyId)
        {
            try
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", FromDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@ToDate", ToDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@CompanyId", CompanyId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
                var ReviewPerDayList = con.Query<UserTrackingViewModel>("sp_UserCompanyReport", parameters, commandType: CommandType.StoredProcedure);
                con.Close();
                return Json(ReviewPerDayList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                con.Close();
                throw;
            }
        }
        [HttpGet]
        public ActionResult GetCompanyList()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 6, DbType.Int32, ParameterDirection.Input);
            con.Open();
            var empList = con.Query<SalesDetailsViewModel>("sp_UserReport", parameters, commandType: CommandType.StoredProcedure);
            con.Close();
            return Json(empList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetUserEmailList(string registerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RegisterId", registerId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
            con.Open();
            var userList = con.Query<UserViewModel>("sp_UserCompanyReport", parameters, commandType: CommandType.StoredProcedure);
            con.Close();
            return Json(userList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DeleteUserEmail(string Id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
            con.Open();
            var DeleteEmail = con.Query<UserViewModel>("sp_UserReport", parameters, commandType: CommandType.StoredProcedure);
            con.Close();
            return Json(DeleteEmail, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetUserTrackReport(DateTime FromDate, DateTime ToDate, string RegisterId)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var search = Request.Form.GetValues("search[value]").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@skip", skip, DbType.String, ParameterDirection.Input);
            parameters.Add("@search", search, DbType.String, ParameterDirection.Input);
            parameters.Add("@PageSize", pageSize, DbType.String, ParameterDirection.Input);
            parameters.Add("@FromDate", FromDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@ToDate", ToDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@RegisterId", RegisterId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
            var userTrackListList = con.Query<UserTrackingViewModel>("sp_UserCompanyReport", parameters, commandType: CommandType.StoredProcedure).ToList();
            if (userTrackListList.Count > 0)
            {
                recordsTotal = userTrackListList[0].usercount;
            }
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = userTrackListList }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetUserTrackingReportList(DateTime FromDate, DateTime ToDate)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var search = Request.Form.GetValues("search[value]").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@skip", skip, DbType.String, ParameterDirection.Input);
            parameters.Add("@search", search, DbType.String, ParameterDirection.Input);
            parameters.Add("@PageSize", pageSize, DbType.String, ParameterDirection.Input);
            parameters.Add("@FromDate", FromDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@ToDate", ToDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            var userTrackListList = con.Query<UserTrackingViewModel>("sp_UserCompanyReport", parameters, commandType: CommandType.StoredProcedure).ToList();
            if (userTrackListList.Count > 0)
            {
                recordsTotal = userTrackListList[0].usercount;
            }
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = userTrackListList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetWriterUserTrackingReportList(DateTime FromDate, DateTime ToDate, string RegisterId)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var search = Request.Form.GetValues("search[value]").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@skip", skip, DbType.String, ParameterDirection.Input);
            parameters.Add("@search", search, DbType.String, ParameterDirection.Input);
            parameters.Add("@PageSize", pageSize, DbType.String, ParameterDirection.Input);
            parameters.Add("@FromDate", FromDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@ToDate", ToDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@RegisterId", RegisterId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
            var userTrackListList = con.Query<UserTrackingViewModel>("sp_UserCompanyReport", parameters, commandType: CommandType.StoredProcedure).ToList();
            if (userTrackListList.Count > 0)
            {
                recordsTotal = userTrackListList[0].usercount;
            }
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = userTrackListList }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSalesList()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 6, DbType.Int32, ParameterDirection.Input);
            con.Open();
            var SalesList = con.Query<RegisterViewModel>("sp_UserCompanyReport", parameters, commandType: CommandType.StoredProcedure);
            con.Close();
            return Json(SalesList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetCompanyLeadList(string registerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RegisterId", registerId, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 7, DbType.Int32, ParameterDirection.Input);
            con.Open();
            var userList = con.Query<SalesViewModel>("sp_UserCompanyReport", parameters, commandType: CommandType.StoredProcedure);
            con.Close();
            return Json(userList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditUserEmail(int Id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 9, DbType.Int32, ParameterDirection.Input);
            con.Open();
            var DeleteEmail = con.Query<UserViewModel>("sp_UserReport", parameters, commandType: CommandType.StoredProcedure);
            con.Close();
            return Json(DeleteEmail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategory()
        {
            List<CategoryViewModel> CategoryList = new List<CategoryViewModel>();

            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
            CategoryList = con.Query<CategoryViewModel>("sp_Salesdetails", parameters, commandType: CommandType.StoredProcedure).ToList();
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            foreach (var emp in CategoryList)
            {
                var item = new SelectListItem { Text = emp.CategoryName, Value = emp.Id.ToString() };
                selectListItems.Add(item);
            }
            ViewBag.Category = selectListItems;
            return Json(selectListItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateUserCategoryData(UserViewModel userViewModel)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userViewModel.Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            con.Open();
            var DeleteEmail = con.Query<UserViewModel>("sp_UserCategory", parameters, commandType: CommandType.StoredProcedure);
            foreach (var category in userViewModel.UserCategoryViewModel)
            {
                var parameter = new DynamicParameters();
                parameter.Add("@UserId", userViewModel.Id, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@CategoryId", category, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var categorysave = connection.ExecuteScalar("sp_UserCategory", parameter, commandType: CommandType.StoredProcedure);
                    con.Close();
                }
            }
            return RedirectToAction("UserReport", "Admin");
        }

        [HttpGet]
        public JsonResult GetRegisterList()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 10, DbType.Int32, ParameterDirection.Input);
            con.Open();
            var RegisterList = con.Query<RegisterViewModel>("sp_UserReport", parameters, commandType: CommandType.StoredProcedure);
            con.Close();
            return Json(RegisterList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditRegisterDetails(int Id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 11, DbType.Int32, ParameterDirection.Input);
            con.Open();
            var GetRegisterList = con.Query<RegisterViewModel>("sp_UserReport", parameters, commandType: CommandType.StoredProcedure);
            con.Close();
            return Json(GetRegisterList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSkipList(DateTime FromDate, DateTime ToDate)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var search = Request.Form.GetValues("search[value]").FirstOrDefault().Trim();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@skip", skip, DbType.String, ParameterDirection.Input);
            parameters.Add("@search", search, DbType.String, ParameterDirection.Input);
            parameters.Add("@PageSize", pageSize, DbType.String, ParameterDirection.Input);
            parameters.Add("@FromDate", FromDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@ToDate", ToDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
            var userSkipListList = con.Query<UserTrackingViewModel>("sp_UserCompanyReport", parameters, commandType: CommandType.StoredProcedure).ToList();
            if (userSkipListList.Count > 0)
            {
                recordsTotal = userSkipListList[0].usercount;
            }
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = userSkipListList }, JsonRequestBehavior.AllowGet);
        }
    }
}