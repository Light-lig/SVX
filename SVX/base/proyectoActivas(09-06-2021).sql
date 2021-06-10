USE master
IF EXISTS(select * from sys.databases where name='ProyectoWeb2021')
DROP DATABASE ProyectoWeb2021
GO
create database ProyectoWeb2021
go
use ProyectoWeb2021
go
create table Departamento(
	idDepartamento int primary key identity(1,1),
	nombre varchar(50) not null
)
create table Conversacion(
	idConversacion int primary key identity(1,1),
	nombre varchar(50) not null
)
create table Mensaje(
	idMensaje int primary key identity(1,1),
	idTo int not null,
	idFrom int not null,
	idConversacion int not null,
	mensaje varchar(200) not null,
	fecha DATETIME default getdate()
)
create table Categoria(
	idCategoria int primary key identity(1,1),
	nombre varchar(50) not null,
	idSuper int
)
--disponible es vendido o no int
--estadoProducto si es nuevo o como tu ex
--estado si se requiere borrado logico
create table Anuncio(
	idAnuncio varchar(100) primary key,
	titulo varchar (35) not null,
	nombre varchar (50) not null,
	descripcion varchar (150) not null,
	modelo varchar(50),
	marca varchar(25) not null,
	idCategoria int not null,
	idUsuario int not null,
	precio money not null,
	disponible int default 1,
	latitud decimal(10,8) not null,
	longitud decimal(11,8) not null,
	fecha date default getdate(),
	estadoProducto int not null,
	estado int default 1
)
-- de vergazo que la primera que suban sea la thumbnail xd
create table Foto(
	idFoto varchar(100) primary key,
	ruta varchar(50) not null,
	idAnuncio varchar(100) not null
)
create table Usuario(
	idUsuario int primary key identity(1,1),
	nombre varchar(50) not null,
	apellido varchar(50) not null,
	email varchar(60) not null,
	pass varchar (100) not null,
	telefono varchar (14) not null,
	idDepartamento int not null
)
--faltaria añadir tipo usuario para ver usuarios vip o algo pero de momento como esta avancemos
create table Puntuacion(
	idPuntuacion int primary key identity(1,1),
	valor float,
	idUsuario int not null
)
GO
ALTER TABLE Usuario 
ADD CONSTRAINT CK_telefono
CHECK (telefono LIKE '([0-9][0-9][0-9])[0-9][0-9][0-9][0-9]-[0-9][0-9][0-9][0-9]')
go
-----------------------
alter table Usuario
add constraint fk_deptoUs
foreign key (idDepartamento)
references Departamento (idDepartamento)
go
-----------------------------
alter table Puntuacion
add constraint fk_PuntUs
foreign key (idUsuario)
references Usuario (idUsuario)
go
alter table Anuncio
add constraint fk_AnunUs
foreign key (idUsuario)
references Usuario (idUsuario)
go
------------------------------
alter table Mensaje
add constraint fk_ConMen
foreign key (idConversacion)
references Conversacion (idConversacion)
go
alter table Mensaje
add constraint fk_MenUs1
foreign key (idTo)
references Usuario (idUsuario)
go
alter table Mensaje
add constraint fk_MenUs2
foreign key (idFrom)
references Usuario (idUsuario)
go
--------------------------------
alter table Anuncio
add constraint fk_AnunCat
foreign key (idCategoria)
references Categoria (idCategoria)
go
alter table Foto
add constraint fk_AnunFot
foreign key (idAnuncio)
references Anuncio (idAnuncio)
go
---------------------------------
alter table Categoria
add constraint fk_CatCat
foreign key (idSuper)
references Categoria (idCategoria)
