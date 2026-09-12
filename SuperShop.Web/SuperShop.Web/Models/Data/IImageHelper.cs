using Microsoft.AspNetCore.Http;

namespace SuperShop.Web.Data
{
    public interface IImageHelper
    {
        // Guarda o ficheiro dentro de wwwroot/images/{folder}/, com um nome
        // gerado aleatoriamente (Guid) para nunca haver colisões entre
        // ficheiros com o mesmo nome, e devolve o caminho relativo (o que se
        // grava na base de dados, no campo ImageUrl).
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
    }
}
