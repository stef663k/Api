namespace LoginTreasureApi.Models;

public class User
{
    public User()
    {
        RefreshTokens = new HashSet<RefreshToken>();
    }


    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string PasswordSalt { get; set; }
    public DateTime Ts { get; set; }
    public bool Active { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

}
