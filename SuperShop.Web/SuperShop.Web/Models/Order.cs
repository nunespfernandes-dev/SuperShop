using SuperShop.Web.Data;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Web.Models
{
    // Aula 22 — A encomenda (cabeçalho). Liga ao User (muitos-para-um) e
    // tem várias linhas OrderDetail (um-para-muitos, através de Items).
    public class Order : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Data da encomenda")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = false)]
        public DateTime OrderDate { get; set; }

        // Nullable: só é preenchida quando o admin marcar a entrega.
        [Display(Name = "Data de entrega")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = false)]
        public DateTime? DeliveryDate { get; set; }

        [Required]
        public User User { get; set; } = null!;

        // É AQUI que se faz a ligação um-para-muitos: uma Order tem uma
        // lista de OrderDetail (as linhas da encomenda).
        public IEnumerable<OrderDetail>? Items { get; set; }

        // Campos calculados (não vão para a tabela):
        [Display(Name = "Linhas")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Lines => Items == null ? 0 : Items.Count();

        [Display(Name = "Quantidade")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity => Items == null ? 0 : Items.Sum(i => i.Quantity);

        [Display(Name = "Valor")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Value => Items == null ? 0 : Items.Sum(i => i.Value);
    }
}
