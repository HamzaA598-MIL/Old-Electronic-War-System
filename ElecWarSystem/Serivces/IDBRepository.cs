namespace ElecWarSystem.Serivces
{
    public interface IDBRepository<TEntity>
    {
        TEntity Find(long? id);
        long? Add(TEntity entity);
        void Update(long? id, TEntity entity);
        void Delete(long? id);
    }
}
