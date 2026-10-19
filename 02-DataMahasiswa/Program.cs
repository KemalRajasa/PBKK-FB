using System;
using System.Collections.Generic;

namespace DataMahasiswa
{
    class Program
    {
        // static => satu list ini dipakai bersama oleh semua method di class Program.
        // List<T> = koleksi dinamis (ukurannya bertambah sendiri), beda dengan array.
        static List<Mahasiswa> daftarMahasiswa = new List<Mahasiswa>();

        static void Main()
        {
            int pilihan;

            // do-while: menu ditampilkan minimal sekali, lalu diulang
            // sampai pengguna memilih 5 (keluar).
            do
            {
                TampilkanMenu();
                Console.Write("Pilih menu: ");

                // TryParse: tidak melempar exception kalau input bukan angka,
                // tapi mengembalikan false. Lebih aman daripada int.Parse().
                if (!int.TryParse(Console.ReadLine(), out pilihan))
                {
                    pilihan = 0; // paksa masuk ke 'default'
                }

                switch (pilihan)
                {
                    case 1:
                        TambahMahasiswa();
                        break;
                    case 2:
                        TampilkanMahasiswa();
                        break;
                    case 3:
                        CariMahasiswa();
                        break;
                    case 4:
                        HapusMahasiswa();
                        break;
                    case 5:
                        Console.WriteLine("Program selesai.");
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid.");
                        break;
                }
            } while (pilihan != 5);
        }

        static void TampilkanMenu()
        {
            Console.WriteLine();
            Console.WriteLine("=== SISTEM DATA MAHASISWA ===");
            Console.WriteLine("1. Tambah Mahasiswa");
            Console.WriteLine("2. Tampilkan Mahasiswa");
            Console.WriteLine("3. Cari Mahasiswa");
            Console.WriteLine("4. Hapus Mahasiswa");
            Console.WriteLine("5. Keluar");
            Console.WriteLine($"(total data: {daftarMahasiswa.Count})");
        }

        static void TambahMahasiswa()
        {
            Console.WriteLine();
            Console.WriteLine("=== Tambah Data Mahasiswa ===");

            string nim = BacaTeks("NIM   : ");

            // Cegah NIM ganda — NIM berperan sebagai primary key.
            if (CariIndeksByNim(nim) >= 0)
            {
                Console.WriteLine($"Gagal: NIM {nim} sudah terdaftar.");
                return;
            }

            string nama = BacaTeks("Nama  : ");
            string prodi = BacaTeks("Prodi : ");
            double ipk = BacaIpk("IPK   : ");

            Mahasiswa mahasiswaBaru = new Mahasiswa(nim, nama, prodi, ipk);
            daftarMahasiswa.Add(mahasiswaBaru);

            Console.WriteLine("Data mahasiswa berhasil ditambahkan.");
        }

        static void TampilkanMahasiswa()
        {
            Console.WriteLine();
            Console.WriteLine("=== Daftar Mahasiswa ===");

            if (daftarMahasiswa.Count == 0)
            {
                Console.WriteLine("Belum ada data mahasiswa.");
                return;
            }

            Console.WriteLine(string.Format("{0,-4} {1,-12} {2,-22} {3,-22} {4}",
                "No", "NIM", "Nama", "Prodi", "IPK"));
            Console.WriteLine(new string('-', 70));

            for (int i = 0; i < daftarMahasiswa.Count; i++)
            {
                // daftarMahasiswa[i] memakai ToString() dari class Mahasiswa.
                Console.WriteLine($"{i + 1,-4} {daftarMahasiswa[i]}");
            }

            Console.WriteLine(new string('-', 70));
            Console.WriteLine($"Jumlah: {daftarMahasiswa.Count} mahasiswa");
        }

        static void CariMahasiswa()
        {
            Console.WriteLine();
            Console.WriteLine("=== Cari Mahasiswa ===");

            if (daftarMahasiswa.Count == 0)
            {
                Console.WriteLine("Belum ada data mahasiswa.");
                return;
            }

            string kunci = BacaTeks("Kata kunci (NIM atau nama): ");
            int ditemukan = 0;

            foreach (Mahasiswa m in daftarMahasiswa)
            {
                // StringComparison.OrdinalIgnoreCase => pencarian tidak peka huruf besar/kecil.
                bool cocok =
                    m.NIM.Contains(kunci, StringComparison.OrdinalIgnoreCase) ||
                    m.Nama.Contains(kunci, StringComparison.OrdinalIgnoreCase);

                if (cocok)
                {
                    if (ditemukan == 0)
                    {
                        Console.WriteLine("Hasil pencarian:");
                    }
                    Console.WriteLine("  " + m);
                    ditemukan++;
                }
            }

            if (ditemukan == 0)
            {
                Console.WriteLine($"Data dengan kata kunci '{kunci}' tidak ditemukan.");
            }
            else
            {
                Console.WriteLine($"{ditemukan} data ditemukan.");
            }
        }

        static void HapusMahasiswa()
        {
            Console.WriteLine();
            Console.WriteLine("=== Hapus Data Mahasiswa ===");

            if (daftarMahasiswa.Count == 0)
            {
                Console.WriteLine("Belum ada data mahasiswa.");
                return;
            }

            string nim = BacaTeks("NIM yang dihapus: ");
            int indeks = CariIndeksByNim(nim);

            if (indeks < 0)
            {
                Console.WriteLine($"Data dengan NIM {nim} tidak ditemukan.");
                return;
            }

            Mahasiswa target = daftarMahasiswa[indeks];
            Console.Write($"Hapus '{target.Nama}' (NIM {target.NIM})? (y/n): ");
            string? jawab = Console.ReadLine();

            if (jawab != null && jawab.Trim().ToLowerInvariant() == "y")
            {
                daftarMahasiswa.RemoveAt(indeks);
                Console.WriteLine("Data berhasil dihapus.");
            }
            else
            {
                Console.WriteLine("Penghapusan dibatalkan.");
            }
        }

        // ---------- Helper: dipakai berulang oleh method di atas ----------

        // Mengembalikan posisi mahasiswa dengan NIM tertentu, atau -1 kalau tidak ada.
        static int CariIndeksByNim(string nim)
        {
            for (int i = 0; i < daftarMahasiswa.Count; i++)
            {
                if (string.Equals(daftarMahasiswa[i].NIM, nim, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        // Console.ReadLine() bertipe string? (bisa null saat input ditutup),
        // jadi hasilnya divalidasi dulu sebelum dipakai.
        static string BacaTeks(string label)
        {
            while (true)
            {
                Console.Write(label);
                string? input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }

                Console.WriteLine("Input tidak boleh kosong.");
            }
        }

        static double BacaIpk(string label)
        {
            while (true)
            {
                Console.Write(label);
                double ipk;

                if (double.TryParse(Console.ReadLine(), out ipk) && ipk >= 0 && ipk <= 4)
                {
                    return ipk;
                }

                Console.WriteLine("IPK harus berada di antara 0.00 sampai 4.00.");
            }
        }
    }
}
