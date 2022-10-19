using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ItsReviewApp.ViewModels
{
    public class UserRoleProvider : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var session = HttpContext.Current.Session;
                if (session == null || string.IsNullOrEmpty(Convert.ToString(session["RegisterId"])) || 
                    string.IsNullOrEmpty(Convert.ToString(session["RoleId"])))
                {
                    session.Abandon();
                    filterContext.Result = new RedirectResult("/Login/Login");
                    return;
                }
                else
                {
                    var absolutePath = filterContext.HttpContext.Request.Url.AbsolutePath;
                    if (!string.IsNullOrEmpty(Convert.ToString(session["RegisterId"])) &&
                        Convert.ToInt32(session["RoleId"]) == 7)
                    {
                        if(absolutePath.StartsWith("/Sales/Index") || absolutePath.StartsWith("/Sales/Create"))
                        {
                            return;
                        }
                        else
                        {
                            filterContext.Result = new RedirectResult("/Login/Login");
                            return;
                        }
                    }
                    //if (!string.IsNullOrEmpty(Convert.ToString(session["RegisterId"])) &&
                    //    Convert.ToInt32(session["RoleId"]) == 7 && !absolutePath.StartsWith("/Sales/Index"))
                    //{
                    //    filterContext.Result = new RedirectResult("/Login/Login");
                    //    return;
                    //}
                    else if (!string.IsNullOrEmpty(Convert.ToString(session["RegisterId"])) &&
                        Convert.ToInt32(session["RoleId"]) == 6 && !absolutePath.StartsWith("/User/Review"))
                    {
                        filterContext.Result = new RedirectResult("/Login/Login");
                        return;
                    }
                    else if (!string.IsNullOrEmpty(Convert.ToString(session["RegisterId"])) &&
                        Convert.ToInt32(session["RoleId"]) == 3 && !absolutePath.StartsWith("/User/Create"))
                    {
                        filterContext.Result = new RedirectResult("/Login/Login");
                        return;
                    }
                    else if (!string.IsNullOrEmpty(Convert.ToString(session["RegisterId"])) &&
                       Convert.ToInt32(session["RoleId"]) == 8 && !absolutePath.StartsWith("/Admin/UserReport"))
                    {
                        filterContext.Result = new RedirectResult("/Login/Login");
                        return;
                    }
                    else if(!string.IsNullOrEmpty(Convert.ToString(session["RegisterId"])) &&
                       Convert.ToInt32(session["RoleId"]) == 2 && !absolutePath.StartsWith("/Writer/Create"))
                    {
                        filterContext.Result = new RedirectResult("/Login/Login");
                        return;
                    }
                    else if (!string.IsNullOrEmpty(Convert.ToString(session["RegisterId"])) &&
                      Convert.ToInt32(session["RoleId"]) == 8 && !absolutePath.StartsWith("/Admin/UserReport"))
                    {
                        filterContext.Result = new RedirectResult("/Login/Login");
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                filterContext.Result = new RedirectResult("/Login/Login");
                return;
            }
            //base.OnActionExecuting(filterContext);
        }

    }
}