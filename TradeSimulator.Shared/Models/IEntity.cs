namespace TradeSimulator.Shared.Models
{
    public interface IEntity<TId>
    {
        TId Id { get; }
    }
}
