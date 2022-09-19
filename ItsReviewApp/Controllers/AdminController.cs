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
    }
}