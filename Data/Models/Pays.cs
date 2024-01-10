using System.ComponentModel.DataAnnotations;

namespace BlazinRoleGame.Data;

public class Pays
{
    public Pays(){}

    public Pays(string nom)
    {
        this.Nom = nom;
    }

    [Key]
    public string Nom {get; set;}
}