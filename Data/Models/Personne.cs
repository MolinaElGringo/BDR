public class Personne{
    public int PersonneId {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public int Age {get; set;}

    public Personne(int PersonneId, string FirstName, string LastName, int Age)
    {
        this.PersonneId = PersonneId;
        this.FirstName = FirstName;
        this.LastName = LastName;
        this.Age = Age;
    }
}