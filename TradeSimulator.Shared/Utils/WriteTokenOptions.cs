using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TradeSimulator.Shared.Utils
{
    public class WriteTokenOptions
    {
        public string SecretKey;
        public IEnumerable<Claim> Claims = null;
        public DateTime? ExpirationDate = null;
    }
}
