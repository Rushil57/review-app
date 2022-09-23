using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ItsReviewApp.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string RegisterId { get; set; }
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<string> UserCategoryViewModel { get; set; }
        public List<UserDetailsViewModel> Users { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public int EmailCount { get; set; }

        public int EmailTotalCount { get; set; }
    }

    public class UserDetailsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        
       // public string Category { get; set; }
        public List<string> UserCategoryViewModel { get; set; }
        //public string Platform { get; set; }
    }

    public class UserCategoryViewModel
    {
        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
    }
}