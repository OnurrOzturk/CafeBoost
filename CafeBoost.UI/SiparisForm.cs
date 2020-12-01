using CafeBoost.Data;
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
    public partial class SiparisForm : Form
    {
        public event EventHandler<MasaTasimaEventsArgs> MasaTasindi;
        private readonly CafeBoostContext db;
        private readonly Siparis siparis;
        private readonly BindingList<SiparisDetay> blsiparisDetaylar;

        public SiparisForm(CafeBoostContext cafeBoostContext, Siparis siparis)
        {
            // constructor parametresi olarak gelen bu nesneleri
            // daha sonra da erişebileceğimiz field'lara aktarıyoruz
            db = cafeBoostContext;
            this.siparis = siparis;
            InitializeComponent();
            dgvSiparisDetaylar.AutoGenerateColumns = false;
            MasalariListele();
            UrunleriListele();
            MasaNoGuncelle();
            OdemeTutariGuncelle();

            blsiparisDetaylar = new BindingList<SiparisDetay>(siparis.SiparisDetaylar.ToList());
            blsiparisDetaylar.ListChanged += BlsiparisDetaylar_ListChanged;
            dgvSiparisDetaylar.DataSource = blsiparisDetaylar;

            #region FormElemanAdıVerme
            //dgvSiparisDetaylar.Columns[0].HeaderText = "Ürün Adı";
            //dgvSiparisDetaylar.Columns[1].HeaderText = "Birim Fiyat";
            //dgvSiparisDetaylar.Columns[2].HeaderText = "Adet";
            //dgvSiparisDetaylar.Columns[3].HeaderText = "Tutar"; 
            #endregion

        }

        private void MasalariListele()
        {
            cboMasalar.Items.Clear();

            for (int i = 1; i <= db.MasaAdet; i++)
            {
                if (!db.Siparisler.Any(x => x.MasaNo == i && x.Durum == SiparisDurum.Aktif))
                {
                    cboMasalar.Items.Add(i);
                }
            }
        }

        private void BlsiparisDetaylar_ListChanged(object sender, ListChangedEventArgs e)
        {
            OdemeTutariGuncelle();
        }

        private void OdemeTutariGuncelle()
        {
            lblOdemeTutar.Text = siparis.ToplamTutarTL;
        }

        private void UrunleriListele()
        {
            cboUrun.DataSource = db.Urunler.ToList();
        }

        private void MasaNoGuncelle()
        {
            Text = $"Masa {siparis.MasaNo:00} - Sipariş Detayları (Açılış: {siparis.AcilisZamani.Value.ToShortTimeString()})";
            lblMasaNo.Text = siparis.MasaNo.ToString("00");
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            Urun secilenUrun = (Urun)cboUrun.SelectedItem;
            int adet = (int)nudAdet.Value;

            SiparisDetay detay = new SiparisDetay()
            {
                UrunId = secilenUrun.Id,
                UrunAd = secilenUrun.UrunAd,
                BirimFiyat = secilenUrun.BirimFiyat,
                Adet = adet
            };
            siparis.SiparisDetaylar.Add(detay);
            db.SaveChanges();
            SiparisDetaylarYenile();
        }

        private void SiparisDetaylarYenile()
        {
            blsiparisDetaylar.Clear();
            siparis.SiparisDetaylar.ToList().ForEach(x => blsiparisDetaylar.Add(x));
        }

        private void dgvSiparisDetaylar_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Seçili detayları silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (dr != DialogResult.Yes)
            {
                e.Cancel = true;
            }

            SiparisDetay sd = (SiparisDetay)e.Row.DataBoundItem;
            db.SiparisDetaylar.Remove(sd);
            db.SaveChanges();
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Sipariş iptal edilerek kapatılacaktır. Emin misiniz?", "İptal Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                SiparisKapat(SiparisDurum.Iptal);
            }
        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Ödeme alındıysa sipariş kapatılacaktır. Emin misiniz?", "Ödeme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                SiparisKapat(SiparisDurum.Odendi, siparis.ToplamTutar());
            }
        }

        private void SiparisKapat(SiparisDurum siparisDurum, decimal odenenTutar = 0)
        {
            siparis.OdenenTutar = odenenTutar;
            siparis.KapanisZamani = DateTime.Now;
            siparis.Durum = siparisDurum;
            db.SaveChanges();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnMasaTasi_Click(object sender, EventArgs e)
        {
            if (cboMasalar.SelectedIndex < 0) return;
            int kaynak = siparis.MasaNo;
            int hedef = (int)cboMasalar.SelectedItem;
            siparis.MasaNo = hedef;
            db.SaveChanges();
            MasaNoGuncelle();
            MasalariListele();

            MasaTasimaEventsArgs args = new MasaTasimaEventsArgs()
            {
                EskiMasaNo = kaynak,
                YeniMasaNo = hedef
            };
            MasaTasindiginda(args);
        }

        protected virtual void MasaTasindiginda(MasaTasimaEventsArgs args)
        {
            if (MasaTasindi != null)
            {
                MasaTasindi(this, args);
            }
        }
    }
}
