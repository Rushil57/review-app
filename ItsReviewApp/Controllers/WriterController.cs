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
    public class WriterController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;


        public WriterController()
        {
            con = new SqlConnection(connectionString);
        }

        // GET: Writer
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            List<SalesViewModel> empList = new List<SalesViewModel>();

            List<SelectListItem> MySkills = new List<SelectListItem>() {
            new SelectListItem {Text = "ASP.NET MVC", Value = "1"},
            new SelectListItem {Text = "ASP.NET WEB API", Value = "2"},
            new SelectListItem {Text = "ENTITY FRAMEWORK", Value = "3"},
            new SelectListItem {Text = "DOCUSIGN", Value = "4"},
            new SelectListItem {Text = "ORCHARD CMS", Value = "5"},};
            ViewBag.MySkills = MySkills;

            List<SelectListItem> Country = new List<SelectListItem>() {
            new SelectListItem {Text = "Afghanistan", Value = "1"},
            new SelectListItem {Text = "Albania", Value = "2"},
            new SelectListItem {Text = "Algeria", Value = "3"},
            new SelectListItem {Text = "Andorra", Value = "4"},
            new SelectListItem {Text = "India", Value = "5"},};
            ViewBag.Country = Country;

            List<SelectListItem> Type = new List<SelectListItem>() {
            new SelectListItem {Text = "Type 1", Value = "1"},
            new SelectListItem {Text = "Type 2", Value = "2"},
            new SelectListItem {Text = "Type 3", Value = "3"},
            new SelectListItem {Text = "Type 4", Value = "4"},
            new SelectListItem {Text = "General", Value = "5"},};
            ViewBag.Type = Type;

            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
            empList = con.Query<SalesViewModel>("sp_Writer", parameters, commandType: CommandType.StoredProcedure).ToList();
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            foreach (var emp in empList)
            {
                var item = new SelectListItem { Text = emp.CompanyName, Value = emp.Id.ToString() };
                selectListItems.Add(item);
            }
            ViewBag.Company = selectListItems;

            return View();
        }

        [HttpPost]
        public ActionResult Create(WriterViewModel data)
        {
            try
            {
                var count = 1.0;
                var parameter = new DynamicParameters();
                parameter.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@CompanyId", data.CompanyId, DbType.Int32, ParameterDirection.Input);
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var versionName = connection.ExecuteScalar("sp_Writer", parameter, commandType: CommandType.StoredProcedure);
                    if (versionName != null)
                    {
                        var parts = versionName.ToString().Split('.');
                        int i1 = int.Parse(parts[0]);
                        count += i1++;
                    }
                    connection.Close();
                }

                foreach (var item in data.Reviews)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ReviewName", item.ReviewName, DbType.String, ParameterDirection.Input);
                    parameters.Add("@CompanyId", data.CompanyId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Version", count.ToString(), DbType.String, ParameterDirection.Input);
                    using (IDbConnection connection = new SqlConnection(connectionString))
                    {
                        var reviewSave = connection.ExecuteScalar("sp_Writer", parameters, commandType: CommandType.StoredProcedure);
                        count += 0.1;
                        connection.Close();
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Create","Writer");
        }

        [HttpGet]
        public bool GetUserData(string ReviewName)
        {
            var result = false;
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ReviewName", ReviewName, DbType.String, ParameterDirection.Input);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var reviewValidation = connection.ExecuteScalar("sp_Writer", parameters, commandType: CommandType.StoredProcedure);
                if (reviewValidation != null)
                {
                    result = true;
                }
            }
            return result;
        }
    }
}