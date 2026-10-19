using System.Windows.Forms;

namespace KalkulatorSederhana
{
    // Button biasa, tapi tidak pernah menerima fokus keyboard.
    // Tanpa ini, setelah tombol diklik lalu pengguna menekan Enter, tombol itu
    // "diklik" lagi oleh Windows di samping Enter yang ditangani form (jadi dobel).
    class TombolKalkulator : Button
    {
        public TombolKalkulator()
        {
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
        }
    }
}
