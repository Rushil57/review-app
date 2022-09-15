using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ItsReviewApp.ViewModels
{
    public class UserTrackingViewModel
    {
        //public int Id { get; set; }
        public string CompanyId { get; set; }
        public string WriterId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public string EmailId { get; set; }
        public string RegisterId { get; set; }
        public string UserListingUrl { get; set; }
    }
}