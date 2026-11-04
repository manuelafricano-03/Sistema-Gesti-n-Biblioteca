
# Sistema de Gestión de Biblioteca – ACA Programación Avanzada

Aplicación de escritorio desarrollada en **C#**, utilizando **Windows Forms (.NET 8)** y **SQL Server**, como proyecto académico de la asignatura Programación Avanzada.

## Tecnologías utilizadas

- C#.
- .NET 8.
- Windows Forms.
- SQL Server Express.
- SQL Server Management Studio (SSMS).
- Visual Studio 2022.

## Requisitos

- Windows 10 o Windows 11.
- Visual Studio 2022 con la carga de trabajo Desarrollo de escritorio de .NET.
- .NET 8 SDK.
- SQL Server Express o una instancia compatible de SQL Server.
- SQL Server Management Studio (SSMS).

## Instalación y configuración

### 1. Crear la base de datos

1. Abrir SQL Server Management Studio (SSMS).
2. Conectarse a la instancia local de SQL Server.
3. Abrir el archivo `SQL/01_BibliotecaACA.sql`.
4. Ejecutar el script completo.

El script crea la base de datos `BibliotecaAca`, sus tablas, relaciones, restricciones y datos de prueba.

### 2. Abrir el proyecto

1. Abrir Visual Studio 2022.
2. Seleccionar Abrir un proyecto o una solución.
3. Buscar y abrir el archivo `BibliotecaACA.sln`.
4. Esperar a que Visual Studio cargue el proyecto y restaure los paquetes necesarios.

### 3. Configurar la conexión

Abrir el archivo `appsettings.json` y verificar la cadena de conexión:

```json
{
  "ConnectionString": "Server=localhost\\SQLEXPRESS;Database=BibliotecaAca;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**Importante:** Si se utiliza otra instancia de SQL Server, se debe modificar el valor de `Server` según la configuración del equipo.

### 4. Ejecutar la aplicación

1. Verificar que SQL Server esté iniciado.
2. Compilar la solución en Visual Studio.
3. Ejecutar la aplicación presionando F5.

## Módulos del sistema

### Libros
- Registrar libros.
- Consultar libros registrados.
- Actualizar información.
- Eliminar registros según las restricciones de la base de datos.

### Autores
- Registrar autores.
- Consultar información.
- Actualizar y administrar registros.

### Editoriales
- Registrar editoriales.
- Consultar y modificar información.

### Usuarios
- Registrar usuarios.
- Consultar y actualizar datos.

### Préstamos
- Registrar préstamos de libros.
- Consultar préstamos.
- Actualizar información.
- Registrar devoluciones.
- Controlar la disponibilidad del inventario.

### Reportes
- Consultar libros registrados.
- Consultar usuarios.
- Visualizar préstamos activos.
- Consultar historial de préstamos.

## Arquitectura del proyecto

El sistema está organizado mediante una estructura modular:

| Carpeta | Descripción |
|---|---|
| Data | Conexión y ejecución de consultas a SQL Server. |
| Forms | Formularios e interfaz gráfica de Windows Forms. |
| Models | Definición de módulos y estructuras de datos. |
| Services | Lógica de negocio y operaciones del sistema. |
| SQL | Scripts de creación y configuración de la base de datos. |
| Capturas | Evidencias visuales del funcionamiento de la aplicación. |

## Consideraciones técnicas

- Se utilizan consultas SQL parametrizadas.
- Se implementan validaciones de datos.
- Se utilizan claves primarias y foráneas.
- Se controlan las relaciones entre las tablas.
- Se administra la disponibilidad de los libros durante los préstamos y devoluciones.
- La aplicación requiere una instancia activa de SQL Server.

## Evidencias del proyecto

En la carpeta `Capturas` se encuentran las imágenes correspondientes a:

- Diagrama de clases.
- Pantalla principal.
- Módulo de libros.
- Módulo de autores.
- Módulo de editoriales.
- Módulo de usuarios.
- Módulo de préstamos.
- Módulo de reportes.

## Datos de prueba

El script SQL incluye registros de prueba para facilitar la verificación del funcionamiento del sistema, incluyendo autores, editoriales, usuarios, libros y préstamos.

## Control de versiones

El proyecto se encuentra alojado en GitHub y cuenta con historial de cambios mediante commits.

## Autor

**Estudiante:** Manuel Andrés Africano Bautista  
**Institución:** Corporación Unificada Nacional – CUN  
**Asignatura:** ACA – Programación Avanzada  
**Docente:** Verónica Castro  
**Fecha de entrega:** 8 de noviembre de 2026
