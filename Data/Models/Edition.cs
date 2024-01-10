public class Edition{
    public int EditionId { get; set; }
    public string Name { get; set; }
    public string MaxNumber { get; set; }

    public Edition(int EditionId, string Name, string MaxNumber)
    {
        this.EditionId = EditionId;
        this.Name = Name;
        this.MaxNumber = MaxNumber;
    }
}