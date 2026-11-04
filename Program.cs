using BibliotecaAca.Data;
using BibliotecaAca.Forms;

namespace BibliotecaAca;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        try
        {
            Database.Initialize();
            Application.Run(new MainForm());
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "No fue posible iniciar la aplicación.\n\n" + ex.Message +
                "\n\nVerifique SQL Server y la cadena de conexión en appsettings.json.",
                "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
