using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirps.Models;

public class Chirp
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(123, MinimumLength = 1, ErrorMessage = "Chirp must be 123 characters or less")]
    public string Content { get; set; } = string.Empty;
    [Required]
    [Display(Name = "Posted Date")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    public virtual ICollection<PeepChirp> PeepChirps { get; set; } = new List<PeepChirp>();
}
