using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperShop.Web.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Preço")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Imagem (URL)")]
        public string ImageUrl { get; set; }

        [Display(Name = "Última Compra")]
        [DataType(DataType.Date)]
        public DateTime LastPurchase { get; set; }

        [Display(Name = "Última Venda")]
        [DataType(DataType.Date)]
        public DateTime LastSale { get; set; }

        [Display(Name = "Disponível")]
        public bool IsAvailable { get; set; }

        [Display(Name = "Stock")]
        public float Stock { get; set; }
    }
}
