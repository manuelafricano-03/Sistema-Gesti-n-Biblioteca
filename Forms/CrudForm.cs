using BibliotecaAca.Models;
using BibliotecaAca.Services;
using System.Drawing;

namespace BibliotecaAca.Forms;

public sealed class CrudForm : UserControl
{
    private readonly ModuleDefinition _module;
    private readonly LibraryService _service = new();
    private readonly DataGridView _grid = new();
    private readonly Dictionary<string, TextBox> _inputs = new();
    private readonly TextBox _search = new();

    public CrudForm(ModuleDefinition module)
    {
        _module = module;
        BackColor = Color.FromArgb(245, 247, 251);
        var title = new Label { Text = "Gestión de " + module.Title, Font = new Font("Segoe UI", 21, FontStyle.Bold),
            ForeColor = Color.FromArgb(22, 42, 74), AutoSize = true, Location = new Point(28, 22) };
        Controls.Add(title);

        var searchPanel = new Panel { Location = new Point(28, 75), Size = new Size(900, 45) };
        _search.SetBounds(0, 5, 300, 30);
        _search.Font = new Font("Segoe UI", 10);
        var searchButton = MakeButton("Buscar", 315, 2, 95, () => LoadRows(_search.Text));
        var allButton = MakeButton("Mostrar todo", 420, 2, 125, () => { _search.Clear(); LoadRows(); });
        searchPanel.Controls.AddRange(new Control[] { _search, searchButton, allButton });
        Controls.Add(searchPanel);

        _grid.SetBounds(28, 130, 900, 280);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        _grid.ScrollBars = ScrollBars.Both;
        _grid.BackgroundColor = Color.White;
        _grid.SelectionChanged += (_, _) => FillInputs();
        Controls.Add(_grid);

        var formPanel = new Panel { Location = new Point(28, 430), Size = new Size(900, 170), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        var x = 0; var y = 0;
        for (int i = 0; i < module.EditableColumns.Length; i++)
        {
            var col = module.EditableColumns[i];
            var labelText = module.Labels[Array.IndexOf(module.Columns, col)];
            var label = new Label { Text = labelText, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            var input = new TextBox { Location = new Point(x, y + 22), Width = 175, Font = new Font("Segoe UI", 9) };
            if (col == "FechaPrestamo") input.Text = DateTime.Today.ToString("yyyy-MM-dd");
            if (col == "Estado") input.Text = "Prestado";
            formPanel.Controls.Add(label); formPanel.Controls.Add(input); _inputs[col] = input;
            x += 205;
            if (x > 615) { x = 0; y += 65; }
        }
        Controls.Add(formPanel);

        var buttons = new FlowLayoutPanel { Location = new Point(28, 610), Size = new Size(900, 50), Anchor = AnchorStyles.Top | AnchorStyles.Left };
        buttons.Controls.Add(MakeButton("Nuevo / Limpiar", 0, 0, 130, ClearInputs));
        buttons.Controls.Add(MakeButton("Guardar", 0, 0, 110, Insert));
        buttons.Controls.Add(MakeButton("Actualizar", 0, 0, 110, Update));
        buttons.Controls.Add(MakeButton("Eliminar", 0, 0, 110, Delete));
        if (module.Table == "Prestamos") buttons.Controls.Add(MakeButton("Registrar devolución", 0, 0, 175, ReturnBook));
        Controls.Add(buttons);
        LoadRows();
    }

    private Button MakeButton(string text, int x, int y, int width, Action action)
    {
        var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(width, 34),
            BackColor = Color.FromArgb(34, 83, 139), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
        b.FlatAppearance.BorderSize = 0;
        b.Click += (_, _) => Safe(action);
        return b;
    }

    private void Safe(Action action)
    {
        try { action(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Operación no realizada", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void LoadRows(string? search = null)
    {
        _grid.DataSource = _service.GetRows(_module, search);
        _grid.ClearSelection();
    }

    private Dictionary<string, object?> Values() =>
        _inputs.ToDictionary(k => k.Key, v => (object?)v.Value.Text.Trim());

    private int? SelectedId()
    {
        if (_grid.CurrentRow?.Cells[0].Value is null) return null;
        return Convert.ToInt32(_grid.CurrentRow.Cells[0].Value);
    }

    private void Insert()
    {
        _service.Insert(_module, Values());
        MessageBox.Show("Registro guardado correctamente.", "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LoadRows(); ClearInputs();
    }

    private void Update()
    {
        var id = SelectedId() ?? throw new InvalidOperationException("Seleccione un registro de la tabla.");
        _service.Update(_module, id, Values());
        MessageBox.Show("Registro actualizado correctamente.", "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LoadRows();
    }

    private void Delete()
    {
        var id = SelectedId() ?? throw new InvalidOperationException("Seleccione un registro de la tabla.");
        if (MessageBox.Show("¿Confirma que desea eliminar el registro seleccionado?", "Confirmar",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        _service.Delete(_module, id);
        LoadRows(); ClearInputs();
    }

    private void ReturnBook()
    {
        var id = SelectedId() ?? throw new InvalidOperationException("Seleccione un préstamo.");
        _service.ReturnBook(id);
        MessageBox.Show("Si el préstamo estaba activo, se registró la devolución.", "Biblioteca");
        LoadRows();
    }

    private void FillInputs()
    {
        if (_grid.CurrentRow is null || _grid.CurrentRow.IsNewRow) return;
        foreach (var column in _module.EditableColumns)
        {
            var index = Array.IndexOf(_module.Columns, column);
            if (index >= 0 && _grid.CurrentRow.Cells[index].Value is not null)
                _inputs[column].Text = Convert.ToString(_grid.CurrentRow.Cells[index].Value) ?? "";
        }
    }

    private void ClearInputs()
    {
        foreach (var input in _inputs.Values) input.Clear();
        if (_inputs.ContainsKey("FechaPrestamo")) _inputs["FechaPrestamo"].Text = DateTime.Today.ToString("yyyy-MM-dd");
        if (_inputs.ContainsKey("Estado")) _inputs["Estado"].Text = "Prestado";
        _grid.ClearSelection();
    }
}
