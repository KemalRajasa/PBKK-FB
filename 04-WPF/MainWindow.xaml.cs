using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LibraryManagementApp
{
    public partial class MainWindow : Window
    {
        public class Book
        {
            public string Isbn { get; set; }
            public string Judul { get; set; }
            public string Penulis { get; set; }
            public string Kategori { get; set; }
            public string Kondisi { get; set; }          // Baru / Bekas
            public DateTime TanggalTerbit { get; set; }
            public bool Dipinjam { get; set; }

            public string Status => Dipinjam ? "Dipinjam" : "Tersedia";
        }

        private readonly ObservableCollection<Book> _books = new ObservableCollection<Book>();
        private ICollectionView _view;
        private Book _editingBook = null;   // null = mode tambah, isi = mode edit

        public MainWindow()
        {
            InitializeComponent();
        }

        // ---------- EVENT 1: Window Loaded ----------
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _view = System.Windows.Data.CollectionViewSource.GetDefaultView(_books);
            dgBuku.ItemsSource = _view;

            // Data contoh
            _books.Add(new Book
            {
                Isbn = "9780132350884",
                Judul = "Clean Code",
                Penulis = "Robert C. Martin",
                Kategori = "Teknologi",
                Kondisi = "Baru",
                TanggalTerbit = new DateTime(2008, 8, 1),
                Dipinjam = false
            });

            UpdateCounter();
        }

        // ---------- EVENT 2: Simpan (Tambah / Update) ----------
        private void BtnSimpan_Click(object sender, RoutedEventArgs e)
        {
            // ----- VALIDASI -----
            if (string.IsNullOrWhiteSpace(txtIsbn.Text))
            {
                MessageBox.Show("ISBN harus diisi!", "Validasi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtIsbn.Focus();
                return;
            }

            if (!txtIsbn.Text.Trim().All(char.IsDigit))
            {
                MessageBox.Show("ISBN hanya boleh berisi angka!", "Validasi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtIsbn.Focus();
                return;
            }

            // Cek duplikat ISBN (abaikan buku yang sedang diedit)
            bool duplikat = _books.Any(b =>
                b.Isbn == txtIsbn.Text.Trim() && b != _editingBook);
            if (duplikat)
            {
                MessageBox.Show("ISBN sudah terdaftar!", "Validasi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtIsbn.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtJudul.Text))
            {
                MessageBox.Show("Judul harus diisi!", "Validasi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtJudul.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPenulis.Text))
            {
                MessageBox.Show("Penulis harus diisi!", "Validasi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPenulis.Focus();
                return;
            }

            if (cmbKategori.SelectedItem == null)
            {
                MessageBox.Show("Pilih kategori!", "Validasi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (rbBaru.IsChecked != true && rbBekas.IsChecked != true)
            {
                MessageBox.Show("Pilih kondisi buku!", "Validasi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpTerbit.SelectedDate == null)
            {
                MessageBox.Show("Pilih tanggal terbit!", "Validasi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ----- AMBIL DATA -----
            string kategori = ((ComboBoxItem)cmbKategori.SelectedItem).Content.ToString();
            string kondisi = rbBaru.IsChecked == true ? "Baru" : "Bekas";

            if (_editingBook == null)
            {
                // Mode TAMBAH
                _books.Add(new Book
                {
                    Isbn = txtIsbn.Text.Trim(),
                    Judul = txtJudul.Text.Trim(),
                    Penulis = txtPenulis.Text.Trim(),
                    Kategori = kategori,
                    Kondisi = kondisi,
                    TanggalTerbit = dpTerbit.SelectedDate.Value,
                    Dipinjam = chkDipinjam.IsChecked == true
                });

                MessageBox.Show("Data buku berhasil disimpan!", "Informasi",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Mode EDIT
                _editingBook.Isbn = txtIsbn.Text.Trim();
                _editingBook.Judul = txtJudul.Text.Trim();
                _editingBook.Penulis = txtPenulis.Text.Trim();
                _editingBook.Kategori = kategori;
                _editingBook.Kondisi = kondisi;
                _editingBook.TanggalTerbit = dpTerbit.SelectedDate.Value;
                _editingBook.Dipinjam = chkDipinjam.IsChecked == true;

                _view.Refresh();

                MessageBox.Show("Data buku berhasil diperbarui!", "Informasi",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            ResetForm();
            UpdateCounter();
        }

        // ---------- EVENT 3: Reset ----------
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        // ---------- EVENT 4: Edit ----------
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgBuku.SelectedItem is Book book)
            {
                _editingBook = book;

                txtIsbn.Text = book.Isbn;
                txtJudul.Text = book.Judul;
                txtPenulis.Text = book.Penulis;

                foreach (ComboBoxItem item in cmbKategori.Items)
                {
                    if (item.Content.ToString() == book.Kategori)
                    {
                        cmbKategori.SelectedItem = item;
                        break;
                    }
                }

                rbBaru.IsChecked = book.Kondisi == "Baru";
                rbBekas.IsChecked = book.Kondisi == "Bekas";
                dpTerbit.SelectedDate = book.TanggalTerbit;
                chkDipinjam.IsChecked = book.Dipinjam;

                lblFormTitle.Text = "EDIT BUKU";
                btnSimpan.Content = "Update";
                txtJudul.Focus();
            }
            else
            {
                MessageBox.Show("Pilih data yang ingin diedit!");
            }
        }

        // ---------- EVENT 5: Hapus (dengan konfirmasi) ----------
        private void BtnHapus_Click(object sender, RoutedEventArgs e)
        {
            if (dgBuku.SelectedItem is Book book)
            {
                var hasil = MessageBox.Show(
                    $"Yakin ingin menghapus buku \"{book.Judul}\"?",
                    "Konfirmasi Hapus",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (hasil == MessageBoxResult.Yes)
                {
                    if (book == _editingBook)
                        ResetForm();

                    _books.Remove(book);
                    UpdateCounter();
                }
            }
            else
            {
                MessageBox.Show("Pilih data yang ingin dihapus!");
            }
        }

        // ---------- EVENT 6: Search (TextChanged) ----------
        private void TxtCari_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_view == null) return;

            string kata = txtCari.Text.Trim().ToLower();

            _view.Filter = obj =>
            {
                if (string.IsNullOrEmpty(kata)) return true;
                var b = (Book)obj;
                return b.Judul.ToLower().Contains(kata)
                    || b.Penulis.ToLower().Contains(kata)
                    || b.Isbn.Contains(kata);
            };

            UpdateCounter();
        }

        // ---------- EVENT 7: SelectionChanged ----------
        private void DgBuku_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Bisa dipakai untuk menampilkan info buku terpilih di judul window
            if (dgBuku.SelectedItem is Book b)
                Title = $"Library Management - {b.Judul}";
            else
                Title = "Library Management";
        }

        // ---------- HELPER ----------
        private void ResetForm()
        {
            txtIsbn.Clear();
            txtJudul.Clear();
            txtPenulis.Clear();
            cmbKategori.SelectedIndex = -1;
            rbBaru.IsChecked = false;
            rbBekas.IsChecked = false;
            dpTerbit.SelectedDate = null;
            chkDipinjam.IsChecked = false;

            _editingBook = null;
            lblFormTitle.Text = "INPUT BUKU";
            btnSimpan.Content = "Simpan";
            txtIsbn.Focus();
        }

        private void UpdateCounter()
        {
            int tampil = _view?.Cast<object>().Count() ?? _books.Count;
            lblJumlah.Text = $"Jumlah Buku: {tampil}";
        }
    }
}
