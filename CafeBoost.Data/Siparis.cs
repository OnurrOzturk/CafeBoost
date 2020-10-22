using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeBoost.Data
{
    public class Siparis
    {
        public int MasaNo { get; set; }
        public List<SiparisDetay> SiparisDetaylar { get; set; }
        public DateTime? AcilisZamani { get; set; }
        public DateTime? KapanisZamani { get; set; }
        public SiparisDurum Durum { get; set; }
        public string ToplamTutarTL => $"{ToplamTutar():0.00}₺";

        //public string ToplamTutarTL { get { return $"{ToplamTutar():0.00}TL"; } }

        public Siparis()
        {
            SiparisDetaylar = new List<SiparisDetay>();
            AcilisZamani = DateTime.Now;
        }
        public decimal ToplamTutar()
        {
            return SiparisDetaylar.Sum(x => x.Tutar());

            //decimal toplam = 0;
            //foreach (var item in SiparisDetaylar)
            //{
            //    toplam += item.Adet * item.BirimFiyat;
            //}
            //return toplam;
        }

        // Yukaridaki metot bu şekilde de tanımlanabilir
        //public decimal ToplamTutar() => SiparisDetaylar.Sum(x => x.Tutar());
    }
}
