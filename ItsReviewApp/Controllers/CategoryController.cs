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
    public class CategoryController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;

        public CategoryController()
        {
            con = new SqlConnection(connectionString);
        }

        // GET: Category
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult Create(CategoryViewModel categoryMasterViewModel)
        {
            List<string> CategoryList = new List<string>();
            con.Open();
            var parameter = new DynamicParameters();
            parameter.Add("@CategoryName", categoryMasterViewModel.CategoryName, DbType.String, ParameterDirection.Input);
            parameter.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);

            using (var conn = new SqlConnection(connectionString))
            {
                var categorySave = conn.ExecuteScalar("sp_Category", parameter, commandType: CommandType.StoredProcedure);
                if (categorySave == null)
                {
                    CategoryList.Add(categoryMasterViewModel.CategoryName);
                }
                conn.Close();
            }
            return Json(CategoryList, JsonRequestBehavior.AllowGet);
        }

    }
}