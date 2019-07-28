using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace S.A___BookRental_LTD.Models
{
    public class BookRent
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public DateTime? SchedualEndDate { get; set; }
        public double? AditionalCharge { get; set; }

        [Required]
        public string RentalDuration { get; set; }

        [Required]
        public double RentalPrice { get; set; }
        [Required]
        public statusEnum Status { get; set; }
        public enum statusEnum
        {
            Requested,
            Approved,
            Rejected,
            Rented,
            Closed
        }
    }
}