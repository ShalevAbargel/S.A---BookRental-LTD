using S.A___BookRental_LTD.Models;
using S.A___BookRental_LTD.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static S.A___BookRental_LTD.Models.BookRent;

namespace S.A___BookRental_LTD.ViewModel
{
    public class BookRentalViewModel
    {
        public int Id { get; set; }

        //book details
        public int BookId { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }

        [Range(0, 1000)]
        public int Availability { get; set; }

        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [DisplayName("Date Added")]
        [DisplayFormat(DataFormatString = "{0: MMM dd yyyy}")]
        public DateTime? DateAdded { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        [DisplayName("Publication Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        public DateTime PublicationDate { get; set; }
        public int Pages { get; set; }

        [DisplayName("Product Dimensions")]
        public string ProductDimensions { get; set; }
        public string Publisher { get; set; }


        //rental datails
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        public DateTime? StartDate { get; set; }

        [DisplayName("Actual End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        public DateTime? ActualEndDate { get; set; }

        [DisplayName("Schedual End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        public DateTime? SchedualEndDate { get; set; }

        [DisplayName("Additional Charge")]
        public double? AdditionalCharge { get; set; }
        public string RentalDuration { get; set; }

        [DisplayName("Rental Price")]
        public double RentalPrice { get; set; }      
        public string Status { get; set; }
        public double RentalPriceOneMonth { get; set; }
        public double RentalPriceSixMonth { get; set; }


        //user details
        public string UserId { get; set; }
        public string Email { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        public string Name { get { return FirstName + " " + LastName; } }

        [DisplayName("Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        public DateTime BirthDate { get; set; }
        public string actionName
        {
            get
            {
                if (Status.ToLower().Contains(SD.requestedLower))
                {
                    return "Approve";
                }
                if (Status.ToLower().Contains(SD.approvedLower))
                {
                    return "PickUp";
                }
                if (Status.ToLower().Contains(SD.rentedLower))
                {
                    return "Return";
                }
                return null;
            }
        }
    }
}