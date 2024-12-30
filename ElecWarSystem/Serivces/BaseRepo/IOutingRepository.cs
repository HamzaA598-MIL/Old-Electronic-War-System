using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ElecWarSystem.Serivces
{
    internal interface IOutingRepository<TEntity, TEntityDetail> where TEntity : class where TEntityDetail : class
    {
        TEntity Get(Expression<Func<TEntity, bool>> match, string[] includes = null);
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> match, string[] includes = null);
        long GetID(object outdoorDetail, Expression<Func<TEntityDetail, bool>> match);
        void Add(TEntity entity);
        int GetCount(Expression<Func<TEntity, bool>> match);
        int getTotal(int unitID);
        void Delete(Expression<Func<TEntity, bool>> match, string[] includes = null);
    }
}
