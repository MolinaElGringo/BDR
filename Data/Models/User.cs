using System.ComponentModel.DataAnnotations;

namespace BlazinRoleGame.Data;

public class User
{
    public User(){}

    public User(int UserID, string FirstName, string? LastName)
    {
        this.UserID = UserID;
        this.FirstName = FirstName;
        this.LastName = LastName;
    }

    [Key]
    public int UserID {get; set;}

    public string FirstName {get; set;} = string.Empty;

    public string? LastName {get; set;}
}