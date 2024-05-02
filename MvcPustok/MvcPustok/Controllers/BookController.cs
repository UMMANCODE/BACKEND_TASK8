using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPustok.Data;
using MvcPustok.Models;

namespace MvcPustok.Controllers
{
	public class BookController:Controller
	{
		 private readonly AppDbContext _context;
		public BookController(AppDbContext context)
		{
			_context = context;
		}
		public IActionResult GetBookById(int id)
		{
			Book book = _context.Books.Include(x => x.Genre).Include(x => x.BookImages.Where(x => x.Status == true)).FirstOrDefault(x => x.Id == id);
			return PartialView("_BookModalPartial",book);
		}
	}
}
