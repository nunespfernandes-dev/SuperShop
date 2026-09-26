using SuperShop.Web.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperShop.Web.Models
{
    // Aula 22 — Linha TEMPORÁRIA de encomenda (o "carrinho").
    // Fica ligada ao User (para saber de quem é o carrinho) e ao Product.
    // Quando a encomenda for confirmada, estas linhas passam para
    // OrderDetail e são apagadas daqui.
    public class OrderDetailTemp : IEntity
    {
        public int Id { get; set; }

        [Required]
        public User User { get; set; } = null!;

        [Required]
        public Product Product { get; set; } = null!;

        [Display(Name = "Preço")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // double: permite quantidades como 3,5 L (líquidos, etc.)
        [Display(Name = "Quantidade")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }

        // Campo calculado (só tem get) -> NÃO vai para a tabela.
        [Display(Name = "Valor")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Value => Price * (decimal)Quantity;
    }
}
