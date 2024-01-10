public class Hero : Personne {
    public string Power {get; set;}

    public Hero(int PersonneId, string Nom, string Prenom, int Age, string Power) : base(PersonneId, Nom, Prenom, Age){
        this.Power = Power;
    }
}