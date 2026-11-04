using BibliotecaAca.Forms;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BibliotecaAca.Forms;

public sealed class MainForm : Form
{
    private readonly Panel _content = new() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 247, 251) };

    public MainForm()
    {
        Text = "Sistema de Gestión de Biblioteca";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1050, 680);
        Size = new Size(1200, 760);
        BackColor = Color.White;

        var sidebar = new Panel { Dock = DockStyle.Left, Width = 230, BackColor = Color.FromArgb(22, 42, 74) };
        var brand = new Label { Text = "    BIBLIOTECA     M.A", ForeColor = Color.White, Font = new Font("Segoe UI", 20, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 115, TextAlign = ContentAlignment.MiddleCenter };
        sidebar.Controls.Add(brand);

        var menu = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(12, 12, 12, 0), Top = 115 };
        sidebar.Controls.Add(menu);
        menu.BringToFront();
        menu.Padding = new Padding(12, 12, 12, 0);
        AddMenu(menu, "Inicio", () => ShowHome());
        AddMenu(menu, "Libros", () => OpenModule("Libros"));
        AddMenu(menu, "Autores", () => OpenModule("Autores"));
        AddMenu(menu, "Editoriales", () => OpenModule("Editoriales"));
        AddMenu(menu, "Usuarios", () => OpenModule("Usuarios"));
        AddMenu(menu, "Préstamos", () => OpenModule("Préstamos"));
        AddMenu(menu, "Reportes", () => ShowReports());
        menu.BringToFront();
        Controls.Add(_content);
        Controls.Add(sidebar);
        ShowHome();
    }

    private void AddMenu(Control parent, string text, Action action)
    {
        var button = new System.Windows.Forms.Button  { Text = text, Width = 198, Height = 44, FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(35, 61, 100), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Margin = new Padding(0, 5, 0, 5), Cursor = Cursors.Hand };
        button.FlatAppearance.BorderSize = 0;
        button.Click += (_, _) => action();
        parent.Controls.Add(button);
    }

    private void SetContent(Control control)
    {
        _content.Controls.Clear();
        control.Dock = DockStyle.Fill;
        _content.Controls.Add(control);
    }

    private void ShowHome()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        {
                };
        var picture = new PictureBox
        {
            Image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Forms", "Images", "fondo-biblioteca.png")),
            SizeMode = PictureBoxSizeMode.StretchImage,
            Dock = DockStyle.Fill
        };

        panel.Controls.AddRange(new Control[] {  picture });
        picture.SendToBack();
        SetContent(panel);
    }

    private void OpenModule(string title)
    {
        var module = BibliotecaAca.Services.LibraryService.Modules.First(m => m.Title == title);
        SetContent(new CrudForm(module));
    }

    private void ShowReports() => SetContent(new ReportsForm());
}
