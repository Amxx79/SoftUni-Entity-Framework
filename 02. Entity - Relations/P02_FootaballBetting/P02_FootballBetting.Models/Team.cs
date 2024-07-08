using P02_FootballBetting.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [MaxLength(ValidationConstants.TeamNameMaxLength)]
        [Required]
        public string Name { get; set; }

        public string LogoUrl { get; set; }

        [MaxLength(ValidationConstants.InitialMaxLength)]
        public string Initials { get; set; }

        public decimal Budget { get; set; }

        public int PrimaryColorId { get; set; }
        [ForeignKey(nameof(PrimaryColorId))]
        public virtual Color PrimaryColor { get; set; }

        public int SecondaryColorId { get; set; }
        [ForeignKey(nameof(SecondaryColorId))]
        public virtual Color SecondaryColor { get; set; }

        public int TownId { get; set; }
        [ForeignKey(nameof(TownId))]
        public Town Town { get; set; }

        public ICollection<Player> Players { get; set; }
        public ICollection<Game> HomeGame { get; set; }
        public ICollection<Game> AwayGame { get; set; }

    }
}
