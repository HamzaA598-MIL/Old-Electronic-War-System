using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ElecWarSystem.Serivces
{
    public class OutingService<TEntity, TEntityDetail> : IOutingRepository<TEntity, TEntityDetail> where TEntity : class where TEntityDetail : class
    {
        private readonly AppDBContext dBContext;
        private readonly PersonService personService;
        private readonly TmamService tmamService;
        public OutingService()
        {
            dBContext = new AppDBContext();
            personService = new PersonService();
            tmamService = new TmamService();
        }

        public void Add(TEntity entity)
        {
            dBContext.Set<TEntity>().Add(entity);
            dBContext.SaveChanges();
        }
        public void Delete(Expression<Func<TEntity, bool>> match, string[] includes = null)
        {
            TEntity entity = Get(match, includes);
            dBContext.Set<TEntity>().Remove(entity);
            dBContext.SaveChanges();
        }
        public TEntity Get(Expression<Func<TEntity, bool>> match, string[] includes = null)
        {
            IQueryable<TEntity> queryable = dBContext.Set<TEntity>();
            if(includes != null)
            {
                foreach (string include in includes)
                {
                    queryable = queryable.Include(include);
                }
            }
            TEntity entity = queryable.FirstOrDefault(match);
            return entity;
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> match, string[] includes = null)
        {
            IQueryable<TEntity> entities = dBContext.Set<TEntity>();

            foreach (string include in includes)
            {
                entities = entities.Include(include);
            }
            entities = entities.Where(match);
            return entities;
        }

        public int GetCount(Expression<Func<TEntity, bool>> match)
        {
            int count = dBContext.Set<TEntity>().Where(match).Count();
            return count;
        }

        public TEntityDetail GetDetail(long id)
        {
            TEntityDetail entityDetail = dBContext.Set<TEntityDetail>().Find(id);
            return entityDetail;
        }
        public long GetID(object entityDetail, Expression<Func<TEntityDetail, bool>> match)
        {
            object entityDetailTemp =
                dBContext.Set<TEntityDetail>().FirstOrDefault(match);
            return (entityDetailTemp != null) ? ((OutdoorDetail)entityDetailTemp).ID : 0;
        }
        public int getTotal(int unitID)
        {
            throw new NotImplementedException();
        }
    }
}