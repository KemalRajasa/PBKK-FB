namespace DataMahasiswa
{
    // Class untuk merepresentasikan satu entitas mahasiswa.
    // Ini "model" / data class: hanya menyimpan data, tanpa logika program.
    class Mahasiswa
    {
        // Auto-property: compiler membuat field penyimpan di belakang layar.
        // { get; set; } => bisa dibaca dan diubah dari luar class.
        public string NIM { get; set; }
        public string Nama { get; set; }
        public string Prodi { get; set; }
        public double IPK { get; set; }

        // Constructor: dipanggil saat objek dibuat dengan 'new Mahasiswa(...)'.
        // Memastikan objek tidak pernah ada dalam keadaan setengah terisi.
        public Mahasiswa(string nim, string nama, string prodi, double ipk)
        {
            NIM = nim;
            Nama = nama;
            Prodi = prodi;
            IPK = ipk;
        }

        // Override ToString() supaya objek punya representasi teks sendiri.
        // Dipakai otomatis saat objek dicetak dengan Console.WriteLine(mahasiswa).
        public override string ToString()
        {
            return string.Format("{0,-12} {1,-22} {2,-22} {3:F2}", NIM, Nama, Prodi, IPK);
        }
    }
}
