using LibraryService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryService.Services.Impl
{
    internal class LibraryRepository : ILibraryRepositoryService
    {
        private readonly ILibraryDBContextService _context;

        public LibraryRepository(ILibraryDBContextService context)
        {
            _context = context;
        }

        public IList<Book> GetByAuthor(string author)
        {
            try
            {
                return _context
                    .Books
                    .Where(b =>
                        b.Authors.Where(a => a.Name.ToLower().Contains(author.ToLower())).Count() > 0)
                    .ToList();
            }
            catch (Exception ex)
            {
                return new List<Book>();
            }
            
        }

        public IList<Book> GetByCategory(string category)
        {
            try
            {
                return _context.Books.Where(b => b.Category.ToLower().Contains(category.ToLower())).ToList();
            }
            catch (Exception ex)
            {
                return new List<Book>();
            }
        }

        public IList<Book> GetByTitle(string title)
        {
            try
            {
                return _context.Books.Where(b => b.Title.ToLower().Contains(title.ToLower())).ToList();
            }
            catch (Exception ex)
            {
                return new List<Book>();
            }
        }
    }
}