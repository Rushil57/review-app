using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ItsReviewApp.ViewModels
{
    public class UserEmailViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string EmailId { get; set; }

        public bool EmailCheck { get; set; }
        public bool RowNextId { get; set; }
    }
}