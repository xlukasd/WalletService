using System;
using CSharpFunctionalExtensions;
using Domain.Model;

namespace Domain
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Maybe<TEntity> GetByIdentifier(Guid guid);

        bool Save(TEntity entity);
    }
}
