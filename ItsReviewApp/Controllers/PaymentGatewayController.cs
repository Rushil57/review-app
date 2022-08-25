using ItsReviewApp.ViewModels;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ItsReviewApp.Controllers
{
    public class PaymentGatewayController : Controller
    {
        // GET: PaymentGateway
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrder(PaymentRequestViewModel _requestData)
        {
            // Generate random receipt number for order
            Random randomObj = new Random();
            string transactionId = randomObj.Next(10000000, 100000000).ToString();
            RazorpayClient client = new Razorpay.Api.RazorpayClient("rzp_test_6xy0Ua7LmM6t4J", "0NCEviTI6eMS4AYPINnJUPzN");
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", _requestData.Amount * 100);  // Amount will in paise
            options.Add("receipt", transactionId);
            options.Add("currency", "INR");
            options.Add("payment_capture", "0"); // 1 - automatic  , 0 - manual
            //options.Add("notes", "-- You can put any notes here --");
            Order orderResponse = client.Order.Create(options);
            string orderId = orderResponse["id"].ToString();
            // Create order model for return on view
            RazorPayOrderViewModel orderModel = new RazorPayOrderViewModel
            {
                OrderId = orderResponse.Attributes["id"],
                RazorPayAPIKey = "rzp_test_6xy0Ua7LmM6t4J",
                Amount = _requestData.Amount * 100,
                Currency = "INR",
                Name = _requestData.Name,
                Email = _requestData.Email,
                //MobileNumber = _requestData.MobileNumber
            };
            // Return on PaymentPage with Order data
            //return View("PaymentPage", orderModel);
            return View("PaymentPage", orderModel);
        }

        //public ActionResult PaymentPage()
        //{
        //    return View();
        //}
        [HttpPost]
        public ActionResult Complete()
        {
            // Payment data comes in url so we have to get it from url
            // This id is razorpay unique payment id which can be use to get the payment details from razorpay server
            string paymentId = HttpContext.Request.Form["rzp_paymentid"].ToString();
            // This is orderId
            string orderId = HttpContext.Request.Form["rzp_orderid"].ToString();
            RazorpayClient client = new Razorpay.Api.RazorpayClient("rzp_test_6xy0Ua7LmM6t4J", "0NCEviTI6eMS4AYPINnJUPzN");
            Payment payment = client.Payment.Fetch(paymentId);
            // This code is for capture the payment 
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", payment.Attributes["amount"]);
            Payment paymentCaptured = payment.Capture(options);
            string amt = paymentCaptured.Attributes["amount"];
            //// Check payment made successfully
            if (paymentCaptured.Attributes["status"] == "captured")
            {
                // Create these action method
                ViewBag.Message = "Paid successfully";
                ViewBag.OrderId = paymentCaptured.Attributes["id"];
                return View("About");
            }
            else
            {
                ViewBag.Message = "Payment failed, something went wrong";
                return View("About");
            }
        }



    }
}