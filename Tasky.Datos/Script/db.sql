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

           go


USE Tasky;


go

CREATE TABLE AspNetUser (
    id NVARCHAR(450) PRIMARY KEY,
    email NVARCHAR(100) NOT NULL UNIQUE,
    idPerfil INT NOT NULL,
    UserName NVARCHAR(256) NULL,
    NormalizedUserName NVARCHAR(256) NULL,
    NormalizedEmail NVARCHAR(256) NULL,
    EmailConfirmed BIT NOT NULL DEFAULT 0,
    PasswordHash NVARCHAR(MAX) NULL,
    SecurityStamp NVARCHAR(MAX) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    PhoneNumber NVARCHAR(MAX) NULL,
    PhoneNumberConfirmed BIT NOT NULL DEFAULT 0,
    TwoFactorEnabled BIT NOT NULL DEFAULT 0,
    LockoutEnd DATETIMEOFFSET NULL,
    LockoutEnabled BIT NOT NULL DEFAULT 0,
    AccessFailedCount INT NOT NULL DEFAULT 0,
   
);

CREATE TABLE AspNetRoles (
    Id NVARCHAR(450) PRIMARY KEY,  
    Name NVARCHAR(256) NOT NULL,
    NormalizedName NVARCHAR(256) NOT NULL,
    ConcurrencyStamp NVARCHAR(MAX)
);


CREATE TABLE AspNetUserRoles (
    UserId NVARCHAR(450) NOT NULL,  
    RoleId NVARCHAR(450) NOT NULL,  
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUser (Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles (Id) ON DELETE CASCADE
);


CREATE TABLE AspNetUserClaims (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL, 
    ClaimType NVARCHAR(MAX),
    ClaimValue NVARCHAR(MAX),
    FOREIGN KEY (UserId) REFERENCES AspNetUser (Id) ON DELETE CASCADE
);


CREATE TABLE AspNetRoleClaims (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoleId NVARCHAR(450) NOT NULL,  -- Cambiado de INT a NVARCHAR(450)
    ClaimType NVARCHAR(MAX),
    ClaimValue NVARCHAR(MAX),
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles (Id) ON DELETE CASCADE
);

-- Tokens de usuario
CREATE TABLE AspNetUserTokens (
    UserId NVARCHAR(450) NOT NULL,  -- Cambiado de INT a NVARCHAR(450)
    LoginProvider NVARCHAR(450) NOT NULL,
    Name NVARCHAR(450) NOT NULL,
    Value NVARCHAR(MAX),
    PRIMARY KEY (UserId, LoginProvider, Name),
    FOREIGN KEY (UserId) REFERENCES AspNetUser (Id) ON DELETE CASCADE
);