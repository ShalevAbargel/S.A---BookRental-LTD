using Microsoft.AspNet.Identity;
using S.A___BookRental_LTD.Models;
using S.A___BookRental_LTD.Utility;
using S.A___BookRental_LTD.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net;

namespace S.A___BookRental_LTD.Controllers
{
    [Authorize]
    public class BookRentController : Controller
    {

        private ApplicationDbContext db;
        public BookRentController()
        {
            db = ApplicationDbContext.Create();
        }

        public ActionResult Create(string title = null, string isbn = null)
        {
            if(title != null && isbn != null)
            {
                BookRentalViewModel model = new BookRentalViewModel
                {
                    Title = title,
                    ISBN = isbn
                };
                return View(model);
            }
            return View(new BookRentalViewModel());
        }

        //post create method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookRentalViewModel bookRent)
        {
            if(ModelState.IsValid)
            {
                var email = bookRent.Email;
                var userDetails = from u in db.Users
                                  where u.Email.Equals(email)
                                  select new {u.Id};
                var isbn = bookRent.ISBN;
                Book bookSelected = db.Books.Where(b => b.ISBN == isbn).FirstOrDefault();
                var rentalDuration = bookRent.RentalDuration;
                var chargeRate = from u in db.Users
                                 //join m in db.MembershipTypes on u.MembershipTypeId equals m.Id
                                 from m in db.MembershipTypes
                                 where (u.MembershipTypeId == m.Id || u.MembershipTypeId == 3) && u.Email.Equals(email)
                                 //where u.Email.Equals(email)
                                 select new { m.ChargeRateOneMonth, m.ChargeRateSixMonth };
                var oneMonthRental = Convert.ToDouble(bookSelected.Price) * Convert.ToDouble(chargeRate.ToList()[0].ChargeRateOneMonth) / 100;
                var SixMonthRental = Convert.ToDouble(bookSelected.Price) * Convert.ToDouble(chargeRate.ToList()[0].ChargeRateSixMonth) / 100;
                double rentalPr = 0;
                if(bookRent.RentalDuration == SD.SixMonthCount)
                {
                    rentalPr = SixMonthRental;
                }
                else
                {
                    rentalPr = oneMonthRental;
                }
                BookRent modelToAddToDb = new BookRent
                {
                    BookId = bookSelected.Id,
                    RentalPrice = rentalPr,
                    SchedualEndDate = bookRent.SchedualEndDate,
                    RentalDuration = bookRent.RentalDuration,
                    Status = BookRent.statusEnum.Approved,
                    UserId = userDetails.ToList()[0].Id
                };

                bookSelected.Availability -= 1;
                db.BookRental.Add(modelToAddToDb);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: BookRent
        public ActionResult Index(int? pageNumber, string option =null,string search =null)
        {
            string userid = User.Identity.GetUserId();
            var model = from br in db.BookRental
                        join b in db.Books on br.BookId equals b.Id
                        join u in db.Users on br.UserId equals u.Id
                        select new BookRentalViewModel
                        {
                            BookId = b.Id,
                            RentalPrice = br.RentalPrice,
                            Price = b.Price,
                            Pages = b.Pages,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            BirthDate = u.BirthDate,
                            SchedualEndDate = br.SchedualEndDate,
                            Author = b.Author,
                            Availability = b.Availability,
                            DateAdded = b.DateAdded,
                            Description = b.Description,
                            Email = u.Email,
                            GenreId = b.GenreId,
                            Genre = db.Genres.Where(g => g.Id == b.GenreId).FirstOrDefault(),
                            ISBN = b.ISBN,
                            ImageUrl = b.ImageUrl,
                            ProductDimensions = b.ProductDimensions,
                            PublicationDate = b.PublicationDate,
                            Publisher = b.Publisher,
                            RentalDuration = br.RentalDuration,
                            Status = br.Status.ToString(),
                            Title = b.Title,
                            UserId = u.Id,
                            Id = br.Id,
                            StartDate = br.StartDate
                        };
            
            if(option == "email" && search.Length >0)
            {
                model = model.Where(u => u.Email.Contains(search));
            }
            if (option == "name" && search.Length > 0)
            {
                model = model.Where(u => u.FirstName.Contains(search) || u.LastName.Contains(search));
            }
            if (option == "status" && search.Length > 0)
            {
                model = model.Where(u => u.Status.Contains(search));
            }
            if (!User.IsInRole(SD.AdminUserRole))
            {
                model = model.Where(u => u.UserId.Equals(userid));
            }
            return View(model.ToList().ToPagedList(pageNumber?? 1,5));
        }
        [HttpPost]
        public ActionResult Reserve(BookRentalViewModel book)
        {
            string userid = User.Identity.GetUserId();
            Book bookToRent = db.Books.Find(book.BookId);
            double rentalPr = 0;
            if(userid != null)
            {
                var chargeRate = from u in db.Users
                                 //join m in db.MembershipTypes on u.MembershipTypeId equals m.Id
                                 //where u.Id.Equals(userid)
                                 from m in db.MembershipTypes where((u.MembershipTypeId == m.Id || u.MembershipTypeId ==3) && u.Id.Equals(userid))
                                 select new { m.ChargeRateOneMonth, m.ChargeRateSixMonth };
                if(book.RentalDuration == SD.SixMonthCount)
                {
                    rentalPr = Convert.ToDouble(bookToRent.Price) * Convert.ToDouble(chargeRate.ToList()[0].ChargeRateSixMonth) / 100;

                }
                else
                {
                    rentalPr = Convert.ToDouble(bookToRent.Price) * Convert.ToDouble(chargeRate.ToList()[0].ChargeRateOneMonth) / 100;
                }

                BookRent bookRent = new BookRent
                {
                    BookId = bookToRent.Id,
                    UserId = userid,
                    RentalDuration = book.RentalDuration,
                    RentalPrice = rentalPr,
                    Status = BookRent.statusEnum.Requested
                };
                db.BookRental.Add(bookRent);

                var bookInDb = db.Books.SingleOrDefault(c => c.Id == book.BookId);
                bookInDb.Availability -= 1;
                db.SaveChanges();
                return RedirectToAction("Index", "BookRent");
            }
            return View();
        }

        public ActionResult Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookRent bookRent = db.BookRental.Find(id);
            var model = getVMFromBookRent(bookRent);
            if(model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        //decline get
        public ActionResult Decline(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookRent bookRent = db.BookRental.Find(id);
            var model = getVMFromBookRent(bookRent);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        //decline POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Decline(BookRentalViewModel model)
        {
            if(model.Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookRent bookRent = db.BookRental.Find(model.Id);
            bookRent.Status = BookRent.statusEnum.Rejected;
            Book bookInDb = db.Books.Find(bookRent.BookId);
            bookInDb.Availability += 1;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Approve GET
        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookRent bookRent = db.BookRental.Find(id);
            var model = getVMFromBookRent(bookRent);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View("Approve",model);
        }

        //Approve POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(BookRentalViewModel model)
        {
            if (model.Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookRent bookRent = db.BookRental.Find(model.Id);
            bookRent.Status = BookRent.statusEnum.Approved;

            db.SaveChanges();

            return RedirectToAction("Index");
        }



        //PickUp GET
        public ActionResult PickUp(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookRent bookRent = db.BookRental.Find(id);
            var model = getVMFromBookRent(bookRent);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View("Approve", model);
        }

        //PickUp POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PickUp(BookRentalViewModel model)
        {
            if (model.Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookRent bookRent = db.BookRental.Find(model.Id);
            bookRent.Status = BookRent.statusEnum.Rented;
            bookRent.StartDate = DateTime.Now;
            if(bookRent.RentalDuration == SD.SixMonthCount)
            {
                bookRent.SchedualEndDate = DateTime.Now.AddMonths(Convert.ToInt32(SD.SixMonthCount));
            }
            else
            {
                bookRent.SchedualEndDate = DateTime.Now.AddMonths(Convert.ToInt32(SD.OneMonthCount));

            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }


        //return GET
        public ActionResult Return(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookRent bookRent = db.BookRental.Find(id);
            var model = getVMFromBookRent(bookRent);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View("Approve", model);
        }

        //return POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Return(BookRentalViewModel model)
        {
            if (model.Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BookRent bookRent = db.BookRental.Find(model.Id);
            bookRent.AditionalCharge = model.AdditionalCharge;
            ///////////////////////////////////////////////////////
            if(bookRent.AditionalCharge != null)
            {
                double additinal = (double)bookRent.AditionalCharge;
                bookRent.RentalPrice += additinal;
            }
            
            ///////////////////////////////////////////////////////
            bookRent.Status = BookRent.statusEnum.Closed; 
            Book bookInDb = db.Books.Find(bookRent.BookId);
            bookRent.ActualEndDate = DateTime.Now;
            bookInDb.Availability += 1;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //delete get
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookRent bookRent = db.BookRental.Find(id);
            var model = getVMFromBookRent(bookRent);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        //delete POST
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookRent bookRent = db.BookRental.Find(id);
            var bookInDb = db.Books.Where(b => b.Id.Equals(bookRent.BookId)).FirstOrDefault();
            if(!bookRent.Status.ToString().Equals("Rented"))
            {
                bookInDb.Availability += 1;
            }
            db.BookRental.Remove(bookRent);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        private BookRentalViewModel getVMFromBookRent(BookRent bookRent)
        {
            Book bookSelected = db.Books.Where(b => b.Id == bookRent.BookId).FirstOrDefault();

            var userDetails = from u in db.Users
                              where u.Id.Equals(bookRent.UserId)
                              select new { u.Id, u.FirstName, u.LastName, u.BirthDate, u.Email };

            BookRentalViewModel model = new BookRentalViewModel
            {
                Id = bookRent.Id,
                BookId = bookSelected.Id,
                RentalPrice = bookRent.RentalPrice,
                Price = bookSelected.Price,
                Pages = bookSelected.Pages,
                FirstName = userDetails.ToList()[0].FirstName,
                LastName = userDetails.ToList()[0].LastName,
                BirthDate = userDetails.ToList()[0].BirthDate,
                Email = userDetails.ToList()[0].Email,
                UserId = userDetails.ToList()[0].Id,
                SchedualEndDate = bookRent.SchedualEndDate,
                Author = bookSelected.Author,
                StartDate = bookRent.StartDate,
                Availability = bookSelected.Availability,
                DateAdded = bookSelected.DateAdded,
                Description = bookSelected.Description,
                GenreId = bookSelected.GenreId,
                //mayby change to == if int32 error apear
                Genre = db.Genres.FirstOrDefault(g => g.Id.Equals(bookSelected.GenreId)),
                ISBN = bookSelected.ISBN,
                ImageUrl = bookSelected.ImageUrl,
                ProductDimensions = bookSelected.ProductDimensions,
                PublicationDate = bookSelected.PublicationDate,
                Publisher = bookSelected.Publisher,
                RentalDuration = bookRent.RentalDuration,
                //maybe tostring will gives an error to
                Status = bookRent.Status.ToString(),
                Title = bookSelected.Title,
                AdditionalCharge = bookRent.AditionalCharge

            };
            return model;
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