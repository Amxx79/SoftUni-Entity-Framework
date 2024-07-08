using P02_FootballBetting.Common;
using P02_FootballBetting.Models;
using System.ComponentModel.DataAnnotations;

namespace P02_FootballBetting.Data.Models;
public class User
{
    public int UserId { get; set; }

    [MaxLength(ValidationConstants.UsernameMaxLength)]
    public string Username { get; set; }

    [MaxLength(ValidationConstants.PasswordMaxLength)]
    public string Password { get; set; }

    [MaxLength(ValidationConstants.EmailMaxLength)]
    public string Email { get; set; }

    [MaxLength(ValidationConstants.UsersnameMaxLength)]
    public string Name { get; set; }

    public decimal Balance { get; set; }

    public ICollection<Bet> Bets { get; set; }

}