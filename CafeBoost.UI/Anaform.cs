using CafeBoost.Data;
using CafeBoost.UI.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeBoost.UI
{
    public partial class Anaform : Form
    {
        int masaAdet = 20;
        KafeVeri db = new KafeVeri();

        public Anaform()
        {
            InitializeComponent();
            OrnekUrunleriYukle();
            MasalariOlustur();
        }

        private void OrnekUrunleriYukle()
        {
            db.Urunler.Add(new Urun
            {
                UrunAd = "Kola",
                BirimFiyat = 6m
            });
            db.Urunler.Add(new Urun
            {
                UrunAd = "Ayran",
                BirimFiyat = 4m
            });
        }

        private void MasalariOlustur()
        {
            #region İmaj Listesinin Oluşturulması
            ImageList il = new ImageList();
            il.Images.Add("bos", Resources.bos);
            il.Images.Add("dolu", Resources.dolu);
            il.ImageSize = new Size(64, 64);
            lvwMasalar.LargeImageList = il;
            #endregion

            #region Masaların Oluşturulması
            ListViewItem lvi;
            for (int i = 1; i <= masaAdet; i++)
            {
                lvi = new ListViewItem("Masa " + i);
                lvi.ImageKey = "bos";
                //önemli bir satır
                lvi.Tag = i;
                lvwMasalar.Items.Add(lvi);
            } 
            #endregion
        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            new UrunlerForm().ShowDialog();
        }

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            new GecmisSiparislerForm().ShowDialog();
        }

        private void lvwMasalar_DoubleClick(object sender, EventArgs e)
        {
            int masaNo = (int)lvwMasalar.SelectedItems[0].Tag;
            Siparis siparis = AktifSiparisBul(masaNo);

            if (siparis == null)
            {
                siparis = new Siparis();
                siparis.MasaNo = masaNo;
                db.AktifSiparisler.Add(siparis);
                lvwMasalar.SelectedItems[0].ImageKey = "dolu";
            }

            SiparisForm frmSiparis = new SiparisForm(db, siparis);
            DialogResult dr = frmSiparis.ShowDialog();

            // Sipariş iptal edildiyse ya da ödeme alındıysa
            if (dr == DialogResult.OK)
            {
                lvwMasalar.SelectedItems[0].ImageKey = "bos";
            }

            // bu masa no ile ne yapacağız?   // iki ihtimalvar // ya bu masa noya ait aktif sipariş vardır   // ya da yoktur.

            // varsa => o siparişi bul getir.   // yoksa => yeni sipariş yarat  // olanı ya da yaratılanı yeni SiparisDetaylar formunda aç
        }

        private Siparis AktifSiparisBul(int masaNo)
        {
            return db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == masaNo);

            #region Foreach Yöntemi
            //foreach (Siparis item in db.AktifSiparisler)
            //{
            //    if (item.MasaNo == masaNo)
            //    {
            //        return item;
            //    }
            //}
            //return null; 
            #endregion
        }
    }
}
