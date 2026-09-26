using SuperShop.Web.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperShop.Web.Models
{
    // Aula 22 — Linha DEFINITIVA de uma encomenda (tabela do meio da
    // relação muitos-para-muitos Orders <-> Products).
    // Igual ao OrderDetailTemp mas SEM User: quem tem o User é a Order.
    // O EF cria sozinho a chave estrangeira OrderId (por causa da lista
    // Items que está na Order) e a ProductId (por causa do Product).
    public class OrderDetail : IEntity
    {
        public int Id { get; set; }

        [Required]
        public Product Product { get; set; } = null!;

        [Display(Name = "Preço")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Quantidade")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }

        [Display(Name = "Valor")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Value => Price * (decimal)Quantity;
    }
}
