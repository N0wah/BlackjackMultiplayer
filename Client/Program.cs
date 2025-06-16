using System;
using System.Windows.Forms;

namespace Client
{
    static class Program
    {
        [STAThread] // Obligatoire pour WinForms
        static void Main()
        {
            Application.EnableVisualStyles(); // Active les styles visuels modernes Windows
            Application.SetCompatibleTextRenderingDefault(false); // Rend le rendu du texte compatible
            Application.Run(new ClientForm()); // Lance la fenêtre principale
        }
    }
}
