using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MyReads.Classes;
using MyReads.Models;


namespace MyReads.Controllers
{
    public class BooksController : Controller
    {
        private MyReadsEntities db = new MyReadsEntities();
        private StatsCalculator statsCalculator = new StatsCalculator();

        // GET: Books
        public ActionResult Index(string userName, string sortorder, string currentFilter, string searchString)
        {
            //Prevent from using features when not logged in
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", null);
            }
            Debug.WriteLine(userName);
            Session["user"] = userName;

            //Prevent users from altering the url to acess other user accounts
            if(User.Identity.Name != userName)
            {
                return RedirectToRoute(new
                {
                    controller = "Books",
                    action = "Index",
                    userName = User.Identity.Name
                });
            }

            var userBooks = db.Users.Where(u => u.User_Name == userName).SelectMany(ub => ub.UserBooks);

            //if the user has books, start calculating stats
            if (userBooks.Count() > 0)
            {
                List <Books> booksList = new List<Books>();
                foreach (var userBook in userBooks)
                {
                    booksList.Add(userBook.Books);
                }
                
                statsCalculator.Books = booksList;
            }
            ViewModel index = new ViewModel();
            index.SetStats(statsCalculator.TotalPages, statsCalculator.FavouriteAuthor, statsCalculator.FavouriteGenre);
            index.UserBooks = userBooks;
            return View(index);
        }

        public ActionResult SortOrFilter(string sortorder, string currentFilter, string searchString)
        {
            Debug.WriteLine("");
            Debug.WriteLine(searchString);
            Debug.WriteLine("");
            string currentUser = Session["user"].ToString();
            ViewBag.SortBookAuthor = String.IsNullOrEmpty(sortorder) ? "Book_Author_Desc" : "";
            ViewBag.SortBookGenre = sortorder == "Book_Genre" ? "Book_Genre_Desc" : "Book_Genre";
            ViewBag.SortBookRating = sortorder == "Book_Rating" ? "Book_Rating_Desc" : "Book_Rating";

            var dbBooks = db.Users.AsEnumerable().Where(u => u.User_Name == currentUser).SelectMany(u => u.UserBooks).OrderBy(ub => ub.Books.Book_Author);
            var books = from b in dbBooks select b;

            if (searchString == null)
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Books.Book_Author.ToUpper().Contains(searchString.ToUpper()) ||
                b.Books.Categories.Category_Genre.ToUpper().Contains(searchString.ToUpper()) ||
                b.Books.Book_Title.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortorder)
            {
                case "Book_Author_Desc":
                    books = books.OrderByDescending(b => b.Books.Book_Author);
                    break;
                case "Book_Genre":
                    books = books.OrderBy(b => b.Books.Book_Genre);
                    break;
                case "Book_Genre_Desc":
                    books = books.OrderByDescending(b => b.Books.Book_Genre);
                    break;
                case "Book_Rating":
                    books = books.OrderBy(b => b.UB_Rating);
                    break;
                case "Book_Rating_Desc":
                    books = books.OrderByDescending(b => b.UB_Rating);
                    break;
                default:
                    books = books.OrderBy(b => b.Books.Book_Author);
                    break;
            }
            ViewModel index = new ViewModel();
            index.UserBooks = books.ToList();
            return View("Index", index);
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            //Prevent from using features when not logged in
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", null);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books books = db.Books.Find(id);
            if (books == null)
            {
                return HttpNotFound();
            }

            if (db.Users.FirstOrDefault(u => u.User_Name == User.Identity.Name).UserBooks.FirstOrDefault(ub => ub.UB_BookID == books.Book_ID) == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(books);
        }

        public ActionResult Register(string userName)
        {
            Debug.WriteLine(userName);
            Users newUser = new Users();
            newUser.User_Name = userName;

            if (ModelState.IsValid)
            {
                db.Users.Add(newUser);
                db.SaveChanges();
            }

            return RedirectToRoute(new
            {
                controller = "Books",
                action = "Index",
                userName = userName
            });
        }

        // GET: Books/Create
        public ActionResult Search()
        {
            //Prevent from using features when not logged in
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", null);
            }
            if(Session["search"] == null)
            {
                Session["search"] = string.Empty;
            }
            ViewBag.Search = Session["search"].ToString();
            Session["search"] = string.Empty;
            return View();
        }

        [HttpPost]
        public ActionResult Search(string search)
        {
            //Prevent from using features when not logged in
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", null);
            }
            Debug.WriteLine(search);
            Session["search"] = search;
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult AddBook(string bookData, string bookGenre)
        {
            // current user
            string currentUser = Session["user"].ToString();
            Debug.WriteLine("");
            Debug.WriteLine("current user is " + currentUser);
            Debug.WriteLine("book json object: " + bookData);
            Debug.WriteLine("");

            // instances to add
            UserBooks userBook = new UserBooks();
            Categories category = new Categories();
            // instantiate a JavaScriptSerializer
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // use the JavaScriptSerializer to deserialize json into the expected object
            userBook.Books = serializer.Deserialize<Books>(bookData);
            // connect instances
            userBook.Books.Categories = category;

            // check if book already exists in db
            Books dbBookCheck = db.Books.FirstOrDefault(b => (b.Book_Title == userBook.Books.Book_Title) && (b.Book_Author == userBook.Books.Book_Author));
            // set to existing if found
            if(dbBookCheck != null)
            {
                Debug.WriteLine("");
                Debug.WriteLine("Book already exists in database");
                userBook.Books = dbBookCheck;
                if(db.Users.FirstOrDefault(u => u.User_Name == currentUser).UserBooks.FirstOrDefault(ub => ub.UB_BookID == userBook.Books.Book_ID) != null)
                {
                    Debug.WriteLine("Book already exists in user collection");
                    return View();
                }
            }
            else
            {
                Debug.WriteLine("");
                Debug.WriteLine("new book");
                // check if genre already exists
                Categories dbCategoryCheck = db.Categories.FirstOrDefault(c => c.Category_Genre == bookGenre);
                // add if it didn't exist
                if (dbCategoryCheck == null)
                {
                    Debug.WriteLine("new genre");
                    userBook.Books.Categories.Category_Genre = bookGenre;
                }
                // if it exists, set it to the book
                else
                {
                    Debug.WriteLine("Genre already exists");
                    userBook.Books.Categories = dbCategoryCheck;
                    userBook.Books.Book_Genre = dbCategoryCheck.Category_ID;
                }
            }

            userBook.Books.Book_Description = userBook.Books.Book_Description.Replace("'", "''");

            Debug.WriteLine("");
            Debug.WriteLine(userBook.Books.Book_Title);
            Debug.WriteLine(userBook.Books.Book_Author);
            Debug.WriteLine(userBook.Books.Book_Genre);
            Debug.WriteLine(userBook.Books.Book_Description);
            Debug.WriteLine(userBook.Books.Book_Pages);
            Debug.WriteLine(userBook.Books.Book_ImageLink);
            Debug.WriteLine(userBook.Books.Book_InfoLink);
            Debug.WriteLine("");
            if (ModelState.IsValid)
            {
                db.Users.First(u => u.User_Name == currentUser).UserBooks.Add(userBook);
                db.SaveChanges();
                return View();
            }

            return View();
        }

        [HttpPost]
        public ActionResult RateBook(string rating, string bookID)
        {
            string currentUser = Session["user"].ToString();
            rating = rating.Replace(".", ",");
            if (string.IsNullOrEmpty(rating))
            {
                rating = "0";
            }
            double bookRating = Convert.ToDouble(rating);
            int userID = db.Users.First(u => u.User_Name == currentUser).User_ID;
            UserBooks bookToRate = db.UserBooks.Find(userID, Convert.ToInt32(bookID));
            Debug.WriteLine("");
            Debug.WriteLine("User: " + currentUser + " rating: " + bookRating + " Book_ID: " + bookID + " Book to rate: " + bookToRate.Books.Book_Title);
            Debug.WriteLine("");

            bookToRate.UB_Rating = bookRating;

            if (ModelState.IsValid)
            {
                db.Entry(bookToRate).State = EntityState.Modified;
                db.SaveChanges();
                return View();
            }

            return View();
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            //Prevent from using features when not logged in
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", null);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books books = db.Books.Find(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            if (db.Users.FirstOrDefault(u => u.User_Name == User.Identity.Name).UserBooks.FirstOrDefault(ub => ub.UB_BookID == books.Book_ID) == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Book_Genre = new SelectList(db.Categories, "Category_ID", "Category_Genre", books.Book_Genre);
            return View(books);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Book_ID,Book_Title,Book_Author,Book_Genre,Book_Description,Book_Pages,Book_ImageLink,Book_InfoLink")] Books books)
        {
            if (ModelState.IsValid)
            {
                db.Entry(books).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Book_Genre = new SelectList(db.Categories, "Category_ID", "Category_Genre", books.Book_Genre);
            return View(books);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books books = db.Books.Find(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            if (db.Users.FirstOrDefault(u => u.User_Name == User.Identity.Name).UserBooks.FirstOrDefault(ub => ub.UB_BookID == books.Book_ID) == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(books);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Books books = db.Books.Find(id);
            db.Books.Remove(books);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteUser(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users user = db.Users.Where(u => u.User_Name == name).First();
            if (user == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                db.Users.Remove(user);
                db.SaveChanges();
                return RedirectToAction("Index", "Books");
            }
            return View();
        }

        public JsonResult DoesUserExist(string UserName)
        {
            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  
            return Json(!db.Users.Any(x => x.User_Name == UserName), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
