using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SuperShop.Web.Helpers
{
    // ViewResult genérico para "entidade não encontrada": em vez de devolver
    // NotFound() (a página 404 em branco do sistema), devolve uma view
    // nossa (ex.: "ProductNotFound", "CustomerNotFound", ...) já com o
    // StatusCode 404 certo. Assim não é preciso repetir esta lógica em cada
    // controlador — só herda ViewResult e recebe o nome da view a mostrar.
    public class NotFoundViewResult : ViewResult
    {
        public NotFoundViewResult(string viewName)
        {
            ViewName = viewName;
            StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
