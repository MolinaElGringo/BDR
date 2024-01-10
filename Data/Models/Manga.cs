using Microsoft.Net.Http.Headers;
using Npgsql.PostgresTypes;

public enum MGenre   {
        Shonen,
        Shojo,
        Seinen,
        Isekai,
        Josei,
        Kodomo
    }

public class Manga
{
    public int MangaId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public bool Collector { get; set; }
    public int Number { get; set; }
    public DateTime Date { get; set; }
    public MGenre Genre { get; set; }
    public int Price { get; set; }

    public Manga(int MangaId, string Title, string Author, bool Collector, int Number, DateTime Date, MGenre Genre, int Price)
    {
        this.MangaId = MangaId;
        this.Title = Title;
        this.Author = Author;
        this.Collector = Collector;
        this.Number = Number;
        this.Date = Date;
        this.Genre = Genre;
        this.Price = Price;
    }

}