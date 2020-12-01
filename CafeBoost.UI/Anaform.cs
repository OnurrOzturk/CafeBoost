﻿using CafeBoost.Data;
using CafeBoost.UI.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeBoost.UI
{
    public partial class Anaform : Form
    {
        CafeBoostContext db = new CafeBoostContext();

        public Anaform()
        {
            InitializeComponent();
            MasalariOlustur();
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
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                lvi = new ListViewItem("Masa " + i);
                lvi.ImageKey = db.Siparisler.Any(x => x.MasaNo == i && x.Durum == SiparisDurum.Aktif) ? "dolu" : "bos";
                //önemli bir satır
                lvi.Tag = i;
                lvwMasalar.Items.Add(lvi);
            }
            #endregion
        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            new UrunlerForm(db).ShowDialog();
        }

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            new GecmisSiparislerForm(db).ShowDialog();
        }

        private void lvwMasalar_DoubleClick(object sender, EventArgs e)
        {
            int masaNo = (int)lvwMasalar.SelectedItems[0].Tag;
            Siparis siparis = AktifSiparisBul(masaNo);

            if (siparis == null)
            {
                siparis = new Siparis();
                siparis.MasaNo = masaNo;
                db.Siparisler.Add(siparis);
                lvwMasalar.SelectedItems[0].ImageKey = "dolu";
            }

            SiparisForm frmSiparis = new SiparisForm(db, siparis);
            frmSiparis.MasaTasindi += FrmSiparis_MasaTasindi;
            DialogResult dr = frmSiparis.ShowDialog();

            // Sipariş iptal edildiyse ya da ödeme alındıysa
            if (dr == DialogResult.OK)
            {
                lvwMasalar.SelectedItems[0].ImageKey = "bos";
            }

            // bu masa no ile ne yapacağız?   // iki ihtimalvar // ya bu masa noya ait aktif sipariş vardır   // ya da yoktur.

            // varsa => o siparişi bul getir.   // yoksa => yeni sipariş yarat  // olanı ya da yaratılanı yeni SiparisDetaylar formunda aç
        }

        private void FrmSiparis_MasaTasindi(object sender, MasaTasimaEventsArgs e)
        {
            MasaTasi(e.EskiMasaNo, e.YeniMasaNo);
        }

        private Siparis AktifSiparisBul(int masaNo)
        {
            return db.Siparisler.FirstOrDefault(x => x.MasaNo == masaNo && x.Durum == SiparisDurum.Aktif);
        }

        private void MasaTasi(int kaynak, int hedef)
        {
            foreach (ListViewItem lvi in lvwMasalar.Items)
            {
                if ((int)lvi.Tag == kaynak)
                {
                    lvi.ImageKey = "bos";
                }
                if ((int)lvi.Tag == hedef)
                {
                    lvi.ImageKey = "dolu";
                }
            }
        }

        private void VeriKaydet()
        {
            string json = JsonConvert.SerializeObject(db, Formatting.Indented);
            File.WriteAllText("veri.json", json);
        }

    }
}
