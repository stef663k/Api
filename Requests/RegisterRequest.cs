using System.ComponentModel.DataAnnotations;

namespace LoginTreasureApi.Requests;

public class RegisterRequest
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string ConfirmPassword { get; set; }
    [Required]
    public DateTime Ts { get; set; }
}
