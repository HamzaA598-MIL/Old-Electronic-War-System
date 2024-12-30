using ElecWarSystem.Models.IModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElecWarSystem.Models
{
    [Table("OutOfCountries", Schema = "FileShare")]
    public class OutOfCountry: Outdoor, IDateLogic, ICleanNav, ICloneable
    {
        public long OutOfCountryDetailID { get; set; }
        [ForeignKey("OutOfCountryDetailID")]
        public OutOfCountryDetail OutOfCountryDetail { get; set; }

        public void CleanNav()
        {
            Tmam = null;
        }

        public object Clone()
        {
            OutOfCountry outOfCountry = new OutOfCountry(); 
            outOfCountry.OutOfCountryDetailID = this.OutOfCountryDetailID;
            outOfCountry.TmamID= this.TmamID;
            return outOfCountry;
        }

        public bool IsDateLogic()
        {
            bool result = OutOfCountryDetail.DateFrom <= Tmam.Date &&
                            OutOfCountryDetail.DateTo >= Tmam.Date;
            return result;
        }
    }
}