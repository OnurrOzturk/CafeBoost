﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeBoost.Data
{
    [Table("Urunler")]
    public class Urun
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string UrunAd { get; set; }
        public decimal BirimFiyat { get; set; }

        public override string ToString()
        {
            return UrunAd + " " + $"({BirimFiyat:0.00}TL)";
            //return string.Format("{0} ({1:0.00})TL"UrunAd, BirimFiyat);
        }

        public virtual ICollection<SiparisDetay> SiparisDetaylar { get; set; }
    }
}
