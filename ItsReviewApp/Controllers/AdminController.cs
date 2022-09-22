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

        public ActionResult UserReport()
        {
            if (Session["RegisterId"] == null)
            {
                return RedirectToAction("Login", "Login", new { area = "" });
            }
            return View();
        }
        [HttpGet]
        public ActionResult GetUserReport(DateTime FromDate,DateTime ToDate)
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
                    var expected= UseReviewList.Where(x=>x.CreatedDate==item.CreatedDate).FirstOrDefault();
                    if (expected != null)
                    {
                        item.UseReview = expected.UseReview;
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
        public ActionResult GetUserTrackReport(DateTime FromDate, DateTime ToDate, string RegisterId)
        {
            try
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", FromDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@ToDate", ToDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@RegisterId", RegisterId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@CompanyId", 0, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
                var ReviewPerDayList = con.Query<UserTrackingViewModel>("sp_UserReport", parameters, commandType: CommandType.StoredProcedure);
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
        public ActionResult GetCompanyTrackReport(DateTime FromDate, DateTime ToDate, string CompanyId)
        {
            try
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", FromDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@ToDate", ToDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@RegisterId", 0, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@CompanyId", CompanyId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
                var ReviewPerDayList = con.Query<UserTrackingViewModel>("sp_UserReport", parameters, commandType: CommandType.StoredProcedure);
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
    }
}