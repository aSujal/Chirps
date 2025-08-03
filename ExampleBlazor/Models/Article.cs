using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExampleBlazor.Models;

public class Article
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Article number is required")]
    [StringLength(50, ErrorMessage = "Article number cannot exceed 50 characters")]
    [Display(Name = "Article Number")]
    public string ArticleNumber { get; set; } = "";

    [Required(ErrorMessage = "Article name is required")]
    [StringLength(200, ErrorMessage = "Article name cannot exceed 200 characters")]
    [Display(Name = "Article Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; }  = string.Empty;
  
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<StockTransaction> Transactions { get; set; } = new List<StockTransaction>();
    [NotMapped]
    [Display(Name = "Current Stock")]
    public int CurrentStock
    {
        get
        {
            if (Transactions == null || !Transactions.Any())
                return 0;

            var stockIn = Transactions.Where(t => t.Type == TransactionType.StockIn).Sum(t => t.Quantity);
            var stockOut = Transactions.Where(t => t.Type == TransactionType.StockOut).Sum(t => t.Quantity);
            var adjustments = Transactions.Where(t => t.Type == TransactionType.Adjustment).Sum(t => t.Quantity);

            return stockIn - stockOut + adjustments;
        }
    }

    [NotMapped]
    [Display(Name = "Total Value")]
    [DataType(DataType.Currency)]
    public decimal TotalValue => CurrentStock * AveragePrice;

    [NotMapped]
    [Display(Name = "Average Price")]
    [DataType(DataType.Currency)]
    public decimal AveragePrice
    {
        get
        {
            if (Transactions == null || !Transactions.Any())
                return 0;

            var transactions = Transactions.ToList();

            if (!transactions.Any())
                return 0;

            var totalQuantity = transactions.Sum(t => t.Quantity);
            var totalValue = transactions.Sum(t => t.TotalValue);

            return totalQuantity > 0 ? totalValue / totalQuantity : 0;
        }
    }
}
