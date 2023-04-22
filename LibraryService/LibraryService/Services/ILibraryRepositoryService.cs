using LibraryService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryService.Services
{
    internal interface ILibraryRepositoryService
    {
        IList<Book> GetByTitle(string title);

        IList<Book> GetByAuthor(string author);
        
        IList<Book> GetByCategory(string category);
    }
}
