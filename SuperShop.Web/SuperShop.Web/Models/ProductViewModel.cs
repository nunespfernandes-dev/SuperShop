using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Web.Models
{
    // ViewModel do Product: tudo o que o Product já tem (herda dele), mais o
    // ficheiro da imagem que vem do formulário. Isto NÃO entra na base de
    // dados como está — o IFormFile nunca é gravado; é só usado para depois
    // fazer o upload e gravar o caminho (string) no Product a sério.
    public class ProductViewModel : Product
    {
        [Display(Name = "Imagem")]
        public IFormFile? ImagesFile { get; set; }
    }
}
