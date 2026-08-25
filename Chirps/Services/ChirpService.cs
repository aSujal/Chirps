using Chirps.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirps.Services;

public class ChirpService
{
    private readonly DataContext _context;
    private readonly PeepService _peepService;

    public ChirpService(DataContext context, PeepService peepService)
    {
        _context = context;
        _peepService = peepService;
    }

    public async Task<List<Chirp>> GetRecentChirpsAsync(int count = 10)
    {
        return await _context.Chirps
            .Include(c => c.User)
            .Include(c => c.Likes)
            .Include(c => c.PeepChirps)
                .ThenInclude(pc => pc.Peep)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Chirp> CreateChirpAsync(string content, int userId)
    {
        if (string.IsNullOrEmpty(content))
            throw new ArgumentException("Chirp content cannot be empty.");

        if (content.Length > 123)
            throw new ArgumentException("Chirp must be 123 characters or less.");

        var chirp = new Chirp
        {
            Content = content,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Chirps.Add(chirp);
        await _context.SaveChangesAsync();

        // Extract and save peeps
        var peepTexts = _peepService.ExtractPeeps(content);
        if (peepTexts.Any())
        {
            await _peepService.AddPeepsToChirpAsync(chirp.Id, peepTexts);
        }

        return chirp;
    }

    public async Task<List<Chirp>> GetChirpsByPeepAsync(string peepText, int count = 10)
    {
        return await _context.PeepChirps
            .Include(pc => pc.Chirp)
                .ThenInclude(c => c.User)
            .Include(pc => pc.Chirp)
                .ThenInclude(c => c.Likes)
            .Include(pc => pc.Chirp)
                .ThenInclude(c => c.PeepChirps)
                    .ThenInclude(pc => pc.Peep)
            .Where(pc => pc.Peep.Text == peepText)
            .Select(pc => pc.Chirp)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> DeleteChirpAsync(int chirpId)
    {
        var chirp = await _context.Chirps
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == chirpId);
        if (chirp == null)
            return false;
        _context.Chirps.Remove(chirp);
        await _context.SaveChangesAsync();
        return true;
    }
}
