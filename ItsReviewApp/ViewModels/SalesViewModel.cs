using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ItsReviewApp.ViewModels
{
    public class SalesViewModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string PhoneNumber { get; set; }
        public List<SalesDetailsViewModel> SalesDetailsViewModel { get; set; }
        public string Remarks { get; set; }

        public string SalesId { get; set; }

        
        //[DisplayFormat(DataFormatString = "{0:yyyy/dd/mm}", ApplyFormatInEditMode = true)]
        public DateTime? FollowUpDate { get; set; }
        public string CompanyName { get; set; }

        public string CountryName { get; set; }

        public string CityName { get; set; }

        public string ReviewsPerDay { get; set; }

        public string ListingUrl { get; set; }
        public string NicheName { get; set; }
        public string CurrentReview { get; set; }
        public string RatePerReview { get; set; }
        public string EmailType { get; set; }
        public string OldBalance { get; set; }
        public string CurrentBalance { get; set; }
        //public string Category { get; set; }
        public string Platform { get; set; }
        public string Status { get; set; }
        public string CategoryId { get; set; }
        public bool FollowUpCheck { get; set; }

        public string RegisterId { get; set; }

        public int salescount { get; set; }
        //public string City { get; set; }
        //public string Keywords { get; set; }
        //public string FoodName { get; set; }
        //public string TreatmentName { get; set; }
        //public string Reviews { get; set; }
    }
    public class SalesDetailsViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }

        public string CityName { get; set; }

        public string ReviewsPerDay { get; set; }

        public string ListingUrl { get; set; }
        public string NicheName { get; set; }
        //[DisplayFormat(DataFormatString = "{0:yyyy/dd/mm}", ApplyFormatInEditMode = true)]
        public DateTime? ReviewDate { get; set; }
        public string CurrentReview { get; set; }

        public string RatePerReview { get; set; }
        public string EmailType { get; set; }
        public string OldBalance { get; set; }
        public string CurrentBalance { get; set; }

        public string Platform { get; set; }
      
        public string Status { get; set; }
        public List<string> CategoryViewModel { get; set; }
        public string WriterId { get; set; }
        public string Address { get; set; }
        public string Version { get; set; }

        public int Days { get; set; }
        public string UserListingUrl { get; set; }
        public Int32 TrackOrder { get; set; }
        public Int32 CompanyCount { get; set; }
        public int CompanyTotalCount { get; set; }
    } 

    public class SalesCategoryViewModel
    {
        public string CategoryId { get; set; }
        public string SalesdetailId { get; set; }
    }
}