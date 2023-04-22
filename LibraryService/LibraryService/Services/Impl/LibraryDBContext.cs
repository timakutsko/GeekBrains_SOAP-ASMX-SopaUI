using LibraryService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LibraryService.Services.Impl
{
    internal class LibraryDBContext : ILibraryDBContextService
    {
        private IList<Book> _libraryDB;

        public LibraryDBContext()
        {
            Initialize();
        }

        public IList<Book> Books => _libraryDB;

        private void Initialize()
        {
            _libraryDB = (List<Book>)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Properties.Resources.Books), typeof(List<Book>));
        }
    }
}