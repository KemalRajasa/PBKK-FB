using System;
using System.Globalization;

namespace KalkulatorSederhana
{
    // Logika kalkulator. Sengaja dipisah dari tampilan: class ini tidak tahu apa pun
    // tentang Form, Button, atau Label — hanya menerima perintah dan menyediakan teks
    // untuk ditampilkan. Dengan begitu logikanya bisa diuji tanpa membuka jendela,
    // dan tampilannya bisa diganti (WPF, web, console) tanpa menyentuh logika.
    class Kalkulator
    {
        private const int MaksDigit = 16;
        private const int MaksDesimalHasilBagi = 15;

        // Keadaan (state) kalkulator:
        private decimal _operandKiri;       // angka pertama, mis. 12 pada "12 + 7" (berlaku jika _operator terisi)
        private char? _operator;            // operator yang menunggu operand kanan: + - * /
        private string _masukan = "0";      // teks yang sedang diketik / ditampilkan
        private bool _mulaiBaru = true;     // true => ketikan angka berikutnya memulai angka baru
        private bool _error;                // true => _masukan berisi pesan kesalahan
        private string _ekspresi = "";      // baris kecil di atas layar, mis. "12 + 7 ="

        // Teks yang ditampilkan: titik desimal internal diganti koma (gaya Indonesia).
        public string Tampilan
        {
            get { return _error ? _masukan : _masukan.Replace('.', ','); }
        }

        public string Ekspresi
        {
            get { return _ekspresi.Replace('.', ','); }
        }

        // ---------- Perintah ----------

        public void MasukkanAngka(char digit)
        {
            if (_error)
            {
                SemuaBersih();
            }

            if (_mulaiBaru)
            {
                // Mengetik angka setelah "=" berarti mulai perhitungan baru.
                if (!_operator.HasValue)
                {
                    _ekspresi = "";
                }
                _masukan = "0";
                _mulaiBaru = false;
            }

            if (_masukan == "0")
            {
                _masukan = digit.ToString();       // "0" diganti, bukan "07"
            }
            else if (JumlahDigit(_masukan) < MaksDigit)
            {
                _masukan += digit;
            }
        }

        public void MasukkanDesimal()
        {
            if (_error)
            {
                SemuaBersih();
            }

            if (_mulaiBaru)
            {
                if (!_operator.HasValue)
                {
                    _ekspresi = "";
                }
                _masukan = "0.";
                _mulaiBaru = false;
                return;
            }

            if (!_masukan.Contains('.'))
            {
                _masukan += ".";
            }
        }

        public void PilihOperator(char op)
        {
            if (_error)
            {
                return;
            }

            decimal sekarang = AmbilNilai();

            if (_operator.HasValue && !_mulaiBaru)
            {
                // Rantai: "5 + 3 +" => hitung 5 + 3 dulu, hasilnya jadi operand kiri.
                decimal hasil;
                if (!TryHitung(_operandKiri, _operator.Value, sekarang, out hasil))
                {
                    return;
                }
                _operandKiri = hasil;
                _masukan = Format(hasil);
            }
            else if (!_operator.HasValue)
            {
                _operandKiri = sekarang;
            }
            // Kalau sudah ada operator tapi belum mengetik operand kanan ("5 + ×"),
            // operator cukup diganti.

            _operator = op;
            _mulaiBaru = true;
            _ekspresi = Format(_operandKiri) + " " + Simbol(op);
        }

        public void SamaDengan()
        {
            if (_error)
            {
                return;
            }

            decimal kanan = AmbilNilai();

            if (!_operator.HasValue)
            {
                _ekspresi = Format(kanan) + " =";
                _mulaiBaru = true;
                return;
            }

            decimal kiri = _operandKiri;
            char op = _operator.Value;
            decimal hasil;

            if (!TryHitung(kiri, op, kanan, out hasil))
            {
                return;
            }

            _ekspresi = Format(kiri) + " " + Simbol(op) + " " + Format(kanan) + " =";
            _masukan = Format(hasil);
            _operandKiri = 0m;
            _operator = null;
            _mulaiBaru = true;
        }

        public void UbahTanda()
        {
            if (_error || AmbilNilai() == 0m)
            {
                return;
            }

            _masukan = _masukan.StartsWith('-') ? _masukan.Substring(1) : "-" + _masukan;
        }

        public void Hapus()
        {
            if (_error)
            {
                SemuaBersih();
                return;
            }

            if (_mulaiBaru)
            {
                return;     // hasil perhitungan tidak boleh dipotong per digit
            }

            bool tinggalSatuDigit = _masukan.Length <= 1 || (_masukan.Length == 2 && _masukan[0] == '-');
            _masukan = tinggalSatuDigit ? "0" : _masukan.Substring(0, _masukan.Length - 1);
        }

        // CE (Clear Entry): hapus angka yang sedang diketik, operasi tertunda tetap.
        public void BersihkanEntri()
        {
            if (_error)
            {
                SemuaBersih();
                return;
            }

            _masukan = "0";
            _mulaiBaru = false;
        }

        // C (Clear): kembali ke keadaan awal.
        public void SemuaBersih()
        {
            _operandKiri = 0m;
            _operator = null;
            _masukan = "0";
            _mulaiBaru = true;
            _error = false;
            _ekspresi = "";
        }

        // ---------- Internal ----------

        private bool TryHitung(decimal a, char op, decimal b, out decimal hasil)
        {
            hasil = 0m;

            try
            {
                switch (op)
                {
                    case '+':
                        hasil = a + b;
                        break;
                    case '-':
                        hasil = a - b;
                        break;
                    case '*':
                        hasil = a * b;
                        break;
                    case '/':
                        // Dibulatkan agar 1 ÷ 3 tidak menghasilkan 28 digit di layar.
                        hasil = Math.Round(a / b, MaksDesimalHasilBagi, MidpointRounding.AwayFromZero);
                        break;
                }
                return true;
            }
            catch (DivideByZeroException)
            {
                TetapkanError("Tidak bisa dibagi nol");
                return false;
            }
            catch (OverflowException)
            {
                TetapkanError("Angka terlalu besar");
                return false;
            }
        }

        private void TetapkanError(string pesan)
        {
            _error = true;
            _masukan = pesan;
            _ekspresi = "";
            _operandKiri = 0m;
            _operator = null;
            _mulaiBaru = true;
        }

        private decimal AmbilNilai()
        {
            // "5." (belum ada angka di belakang titik) dipotong dulu agar bisa di-parse.
            return decimal.Parse(_masukan.TrimEnd('.'), NumberStyles.Number, CultureInfo.InvariantCulture);
        }

        private static int JumlahDigit(string teks)
        {
            int jumlah = 0;
            foreach (char c in teks)
            {
                if (char.IsDigit(c))
                {
                    jumlah++;
                }
            }
            return jumlah;
        }

        // Mengubah decimal ke teks tanpa nol di belakang koma: 2.50 => "2.5", 3.0 => "3".
        private static string Format(decimal nilai)
        {
            string teks = nilai.ToString(CultureInfo.InvariantCulture);

            if (teks.Contains('.'))
            {
                teks = teks.TrimEnd('0').TrimEnd('.');
            }

            return teks == "-0" ? "0" : teks;
        }

        private static string Simbol(char op)
        {
            switch (op)
            {
                case '-':
                    return "−";    // −
                case '*':
                    return "×";    // ×
                case '/':
                    return "÷";    // ÷
                default:
                    return op.ToString();
            }
        }
    }
}
