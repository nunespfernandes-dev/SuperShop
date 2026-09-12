using SuperShop.Web.Models;

namespace SuperShop.Web.Data
{
    public interface IConverterHelper
    {
        // isNew: true quando vem do Create — o Id fica a 0 e é o SQL Server
        // (identity) que gera o Id verdadeiro. false quando vem do Edit —
        // mantém-se o Id que já existia, para o UPDATE atualizar a linha certa.
        Product ToProduct(ProductViewModel model, string imageUrl, bool isNew);

        ProductViewModel ToProductViewModel(Product product);
    }
}
