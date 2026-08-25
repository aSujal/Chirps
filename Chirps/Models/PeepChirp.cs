using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirps.Models;

public class PeepChirp
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int PeepId { get; set; }
    [Required]
    public int ChirpId { get; set; }
    [ForeignKey(nameof(PeepId))]
    public Peep Peep { get; set; } = null!;
    [ForeignKey(nameof(ChirpId))]
    public Chirp Chirp { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
