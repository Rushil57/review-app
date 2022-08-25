using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ItsReviewApp.ViewModels
{
    public class RazorPayOrderViewModel
    {
        public string OrderId { get; set; }
        public string RazorPayAPIKey { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
    }
}