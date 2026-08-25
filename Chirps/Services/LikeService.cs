using Chirps.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirps.Services;

public class LikeService
{
    private readonly DataContext _context;

    public LikeService(DataContext context)
    {
        _context = context;
    }

    public async Task<bool> IsChirpLikedByUserAsync(int chirpId, int userId)
    {
        return await _context.Likes
            .AnyAsync(l => l.ChirpId == chirpId && l.UserId == userId);
    }

    public async Task<bool> ToggleLikeAsync(int chirpId, int userId)
    {
        var existingLike = await _context.Likes
          .FirstOrDefaultAsync(l => l.ChirpId == chirpId && l.UserId == userId);
        if (existingLike != null)
        {
            // Unlike
            _context.Likes.Remove(existingLike);
            await _context.SaveChangesAsync();
            return false;
        }
        else
        {

            var like = new Like
            {
                ChirpId = chirpId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            return true;
        }
    }
    public async Task<int> GetChirpLikesCountAsync(int chirpId)
    {
        return await _context.Likes
            .CountAsync(l => l.ChirpId == chirpId);
    }
}
