using System;
using System.Drawing;
using System.Windows.Forms;

namespace KalkulatorSederhana
{
    // Jendela utama. Seluruh tampilan dibangun lewat kode (tanpa file Designer)
    // supaya bisa dibuild dan dibaca dari VS Code, yang tidak punya visual designer.
    class FormKalkulator : Form
    {
        // Simbol tombol ditulis sebagai escape Unicode agar aman di editor mana pun.
        private const string TombolBagi = "÷";     // ÷
        private const string TombolKali = "×";     // ×
        private const string TombolKurang = "−";   // −
        private const string TombolHapus = "⌫";    // ⌫
        private const string TombolTanda = "±";    // ±

        private enum JenisTombol
        {
            Angka,
            Operator,
            Fungsi,
            SamaDengan
        }

        private static readonly Color WarnaLatar = Color.FromArgb(243, 243, 243);
        private static readonly Color WarnaAksen = Color.FromArgb(0, 103, 192);

        private readonly Kalkulator _kalkulator = new Kalkulator();
        private readonly Label _lblEkspresi;
        private readonly Label _lblTampilan;
        private readonly Font _fontTombol = new Font("Segoe UI Semibold", 15f);

        public FormKalkulator()
        {
            Text = "Kalkulator Sederhana";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(340, 500);
            MinimumSize = new Size(320, 480);
            BackColor = WarnaLatar;
            KeyPreview = true;      // form menerima ketikan keyboard lebih dulu

            // Grid 4 kolom x 7 baris: baris 0 = ekspresi, baris 1 = layar, baris 2-6 = tombol.
            var tata = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 7,
                Padding = new Padding(8)
            };

            for (int k = 0; k < 4; k++)
            {
                tata.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            }

            tata.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f));
            tata.RowStyles.Add(new RowStyle(SizeType.Percent, 20f));
            for (int b = 0; b < 5; b++)
            {
                tata.RowStyles.Add(new RowStyle(SizeType.Percent, 16f));
            }

            _lblEkspresi = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.FromArgb(110, 110, 110),
                Font = new Font("Segoe UI", 11f),
                Margin = new Padding(3, 0, 3, 0)
            };

            _lblTampilan = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI Semibold", 32f),
                Margin = new Padding(3, 0, 3, 0)
            };
            _lblTampilan.SizeChanged += (pengirim, e) => SesuaikanFontTampilan();

            tata.Controls.Add(_lblEkspresi, 0, 0);
            tata.SetColumnSpan(_lblEkspresi, 4);
            tata.Controls.Add(_lblTampilan, 0, 1);
            tata.SetColumnSpan(_lblTampilan, 4);

            string[,] susunan =
            {
                { "CE", "C", TombolHapus, TombolBagi },
                { "7", "8", "9", TombolKali },
                { "4", "5", "6", TombolKurang },
                { "1", "2", "3", "+" },
                { TombolTanda, "0", ",", "=" }
            };

            for (int baris = 0; baris < 5; baris++)
            {
                for (int kolom = 0; kolom < 4; kolom++)
                {
                    string teks = susunan[baris, kolom];
                    tata.Controls.Add(BuatTombol(teks, TentukanJenis(teks)), kolom, baris + 2);
                }
            }

            Controls.Add(tata);
            Perbarui();
        }

        // ---------- Tombol ----------

        private static JenisTombol TentukanJenis(string teks)
        {
            switch (teks)
            {
                case "=":
                    return JenisTombol.SamaDengan;
                case "+":
                case TombolKurang:
                case TombolKali:
                case TombolBagi:
                    return JenisTombol.Operator;
                case "CE":
                case "C":
                case TombolHapus:
                case TombolTanda:
                    return JenisTombol.Fungsi;
                default:
                    return JenisTombol.Angka;
            }
        }

        private TombolKalkulator BuatTombol(string teks, JenisTombol jenis)
        {
            var tombol = new TombolKalkulator
            {
                Text = teks,
                Dock = DockStyle.Fill,
                Margin = new Padding(3),
                Font = _fontTombol,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            tombol.FlatAppearance.BorderSize = 1;
            tombol.FlatAppearance.BorderColor = Color.FromArgb(218, 218, 218);

            switch (jenis)
            {
                case JenisTombol.Angka:
                    Warnai(tombol, Color.White, Color.Black, Color.FromArgb(236, 236, 236), Color.FromArgb(222, 222, 222));
                    break;
                case JenisTombol.Fungsi:
                    Warnai(tombol, Color.FromArgb(232, 232, 232), Color.Black, Color.FromArgb(220, 220, 220), Color.FromArgb(206, 206, 206));
                    break;
                case JenisTombol.Operator:
                    Warnai(tombol, Color.FromArgb(222, 235, 250), Color.FromArgb(0, 70, 140), Color.FromArgb(205, 224, 245), Color.FromArgb(188, 212, 240));
                    break;
                case JenisTombol.SamaDengan:
                    Warnai(tombol, WarnaAksen, Color.White, Color.FromArgb(0, 88, 165), Color.FromArgb(0, 74, 140));
                    break;
            }

            // Event handler: kode ini dijalankan ketika tombol diklik.
            tombol.Click += (pengirim, e) => Tekan(teks);
            return tombol;
        }

        private static void Warnai(Button tombol, Color latar, Color teks, Color hover, Color tekan)
        {
            tombol.BackColor = latar;
            tombol.ForeColor = teks;
            tombol.FlatAppearance.MouseOverBackColor = hover;
            tombol.FlatAppearance.MouseDownBackColor = tekan;
        }

        // ---------- Perintah ----------

        // Satu pintu untuk semua masukan: klik tombol dan ketikan keyboard sama-sama
        // lewat sini, jadi perilakunya pasti identik.
        private void Tekan(string perintah)
        {
            switch (perintah)
            {
                case "CE":
                    _kalkulator.BersihkanEntri();
                    break;
                case "C":
                    _kalkulator.SemuaBersih();
                    break;
                case TombolHapus:
                    _kalkulator.Hapus();
                    break;
                case TombolTanda:
                    _kalkulator.UbahTanda();
                    break;
                case ",":
                    _kalkulator.MasukkanDesimal();
                    break;
                case "=":
                    _kalkulator.SamaDengan();
                    break;
                case "+":
                    _kalkulator.PilihOperator('+');
                    break;
                case TombolKurang:
                    _kalkulator.PilihOperator('-');
                    break;
                case TombolKali:
                    _kalkulator.PilihOperator('*');
                    break;
                case TombolBagi:
                    _kalkulator.PilihOperator('/');
                    break;
                default:
                    if (perintah.Length == 1 && char.IsDigit(perintah[0]))
                    {
                        _kalkulator.MasukkanAngka(perintah[0]);
                    }
                    break;
            }

            Perbarui();
        }

        private void Perbarui()
        {
            _lblEkspresi.Text = _kalkulator.Ekspresi;
            _lblTampilan.Text = _kalkulator.Tampilan;
            SesuaikanFontTampilan();
        }

        // Perkecil font layar sampai teks muat, supaya angka panjang tidak terpotong.
        private void SesuaikanFontTampilan()
        {
            int lebarTersedia = _lblTampilan.ClientSize.Width - 8;
            if (lebarTersedia <= 0)
            {
                return;
            }

            float ukuran = 32f;
            while (ukuran > 12f)
            {
                using (var uji = new Font("Segoe UI Semibold", ukuran))
                {
                    if (TextRenderer.MeasureText(_lblTampilan.Text, uji).Width <= lebarTersedia)
                    {
                        break;
                    }
                }
                ukuran -= 2f;
            }

            if (Math.Abs(_lblTampilan.Font.Size - ukuran) > 0.1f)
            {
                Font lama = _lblTampilan.Font;
                _lblTampilan.Font = new Font("Segoe UI Semibold", ukuran);
                lama.Dispose();
            }
        }

        // ---------- Keyboard ----------

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            string? perintah = PetaKeyboard(e.KeyChar);

            if (perintah != null)
            {
                Tekan(perintah);
                e.Handled = true;
            }

            base.OnKeyPress(e);
        }

        private static string? PetaKeyboard(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return c.ToString();
            }

            switch (c)
            {
                case '.':
                case ',':
                    return ",";
                case '+':
                    return "+";
                case '-':
                    return TombolKurang;
                case '*':
                    return TombolKali;
                case '/':
                    return TombolBagi;
                case '=':
                case '\r':          // Enter
                    return "=";
                case '\b':          // Backspace
                    return TombolHapus;
                case '\u001B':      // Esc
                    return "C";
                default:
                    return null;
            }
        }
    }
}
