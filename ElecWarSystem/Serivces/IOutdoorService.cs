using System.Collections.Generic;

namespace ElecWarSystem.Serivces
{
    internal interface IOutdoorService<TEntity, TEntityDetail>
    {
        TEntity Get(long id);
        TEntityDetail GetDetail(long id);
        List<TEntity> GetAll(int unitID);
        long GetID(TEntityDetail entityDetail);
        long AddDetail(TEntityDetail entityDetail);
        long Add(TEntity entity);
        int GetCount(long detailID);
        int getTotal(int unitID);
        int getEntered(int unitID);
        void Delete(long id);
    }
}
