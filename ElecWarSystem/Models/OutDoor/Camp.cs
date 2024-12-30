using ElecWarSystem.Models.IModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElecWarSystem.Models
{
    public class Camp : Outdoor, IDateLogic, ICleanNav, ICloneable
    {
        public long CampDetailID { get; set; }
        [ForeignKey("CampDetailID")]
        public CampDetail CampDetail { get; set; }

        public void CleanNav()
        {
            Tmam = null;
        }

        public object Clone()
        {
            Camp camp = new Camp();
            camp.CampDetailID = this.CampDetailID;
            camp.TmamID= this.TmamID;
            return camp;
        }

        public bool IsDateLogic()
        {
            bool result = CampDetail.DateFrom <= Tmam.Date &&
                        CampDetail.DateTo >= Tmam.Date;
            return result;
        }
    }
}