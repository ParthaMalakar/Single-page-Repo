using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.DB;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var db = new MeetingEntities();
            List<Corporate_Customer_Tbl> Corporate = db.Corporate_Customer_Tbl.ToList();
            ViewBag.Corporate = Corporate;

            List<Individual_Customer_Tbl> Individual = db.Individual_Customer_Tbl.ToList();
            ViewBag.Individual = Individual;
            var productsServices = db.Products_Service_Tbl.ToList();
            ViewBag.productsServices = productsServices;
            return View();
        }
        public static string GetUnitByProductName(string Name)
        {
            using (var dbContext = new MeetingEntities())
            {
                var product = dbContext.Products_Service_Tbl.FirstOrDefault(p => p.Name == Name);

                if (product != null)
                {
                    return product.Unit;
                }
                else
                {
                    return "Unit Not Found";
                }
            }
        }

        [HttpGet]
        public JsonResult GetUnit(string Name)
        {
            var db = new MeetingEntities();

            string unit = GetUnitByProductName(Name);

            return Json(unit, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveMeetingMinutes(MeetingDataViewModel meetingData)
        {
            try
            {
                var meetingService = new MeetingService();
                meetingService.SaveMeetingData(meetingData);

                return Json(new { success = true, message = "Meeting minutes saved successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving meeting minutes", error = ex.Message });
            }
        }
    }

    public class MeetingDataViewModel
    {
        
        public string CustomerType { get; set; }
        public string MeetingDate { get; set; }
        public string MeetingTime { get; set; }
        public string MeetingPlace { get; set; }
        public string ClientAttends { get; set; }
        public string HostAttends { get; set; }
        public List<Meeting_Minutes_Details_Tbl> Products { get; set; }
    }

   



    public class MeetingService
    {
       

        public void SaveMeetingData(MeetingDataViewModel meetingData)
        {
            var db = new MeetingEntities();
            var masterRecord = new Meeting_Minutes_Master_Tbl
            {
                CustomerType = meetingData.CustomerType,
                MeetingDate = meetingData.MeetingDate, 
                MeetingTime = meetingData.MeetingTime, 
                MeetingPlace = meetingData.MeetingPlace,
                ClientAttends = meetingData.ClientAttends,
                HostAttends = meetingData.HostAttends
                
            };
            
            db.Meeting_Minutes_Master_Tbl.Add(masterRecord);
            db.SaveChanges();

          
            foreach (var product in meetingData.Products)
            {
                var detailRecord = new Meeting_Minutes_Details_Tbl
                {
                     
                    Name = product.Name,
                    Unit = product.Unit,
                    Quantity = product.Quantity 
                                                                
                };

                db.Meeting_Minutes_Details_Tbl.Add(detailRecord);
            }

            db.SaveChanges();
        }
    }
}
    


