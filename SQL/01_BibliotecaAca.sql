IF DB_ID('BibliotecaAca') IS NULL CREATE DATABASE BibliotecaAca;
GO
USE BibliotecaAca;
GO

IF OBJECT_ID('Prestamos','U') IS NOT NULL DROP TABLE Prestamos;
IF OBJECT_ID('Libros','U') IS NOT NULL DROP TABLE Libros;
IF OBJECT_ID('Usuarios','U') IS NOT NULL DROP TABLE Usuarios;
IF OBJECT_ID('Autores','U') IS NOT NULL DROP TABLE Autores;
IF OBJECT_ID('Editoriales','U') IS NOT NULL DROP TABLE Editoriales;
GO

CREATE TABLE Autores(
    IdAutor INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(80) NOT NULL,
    Apellido NVARCHAR(80) NOT NULL
);

CREATE TABLE Editoriales(
    IdEditorial INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(120) NOT NULL,
    Ciudad NVARCHAR(80) NULL,
    Telefono NVARCHAR(25) NULL,
    Correo NVARCHAR(120) NULL,
    CONSTRAINT UQ_Editoriales_Nombre UNIQUE(Nombre)
);

CREATE TABLE Usuarios(
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(80) NOT NULL,
    Apellido NVARCHAR(80) NOT NULL,
    Documento NVARCHAR(30) NOT NULL,
    Telefono NVARCHAR(25) NULL,
    Correo NVARCHAR(120) NULL,
    CONSTRAINT UQ_Usuarios_Documento UNIQUE(Documento)
);

CREATE TABLE Libros(
    IdLibro INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(30) NOT NULL,
    Titulo NVARCHAR(180) NOT NULL,
    AnioPublicacion INT NULL,
    IdAutor INT NOT NULL,
    IdEditorial INT NOT NULL,
    Existencias INT NOT NULL CONSTRAINT DF_Libros_Existencias DEFAULT(1),
    CONSTRAINT UQ_Libros_Codigo UNIQUE(Codigo),
    CONSTRAINT CK_Libros_Existencias CHECK(Existencias >= 0),
    CONSTRAINT FK_Libros_Autores FOREIGN KEY(IdAutor) REFERENCES Autores(IdAutor),
    CONSTRAINT FK_Libros_Editoriales FOREIGN KEY(IdEditorial) REFERENCES Editoriales(IdEditorial)
);

CREATE TABLE Prestamos(
    IdPrestamo INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario INT NOT NULL,
    IdLibro INT NOT NULL,
    FechaPrestamo DATE NOT NULL CONSTRAINT DF_Prestamos_Fecha DEFAULT(GETDATE()),
    FechaDevolucion DATE NULL,
    Estado NVARCHAR(20) NOT NULL CONSTRAINT DF_Prestamos_Estado DEFAULT('Prestado'),
    CONSTRAINT CK_Prestamos_Estado CHECK(Estado IN ('Prestado','Devuelto')),
    CONSTRAINT CK_Prestamos_Fechas CHECK(FechaDevolucion IS NULL OR FechaDevolucion >= FechaPrestamo),
    CONSTRAINT FK_Prestamos_Usuarios FOREIGN KEY(IdUsuario) REFERENCES Usuarios(IdUsuario),
    CONSTRAINT FK_Prestamos_Libros FOREIGN KEY(IdLibro) REFERENCES Libros(IdLibro)
);
GO

INSERT INTO Autores(Nombre,Apellido) VALUES
('Gabriel','García Márquez'),('Isabel','Allende'),('Jorge Luis','Borges');
INSERT INTO Editoriales(Nombre,Ciudad,Telefono,Correo) VALUES
('Editorial Sudamericana','Buenos Aires','0000000','contacto@sudamericana.com'),
('Penguin Random House','Bogotá','0000000','contacto@penguinrandomhouse.com');
INSERT INTO Usuarios(Nombre,Apellido,Documento,Telefono,Correo) VALUES
('Laura','Gómez','100000001','3000000001','laura@email.com'),
('Carlos','Pérez','100000002','3000000002','carlos@email.com');
INSERT INTO Libros(Codigo,Titulo,AnioPublicacion,IdAutor,IdEditorial,Existencias) VALUES
('ISBN-001','Cien años de soledad',1967,1,1,5),
('ISBN-002','La casa de los espíritus',1982,2,2,3),
('ISBN-003','Ficciones',1944,3,2,2);
INSERT INTO Prestamos(IdUsuario,IdLibro,FechaPrestamo,Estado) VALUES
(1,1,CAST(GETDATE() AS DATE),'Prestado');
GO
