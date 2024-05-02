using System;
using Microsoft.EntityFrameworkCore;
using MvcPustok.Models;

namespace MvcPustok.Data
{
	public class AppDbContext:DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { }
		public DbSet<Author> Authors { get; set; }

		public DbSet<BookImages> BookImages { get; set; }

		public DbSet<Book> Books { get; set; }

		public DbSet<BookTag> BookTags { get; set; }

		public DbSet<Genre> Genres { get; set; }

		public DbSet<Slider> Sliders { get; set; }

		public DbSet<Tag> Tags { get; set; }

		public DbSet<Feature> Features { get; set; }

		public DbSet<Setting> Settings { get; set; }
	} 
}

