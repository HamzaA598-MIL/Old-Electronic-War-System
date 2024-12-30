using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ElecWarSystem.Serivces
{
    public class StorageManager
    {
        private readonly AppDBContext appDBContext;
        public StorageManager()
        {
            appDBContext = new AppDBContext();
        }
        public Space getCapacityPerUnit(int userId)
        {
            long capacityPerUnit = appDBContext.Units.Find(userId).AllowedStrogeSize;
            return new Space(capacityPerUnit);
        }
        public Space getUsedPerUnit(int userId)
        {
            long usedPerUnitActual = 0; 
            IQueryable<Email> emails = appDBContext.Emails.Include("Documents").Where(row => row.SenderUserID == userId);
            foreach(Email email in emails)
            {
                foreach (Document document in email.Documents)
                {
                    if (File.Exists(document.FilePath))
                    {
                        usedPerUnitActual += new FileInfo(document.FilePath).Length;
                    }
                }
            }
            long usedPerUnitExpected = appDBContext.Units.Find(userId).UsedStrogeSize;
            if (usedPerUnitActual != usedPerUnitExpected) 
            {
                appDBContext.Units.Find(userId).UsedStrogeSize = usedPerUnitActual;
                appDBContext.SaveChanges();
                usedPerUnitExpected = usedPerUnitActual;
            }
            return new Space(usedPerUnitExpected);
        }
        public bool increaseUsed(int unitId, long space)
        {
            long used = getUsedPerUnit(unitId).SpaceInBytes;
            long capacity = getCapacityPerUnit(unitId).SpaceInBytes;
            long avaliable = capacity - used;
            if(space > avaliable)
            {
                return false;
            }
            else
            {
                appDBContext.Units.Find(unitId).UsedStrogeSize += space;
                appDBContext.SaveChanges();
                return true;
            }
        }
        public bool decreaseUsed(int unitId, long space)
        {
            long used = getUsedPerUnit(unitId).SpaceInBytes;
            
            if (space > used)
            {
                return false;
            }
            else
            {
                appDBContext.Units.Find(unitId).UsedStrogeSize -= space;
                appDBContext.SaveChanges();
                return true;
            }
        }
    }
}