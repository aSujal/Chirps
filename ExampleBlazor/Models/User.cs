using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace ExampleBlazor.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required, StringLength(100, MinimumLength = 3), ]
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //public List<Ticket> Tickets { get; set; }
}

