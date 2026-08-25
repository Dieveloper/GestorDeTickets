-- 1. Creamos la base de datos
CREATE DATABASE SistemaPedidosCorp;
GO

-- 2. Nos movemos a esa base de datos
USE SistemaPedidosCorp;
GO

-- 3. Creamos nuestra tabla de negocio
CREATE TABLE TicketsSoporte (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClienteEmail NVARCHAR(100) NOT NULL,
    TextoTicket NVARCHAR(MAX) NOT NULL,
    Estado NVARCHAR(50) DEFAULT 'Pendiente', 
    CategoriaIA NVARCHAR(100) NULL, 
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaProcesamiento DATETIME NULL
);
GO

-- 4. Metemos un par de datos de prueba
INSERT INTO TicketsSoporte (ClienteEmail, TextoTicket)
VALUES 
('cliente1@empresa.com', 'Hola, la factura de este mes me ha llegado duplicada. Necesito un abono.'),
('cliente2@empresa.com', 'El sistema no me deja hacer login, da error 500.');
GO