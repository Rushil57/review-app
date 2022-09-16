using Dapper;
using ItsReviewApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
            List<SalesViewModel> salesViewModels = new List<SalesViewModel>();


            con.Open();
            List<SelectListItem> Listing = new List<SelectListItem>() {
            new SelectListItem {Text = "Type 1", Value = "1"},
            new SelectListItem {Text = "Type 2", Value = "2"},
            new SelectListItem {Text = "Type 3", Value = "3"},
            new SelectListItem {Text = "Type 4", Value = "4"},
            new SelectListItem {Text = "General", Value = "5"},};
            ViewBag.Listing = Listing;

            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            salesViewModels = con.Query<SalesViewModel>("sp_Lead", parameters, commandType: CommandType.StoredProcedure).ToList();
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            foreach (var emp in salesViewModels)
            {
                var item = new SelectListItem { Text = emp.ClientName, Value = emp.Id.ToString() };
                selectListItems.Add(item);
            }
            ViewBag.Client = selectListItems;


            //var mail = Session["EmailId"];
            //parameters = new DynamicParameters();
            //parameters.Add("@EmailId", mail, DbType.String, ParameterDirection.Input);
            //parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
            //using (IDbConnection connection = new SqlConnection(connectionString))
            //{
            //    var getdata = connection.ExecuteScalar("sp_Lead", parameters, commandType: CommandType.StoredProcedure);
            //}
            con.Close();
            return View();

        }

        [HttpPost]
        public ActionResult Create(SalesViewModel salesViewModel)
        {
            List<string> CompanyList = new List<string>();
            try
            {
                var salesId = 0;
                if (Session["RegisterId"] != null)
                {
                    salesId = Convert.ToInt32(Session["RegisterId"]);
                }
                else
                {
                    new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                var mode = 0;
                if (salesViewModel.Id == 0)
                {
                    mode = 1;
                }
                else
                {
                    mode = 4;
                }
                var parameters = new DynamicParameters();
                parameters.Add("@Id", salesViewModel.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@ClientName", salesViewModel.ClientName, DbType.String, ParameterDirection.Input);
                parameters.Add("@PhoneNumber", salesViewModel.PhoneNumber, DbType.String, ParameterDirection.Input);
                parameters.Add("@FollowUpDate", salesViewModel.FollowUpDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@CountryName", salesViewModel.CountryName, DbType.String, ParameterDirection.Input);
                parameters.Add("@Remarks", salesViewModel.Remarks, DbType.String, ParameterDirection.Input);
                parameters.Add("@FollowUpCheck", salesViewModel.FollowUpCheck, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@SalesId", salesId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Mode", mode, DbType.Int32, ParameterDirection.Input);
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var reviewSave = connection.ExecuteScalar("sp_Sales", parameters, commandType: CommandType.StoredProcedure);
                    if (reviewSave != null)
                    {
                        salesViewModel.SalesId = reviewSave.ToString();
                    }
                    else
                    {
                        salesViewModel.SalesId = salesViewModel.Id.ToString();
                    }

                    connection.Close();
                }
                if (salesViewModel.SalesDetailsViewModel != null)
                {
                    SaveSalesDetail(salesViewModel.SalesDetailsViewModel, salesViewModel.SalesId,ref CompanyList);
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Error Message: " + ex.Message + "             StackTrace:" + ex.StackTrace);
                var filepath = Server.MapPath("~/ErrorLog/errorlog.txt");
                System.IO.File.AppendAllText(filepath, sb.ToString());
                sb.Clear();
            }
            //return View(salesViewModel);
            //return new HttpStatusCodeResult(HttpStatusCode.OK);
            return Json(CompanyList, JsonRequestBehavior.AllowGet);

        }

        public void SaveSalesDetail(List<SalesDetailsViewModel> SalesDetailsViewModel, string SalesId, ref List<string> CompanyList)
        {
            foreach (var item in SalesDetailsViewModel)
            {
                var salesdetailId = Convert.ToInt32(item.Id);
                var mode = 0;
                if (salesdetailId == 0)
                {
                    mode = 1;
                }
                else
                {
                    mode = 5;
                }
                var parameters = new DynamicParameters();
                parameters.Add("@id", salesdetailId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@NicheName", item.NicheName, DbType.String, ParameterDirection.Input);
                parameters.Add("@ListingUrl", item.ListingUrl, DbType.String, ParameterDirection.Input);
                parameters.Add("@CompanyName", item.CompanyName, DbType.String, ParameterDirection.Input);
                parameters.Add("@CityName", item.CityName, DbType.String, ParameterDirection.Input);
                parameters.Add("@ReviewsPerDay", item.ReviewsPerDay, DbType.String, ParameterDirection.Input);
                parameters.Add("@Platform", item.Platform, DbType.String, ParameterDirection.Input);
                parameters.Add("@ReviewDate", item.ReviewDate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@CurrentReview", item.CurrentReview, DbType.String, ParameterDirection.Input);
                parameters.Add("@SalesId", SalesId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@RatePerReview", item.RatePerReview, DbType.String, ParameterDirection.Input);
                parameters.Add("@EmailType", item.EmailType, DbType.String, ParameterDirection.Input);
                parameters.Add("@OldBalance", item.OldBalance, DbType.String, ParameterDirection.Input);
                parameters.Add("@CurrentBalance", item.CurrentBalance, DbType.String, ParameterDirection.Input);
                parameters.Add("@Status", item.Status, DbType.String, ParameterDirection.Input);
                parameters.Add("@Address", item.Address, DbType.String, ParameterDirection.Input);
                parameters.Add("@Mode", mode, DbType.Int32, ParameterDirection.Input);

                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var reviewSave = connection.ExecuteScalar("sp_Salesdetails", parameters, commandType: CommandType.StoredProcedure);
                    if (reviewSave == null)
                    {
                        reviewSave = salesdetailId;
                    }
                    if(reviewSave.ToString().ToUpper() == "FALSE")
                    {
                        CompanyList.Add(item.CompanyName);
                    }
                    if (mode == 5)
                    {
                        var parameter = new DynamicParameters();
                        parameter = new DynamicParameters();
                        parameter.Add("@SalesdetailId", salesdetailId, DbType.Int32, ParameterDirection.Input);
                        parameter.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
                        var deletelist = con.Query<SalesCategoryViewModel>("sp_Salescategory", parameter, commandType: CommandType.StoredProcedure).ToList();
                    }


                    if (item.CategoryViewModel != null && reviewSave.ToString().ToUpper() != "FALSE")
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
            UpdateTrackOrder();
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
                var salesId = 0;
                if (Session["RegisterId"] != null)
                {
                    salesId = Convert.ToInt32(Session["RegisterId"]);
                }
                else
                {
                    new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@SalesId", salesId, DbType.Int32, ParameterDirection.Input);
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
            new SelectListItem {Text = "Packers Movers", Value = "1"},
            new SelectListItem {Text = "Niche 1", Value = "2"},
            new SelectListItem {Text = "Niche 2", Value = "3"},
            new SelectListItem {Text = "Niche 3", Value = "4"},
            new SelectListItem {Text = "Niche 4", Value = "5"},
            new SelectListItem {Text = "Niche 5", Value = "6"},};
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

        public JsonResult GetListing()
        {
            List<SelectListItem> Listing = new List<SelectListItem>() {
            new SelectListItem {Text = "Type 1", Value = "1"},
            new SelectListItem {Text = "Type 2", Value = "2"},
            new SelectListItem {Text = "Type 3", Value = "3"},
            new SelectListItem {Text = "Type 4", Value = "4"},
            new SelectListItem {Text = "General", Value = "5"},};
            ViewBag.Listing = Listing;
            return Json(Listing, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindLeadData()
        {
            //var mail = Session["EmailId"];
            //var getdata = (dynamic)null;
            List<LeadViewModel> getdata = new List<LeadViewModel>();
            var parameters = new DynamicParameters();
            parameters = new DynamicParameters();
            //parameters.Add("@EmailId", mail, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                getdata = con.Query<LeadViewModel>("sp_Lead", parameters, commandType: CommandType.StoredProcedure).ToList();

            }
            return Json(getdata, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public ActionResult LeadCreate(LeadViewModel leadViewModel)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SalesId", leadViewModel.SalesId, DbType.String, ParameterDirection.Input);
            parameters.Add("@CompanyName", leadViewModel.CompanyName, DbType.String, ParameterDirection.Input);
            parameters.Add("@NumberOfReview", leadViewModel.NumberOfReview, DbType.String, ParameterDirection.Input);
            parameters.Add("@Rating", leadViewModel.Rating, DbType.String, ParameterDirection.Input);
            parameters.Add("@NicheName", leadViewModel.NicheName, DbType.String, ParameterDirection.Input);
            parameters.Add("@Url", leadViewModel.Url, DbType.String, ParameterDirection.Input);
            parameters.Add("@City", leadViewModel.City, DbType.String, ParameterDirection.Input);
            parameters.Add("@Listing", leadViewModel.Listing, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var leadsave = connection.ExecuteScalar("sp_Lead", parameters, commandType: CommandType.StoredProcedure);
                connection.Close();
            }
            return RedirectToAction("Create", "Sales");
        }
        public void UpdateTrackOrder()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 7, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var companyList = connection.Query<SalesDetailsViewModel>("sp_Salesdetails", parameters, commandType: CommandType.StoredProcedure).ToList();
                int i=1;
                foreach (var item in companyList)
                {
                    SalesDetailsViewModel salesDetailsViewModel = new SalesDetailsViewModel();
                    salesDetailsViewModel.Id=item.Id;
                    salesDetailsViewModel.TrackOrder=item.TrackOrder;

                    parameters = new DynamicParameters();
                    parameters.Add("@id", item.Id, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@TrackOrder", i++, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
                    connection.ExecuteScalar("sp_Salesdetails", parameters, commandType: CommandType.StoredProcedure);
                }
                connection.Close();
            }
        }
    }
}