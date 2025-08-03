using ExampleBlazor.Models;
using Microsoft.EntityFrameworkCore;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace ExampleBlazor.Services;

public class ArticleService
{
    private readonly DataContext _context;

    public ArticleService(DataContext context)
    {
        _context = context;
    }
    public async Task<List<Article>> GetAllArticlesAsync()
    {
        //add includes if needed, e.g., Tickets, Sprints
        //example //return await _context.Users.Include(u => u.Tickets).ToListAsync();
        return await _context.Articles.Include(a => a.Transactions).ToListAsync();
    }
    public async Task<Article?> GetArticleByIdAsync(int id)
    {
        //add includes if needed, e.g., Tickets, Sprints
        //example //return await _context.Users.Include(u => u.Tickets).FirstOrDefaultAsync(u => u.Id == id);
        return await _context.Articles.Include(a => a.Transactions).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Article?> GetArticleByNumberAsync(string articleNumber)
    {
        //add includes if needed, e.g., Tickets, Sprints
        //example //return await _context.Users.Include(u => u.Tickets).FirstOrDefaultAsync(u => u.Username == username);
        return await _context.Articles.Include(a => a.Transactions).FirstOrDefaultAsync(u => u.ArticleNumber == articleNumber);
    }
    public async Task<bool> ArticleNumberExistsAsync(string articleNumber, int? excludeId = null)
    {
        var query = _context.Articles.Where(a => a.ArticleNumber == articleNumber);

        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task AddArticleAsync(Article article)
    {
        if (await ArticleNumberExistsAsync(article.ArticleNumber))
            throw new InvalidOperationException("Article number already exists");

        _context.Articles.Add(article);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateArticleAsync(Article article)
    {
        var articleToUpdate = await GetArticleByIdAsync(article.Id);
        if (articleToUpdate == null)
            throw new InvalidOperationException("Article not found");

        if (await ArticleNumberExistsAsync(article.ArticleNumber, article.Id))
            throw new InvalidOperationException("Article number already exists");

        _context.Articles.Update(article);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteArticleAsync(int id)
    {
        var article = await GetArticleByIdAsync(id);
        if (article != null)
        {
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<List<Article>> SearchArticlesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllArticlesAsync();

        return await _context.Articles
            .Include(a => a.Transactions)
            .Where(a => a.ArticleNumber.Contains(searchTerm) ||
                       a.Name.Contains(searchTerm) ||
                       a.Description.Contains(searchTerm))
            .OrderBy(a => a.ArticleNumber)
            .ToListAsync();
    }

    public async Task<List<Article>> GetLowStockArticlesAsync(int threshold = 10)
    {
        var articles = await GetAllArticlesAsync();
        return articles.Where(a => a.CurrentStock <= threshold).ToList();
    }

    public async Task<List<Article>> GetTopValueArticlesAsync(int count = 3)
    {
        var articles = await GetAllArticlesAsync();
        return articles
            .OrderByDescending(a => a.TotalValue)
            .Take(count)
            .ToList();
    }
}
