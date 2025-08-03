using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExampleBlazor.Models;
public enum TransactionType
{
    [Display(Name = "Stock In")]
    StockIn = 1,

    [Display(Name = "Stock Out")]
    StockOut = 2,

    [Display(Name = "Adjustment")]
    Adjustment = 3
}


public class StockTransaction
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Transaction type is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select an article")]
    [Display(Name = "Transaction Type")]
    public int ArticleId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    [Display(Name = "Quantity")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Unit Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    [Display(Name = "Unit Price")]
    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; set; }

    [Display(Name = "Total Value")]
    [DataType(DataType.Currency)]
    public decimal TotalValue => Quantity * UnitPrice;

    [Required(ErrorMessage = "Transaction type is required")]
    [Display(Name = "Transaction Type")]
    public TransactionType Type { get; set; } = TransactionType.StockIn;

    [Required(ErrorMessage = "Date is required")]
    [Display(Name = "Transaction Date")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    [Display(Name = "Notes")]
    [DataType(DataType.MultilineText)]
    public string Notes { get; set; } = "";
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    [ForeignKey("ArticleId")]
    public virtual Article Article { get; set; } = null!;

}
