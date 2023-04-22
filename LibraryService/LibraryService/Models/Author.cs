namespace LibraryService.Models
{
    public class Author
    {
        public string Name { get; set; }

        public string Language { get; set; }

        public override string ToString() => $"{Name} ({Language})";
    }
}