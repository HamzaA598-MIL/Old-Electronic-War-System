using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElecWarSystem.Models
{
    public class Space
    {
        public string Unit { get; }
        public decimal Value { get; }
        public long SpaceInBytes { get; }
        public Space(long spaceInBytes)
        {
            this.SpaceInBytes = spaceInBytes;
            if(spaceInBytes >= 0 && spaceInBytes < 1024)
            {
                Unit = "B";
                Value = spaceInBytes;
            }
            else if(spaceInBytes >= 1024 && spaceInBytes < (1024 * 1024))
            {
                Unit = "KB";
                Value = ((decimal)spaceInBytes / 1024);
            }
            else if (spaceInBytes >= (1024 * 1024) && spaceInBytes < (1024 * 1024 * 1024))
            {
                Unit = "MB";
                Value = ((decimal)spaceInBytes / (1024 * 1024));
            }
            else if (spaceInBytes >= (1024 * 1024 * 1024))
            {
                Unit = "GB";
                Value = ((decimal)spaceInBytes / (1024 * 1024 * 1024));
            }
        }
        public override string ToString()
        {
            return String.Format($"{this.Value.ToString("0.##")}{this.Unit}");
        }
    }
}