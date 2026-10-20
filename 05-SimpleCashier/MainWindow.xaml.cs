using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;

namespace StudentRegistrationApp
{
    public static class Fmt
    {
        private static readonly CultureInfo Id = new CultureInfo("id-ID");

        public static string Rp(int angka)
        {
            return "Rp " + angka.ToString("N0", Id);
        }
    }

    public class Produk
    {
        public string Nama { get; set; } = "";
        public int Harga { get; set; }
        public string Label
        {
            get { return Nama + " - " + Fmt.Rp(Harga); }
        }
    }

    public class ItemKeranjang
    {
        public string Nama { get; set; } = "";
        public int Harga { get; set; }
        public int Qty { get; set; }
        public int Subtotal
        {
            get { return Harga * Qty; }
        }

        public override string ToString()
        {
            return Nama + "  x" + Qty + "  @ " + Fmt.Rp(Harga) + "  =  " + Fmt.Rp(Subtotal);
        }
    }

    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<ItemKeranjang> keranjang
            = new ObservableCollection<ItemKeranjang>();

        public MainWindow()
        {
            InitializeComponent();

            cmbProduk.ItemsSource = new List<Produk>
            {
                new Produk { Nama = "Kopi Susu",     Harga = 18000 },
                new Produk { Nama = "Teh Manis",     Harga = 8000 },
                new Produk { Nama = "Roti Bakar",    Harga = 15000 },
                new Produk { Nama = "Nasi Goreng",   Harga = 25000 },
                new Produk { Nama = "Air Mineral",   Harga = 5000 }
            };

            lstKeranjang.ItemsSource = keranjang;
            rbTunai.IsChecked = true;
            UpdateTotal();
        }

        // EVENT 1: Tambah ke keranjang
        private void BtnTambah_Click(object sender, RoutedEventArgs e)
        {
            // ===== VALIDASI =====
            if (cmbProduk.SelectedItem == null)
            {
                MessageBox.Show("Pilih produk terlebih dahulu!");
                return;
            }

            int jumlah;
            if (!int.TryParse(txtJumlah.Text, out jumlah) || jumlah <= 0)
            {
                MessageBox.Show("Jumlah harus berupa angka lebih dari 0!");
                txtJumlah.Focus();
                return;
            }

            // ===== PROSES =====
            Produk p = (Produk)cmbProduk.SelectedItem;

            int idx = -1;
            for (int i = 0; i < keranjang.Count; i++)
            {
                if (keranjang[i].Nama == p.Nama)
                {
                    idx = i;
                    break;
                }
            }

            if (idx >= 0)
            {
                // produk sudah ada: gabungkan jumlahnya
                keranjang[idx] = new ItemKeranjang
                {
                    Nama = p.Nama,
                    Harga = p.Harga,
                    Qty = keranjang[idx].Qty + jumlah
                };
            }
            else
            {
                keranjang.Add(new ItemKeranjang
                {
                    Nama = p.Nama,
                    Harga = p.Harga,
                    Qty = jumlah
                });
            }

            UpdateTotal();
            cmbProduk.SelectedIndex = -1;
            txtJumlah.Text = "1";
        }

        // EVENT 2: Hapus item
        private void BtnHapus_Click(object sender, RoutedEventArgs e)
        {
            ItemKeranjang terpilih = lstKeranjang.SelectedItem as ItemKeranjang;

            if (terpilih == null)
            {
                MessageBox.Show("Pilih item yang ingin dihapus!");
                return;
            }

            MessageBoxResult hasil = MessageBox.Show(
                "Hapus \"" + terpilih.Nama + "\" dari keranjang?",
                "Konfirmasi",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (hasil == MessageBoxResult.Yes)
            {
                keranjang.Remove(terpilih);
                UpdateTotal();
            }
        }

        // EVENT 3: Bayar
        private void BtnBayar_Click(object sender, RoutedEventArgs e)
        {
            // ===== VALIDASI =====
            if (keranjang.Count == 0)
            {
                MessageBox.Show("Keranjang masih kosong!");
                return;
            }

            if (rbTunai.IsChecked != true && rbQris.IsChecked != true)
            {
                MessageBox.Show("Pilih metode pembayaran!");
                return;
            }

            int total = keranjang.Sum(k => k.Subtotal);
            int bayar = total;
            int kembalian = 0;
            string metode = "QRIS";

            if (rbTunai.IsChecked == true)
            {
                metode = "Tunai";

                if (!int.TryParse(txtBayar.Text, out bayar))
                {
                    MessageBox.Show("Uang dibayar harus berupa angka!");
                    txtBayar.Focus();
                    return;
                }

                if (bayar < total)
                {
                    MessageBox.Show("Uang kurang " + Fmt.Rp(total - bayar) + "!");
                    txtBayar.Focus();
                    return;
                }

                kembalian = bayar - total;
            }

            // ===== STRUK =====
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== STRUK PEMBELIAN ===");
            sb.AppendLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            sb.AppendLine();
            foreach (ItemKeranjang k in keranjang)
            {
                sb.AppendLine(k.ToString());
            }
            sb.AppendLine();
            sb.AppendLine("Total    : " + Fmt.Rp(total));
            sb.AppendLine("Metode   : " + metode);
            if (metode == "Tunai")
            {
                sb.AppendLine("Dibayar  : " + Fmt.Rp(bayar));
                sb.AppendLine("Kembali  : " + Fmt.Rp(kembalian));
            }

            MessageBox.Show(sb.ToString(), "Pembayaran Berhasil",
                MessageBoxButton.OK, MessageBoxImage.Information);

            ResetSemua();
        }

        // EVENT 4: Reset
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetSemua();
        }

        // EVENT 5 & 6: RadioButton metode pembayaran
        private void RbTunai_Checked(object sender, RoutedEventArgs e)
        {
            txtBayar.IsEnabled = true;
        }

        private void RbQris_Checked(object sender, RoutedEventArgs e)
        {
            txtBayar.Clear();
            txtBayar.IsEnabled = false;
        }

        // ===== HELPER =====
        private void UpdateTotal()
        {
            int total = keranjang.Sum(k => k.Subtotal);
            lblTotal.Text = "Total: " + Fmt.Rp(total);
        }

        private void ResetSemua()
        {
            keranjang.Clear();
            cmbProduk.SelectedIndex = -1;
            txtJumlah.Text = "1";
            txtBayar.Clear();
            rbTunai.IsChecked = true;
            UpdateTotal();
        }
    }
}
