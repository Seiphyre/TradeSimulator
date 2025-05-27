namespace TradeSimulator.Backend.Models
{
    public abstract class EntityBase : IEntity<string>
    {
        public string Id { get; private set; }

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
