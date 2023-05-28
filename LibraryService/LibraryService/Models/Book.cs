namespace LibraryService.Models
{
    public class Book
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public string Language { get; set; }

        public uint Pages { get; set; }

        public uint AgeLimit { get; set; }

        public uint PublicationDate { get; set; }

        public Author[] Authors { get; set; }

        public override string ToString() => $"{Title} [{Category} - {Language} - {PublicationDate} - {Pages}] [{string.Join<Author>(", ", Authors)}]";
    }
}