using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Web.Models
{
    // Aula 23 — Modelo SÓ para a view AddProduct (não é entidade, não vai
    // para a base de dados).
    public class AddItemViewModel
    {
        // Range a começar em 1: a primeira opção da combobox
        // "(Selecione um produto...)" tem valor 0, por isso não passa na
        // validação e obriga o utilizador a escolher um produto a sério.
        [Display(Name = "Produto")]
        [Range(1, int.MaxValue, ErrorMessage = "Tem de selecionar um produto.")]
        public int ProductId { get; set; }

        [Display(Name = "Quantidade")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "A quantidade tem de ser um número positivo.")]
        public double Quantity { get; set; }

        // Itens da combobox (<select>): cada SelectListItem tem Text (nome
        // do produto) e Value (Id do produto, em string).
        public IEnumerable<SelectListItem>? Products { get; set; }
    }
}
