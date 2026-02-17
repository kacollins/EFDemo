namespace EFDemo
{
    public class Film
    {
        public int FilmId { get; set; }
        public string Title { get; set; } = "";

        public string? Description { get; set; }

        public int ReleaseYear { get; set; }
        public int LanguageId { get; set; } = 1;
        public string Rating { get; set; } = "";
        public DateTime LastUpdate { get; set; }

        public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    }
}
