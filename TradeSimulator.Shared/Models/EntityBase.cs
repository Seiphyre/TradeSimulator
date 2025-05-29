namespace TradeSimulator.Shared.Models
{
    public abstract class EntityBase : IEntity<string>
    {
        public string Id { get; set; }

        public EntityBase()
        {
            Id = GenerateId();
        }

        protected virtual string GenerateId()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}
