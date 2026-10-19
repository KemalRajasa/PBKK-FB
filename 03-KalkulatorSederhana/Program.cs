using System;
using System.Windows.Forms;

namespace KalkulatorSederhana
{
    static class Program
    {
        // [STAThread] wajib untuk aplikasi Windows Forms: komponen UI Windows
        // (clipboard, dialog, dll.) hanya aman dipakai dari thread ber-model STA.
        [STAThread]
        static void Main()
        {
            // Mengatur default aplikasi (visual styles, DPI, font) dari properti di .csproj.
            ApplicationConfiguration.Initialize();

            // Application.Run memulai "message loop": program menunggu event
            // (klik, ketikan, resize) dan baru bereaksi ketika event datang.
            Application.Run(new FormKalkulator());
        }
    }
}
