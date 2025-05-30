namespace TradeSimulator.Shared.Models
{
    public abstract class EntityBase : IEntity<string>, IEquatable<EntityBase>
    {
        public string Id { get; set; }



        /* ---------------------------------------------------- */

        public EntityBase()
        {
            Id = GenerateId();
        }



        /* ---------------------------------------------------- */

        protected virtual string GenerateId()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }



        /* ---------------------------------------------------- */

        public bool Equals(EntityBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Id == other.Id;
        }

        public override bool Equals(object obj) => obj is EntityBase state && Equals(state);

        public override int GetHashCode() => Id.GetHashCode();
    }
}
