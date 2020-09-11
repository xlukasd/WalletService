using System;

namespace DataContract.V1.Response
{
    public class PlayerBalanceDto
    {
        public Guid PlayerIdentifier { get; set; }
        public decimal Balance { get; set; }
    }
}
