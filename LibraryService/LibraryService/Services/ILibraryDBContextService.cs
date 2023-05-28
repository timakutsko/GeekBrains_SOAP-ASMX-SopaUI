using LibraryService.Models;
using System.Collections.Generic;

namespace LibraryService.Services
{
    internal interface ILibraryDBContextService
    {
        IList<Book> Books { get; }
    }
}
