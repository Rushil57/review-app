using Dapper;
using ExcelDataReader;
using ItsReviewApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ItsReviewApp.Controllers
{
    public class WriterController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        List<string> ReviewList = new List<string>();

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
        public async Task<ActionResult> Create(WriterViewModel data)
        {
            try
            {
                await SaveReview(data);
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Create", "Writer");
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

        public ActionResult GetDataByComapnyID(string companyId)
        {
            var id = Convert.ToInt32(companyId);
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);

            var empList = con.Query<SalesDetailsViewModel>("sp_Writer", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();

            return Json(empList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> Upload()
        {

            try
            {
                string companyId = Request.Form["companyId"];
                if (Request.Files.Count > 0)
                {
                    var files = Request.Files;
                    foreach (string str in files)
                    {
                        HttpPostedFileBase file = Request.Files[str] as HttpPostedFileBase;
                        if (file != null)
                        {
                            Stream stream = file.InputStream;
                            IExcelDataReader reader = null;
                            if (file.FileName.EndsWith(".xls"))
                            {
                                reader = ExcelReaderFactory.CreateBinaryReader(stream);
                            }
                            else if (file.FileName.EndsWith(".xlsx"))
                            {
                                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                            }
                            else
                            {
                                return Json("This file format is not supported");
                            }
                            int fieldcount = reader.FieldCount;
                            int rowcount = reader.RowCount;
                            DataTable dt = new DataTable();
                            DataRow row;
                            DataTable dt_ = new DataTable();

                            try
                            {
                                dt_ = reader.AsDataSet().Tables[0];
                                for (int i = 0; i < dt_.Columns.Count; i++)
                                {
                                    dt.Columns.Add(dt_.Rows[0][i].ToString());
                                }
                                int rowcounter = 0;
                                for (int row_ = 1; row_ < dt_.Rows.Count; row_++)
                                {
                                    row = dt.NewRow();

                                    for (int col = 0; col < dt_.Columns.Count; col++)
                                    {
                                        var test = row[col] = dt_.Rows[row_][col].ToString();
                                        rowcounter++;
                                    }
                                    dt.Rows.Add(row);
                                }
                            }
                            catch (Exception)
                            {
                                return Json("Unable to Upload file!");
                            }

                            DataSet result = new DataSet();
                            result.Tables.Add(dt);
                            reader.Close();
                            reader.Dispose();
                            DataTable tmp = result.Tables[0];

                            WriterViewModel writerViewModel = new WriterViewModel();
                            writerViewModel.CompanyId = companyId;

                            List<ReviewModel> listReviewModel = new List<ReviewModel>();

                            for (int i = 0; i < tmp.Rows.Count; i++)
                            {
                                ReviewModel reviewModel = new ReviewModel();
                                reviewModel.ReviewName = tmp.Rows[i][0].ToString();
                                listReviewModel.Add(reviewModel);
                            }
                            writerViewModel.Reviews = listReviewModel;
                            await SaveReview(writerViewModel);
                        }

                    }
                    //return Json("Success");
                    return Json(ReviewList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Please Upload Your file");
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString());
            }
        }

        public async Task<bool> SaveReview(WriterViewModel data)
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
                        if (reviewSave.ToString().ToUpper() == "FALSE")
                        {
                            ReviewList.Add(item.ReviewName);
                        }
                        else
                        {
                            count += 0.1;
                          
                        }
                        connection.Close();

                    }
                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        public ActionResult GetReviewByComapnyID(WriterViewModel writerViewModel)
        {
            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", writerViewModel.CompanyId, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 6, DbType.Int32, ParameterDirection.Input);
            var reviewList = con.Query<WriterViewModel>("sp_Writer", parameters, commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            return Json(reviewList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteReviewByComapnyID(WriterViewModel writerViewModel)
        {
            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", writerViewModel.Id, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 7, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var reviewDelete = connection.ExecuteScalar("sp_Writer", parameters, commandType: CommandType.StoredProcedure);
            }
            con.Close();
            //return Json(reviewList, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Create", "Writer");
        }

        [HttpPost]
        public ActionResult UpdateReviewByComapnyID(WriterViewModel writerViewModel)
        {
            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", writerViewModel.Id, DbType.String, ParameterDirection.Input);
            parameters.Add("@ReviewName", writerViewModel.ReviewName, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var reviewUpdate = connection.ExecuteScalar("sp_Writer", parameters, commandType: CommandType.StoredProcedure);
            }
            con.Close();
            return RedirectToAction("Create", "Writer");
        }

        public ActionResult GetSalesDetails()
        {
            List<SalesDetailsViewModel> salesDetailsViewModelList = new List<SalesDetailsViewModel>();
            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 6, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var reader = con.QueryMultiple("sp_Salesdetails", parameters, commandType: CommandType.StoredProcedure);
                var reviewPerDayList = reader.Read<SalesDetailsViewModel>().ToList();
                var companyList = reader.Read<WriterViewModel>().ToList();

                foreach (var item in reviewPerDayList)
                {
                    SalesDetailsViewModel salesDetailsViewModel = new SalesDetailsViewModel();
                    salesDetailsViewModel.Id = item.Id;
                    salesDetailsViewModel.CompanyName = item.CompanyName;
                    salesDetailsViewModel.CityName = item.CityName;
                    salesDetailsViewModel.ReviewsPerDay = item.ReviewsPerDay;
                    var companyCount = companyList.Where(x => x.CompanyId == item.Id.ToString()).FirstOrDefault();
                    if (companyCount != null && companyCount.CompanyCount > 0)
                    {
                        if (item.ReviewsPerDay != null && item.ReviewsPerDay != "")
                        {
                            var reviewPerDay = Convert.ToInt32(item.ReviewsPerDay);
                            salesDetailsViewModel.Days = companyCount.CompanyCount / reviewPerDay;
                        }
                        else
                        {
                            salesDetailsViewModel.Days = 0;
                        }
                        salesDetailsViewModel.CompanyCount = companyCount.CompanyCount;
                    }
                    else
                    {
                        salesDetailsViewModel.CompanyCount = 0;
                        salesDetailsViewModel.Days = 0;
                    }
                    
                    salesDetailsViewModelList.Add(salesDetailsViewModel);
                }
            }
            con.Close();
            //salesDetailsViewModelList = salesDetailsViewModelList.OrderBy(x => x.Days).ToList();
            return Json(salesDetailsViewModelList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReviewsPerDay(string CompanyId)
        {
            var totalreview = 0;
            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", CompanyId, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 9, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                //var ReviewCount = connection.ExecuteScalar("sp_Writer", parameters, commandType: CommandType.StoredProcedure);
                var ReviewCount = con.Query<SalesDetailsViewModel>("sp_Writer", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                if (ReviewCount != null)
                {
                    if (ReviewCount.CurrentReview == null || ReviewCount.ReviewsPerDay == null)
                    {
                        if (ReviewCount.ReviewsPerDay == null)
                        {
                            totalreview = 0;
                        }
                        else
                        {
                            totalreview = Int32.Parse(ReviewCount.ReviewsPerDay) * 5;
                        }

                    }
                    else
                    {
                        var totalreviewperday = Int32.Parse(ReviewCount.ReviewsPerDay) * 5;
                        totalreview = totalreviewperday - Int32.Parse(ReviewCount.CurrentReview);
                        if (totalreviewperday < 1)
                        {
                            totalreview = 0;
                        }
                    }

                }
                else
                {
                    totalreview = 0;
                }

            }

            con.Close();
            return Json(totalreview, JsonRequestBehavior.AllowGet);
        }
    }
}