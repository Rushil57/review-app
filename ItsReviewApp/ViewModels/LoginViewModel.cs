using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ItsReviewApp.ViewModels
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }

        public int RegisterId { get; set; }
        public string RoleName { get; set; }
    }
}