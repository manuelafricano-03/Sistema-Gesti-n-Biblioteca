namespace BibliotecaAca.Models;

public sealed class ModuleDefinition
{
    public required string Title { get; init; }
    public required string Table { get; init; }
    public required string[] Columns { get; init; }
    public required string[] Labels { get; init; }
    public required string[] EditableColumns { get; init; }
    public string? SearchColumn { get; init; }
}
