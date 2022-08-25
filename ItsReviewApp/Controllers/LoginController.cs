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
    public class LoginController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;

        public LoginController()
        {
            con = new SqlConnection(connectionString);
        }

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            Session["EmailId"] = loginViewModel.EmailId;
            var logindata = (dynamic)null;
            var parameters = new DynamicParameters();
            parameters.Add("@EmailId", loginViewModel.EmailId, DbType.String, ParameterDirection.Input);
            parameters.Add("@password", loginViewModel.Password, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                logindata = connection.ExecuteScalar("sp_Register", parameters, commandType: CommandType.StoredProcedure);
                connection.Close();
            }

            //return View();
            return Json(logindata, JsonRequestBehavior.AllowGet);

        }
    }
}