using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using S.A___BookRental_LTD.Models;
using Microsoft.AspNet.Identity;
using S.A___BookRental_LTD.Utility;
using S.A___BookRental_LTD.ViewModel;

namespace S.A___BookRental_LTD.Controllers
{
    public class BookDetailController : Controller
    {
        private ApplicationDbContext db;
        public BookDetailController()
        {
            db = ApplicationDbContext.Create();
        }
        // GET: BookDetail
        public ActionResult Index(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(u => u.Id == userId);

            var bookModel = db.Books.Include(b => b.Genre).SingleOrDefault(b => b.Id == id);
            var rentalPrice = 0.0;
            var oneMonthRental = 0.0;
            var sixMonthRental = 0.0;
         
            if (user != null && !User.IsInRole(SD.AdminUserRole))
            {
                var chargeRate = from u in db.Users
                                 //join m in db.MembershipTypes on u.MembershipTypeId equals m.Id                                                                                              
                                 from m in db.MembershipTypes where (u.MembershipTypeId == m.Id || u.MembershipTypeId ==3) && u.Id.Equals(userId)
                                 //where u.Id.Equals(userId)
                                 select new { m.ChargeRateOneMonth, m.ChargeRateSixMonth };
                oneMonthRental = Convert.ToDouble(bookModel.Price) * Convert.ToDouble(chargeRate.ToList()[0].ChargeRateOneMonth) / 100;
                sixMonthRental = Convert.ToDouble(bookModel.Price) * Convert.ToDouble(chargeRate.ToList()[0].ChargeRateSixMonth) / 100;
                if (user.Disable == true)
                {
                    oneMonthRental = 0.0;
                    sixMonthRental = 0.0;
                }

            }

            BookRentalViewModel model = new BookRentalViewModel
            {
                BookId = bookModel.Id,
                ISBN = bookModel.ISBN,
                Author = bookModel.Author,
                Availability = bookModel.Availability,
                DateAdded = bookModel.DateAdded,
                Description = bookModel.Description,
                Genre = db.Genres.FirstOrDefault(g=>g.Id.Equals(bookModel.GenreId)),
                GenreId = bookModel.GenreId,
                ImageUrl = bookModel.ImageUrl,
                Pages = bookModel.Pages,
                Price = bookModel.Price,
                PublicationDate = bookModel.PublicationDate,
                ProductDimensions = bookModel.ProductDimensions,
                Title = bookModel.Title,
                UserId = userId,
                RentalPrice = rentalPrice,
                RentalPriceOneMonth = oneMonthRental,
                RentalPriceSixMonth = sixMonthRental,
                Publisher = bookModel.Publisher
                
            };

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
           if(disposing)
            {
                db.Dispose();
            }
        }
    }
}