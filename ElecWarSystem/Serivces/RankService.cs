using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace ElecWarSystem.Serivces
{
    public class RankService
    {
        private readonly AppDBContext appDBContext;
        public RankService()
        {
            appDBContext = new AppDBContext();
        }
        public List<Rank> GetRanksOf(int type)
        {
            List<Rank> ranks = appDBContext.Ranks.Where(row => row.RankType == type).ToList();
            ranks = InsertDefault(ranks);
            return ranks;
        }
        public List<Rank> GetAllRanks()
        {
            List<Rank> ranks = appDBContext.Ranks.ToList();
            ranks = InsertDefault(ranks);
            return ranks;
        }
        private List<Rank> InsertDefault(List<Rank> ranks)
        {
            Rank defaultRank = new Rank { ID = 0, RankName = "", RankType = 1 };
            ranks.Insert(0, defaultRank);
            return ranks;
        }
    }
}