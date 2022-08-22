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
    public class SalesController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;

        public SalesController()
        {
            con = new SqlConnection(connectionString);
        }

        // GET: Sales
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(SalesViewModel salesViewModel)
        {
            if (salesViewModel.Id == 0)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ClientName", salesViewModel.ClientName, DbType.String, ParameterDirection.Input);
                parameters.Add("@PhoneNumber", salesViewModel.PhoneNumber, DbType.String, ParameterDirection.Input);
                parameters.Add("@FollowUpDate", salesViewModel.FollowUpDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@CountryName", salesViewModel.CountryName, DbType.String, ParameterDirection.Input);
                //parameters.Add("@City", salesViewModel.SalesPropertyViewModel, DbType.String, ParameterDirection.Input);
                //parameters.Add("@Keywords", salesViewModel.SalesPropertyViewModel, DbType.String, ParameterDirection.Input);
                //parameters.Add("@FoodName", salesViewModel.SalesPropertyViewModel, DbType.String, ParameterDirection.Input);
                //parameters.Add("@TreatmentName", salesViewModel.SalesPropertyViewModel, DbType.String, ParameterDirection.Input);
                parameters.Add("@Remarks", salesViewModel.Remarks, DbType.String, ParameterDirection.Input);
                parameters.Add("@FollowUpCheck", salesViewModel.FollowUpCheck, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var reviewSave = connection.ExecuteScalar("sp_Sales", parameters, commandType: CommandType.StoredProcedure);
                    salesViewModel.SalesId = reviewSave.ToString();
                    connection.Close();
                }

                foreach (var item in salesViewModel.SalesDetailsViewModel)
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@NicheName", item.NicheName, DbType.String, ParameterDirection.Input);
                    parameters.Add("@ListingUrl", item.ListingUrl, DbType.String, ParameterDirection.Input);
                    parameters.Add("@CompanyName", item.CompanyName, DbType.String, ParameterDirection.Input);
                    parameters.Add("@CityName", item.CityName, DbType.String, ParameterDirection.Input);
                    parameters.Add("@ReviewsPerDay", item.ReviewsPerDay, DbType.String, ParameterDirection.Input);
                    parameters.Add("@Platform", item.Platform, DbType.String, ParameterDirection.Input);
                    parameters.Add("@ReviewDate", item.ReviewDate, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@CurrentReview", item.CurrentReview, DbType.String, ParameterDirection.Input);
                    parameters.Add("@SalesId", salesViewModel.SalesId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@RatePerReview", item.RatePerReview, DbType.String, ParameterDirection.Input);
                    parameters.Add("@EmailType", item.EmailType, DbType.String, ParameterDirection.Input);
                    parameters.Add("@OldBalance", item.OldBalance, DbType.String, ParameterDirection.Input);
                    parameters.Add("@CurrentBalance", item.CurrentBalance, DbType.String, ParameterDirection.Input);
                    parameters.Add("@Status", item.Status, DbType.String, ParameterDirection.Input);
                    parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);

                    using (IDbConnection connection = new SqlConnection(connectionString))
                    {
                        var reviewSave = connection.ExecuteScalar("sp_Salesdetails", parameters, commandType: CommandType.StoredProcedure);
                        if (reviewSave != null)
                        {
                            foreach (var category in item.CategoryViewModel)
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("@CategoryId", category, DbType.Int32, ParameterDirection.Input);
                                parameters.Add("@SalesdetailId", reviewSave, DbType.Int32, ParameterDirection.Input);
                                parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
                                using (IDbConnection conn = new SqlConnection(connectionString))
                                {
                                    var categorysave = conn.ExecuteScalar("sp_Salescategory", parameters, commandType: CommandType.StoredProcedure);
                                    connection.Close();
                                }
                            }
                        }

                        connection.Close();
                    }

                 }


            }

            else
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", salesViewModel.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ClientName", salesViewModel.ClientName, DbType.String, ParameterDirection.Input);
                parameters.Add("@PhoneNumber", salesViewModel.PhoneNumber, DbType.String, ParameterDirection.Input);
                parameters.Add("@FollowUpDate", salesViewModel.FollowUpDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@CountryName", salesViewModel.CountryName, DbType.String, ParameterDirection.Input);
                //parameters.Add("@City", salesViewModel.SalesPropertyViewModel, DbType.String, ParameterDirection.Input);
                //parameters.Add("@Keywords", salesViewModel.SalesPropertyViewModel, DbType.String, ParameterDirection.Input);
                //parameters.Add("@FoodName", salesViewModel.SalesPropertyViewModel, DbType.String, ParameterDirection.Input);
                //parameters.Add("@TreatmentName", salesViewModel.SalesPropertyViewModel, DbType.String, ParameterDirection.Input);
                parameters.Add("@Remarks", salesViewModel.Remarks, DbType.String, ParameterDirection.Input);
                parameters.Add("@FollowUpCheck", salesViewModel.FollowUpCheck, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var reviewSave = connection.ExecuteScalar("sp_Sales", parameters, commandType: CommandType.StoredProcedure);
                    salesViewModel.SalesId = salesViewModel.Id.ToString();
                    connection.Close();
                }

                var parameter = new DynamicParameters();
                parameter = new DynamicParameters();
                parameter.Add("@SalesdetailId", salesViewModel.SalesId, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
                var deletelist = con.Query<SalesCategoryViewModel>("sp_Salescategory", parameter, commandType: CommandType.StoredProcedure).ToList();


                var para = new DynamicParameters();
                para = new DynamicParameters();
                para.Add("@SalesId", salesViewModel.SalesId, DbType.Int32, ParameterDirection.Input);
                para.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
                var empList = con.Query<SalesDetailsViewModel>("sp_Salesdetails", para, commandType: CommandType.StoredProcedure).ToList();


                foreach (var item in salesViewModel.SalesDetailsViewModel)
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@NicheName", item.NicheName, DbType.String, ParameterDirection.Input);
                    parameters.Add("@ListingUrl", item.ListingUrl, DbType.String, ParameterDirection.Input);
                    parameters.Add("@CompanyName", item.CompanyName, DbType.String, ParameterDirection.Input);
                    parameters.Add("@CityName", item.CityName, DbType.String, ParameterDirection.Input);
                    parameters.Add("@ReviewsPerDay", item.ReviewsPerDay, DbType.String, ParameterDirection.Input);
                    parameters.Add("@Platform", item.Platform, DbType.String, ParameterDirection.Input);
                    parameters.Add("@ReviewDate", item.ReviewDate, DbType.DateTime, ParameterDirection.Input);
                    parameters.Add("@CurrentReview", item.CurrentReview, DbType.String, ParameterDirection.Input);
                    parameters.Add("@SalesId", salesViewModel.SalesId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@RatePerReview", item.RatePerReview, DbType.String, ParameterDirection.Input);
                    parameters.Add("@EmailType", item.EmailType, DbType.String, ParameterDirection.Input);
                    parameters.Add("@OldBalance", item.OldBalance, DbType.String, ParameterDirection.Input);
                    parameters.Add("@CurrentBalance", item.CurrentBalance, DbType.String, ParameterDirection.Input);
                    parameters.Add("@Status", item.Status, DbType.String, ParameterDirection.Input);
                    parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);

                    using (IDbConnection connection = new SqlConnection(connectionString))
                    {
                        var reviewSave = connection.ExecuteScalar("sp_Salesdetails", parameters, commandType: CommandType.StoredProcedure);
                        if (reviewSave != null)
                        {
                            foreach (var category in item.CategoryViewModel)
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("@CategoryId", category, DbType.Int32, ParameterDirection.Input);
                                parameters.Add("@SalesdetailId", reviewSave, DbType.Int32, ParameterDirection.Input);
                                parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
                                using (IDbConnection conn = new SqlConnection(connectionString))
                                {
                                    var categorysave = conn.ExecuteScalar("sp_Salescategory", parameters, commandType: CommandType.StoredProcedure);
                                    connection.Close();
                                }
                            }
                        }
                        connection.Close();
                    }
                }

            }
            return View(salesViewModel);

        }

        [HttpGet]
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

        [HttpGet]
        public JsonResult GetList()
        {
            List<SalesViewModel> empList = new List<SalesViewModel>();
            try
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
                empList = con.Query<SalesViewModel>("sp_Sales", parameters, commandType: CommandType.StoredProcedure).ToList();
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
            return Json(empList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFollowUpList()
        {
            List<SalesViewModel> empList = new List<SalesViewModel>();
            try
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
                empList = con.Query<SalesViewModel>("sp_Sales", parameters, commandType: CommandType.StoredProcedure).ToList();
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
            return Json(empList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCountry()
        {
            List<SelectListItem> Country = new List<SelectListItem>() {
            new SelectListItem {Text = "Select Country", Value = "0"},
            new SelectListItem {Text = "Afghanistan", Value = "1"},
            new SelectListItem {Text = "Albania", Value = "2"},
            new SelectListItem {Text = "Algeria", Value = "3"},
            new SelectListItem {Text = "Andorra", Value = "4"},
            new SelectListItem {Text = "India", Value = "5"}};
            return Json(Country, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetCompany()
        {
            List<SelectListItem> Company = new List<SelectListItem>() {
            new SelectListItem {Text = "Select Company", Value = "0"},
            new SelectListItem {Text = "Reliance Industries Ltd.", Value = "1"},
            new SelectListItem {Text = "Indian Oil Corporation Ltd.", Value = "2"},
            new SelectListItem {Text = "Rajesh Exports Ltd.", Value = "3"},
            new SelectListItem {Text = "Tata Motors Ltd.", Value = "4"},
            new SelectListItem {Text = "Bharat Petroleum Corporation Ltd.", Value = "5"},};
            return Json(Company, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNiche()
        {
            List<SelectListItem> Niche = new List<SelectListItem>() {
            new SelectListItem {Text = "Select Niche", Value = "0"},
            new SelectListItem {Text = "Niche 1", Value = "1"},
            new SelectListItem {Text = "Niche 2", Value = "2"},
            new SelectListItem {Text = "Niche 3", Value = "3"},
            new SelectListItem {Text = "Niche 4", Value = "4"},
            new SelectListItem {Text = "Niche 5", Value = "5"},};
            return Json(Niche, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Status()
        {
            List<SelectListItem> category = new List<SelectListItem>() {
            new SelectListItem {Text = "Select Status", Value = "0"},
            new SelectListItem {Text = "Start", Value = "1"},
            new SelectListItem {Text = "Stop", Value = "2"},
            new SelectListItem {Text = "Suspended", Value = "3"},
            new SelectListItem {Text = "Permanently Close", Value = "4"},
            new SelectListItem {Text = "Temporary Close", Value = "5"},
            new SelectListItem {Text = "Bill Pending", Value = "6"},};
            return Json(category, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCity()
        {
            List<SelectListItem> city = new List<SelectListItem>() {
            new SelectListItem {Text = "Select City", Value = "0"},
            new SelectListItem {Text = "Nadiad", Value = "1"},
            new SelectListItem {Text = "Ahemdabad", Value = "2"},
            new SelectListItem {Text = "Vadodara", Value = "3"},
            new SelectListItem {Text = "Surat", Value = "4"},
            new SelectListItem {Text = "Gandhinagar", Value = "5"},};
            return Json(city, JsonRequestBehavior.AllowGet);
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

        public ActionResult GetDataById(string Id)
        {
            var id = Convert.ToInt32(Id);
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
            //var empList = con.Query<SalesViewModel>("sales", parameters, commandType: CommandType.StoredProcedure).ToList();
            //return Json(empList, JsonRequestBehavior.AllowGet);

            var reader = con.QueryMultiple("sp_Sales", parameters, commandType: CommandType.StoredProcedure);
            var saleslist = reader.Read<SalesViewModel>().ToList();
            var salesdetailslist = reader.Read<SalesDetailsViewModel>().ToList();
            var categorydetails = reader.Read<SalesCategoryViewModel>().ToList();
            var dynamiclist = new
            {
                saleslist = saleslist,
                detailslist = salesdetailslist,
                categorylist = categorydetails
            };
        
            return Json(dynamiclist, JsonRequestBehavior.AllowGet);
        }


    }
}