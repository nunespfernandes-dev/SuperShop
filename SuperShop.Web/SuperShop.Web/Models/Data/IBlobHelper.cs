using Microsoft.AspNetCore.Http;

namespace SuperShop.Web.Data
{
    // Tudo o que seja upload de imagens para o Azure Blob Storage passa por
    // aqui. Para os produtos, substitui o antigo IImageHelper (que gravava
    // dentro de wwwroot): as imagens deixam de ficar no disco do servidor e
    // passam a ficar guardadas num contentor do Azure Storage.
    public interface IBlobHelper
    {
        // Recebe o ficheiro tal como vem do formulário (o caso que usamos
        // aqui, no MVC) e devolve o Guid que identifica o blob criado
        // dentro do contentor indicado.
        Task<Guid> UploadBlobAsync(IFormFile imageFile, string containerName);

        // Recebe um array de bytes — o caso, por exemplo, de uma app mobile
        // que envia a imagem já convertida em bytes em vez de um IFormFile.
        Task<Guid> UploadBlobAsync(byte[] imageFile, string containerName);
    }
}
