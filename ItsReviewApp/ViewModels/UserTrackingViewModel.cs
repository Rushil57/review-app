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
        public int TrackOrder { get; set; }
        public int TodayCount { get; set; }
        public int YesterDayCount { get; set; }
        public Int32 ExpectedReview { get; set; }
        public Int32 UseReview { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FirstName { get; set; }
        public string CompanyName { get; set; }
        public string ListingUrl { get; set; }
        public string RegisterName { get; set; }
        public string ReviewName { get; set; }
    }
}