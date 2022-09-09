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
using System.Web;
using System.Web.Mvc;

namespace ItsReviewApp.Controllers
{
    public class UserController : Controller
    {

        string connectionString = ConfigurationManager.ConnectionStrings["DbEntities"].ToString();
        SqlConnection con;
        List<string> EmailList = new List<string>();
        public UserController()
        {
            con = new SqlConnection(connectionString);
        }

        // GET: Role
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Create()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
            var userList = con.Query<RegisterViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).ToList();
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            foreach (var user in userList)
            {
                var item = new SelectListItem { Text = user.Name, Value = user.Id.ToString() };
                selectListItems.Add(item);
            }
            ViewBag.Name = selectListItems;

            return View();
        }

        [HttpPost]
        public ActionResult Create(UserViewModel userViewModel)
        {
            SaveUserEmail(userViewModel);
            return Json(EmailList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Review()
        {
            return View();
        }


        public JsonResult category()
        {
            List<CategoryViewModel> CategoryList = new List<CategoryViewModel>();
            var parameter = new DynamicParameters();
            parameter.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
            CategoryList = con.Query<CategoryViewModel>("sp_Salesdetails", parameter, commandType: CommandType.StoredProcedure).ToList();
            List<SelectListItem> ListItems = new List<SelectListItem>();
            foreach (var emp in CategoryList)
            {
                var item = new SelectListItem { Text = emp.CategoryName, Value = emp.Id.ToString() };
                ListItems.Add(item);
            }
            ViewBag.Category = ListItems;
            return Json(ListItems, JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        public JsonResult GetList()
        {
            List<UserViewModel> RoleList = new List<UserViewModel>();
            try
            {
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Mode", 3, DbType.Int32, ParameterDirection.Input);
                RoleList = con.Query<UserViewModel>("sp_User", parameters, commandType: CommandType.StoredProcedure).ToList();

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

        public JsonResult GetPaltform()
        {
            List<SelectListItem> palform = new List<SelectListItem>() {

                new SelectListItem {Text = "Google", Value = "1"},
                new SelectListItem {Text = "TripAdvisor", Value = "2"},
                new SelectListItem {Text = "FaceBook", Value = "3"},};
            return Json(palform, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Upload()
        {
            string RegisterId = Request.Form["RegisterId"];
            if (Request.Files.Count > 0)
            {
                //string id = Request.Files.(x => x.Key == "id").FirstOrDefault().Value;
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
                        UserViewModel userViewModel = new UserViewModel();
                        IEnumerable<CategoryViewModel> categories = new List<CategoryViewModel>();

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
                        var parameter = new DynamicParameters();
                        parameter.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
                        using (var conn = new SqlConnection(connectionString))
                        {
                            categories = con.Query<CategoryViewModel>("sp_Salesdetails", parameter, commandType: CommandType.StoredProcedure);
                        }

                        for (int i = 0; i < tmp.Rows.Count; i++)
                        {
                            UserDetailsViewModel userDetailsViewModel = new UserDetailsViewModel();
                            UserCategoryViewModel userCategoryViewModel = new UserCategoryViewModel();
                            List<UserDetailsViewModel> userDetailsViewModelList = new List<UserDetailsViewModel>();
                            userDetailsViewModel.FirstName = tmp.Rows[i][0].ToString().Trim();
                            //userViewModel.Name = tmp.Rows[i][0].ToString();
                            //userDetailsViewModel.LastName = tmp.Rows[i][1].ToString();
                            userDetailsViewModel.EmailId = tmp.Rows[i][1].ToString().Trim();
                            var categoryList = tmp.Rows[i][2].ToString().Split(',');
                            List<string> colums = new List<string>();
                            foreach (var item in categoryList)
                            {
                                var catName = categories.ToList().Where(x => x.CategoryName.ToLower().Trim() == item.ToLower().Trim()).FirstOrDefault();
                                //var category= categories.whe
                                if (catName != null)
                                {
                                    colums.Add(catName.Id.ToString());
                                }
                            }
                            userDetailsViewModel.UserCategoryViewModel = colums;
                            userDetailsViewModelList.Add(userDetailsViewModel);
                            userViewModel.Users = userDetailsViewModelList;
                            userViewModel.RegisterId = RegisterId;
                            SaveUserEmail(userViewModel);
                        }
                        //return Json("Upload Successfully");
                        return Json(EmailList, JsonRequestBehavior.AllowGet);


                    }

                }
                return View();
            }
            else
            {
                return Json("Please Upload Your file");
            }
        }

        public ActionResult SaveUserEmail(UserViewModel userViewModel)
        {
            //List<string> EmailList = new List<string>();
            foreach (var item in userViewModel.Users)
            {
                var parameter = new DynamicParameters();
                parameter.Add("@RegisterId", userViewModel.RegisterId, DbType.String, ParameterDirection.Input);
                parameter.Add("@Name", userViewModel.Name, DbType.String, ParameterDirection.Input);
                parameter.Add("@FirstName", item.FirstName, DbType.String, ParameterDirection.Input);
                parameter.Add("@LastName", item.LastName, DbType.String, ParameterDirection.Input);
                parameter.Add("@EmailId", item.EmailId, DbType.String, ParameterDirection.Input);
                //parameter.Add("@Category", roleViewModel.Category, DbType.String, ParameterDirection.Input);
                parameter.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
                using (var conn = new SqlConnection(connectionString))
                {
                    var userSave = conn.ExecuteScalar("sp_User", parameter, commandType: CommandType.StoredProcedure);
                    if (userSave == null)
                    {
                        EmailList.Add(item.EmailId);
                    }
                    else
                    {
                        foreach (var category in item.UserCategoryViewModel)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("@CategoryId", category, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@UserId", userSave, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
                            using (IDbConnection connection = new SqlConnection(connectionString))
                            {
                                var categorysave = connection.ExecuteScalar("sp_UserCategory", parameters, commandType: CommandType.StoredProcedure);
                                con.Close();
                            }
                        }
                    }
                    conn.Close();
                }

            }
            return Json(EmailList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Email()
        {
            con.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@Mode", 4, DbType.Int32, ParameterDirection.Input);
            var companyList = con.Query<SalesViewModel>("sp_Writer", parameters, commandType: CommandType.StoredProcedure).ToList();
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            foreach (var emp in companyList)
            {
                var item = new SelectListItem { Text = emp.CompanyName, Value = emp.Id.ToString() };
                selectListItems.Add(item);
            }
            ViewBag.Company = selectListItems;
            con.Close();
            return View();
        }

        public ActionResult TrackingEmailUpload()
        {
            var CompanyId = Request.Form["CompanyId"];
            if (Request.Files.Count > 0)
            {
                //string id = Request.Files.(x => x.Key == "id").FirstOrDefault().Value;
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

                        for (int i = 0; i < tmp.Rows.Count; i++)
                        {
                            var UserId = (dynamic)null;
                            var UserRegisterId = (dynamic)null;
                            UserViewModel userViewModel = new UserViewModel();
                            UserTrackingViewModel userTrackingViewModel = new UserTrackingViewModel();
                            List<UserTrackingViewModel> userTrackingViewModelList = new List<UserTrackingViewModel>();
                            userTrackingViewModel.EmailId = tmp.Rows[i][0].ToString();
                            if(userTrackingViewModel.EmailId != null)
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("@EmailId", userTrackingViewModel.EmailId, DbType.String, ParameterDirection.Input);
                                parameters.Add("@Mode", 2, DbType.Int32, ParameterDirection.Input);
                                using (IDbConnection connection = new SqlConnection(connectionString))
                                {
                                    userViewModel = con.Query<UserViewModel>("sp_UserTracking", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                                    if (userViewModel != null)
                                    {
                                        UserId = userViewModel.Id;
                                        UserRegisterId = userViewModel.RegisterId;
                                    }  
                                }
                            }
                            userTrackingViewModel.UserId = UserId;
                            userTrackingViewModel.RegisterId = UserRegisterId;
                            userTrackingViewModel.CompanyId = CompanyId;
                            userTrackingViewModel.Status = "Existing Record";
                            userTrackingViewModelList.Add(userTrackingViewModel);
                            SaveTrackingData(userTrackingViewModel);
                        }
                        return Json(EmailList, JsonRequestBehavior.AllowGet);


                    }

                }
                return View();
            }
            else
            {
                return Json("Please Upload Your file");
            }
        }


        [HttpPost]
        public ActionResult SaveTrackingData(UserTrackingViewModel userTrackingViewModel)
        {
            var trackdata = (dynamic)null;
            var parameters = new DynamicParameters();
           // int registerId = 0;
            if (Session["RegisterId"] != null)
            {
                //registerId = Convert.ToInt32(Session["RegisterId"]);
                userTrackingViewModel.RegisterId = Session["RegisterId"].ToString();
            }
            parameters.Add("@CompanyId", userTrackingViewModel.CompanyId, DbType.String, ParameterDirection.Input);
            parameters.Add("@WriterId", userTrackingViewModel.WriterId, DbType.String, ParameterDirection.Input);
            parameters.Add("@UserId", userTrackingViewModel.UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("@Status", userTrackingViewModel.Status, DbType.String, ParameterDirection.Input);
            parameters.Add("@EmailId", userTrackingViewModel.EmailId, DbType.String, ParameterDirection.Input);
            //parameters.Add("@RegisterId", registerId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@RegisterId", userTrackingViewModel.RegisterId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Mode", 1, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                trackdata = connection.ExecuteScalar("sp_UserTracking", parameters, commandType: CommandType.StoredProcedure);
                if (trackdata == "FALSE")
                {
                    EmailList.Add(userTrackingViewModel.EmailId);
                }
                else
                {
                    connection.Close();
                }
               
            }

            //return View();
            // return RedirectToAction("Create", "Register");
            return Json(EmailList, JsonRequestBehavior.AllowGet);
        }


    }
}