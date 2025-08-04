using Chirps.Models;
using Microsoft.EntityFrameworkCore;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Chirps.Services;
public class UserStats
{
    public int ChirpCount { get; set; }
    public int LikesReceived { get; set; }
    public int LikesGiven { get; set; }
}
public class UserService
{
    private readonly DataContext _context;

    public UserService(DataContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        //add includes if needed, e.g., Tickets, Sprints
        //example //return await _context.Users.Include(u => u.Tickets).FirstOrDefaultAsync(u => u.Id == id);
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        //add includes if needed, e.g., Tickets, Sprints
        //example //return await _context.Users.Include(u => u.Tickets).FirstOrDefaultAsync(u => u.Username == username);
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    private string CreateSalt(int size = 32)
    {
        byte[] buff = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(buff);
        }

        // Return a Base64 string representation of the random number.
        return Convert.ToBase64String(buff);
    }

    public static string CreatePasswordHash(string password, string saltBase64)
    {
        var saltBytes = Convert.FromBase64String(saltBase64);
        using var deriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
        var hashBytes = deriveBytes.GetBytes(32);
        return Convert.ToBase64String(hashBytes);
    }
    public static bool VerifyPassword(string enteredPassword, string storedHashBase64, string storedSaltBase64)
    {
        var hashOfEntered = CreatePasswordHash(enteredPassword, storedSaltBase64);
        return hashOfEntered == storedHashBase64;
    }

    public async Task<bool> RegisterUserAsync(string username, string email, string password, string? description)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == username || x.Email == email);
        if (existingUser != null)
        {
            throw new Exception("Username or Email already exists.");
        }
        string salt = CreateSalt(32);
        var user = new User
        {
            Username = username,
            PasswordSalt = salt,
            Email = email,
            PasswordHash = CreatePasswordHash(password, salt),
            Description = description ?? string.Empty,
        };


        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<User?> LoginAsync(string usernameEmail, string password)
    {
        var FoundUser = await _context.Users.FirstOrDefaultAsync(x => (x.Username == usernameEmail) || (x.Email == usernameEmail));

        if (FoundUser == null)
        {
            throw new SecurityException("Invalid username.");
        }
        else if (!VerifyPassword(password, FoundUser.PasswordHash, FoundUser.PasswordSalt))
        {
            throw new SecurityException("Invalid password.");
        }

        return FoundUser;
    }

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
    public async Task<List<Chirp>> GetUserRecentChirpsAsync(int userId, int count = 5)
    {
        return await _context.Chirps
            .Where(c => c.UserId == userId)
            .Include(c => c.Likes)
            .Include(c => c.PeepChirps)
                .ThenInclude(pc => pc.Peep)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<UserStats> GetUserStatsAsync(int userId)
    {
        var chirpCount = await _context.Chirps
            .CountAsync(c => c.UserId == userId);

        var likesReceived = await _context.Likes
            .CountAsync(l => l.Chirp.UserId == userId);

        var likesGiven = await _context.Likes
            .CountAsync(l => l.UserId == userId);

        return new UserStats
        {
            ChirpCount = chirpCount,
            LikesReceived = likesReceived,
            LikesGiven = likesGiven
        };
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await GetUserByIdAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
