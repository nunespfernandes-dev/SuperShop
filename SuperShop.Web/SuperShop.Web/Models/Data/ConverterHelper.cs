using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    public class ConverterHelper : IConverterHelper
    {
        public Product ToProduct(ProductViewModel model, Guid imagesId, bool isNew)
        {
            return new Product
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                Price = model.Price,
                ImagesId = imagesId,
                LastPurchase = model.LastPurchase,
                LastSale = model.LastSale,
                IsAvailable = model.IsAvailable,
                Stock = model.Stock,
                User = model.User,
            };
        }

        public ProductViewModel ToProductViewModel(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImagesId = product.ImagesId,
                LastPurchase = product.LastPurchase,
                LastSale = product.LastSale,
                IsAvailable = product.IsAvailable,
                Stock = product.Stock,
                User = product.User,
            };
        }
    }
}
