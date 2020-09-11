using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Domain.Model;

namespace Domain
{
    public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly HashSet<TEntity> _entities = new HashSet<TEntity>();

        public Maybe<TEntity> GetByIdentifier(Guid guid)
        {
            return Maybe<TEntity>.From(_entities.SingleOrDefault(x => x.Identifier == guid));
        }

        public bool Save(TEntity entity)
        {
            return _entities.Add(entity);
        }
    }
}
