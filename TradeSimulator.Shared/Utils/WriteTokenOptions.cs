using System.Security.Claims;

namespace TradeSimulator.Shared.Utils
{
    public class WriteTokenOptions
    {
        public string SecretKey;
        public IEnumerable<Claim> Claims = null;
        public DateTime? ExpirationDate = null;
    }
}
