﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ItsReviewApp.ViewModels
{
    public class LeadViewModel
    {
        public int id  { get; set; }

        public string SalesId { get; set; }
        public string CompanyName { get; set; }

        public string NumberOfReview { get; set; }
        public string Rating { get; set; }
        public string NicheName { get; set; }
        public string Url { get; set; }
        public string City { get; set; }
        public string Listing { get; set; }
        public string PhoneNumber { get; set; }

        public string EmailId { get; set; }
        public string RegisterId { get; set; }
        public string ClientName { get; set; }
        public string BussinessType { get; set; }
        public string Remarks { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public bool IsActive { get; set; }

        public string Type { get; set; }

        public int leadcount { get; set; }

    }
}