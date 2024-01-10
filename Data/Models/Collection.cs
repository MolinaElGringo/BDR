public class Collection{
    public int CollectionId { get; set; }
    public String Abreviation { get; set; }
    public bool Complet { get; set; }

    public Collection(int CollectionId, String Abreviation, bool Complet)
    {
        this.CollectionId = CollectionId;
        this.Abreviation = Abreviation;
        this.Complet = Complet;
    }
}