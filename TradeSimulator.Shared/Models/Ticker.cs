using System.Xml.Linq;

namespace TradeSimulator.Shared.Models
{
    public class Ticker : EntityBase, IEquatable<Ticker>
    {
        public string DisplayName { get; set; }

        public List<Order> Orders { get; set; }

        public bool Equals(Ticker other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Id == other.Id;
        }

        public override bool Equals(object obj) => obj is Ticker state && Equals(state);

        public override int GetHashCode() => Id.GetHashCode();

        // Also important to override ToString, otherwise the object will be displayed as the full class name
        // Alternatively you can use ToStringFunc="x => x.Name" in MudSelect
        public override string ToString() => DisplayName;
    }
}
