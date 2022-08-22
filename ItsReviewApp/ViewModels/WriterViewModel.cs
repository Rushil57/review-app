using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ItsReviewApp.ViewModels
{
    public class WriterViewModel
    {

        //public string NicheValue { get; set; }
        //public string CountryValue { get; set; }
        //public string TypeValue { get; set; }

        public string Id { get; set; }
        public string CompanyId { get; set; }
        public string ReviewName { get; set; }

        public List<ReviewModel> Reviews { get; set; }
        public Decimal Version { get; set; }

        public bool IsActive { get; set; }

        public string CompanyName { get; set; }
        public int Mode { get; set; }
    }

    public class ReviewModel
    {
        //public string Version { get; set; }
        public string ReviewName { get; set; }
    }
}