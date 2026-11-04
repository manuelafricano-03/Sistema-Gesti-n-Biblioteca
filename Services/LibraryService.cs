using BibliotecaAca.Data;
using System.Data;
using BibliotecaAca.Models;
using Microsoft.Data.SqlClient;

namespace BibliotecaAca.Services;

public sealed class LibraryService
{
    // Lista blanca: evita que nombres de tablas o columnas provengan directamente del usuario.
    public static readonly IReadOnlyList<ModuleDefinition> Modules = new[]
    {
        new ModuleDefinition { Title="Libros", Table="Libros",
            Columns=new[]{"IdLibro","Codigo","Titulo","AnioPublicacion","IdAutor","IdEditorial","Existencias"},
            Labels=new[]{"ID","Código / ISBN","Título","Año","ID autor","ID editorial","Existencias"},
            EditableColumns=new[]{"Codigo","Titulo","AnioPublicacion","IdAutor","IdEditorial","Existencias"}, SearchColumn="Titulo" },
        new ModuleDefinition { Title="Autores", Table="Autores",
            Columns=new[]{"IdAutor","Nombre","Apellido"},
            Labels=new[]{"ID","Nombre","Apellido"},
            EditableColumns=new[]{"Nombre","Apellido"}, SearchColumn="Apellido" },
        new ModuleDefinition { Title="Editoriales", Table="Editoriales",
            Columns=new[]{"IdEditorial","Nombre","Ciudad","Telefono","Correo"},
            Labels=new[]{"ID","Nombre","Ciudad","Teléfono","Correo"},
            EditableColumns=new[]{"Nombre","Ciudad","Telefono","Correo"}, SearchColumn="Nombre" },
        new ModuleDefinition { Title="Usuarios", Table="Usuarios",
            Columns=new[]{"IdUsuario","Nombre","Apellido","Documento","Telefono","Correo"},
            Labels=new[]{"ID","Nombre","Apellido","Documento","Teléfono","Correo"},
            EditableColumns=new[]{"Nombre","Apellido","Documento","Telefono","Correo"}, SearchColumn="Documento" },
        new ModuleDefinition { Title="Préstamos", Table="Prestamos",
            Columns=new[]{"IdPrestamo","IdUsuario","IdLibro","FechaPrestamo","FechaDevolucion","Estado"},
            Labels=new[]{"ID","ID usuario","ID libro","Fecha préstamo","Fecha devolución","Estado"},
            EditableColumns=new[]{"IdUsuario","IdLibro","FechaPrestamo","FechaDevolucion","Estado"}, SearchColumn="Estado" }
    };

    public DataTable GetRows(ModuleDefinition module, string? search = null)
    {
        var columns = string.Join(",", module.Columns.Select(c => $"[{c}]"));
        var sql = $"SELECT {columns} FROM [{module.Table}]";
        if (!string.IsNullOrWhiteSpace(search) && module.SearchColumn is not null)
            sql += $" WHERE [{module.SearchColumn}] LIKE @search";
        sql += $" ORDER BY [{module.Columns[0]}] DESC";
        return string.IsNullOrWhiteSpace(search) || module.SearchColumn is null
            ? Database.Query(sql)
            : Database.Query(sql, Database.P("@search", "%" + search.Trim() + "%"));
    }

    public void Insert(ModuleDefinition module, IReadOnlyDictionary<string, object?> values)
    {
        Validate(module, values);

        var cols = module.EditableColumns;
        var names = string.Join(",", cols.Select(c => $"[{c}]"));
        var args = string.Join(",", cols.Select((c, i) => $"@p{i}"));

        var parameters = cols.Select((c, i) =>
            Database.P($"@p{i}", ConvertValue(c, values[c]))).ToArray();

        // Si es un préstamo, descontar una existencia.
        if (module.Table == "Prestamos")
        {
            var idLibro = ConvertValue("IdLibro", values["IdLibro"]);

            using var connection = Database.GetConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                using var stockCommand = new SqlCommand(
                    @"UPDATE Libros
                  SET Existencias = Existencias - 1
                  WHERE IdLibro = @idLibro AND Existencias > 0",
                    connection, transaction);

                stockCommand.Parameters.AddWithValue("@idLibro", idLibro);

                if (stockCommand.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException(
                        "No hay existencias disponibles de este libro.");

                using var insertCommand = new SqlCommand(
                    $"INSERT INTO [{module.Table}] ({names}) VALUES ({args})",
                    connection, transaction);

                insertCommand.Parameters.AddRange(parameters);
                insertCommand.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        else
        {
            Database.Execute(
                $"INSERT INTO [{module.Table}] ({names}) VALUES ({args})",
                parameters);
        }
    }
    public void Update(ModuleDefinition module, int id, IReadOnlyDictionary<string, object?> values)
    {
        Validate(module, values);
        var cols = module.EditableColumns;
        var set = string.Join(",", cols.Select((c, i) => $"[{c}]=@p{i}"));
        var parameters = cols.Select((c, i) => Database.P($"@p{i}", ConvertValue(c, values[c]))).ToList();
        parameters.Add(Database.P("@id", id));
        Database.Execute($"UPDATE [{module.Table}] SET {set} WHERE [{module.Columns[0]}]=@id", parameters.ToArray());
    }

    public void Delete(ModuleDefinition module, int id)
    {
        Database.Execute($"DELETE FROM [{module.Table}] WHERE [{module.Columns[0]}]=@id", Database.P("@id", id));
    }

    public void ReturnBook(int loanId)
    {
        using var connection = Database.GetConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            int idLibro;

            using (var loanCommand = new SqlCommand(
                @"UPDATE Prestamos
              SET Estado='Devuelto', FechaDevolucion=CAST(GETDATE() AS date)
              OUTPUT INSERTED.IdLibro
              WHERE IdPrestamo=@id AND Estado='Prestado'",
                connection, transaction))
            {
                loanCommand.Parameters.AddWithValue("@id", loanId);
                var result = loanCommand.ExecuteScalar();

                if (result is null || result is DBNull)
                    throw new InvalidOperationException("Este préstamo ya fue devuelto o no existe.");

                idLibro = Convert.ToInt32(result);
            }

            using (var stockCommand = new SqlCommand(
                @"UPDATE Libros
              SET Existencias = Existencias + 1
              WHERE IdLibro=@idLibro",
                connection, transaction))
            {
                stockCommand.Parameters.AddWithValue("@idLibro", idLibro);

                if (stockCommand.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("No se encontró el libro para actualizar las existencias.");
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    

    public DataTable GetReports(string report)
    {
        string sql = report switch
        {
            "Libros registrados" => @"SELECT l.Codigo,l.Titulo,l.AnioPublicacion,a.Nombre+' '+a.Apellido AS Autor,
                                      e.Nombre AS Editorial,l.Existencias FROM Libros l
                                      LEFT JOIN Autores a ON l.IdAutor=a.IdAutor
                                      LEFT JOIN Editoriales e ON l.IdEditorial=e.IdEditorial ORDER BY l.Titulo",
            "Préstamos activos" => @"SELECT p.IdPrestamo,u.Nombre+' '+u.Apellido AS Usuario,l.Titulo,p.FechaPrestamo,p.Estado
                                      FROM Prestamos p JOIN Usuarios u ON p.IdUsuario=u.IdUsuario
                                      JOIN Libros l ON p.IdLibro=l.IdLibro WHERE p.Estado='Prestado' ORDER BY p.FechaPrestamo DESC",
            "Historial de préstamos" => @"SELECT p.IdPrestamo,u.Nombre+' '+u.Apellido AS Usuario,l.Titulo,p.FechaPrestamo,p.FechaDevolucion,p.Estado
                                      FROM Prestamos p JOIN Usuarios u ON p.IdUsuario=u.IdUsuario
                                      JOIN Libros l ON p.IdLibro=l.IdLibro ORDER BY p.IdPrestamo DESC",
            "Usuarios registrados" => "SELECT IdUsuario,Nombre,Apellido,Documento,Telefono,Correo FROM Usuarios ORDER BY Apellido",
            _ => "SELECT COUNT(*) AS TotalLibros FROM Libros"
        };
        return Database.Query(sql);
    }

    private static object ConvertValue(string column, object? value)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString())) return DBNull.Value;
        if (column is "AnioPublicacion" or "IdAutor" or "IdEditorial" or "IdUsuario" or "IdLibro" or "Existencias")
        {
            if (!int.TryParse(value.ToString(), out var number))
                throw new ArgumentException($"El campo {column} debe ser un número entero.");
            if (number < 0) throw new ArgumentException($"El campo {column} no puede ser negativo.");
            return number;
        }
        if (column is "FechaPrestamo" or "FechaDevolucion")
        {
            if (!DateTime.TryParse(value.ToString(), out var date))
                throw new ArgumentException($"La fecha de {column} no es válida.");
            return date.Date;
        }
        return value.ToString()!.Trim();
    }

    private static void Validate(ModuleDefinition module, IReadOnlyDictionary<string, object?> values)
    {
        string[] required = module.Table switch
        {
            "Libros" => new[] { "Codigo", "Titulo", "IdAutor", "IdEditorial", "Existencias" },
            "Autores" => new[] { "Nombre", "Apellido" },
            "Editoriales" => new[] { "Nombre" },
            "Usuarios" => new[] { "Nombre", "Apellido", "Documento" },
            "Prestamos" => new[] { "IdUsuario", "IdLibro", "FechaPrestamo", "Estado" },
            _ => Array.Empty<string>()
        };
        foreach (var column in required)
            if (!values.TryGetValue(column, out var value) || string.IsNullOrWhiteSpace(value?.ToString()))
                throw new ArgumentException($"El campo {column} es obligatorio.");
        if (module.Table == "Prestamos")
        {
            if (!int.TryParse(values["IdUsuario"]?.ToString(), out _) ||
                !int.TryParse(values["IdLibro"]?.ToString(), out _))
                throw new ArgumentException("Seleccione un ID de usuario y un ID de libro válidos.");
            if (!DateTime.TryParse(values["FechaPrestamo"]?.ToString(), out var loanDate))
                throw new ArgumentException("Ingrese una fecha de préstamo válida.");
            var returnText = values["FechaDevolucion"]?.ToString();
            if (!string.IsNullOrWhiteSpace(returnText) &&
                DateTime.TryParse(returnText, out var returnDate) && returnDate < loanDate)
                throw new ArgumentException("La devolución no puede ser anterior al préstamo.");
        }
    }
}
