CREATE DATABASE SmartTask;

go 


CREATE DATABASE TareaInteligente;
GO

USE SmartTask;
GO


CREATE TABLE Categoria (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre NVARCHAR(100) NOT NULL
);
GO


CREATE TABLE Perfil (
    id INT PRIMARY KEY IDENTITY(1,1),
    descripcion NVARCHAR(250) NOT NULL
);
GO

-- Crear la tabla Usuario manualmente de idenity sino se puede hacer con el commando de inicalizacion
CREATE TABLE Usuario (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) UNIQUE NOT NULL,
    idPerfil INT NOT NULL,
    normalizedUserName NVARCHAR(256), 
    normalizedEmail NVARCHAR(256),     
    accessFailedCount INT NOT NULL DEFAULT 0,
    emailConfirmed BIT NOT NULL DEFAULT 0,    
    lockoutEnabled BIT NOT NULL DEFAULT 0,    
    lockoutEnd DATETIMEOFFSET NULL,            
    securityStamp NVARCHAR(256),              
    twoFactorEnabled BIT NOT NULL DEFAULT 0,   
    CONSTRAINT FK_Usuario_Perfil FOREIGN KEY (idPerfil)
    REFERENCES Perfil(id) ON DELETE CASCADE
);
GO

-- Crear la tabla Actividad, relacionada con Usuario
CREATE TABLE Actividad (
    id INT PRIMARY KEY IDENTITY(1,1),
    descripcion NVARCHAR(250) NOT NULL,
    fecha DATETIME NOT NULL,
    idUsuario INT NOT NULL,
    CONSTRAINT FK_Actividad_Usuario FOREIGN KEY (idUsuario)
    REFERENCES Usuario(id) ON DELETE CASCADE
);
GO


INSERT INTO [dbo].[Perfil]
           ([descripcion])
     VALUES
           ('Usuario');
