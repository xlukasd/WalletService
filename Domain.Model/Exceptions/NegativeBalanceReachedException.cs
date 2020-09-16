using System;

namespace Domain.Model.Exceptions
{
    public class NegativeBalanceReachedException : Exception
    {
        public NegativeBalanceReachedException(string message) : base(message)
        {
        }
    }
}
