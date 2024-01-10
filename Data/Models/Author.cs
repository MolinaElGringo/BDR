public class Author : Personne {
    public string PenName {get; set;}
    public string Nationalite {get; set;}

    public Author(int PersonneId, string Nom, string Prenom, int Age, string PenName, string Nationalite) : base(PersonneId, Nom, Prenom, Age){
        this.PenName = PenName;
        this.Nationalite = Nationalite;
    }
}