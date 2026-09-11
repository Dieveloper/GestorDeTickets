-- Base de datos de TicketProcessor
CREATE DATABASE TicketProcessorDb;
GO

USE TicketProcessorDb;
GO

-- Tickets pendientes de clasificación o ya procesados.
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

SELECT * FROM TicketsSoporte