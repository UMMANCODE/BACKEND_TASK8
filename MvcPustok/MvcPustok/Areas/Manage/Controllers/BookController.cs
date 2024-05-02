using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPustok.Areas.Manage.ViewModels;
using MvcPustok.Data;
using MvcPustok.Helpers;
using MvcPustok.Models;

namespace MvcPustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BookController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int page = 1)
        {
            var query = _context.Books.Include(x => x.Author).Include(x => x.Genre)
              .Include(x => x.BookImages.Where(x => x.Status == true)).Include(x => x.BookTags).ThenInclude(x => x.Tag);
            return View(PaginatedList<Book>.Create(query, page, 3));
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            if (book.PosterFile is null) ModelState.AddModelError("PosterFile", "PosterFile is required");
            if (book.HoverFile is null) ModelState.AddModelError("HoverFile", "HoverFile is required");

            if (!ModelState.IsValid)
            {
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                ViewBag.Tags = _context.Tags.ToList();
                return View(book);
            }
            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                return RedirectToAction("notfound", "error");
            }
            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                return RedirectToAction("notfound", "error");
            }

            foreach (var tagId in book.TagIds)
            {
                if (!_context.Tags.Any(x => x.Id == tagId)) return RedirectToAction("notfound", "error");
                BookTag bookTag = new()
                {
                    TagId = tagId,
                };
                book.BookTags.Add(bookTag);
            }

            BookImages poster = new()
            {
                Name = FileManager.Save(book.PosterFile, _env.WebRootPath, "uploads/book"),
                Status = true
            };

            BookImages hover = new()
            {
                Name = FileManager.Save(book.HoverFile, _env.WebRootPath, "uploads/book"),
                Status = false
            };

            List<BookImages> images = new();

            foreach (var item in book.ImageFiles)
            {
                images.Add(new BookImages
                {
                    Name = FileManager.Save(item, _env.WebRootPath, "uploads/book"),
                    Status = null
                });
            }

            book.BookImages = images;
            book.BookImages.Add(poster);
            book.BookImages.Add(hover);

            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("index");

        }

        public IActionResult Edit(int id)
        {
            Book book = _context.Books.Include(x => x.BookImages)
        .Include(x => x.BookTags).FirstOrDefault(x => x.Id == id);

            if (book is null) return RedirectToAction("notfound", "error");

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            book.TagIds = book.BookTags.Select(x => x.TagId).ToList();
            return View(book);
        }

        [HttpPost]
        public IActionResult Edit(Book book)
        {
            Book? existBook = _context.Books.Include(x => x.BookTags).FirstOrDefault(x => x.Id == book.Id);
            if (existBook is null) return RedirectToAction("notfound", "error");


            if (book.AuthorId != existBook.AuthorId && !_context.Authors.Any(x => x.Id == book.AuthorId))
                return RedirectToAction("notfound", "error");

            if (book.GenreId != existBook.GenreId && !_context.Genres.Any(x => x.Id == book.GenreId))
                return RedirectToAction("notfound", "error");

            existBook.BookTags.Where(x => !book.TagIds.Contains(x.TagId)).ToList().ForEach(x => _context.BookTags.Remove(x));

            book.TagIds.Where(x => !existBook.BookTags.Select(x => x.TagId).Contains(x)).ToList().ForEach(tagId =>
            {
                if (!_context.Tags.Any(x => x.Id == tagId)) return;
                _context.BookTags.Add(new BookTag
                {
                    TagId = tagId,
                    BookId = existBook.Id,
                });
            });


            existBook.Name = book.Name;
            existBook.Desc = book.Desc;
            existBook.SalePrice = book.SalePrice;
            existBook.CostPrice = book.CostPrice;
            existBook.DiscountPercent = book.DiscountPercent;
            existBook.IsNew = book.IsNew;
            existBook.IsFeatured = book.IsFeatured;
            existBook.StockStatus = book.StockStatus;

            existBook.ModifiedAt = DateTime.UtcNow;

            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            Book book = _context.Books.Include(x => x.BookImages).FirstOrDefault(b => b.Id == id);
            if (book is null) return RedirectToAction("notfound", "error");

            foreach (var item in book.BookImages)
            {
                _ = FileManager.Delete(_env.WebRootPath, "uploads/book", item.Name);
            }

            book.IsDeleted = true;

            _context.Books.Remove(book);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

