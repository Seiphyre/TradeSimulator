namespace TradeSimulator.Backend.Models
{
    public interface IEntity<TId>
    {
        TId Id { get; }
    }
}
