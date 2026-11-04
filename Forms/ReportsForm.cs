using BibliotecaAca.Services;
using System.Drawing;

namespace BibliotecaAca.Forms;

public sealed class ReportsForm : UserControl
{
    private readonly LibraryService _service = new();
    private readonly DataGridView _grid = new() { ReadOnly = true, AllowUserToAddRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        BackgroundColor = Color.White, Dock = DockStyle.Fill };
    private readonly ComboBox _reports = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 260 };

    public ReportsForm()
    {
        BackColor = Color.FromArgb(245, 247, 251);
        var title = new Label { Text = "Reportes", Font = new Font("Segoe UI", 21, FontStyle.Bold),
            ForeColor = Color.FromArgb(22, 42, 74), AutoSize = true, Location = new Point(28, 22) };
        _reports.Items.AddRange(new object[] { "Libros registrados", "Usuarios registrados", "Préstamos activos", "Historial de préstamos" });
        _reports.SelectedIndex = 0;
        var run = new Button { Text = "Generar reporte", Location = new Point(310, 58), Size = new Size(145, 32),
            BackColor = Color.FromArgb(34, 83, 139), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        run.Click += (_, _) => LoadReport();
        var top = new Panel { Dock = DockStyle.Top, Height = 110 };
        top.Controls.Add(title); _reports.Location = new Point(28, 60); top.Controls.Add(_reports); top.Controls.Add(run);
        Controls.Add(_grid); Controls.Add(top);
        LoadReport();
    }

    private void LoadReport()
    {
        try { _grid.DataSource = _service.GetReports(_reports.SelectedItem?.ToString() ?? "Libros registrados"); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Error al generar reporte"); }
    }
}
