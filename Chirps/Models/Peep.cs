using System.ComponentModel.DataAnnotations;

namespace Chirps.Models;

public class Peep
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(16, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Peep can only contain alphanumeric characters")]
    [Display(Name = "Peep Text")]
    public string Text { get; set; }

    // Navigation properties
    public virtual ICollection<PeepChirp> PeepChirps { get; set; } = new List<PeepChirp>();
}
