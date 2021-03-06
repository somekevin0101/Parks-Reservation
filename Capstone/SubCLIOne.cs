﻿using Capstone.DAL;
using Capstone.Interfaces;
using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone
{
    public class SubCLIOne
    {
        readonly string databaseConnection = System.Configuration.ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        Reservation userReservation = new Reservation();


        public void Display(Park park)
        {

            ICampgroundDAL campgroundsInPark = new CampgroundSqlDAL(databaseConnection);
            //campgroundsInPark.GetAllCampgrounds(park.ParkId);
            List<Campground> campgrounds = campgroundsInPark.GetAllCampgrounds(park.ParkId);

            Console.WriteLine("Camp ID".PadRight(10) + "Camp Name".PadRight(35) + "Camp Open Date".PadRight(20) + "Camp Closing Date".PadRight(20) + "Daily Fee".PadRight(20));
            foreach (Campground camp in campgrounds)
            {
                Console.WriteLine(camp.CampgroundId.ToString().PadRight(10) + camp.Name.PadRight(35) + camp.GetMonthName(camp.Open_From).PadRight(20) + camp.GetMonthName(camp.Open_To).PadRight(20) + camp.DailyFee.ToString("C2").PadRight(20));
            }

            Console.WriteLine();
            CampGroundChoices(campgrounds);

        }

        public void CampGroundChoices(List<Campground> campgrounds)
        {

            Console.WriteLine("Select a campground to check for availability. Please select a number");

            for (int i = 0; i < campgrounds.Count; i++)
            {
                Console.WriteLine((i + 1).ToString().PadRight(10) + campgrounds[i].Name);
            }

            int parsedUserInput = 0;

            while (parsedUserInput <= 0 || parsedUserInput > campgrounds.Count)
            {
                string userInput = Console.ReadLine();
                Int32.TryParse(userInput, out parsedUserInput);
                if (parsedUserInput <= 0 || parsedUserInput > campgrounds.Count)
                {
                    Console.WriteLine("Please enter a valid number");
                }
            }

            Campground userCampground = campgrounds[parsedUserInput - 1];
            Console.WriteLine("You Selected campground " + userCampground.Name);
            Console.WriteLine();

            DateTime startDate = DateTime.MinValue;

            while (startDate < DateTime.Today)
            {
                Console.WriteLine("Please enter a valid start date (MM/DD/YYYY)");
                Console.WriteLine();
                string userStartDate = Console.ReadLine();
                DateTime.TryParse(userStartDate, out startDate);
            }

            Console.WriteLine("Your start date is " + startDate.ToShortDateString());
            Console.WriteLine();

            DateTime endDate = DateTime.MinValue;

            while (endDate < startDate)
            {
                Console.WriteLine("Please enter a valid end date (MM/DD/YYYY)");
                Console.WriteLine();
                string userEndDate = Console.ReadLine();
                DateTime.TryParse(userEndDate, out endDate);
            }

            Console.WriteLine("Your end date is " + endDate.ToShortDateString());
            Console.WriteLine();

            IReservationDAL dal = new ReservationSqlDAL(databaseConnection);
            List<int> numberOfSites = dal.GetTotalSites(userCampground.CampgroundId);
            List<Reservation> allReservations = dal.GetAllReservations();
            List<int> openSites = dal.IsReservationOpen(startDate, endDate, allReservations, numberOfSites);

            bool isOpen = dal.IsCampgroundOpen(userCampground, startDate, endDate);

            if (isOpen)
            {
                IReservationDAL reservation = new ReservationSqlDAL(databaseConnection);

                ISiteDAL siteDAL = new SiteSqlDAL(databaseConnection);
                List<Site> availableSites = siteDAL.GetAvailableSites(userCampground.CampgroundId);

                Console.WriteLine("Available Camp Sites:");
                foreach (Site camp in availableSites)
                {
                    if (numberOfSites.Contains(camp.SiteId))
                    {
                        Console.WriteLine("Site ID: " + camp.SiteId);
                        Console.WriteLine("Site #" + camp.SiteNumber);
                        Console.WriteLine(" Max Occupancy: " + camp.MaxOccupancy);
                        Console.WriteLine(" Handicap Accessible: " + camp.yesOrNo(camp.Accessible));
                        Console.WriteLine(" Max RV Length: " + camp.RvLength.ToString());
                        Console.WriteLine(" Utilities Available: " + camp.yesOrNo(camp.HasUtilities));
                        Console.WriteLine(" Total Fee: " + (userCampground.DailyFee * Convert.ToInt32((endDate.Subtract(startDate)).TotalDays)).ToString("C2"));
                        Console.WriteLine();
                    }
                    else if (numberOfSites.Count == 0)
                    {
                        Console.WriteLine("Sorry, no camp sites are available during that time.");
                    }
                }
            }
            else
            {
                Console.WriteLine("The campground is not open during that period");
            }

            bool userInputId = false;
            while (userInputId == false)
            {
                Console.WriteLine("Please enter the Site ID for your desired site:");
                string response = Console.ReadLine();
                int userSite = 0;
                if (int.TryParse(response, out int result))
                {
                    userSite = int.Parse(response);
                }

                if (numberOfSites.Contains(userSite))
                {
                    userReservation.SiteId = userSite;
                    userInputId = true;
                }
                else
                {
                    Console.WriteLine("That was not a valid site number");
                }
            }
            Console.WriteLine("What name would you like to book this reservation under? ");
            string userName = Console.ReadLine();

            userReservation.Name = userName;
            userReservation.From_Date = startDate;
            userReservation.To_Date = endDate;
            userReservation.Create_Date = DateTime.Now;

            bool reservationSuccess = dal.CreateReservation(userReservation);
            if (reservationSuccess)
            {
                IReservationDAL dalUpdated = new ReservationSqlDAL(databaseConnection);
                List<Reservation> updatedReservations = dalUpdated.GetAllReservations();

                Console.WriteLine("Your reservation has been successfully booked!");
                
                Console.WriteLine("Your confirmation id is " + (updatedReservations.Count));
            }

        }

    }

}






