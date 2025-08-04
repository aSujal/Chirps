using Chirps.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Chirps.Services;

public class PeepService
{
    private readonly DataContext _context;

    public PeepService(DataContext context)
    {
        _context = context;
    }

    public List<string> ExtractPeeps(string content)
    {
        var peeps = new List<string>();
        var parts = content.Split('<');
        for (int i = 1; i < parts.Length; i++)
        {
            var peepText = "";
            foreach(char c in parts[i])
            {
                if (char.IsLetterOrDigit(c))
                    peepText += c;
                else
                    break;
            }

            //Validate peep
            if(peepText.Length >= 3 && peepText.Length <= 16)
            {
                peeps.Add(peepText);
            }
        }
        return peeps;
    }

    public bool IsValidPeep(string peepText)
    {
        return !string.IsNullOrEmpty(peepText) && 
               peepText.Length >= 3 && 
               peepText.Length <= 16 && 
               peepText.All(c => char.IsLetterOrDigit(c));
    }

    public async Task AddPeepsToChirpAsync(int chirpId, List<string> peepTexts)
    {
        foreach (var peepText in peepTexts)
        {
            var peep = await _context.Peeps.FirstOrDefaultAsync(p => p.Text == peepText);
            if (!IsValidPeep(peepText)) return;
            if (peep == null)
            {
                peep = new Peep { Text = peepText };
                _context.Peeps.Add(peep);
                await _context.SaveChangesAsync();
            }
            var peepChirp = new PeepChirp
            {
                ChirpId = chirpId,
                PeepId = peep.Id
            };
            _context.PeepChirps.Add(peepChirp);
        }
        await _context.SaveChangesAsync();
    }

    public async Task<List<string>> GetTrendingPeepsAsync(int count = 5)
    {
        var oneDayAgo = DateTime.UtcNow.AddDays(-1);

        return await _context.PeepChirps
            .Where(pc => pc.Chirp.CreatedAt >= oneDayAgo)
            .GroupBy(pc => pc.Peep.Text)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .Take(count)
            .ToListAsync();
    }

}
