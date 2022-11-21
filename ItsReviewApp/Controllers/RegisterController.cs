using Dapper;
using ItsReviewApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ItsReviewApp.Controllers
{
    public class RegisterController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        //int companyId = 0;
        //bool companyResult = false;
        object emailresult;
        //int emailCount = 0;
        //int userId = -1;
        //int OrderTrackingId = 0;
        //bool recursion = false;
        //int emailUserCount = 0;
        //int emailflag = 0;
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
        public async Task<ActionResult> GetList()
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
            else
            {
                emailresult = new { user = "usernotlogin", reviews = "", company = "" };
                return Json(emailresult, JsonRequestBehavior.AllowGet);
            }
            try
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@RegisterId", registerId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Mode", 8, DbType.Int32, ParameterDirection.Input);
                userTrackingViewModel = con.Query<UserTrackingViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();

                parameters = new DynamicParameters();
                parameters.Add("@trackOrderId", userTrackingViewModel.TrackOrder, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Mode", 7, DbType.Int32, ParameterDirection.Input);
                companylist = con.Query<SalesDetailsViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                if (companylist != null && companylist.Id > 0)
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@trackOrderId", userTrackingViewModel.TrackOrder, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@CompanyId", companylist.Id, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Mode", 10, DbType.Int32, ParameterDirection.Input);
                    //var status = await con.Query<SalesDetailsViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    var statusList = await con.QueryAsync<SalesDetailsViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure);
                    var status= statusList.FirstOrDefault();
                    if (status.ReviewsPerDay == "Work Completed")
                    {
                        emailresult = new { company = "WorkCompleted" };
                        return Json(emailresult, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        companylist.Id = status.Id;
                        companylist.TrackOrder = status.TrackOrder;
                        companylist.CompanyName = status.CompanyName;
                        companylist.ListingUrl = status.ListingUrl;
                        companylist.CityName = status.CityName;
                        if (companylist != null)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("@CompanyId", companylist.Id, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@RegisterId", registerId, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@id", userTrackingViewModel.UserId, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@Mode", 5, DbType.Int32, ParameterDirection.Input);
                            var userlist1 = await con.QueryAsync<UserViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure);
                            userlist = userlist1.FirstOrDefault();
                            if (userlist != null && userlist.Error==false)
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("@Mode", 6, DbType.Int32, ParameterDirection.Input);
                                parameters.Add("@CompanyId", companylist.Id, DbType.Int32, ParameterDirection.Input);
                                review = con.Query<WriterViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                                if (review != null && review.Id>0)
                                {
                                    UserTrackingViewModel objUserTrackingViewModel = new UserTrackingViewModel();
                                    objUserTrackingViewModel.CompanyId = companylist.Id.ToString();
                                    objUserTrackingViewModel.WriterId = review.Id.ToString();
                                    objUserTrackingViewModel.EmailId = userlist.EmailId;
                                    objUserTrackingViewModel.RegisterId = registerId.ToString();
                                    objUserTrackingViewModel.UserId = userlist.Id;
                                    objUserTrackingViewModel.TrackOrder = companylist.TrackOrder;
                                    SaveTrackingData(ref objUserTrackingViewModel);
                                    if (objUserTrackingViewModel.Status == "FALSE")
                                    {
                                        emailresult = new { user = "concurrencyissue", reviews = "", company = "", userTrack = "" };
                                    }
                                    else
                                    {
                                        parameters = new DynamicParameters();
                                        parameters.Add("@CompanyId", companylist.Id, DbType.Int32, ParameterDirection.Input);
                                        parameters.Add("@@WriterId", review.Id, DbType.Int32, ParameterDirection.Input);
                                        parameters.Add("@Mode", 12, DbType.Int32, ParameterDirection.Input);
                                        var Companyvalidation = con.Query<bool>("sp_User", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                                        if (Companyvalidation)
                                        {
                                            emailresult = new { user = "concurrencyissue", reviews = "", company = "", userTrack = "" };
                                        }
                                        else
                                        {
                                            userlist.RegisterId = Convert.ToString(registerId);
                                            emailresult = new { user = userlist, reviews = review, company = companylist, userTrack = objUserTrackingViewModel };
                                        }
                                    }

                                    //emailflag = 1;
                                }
                                else
                                {
                                    if(review != null && review.Id == 0)
                                    {
                                        emailresult = new { user = "", reviews = "companylimited", company = "" };
                                    }
                                    else
                                    {
                                        emailresult = new { user = "", reviews = "reviewnotfound", company = "" };
                                    }
                                }
                            }
                            else
                            {
                                if(userlist!=null && userlist.Status== "emailnotfound")
                                {
                                    emailresult = new { user = "", reviews = "useremailnotfoundforcompany", company = "" };
                                }
                                if (userlist != null && userlist.Status == "perdayemaillimitexceed")
                                {
                                    emailresult = new { user = "", reviews = "emailperdaylimitexceed", company = "" };
                                }
                                else
                                {
                                    emailresult = new { user = "emailnotfound", reviews = "", company = "" };
                                }
                            }
                        }
                        else
                        {
                            emailresult = new { user = "nocontent", reviews = "", company = "" };
                        }
                    }
                }
                else
                {
                    emailresult = new { user = "", reviews = "companylimited", company = "" };
                }
            }
            catch (Exception ex)
            {
                con.Close();
            }
            finally
            {
                con.Close();
            }

            return Json(emailresult, JsonRequestBehavior.AllowGet);
        }

        public void SaveTrackingData(ref UserTrackingViewModel userTrackingViewModel)
        {
            //var trackdata = (dynamic)null;
            var parameters = new DynamicParameters();
            // int registerId = 0;
            parameters.Add("@TrackOrder", userTrackingViewModel.TrackOrder, DbType.String, ParameterDirection.Input);
            parameters.Add("@CompanyId", userTrackingViewModel.CompanyId, DbType.String, ParameterDirection.Input);
            parameters.Add("@WriterId", userTrackingViewModel.WriterId, DbType.String, ParameterDirection.Input);
            parameters.Add("@UserId", userTrackingViewModel.UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("@Status", userTrackingViewModel.Status, DbType.String, ParameterDirection.Input);
            parameters.Add("@EmailId", userTrackingViewModel.EmailId, DbType.String, ParameterDirection.Input);
            parameters.Add("@RegisterId", userTrackingViewModel.RegisterId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                userTrackingViewModel = con.Query<UserTrackingViewModel>("sp_UserTracking", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
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
            var mode = 0;
            if (userViewModel.Id == 0)
            {
                mode = 1;
            }
            else
            {
                mode = 4;
            }
            var parameters = new DynamicParameters();
            parameters.Add("@Id", userViewModel.Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Name", userViewModel.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@Address", userViewModel.Address, DbType.String, ParameterDirection.Input);
            parameters.Add("@PhoneNumber", userViewModel.PhoneNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("@ReferencePhoneNumber", userViewModel.ReferencePhoneNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("@EmailId", userViewModel.EmailId, DbType.String, ParameterDirection.Input);
            parameters.Add("@Password", userViewModel.Password, DbType.String, ParameterDirection.Input);
            parameters.Add("@Role", userViewModel.Role, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mode", mode, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var register = connection.ExecuteScalar("sp_Register", parameters, commandType: CommandType.StoredProcedure);
                connection.Close();
            }
            return RedirectToAction("Create", "Register");

        }

    }
}