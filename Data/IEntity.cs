namespace SuperShop.Web.Data
{
    // Interface comum a todas as entidades que vão ser guardadas na base de
    // dados: todas têm, no mínimo, um Id. É isto que permite ao repositório
    // genérico trabalhar com qualquer entidade sem saber à partida qual é.
    public interface IEntity
    {
        int Id { get; set; }
    }
}
