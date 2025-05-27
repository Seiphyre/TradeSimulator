namespace TradeSimulator.Server.Models
{
    public interface IEntity<TId>
    {
        TId Id { get; }
    }
}
