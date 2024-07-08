using P02_FootballBetting.Common;
using System.ComponentModel.DataAnnotations;

namespace P02_FootballBetting.Models
{
    public class Color
    {
        public Color()
        {
            PrimaryKitTeams = new HashSet<Team>();
            SecondaryKitTeams = new HashSet<Team>();
        }
        public int ColorId { get; set; }

        [MaxLength(ValidationConstants.ColorNameMaxLength)]
        public string Name { get; set; }

        public virtual ICollection<Team> PrimaryKitTeams { get; set; }
        public virtual ICollection<Team> SecondaryKitTeams { get; set; }
    }
}
