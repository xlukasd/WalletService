using System;

namespace Domain.Model
{
    public abstract class Entity
    {
        public Guid Identifier { get; }

        protected Entity(Guid identifier)
        {
            Identifier = identifier;
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null || obj.GetType() != GetType())
            {
                return false;
            }

            if (ReferenceEquals(obj, this))
            {
                return true;
            }

            return Identifier == ((Entity) obj).Identifier;
        }
    }
}
