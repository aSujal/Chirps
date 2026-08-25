using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace Chirps.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required, StringLength(16, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
    public string Username { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [AllowNull, StringLength(300, MinimumLength = 0)]
    public string? Description { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual ICollection<Chirp> Chirps { get; set; } = new List<Chirp>();
    public virtual ICollection<Like> LikesGiven { get; set; } = new List<Like>();
}

