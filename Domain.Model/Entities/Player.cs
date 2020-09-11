using System;

namespace Domain.Model.Entities
{
    public class Player : Entity
    {
        public Player(Guid identifier) : base(identifier)
        {
            Wallet = new Wallet(Guid.NewGuid());
        }

        public Wallet Wallet { get; }
    }
}
