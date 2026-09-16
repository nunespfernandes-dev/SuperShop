using SuperShop.Web.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperShop.Web.Models
{

    public class Product : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "O campo {0} não pode ter mais do que {1} caracteres.")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Preço")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Já não guardamos aqui um caminho (string) para a imagem. Guardamos
        // só o Guid que identifica o blob dentro do contentor "products" no
        // Azure Blob Storage (Guid.Empty quando o produto não tem imagem
        // nenhuma associada).
        [Display(Name = "Imagem")]
        public Guid ImagesId { get; set; }

        [Display(Name = "Última Compra")]
        [DataType(DataType.Date)]
        public DateTime? LastPurchase { get; set; }

        [Display(Name = "Última Venda")]
        [DataType(DataType.Date)]
        public DateTime? LastSale { get; set; }

        [Display(Name = "Disponível")]
        public bool IsAvailable { get; set; }

        [Display(Name = "Stock")]
        public float Stock { get; set; }

        // Utilizador que criou/é dono deste produto. Nullable porque, por
        // agora, pode haver produtos antigos sem utilizador associado — é
        // sempre preenchido a partir do Create/Edit no controlador.
        [Display(Name = "Utilizador")]
        public User? User { get; set; }

        // Caminho a usar nas views para mostrar a imagem: se não houver
        // nenhuma (ImagesId vazio), usa a imagem estática local de
        // "sem imagem"; caso contrário, vai buscar o blob ao contentor
        // "products" no Storage indicado por blobBaseUrl
        // (appsettings.json, chave "Blob:BaseUrl").
        public string GetImageFullPath(string? blobBaseUrl)
        {
            if (ImagesId == Guid.Empty)
            {
                return "/images/noimage.png";
            }

            return $"{blobBaseUrl?.TrimEnd('/')}/products/{ImagesId}";
        }
    }
}
