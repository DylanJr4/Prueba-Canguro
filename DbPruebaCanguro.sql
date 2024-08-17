
create database DBPruebaCanguro
go

use DBPruebaCanguro
go

create table Usuario(
IdUsuario int primary key identity,
Nombre varchar(50),
Correo varchar(50),
Clave varchar(100)
)

go

create table Moneda(
IdMoneda int primary key identity,
NombreMoneda varchar(150)
)

go

create table Sucursal(
IdSucursal int primary key identity,
Descripcion varchar(250) not null,
Direccion varchar(250) not null,
Identificacion varchar(50) not null,
FechaCreacion datetime,
IdMoneda int,
CONSTRAINT FK_Sucursal_Moneda FOREIGN KEY (idMoneda) REFERENCES Moneda(IdMoneda)
)
go

create table Logs(
IdLog int primary key identity,
LogDate datetime not null,
Description Varchar(250) not null,
LogLevel Varchar(250) null,
UserId int null,
ControllerName Varchar(100) null,
ActionName Varchar(100) null,
RequestUrl Varchar(250) null
)
go

INSERT INTO [dbo].[Moneda] ([Nombre])
VALUES ('Euro');

INSERT INTO [dbo].[Moneda] ([Nombre])
VALUES ('Yen Japonés');

INSERT INTO [dbo].[Moneda] ([Nombre])
VALUES ('Libra Esterlina');
go

select * from Usuario
select * from Moneda
select * from Sucursal
select * from Logs


-- creamos el token  del jwt con el select new id 
select NEWID()
