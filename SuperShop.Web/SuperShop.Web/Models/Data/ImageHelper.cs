namespace SuperShop.Web.Data
{
    public class ImageHelper : IImageHelper
    {
        public async Task<string> UploadImageAsync(IFormFile imageFile, string folder)
        {
            // Nome aleatório (Guid) + extensão original do ficheiro. Assim
            // dois utilizadores podem fazer upload de um ficheiro chamado
            // "foto.jpg" ao mesmo tempo sem um apagar o outro.
            var guid = Guid.NewGuid().ToString();
            var fileName = $"{guid}{Path.GetExtension(imageFile.FileName)}";

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Caminho relativo (a partir da raiz do site) — é isto que fica
            // gravado na base de dados, nunca o caminho absoluto do servidor.
            return $"/images/{folder}/{fileName}";
        }
    }
}
