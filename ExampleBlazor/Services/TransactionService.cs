// Services/StockTransactionService.cs
using ExampleBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace ExampleBlazor.Services
{
    public class StockTransactionService
    {
        private readonly DataContext _context;

        public StockTransactionService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<StockTransaction>> GetAllTransactionsAsync()
        {
            return await _context.StockTransactions
                .Include(t => t.Article)
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<StockTransaction>> GetTransactionsByArticleAsync(int articleId)
        {
            return await _context.StockTransactions
                .Include(t => t.Article)
                .Where(t => t.ArticleId == articleId)
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task CreateTransactionAsync(StockTransaction stockTransaction)
        {
            var article = await _context.Articles.FindAsync(stockTransaction.ArticleId);
            if (article == null)
                throw new InvalidOperationException("Article not found");


            _context.StockTransactions.Add(stockTransaction);


            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transaction = await _context.StockTransactions.FindAsync(id);
            if (transaction == null)
                return false;

            _context.StockTransactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            var articles = await _context.Articles
                .Include(a => a.Transactions)
                .ToListAsync();

            return articles.Sum(a => a.TotalValue);
        }

        public async Task<int> GetTotalArticleCountAsync()
        {
            return await _context.Articles.CountAsync();
        }

        public async Task<int> GetTotalStockQuantityAsync()
        {
            var articles = await _context.Articles
                .Include(a => a.Transactions)
                .ToListAsync();

            return articles.Sum(a => a.CurrentStock);
        }
    }
}