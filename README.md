# Sistema de Gestión de Biblioteca – ACA Programación Avanzada

Aplicación de escritorio desarrollada en **C# Windows Forms (.NET 8)** y **SQL Server**.

## Requisitos
- Windows 10/11.
- Visual Studio 2022 con la carga de trabajo **Desarrollo de escritorio de .NET**.
- SQL Server Express, Developer o instancia local de SQL Server.
- .NET 8 SDK.

## Instalación
1. Abra SQL Server Management Studio (SSMS).
2. Abra y ejecute `SQL/01_BibliotecaAca.sql`. El script crea la base `BibliotecaAca`, las tablas, relaciones y datos de prueba.
3. Abra `BibliotecaAca.csproj` con Visual Studio.
4. Revise `appsettings.json`. La configuración inicial utiliza:
   `Server=localhost;Database=BibliotecaAca;Trusted_Connection=True;TrustServerCertificate=True;`
5. Si su instancia es SQL Express, cambie `Server=localhost` por `Server=.\SQLEXPRESS`.
6. Restaure los paquetes NuGet y ejecute con F5.

## Módulos
- Libros: consulta, creación, edición y eliminación.
- Autores: CRUD.
- Editoriales: CRUD.
- Usuarios: CRUD.
- Préstamos: registro, consulta, edición, eliminación y devolución.
- Reportes: libros, usuarios, préstamos activos e historial.

## Arquitectura
- `Forms`: presentación Windows Forms.
- `Services`: reglas y coordinación de operaciones.
- `Data`: conexión y ejecución de consultas SQL parametrizadas.
- `Models`: definiciones de módulos.
- `SQL`: creación de base de datos y datos de prueba.

## Consideraciones
- Las claves foráneas impiden eliminar registros relacionados.
- Los datos numéricos y fechas se validan antes de guardarse.
- Las consultas reciben parámetros para evitar inyección SQL.
- La aplicación requiere que SQL Server esté iniciado y que la cadena de conexión corresponda a la instancia local.

## Git
```bash
git init
git add .
git commit -m "Entrega inicial sistema de gestión de biblioteca"
git branch -M main
git remote add origin URL_DEL_REPOSITORIO
git push -u origin main
```

## Datos de prueba
El script SQL incorpora autores, editoriales, usuarios, libros y un préstamo de ejemplo.
