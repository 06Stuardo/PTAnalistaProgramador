----- Creaci�n base de datos ---------
CREATE DATABASE BD_PruebaTecnicaNeonet
USE BD_PruebaTecnicaNeonet

------ Creaci�n de tablas ------------
CREATE TABLE Productos(
	id INT IDENTITY(1,1) NOT NULL,
	nombre VARCHAR(500) NOT NULL,
	precio DECIMAL (18, 2) NOT NULL,
	stock INT NOT NULL

	CONSTRAINT PK_Productos PRIMARY KEY(id),
	CONSTRAINT CK_ProductosPrecio CHECK (precio >= 0),
	CONSTRAINT CK_ProductosStock CHECK (stock >= 0)
)

CREATE TABLE Clientes (
	id INT IDENTITY(1,1) NOT NULL,
    nombre VARCHAR(500) NOT NULL,
    email VARCHAR(320) NOT NULL,

    CONSTRAINT PK_Clientes PRIMARY KEY (id),
)

CREATE TABLE Ventas(
	id INT IDENTITY(1,1) NOT NULL,
    fecha DATETIME NOT NULL,
    clienteId INT NOT NULL,
    total DECIMAL(18,2) NOT NULL,
	
	CONSTRAINT PK_Ventas PRIMARY KEY (id),
    CONSTRAINT FK_VentasClientes
        FOREIGN KEY (clienteId)
        REFERENCES Clientes(id),
    CONSTRAINT CK_VentasTotal CHECK (total >= 0)
)

CREATE TABLE DetalleVenta
(
    id INT IDENTITY(1,1) NOT NULL,
    ventaId INT NOT NULL,
    productoId INT NOT NULL,
    cantidad INT NOT NULL,
    precioUnitario DECIMAL(18,2) NOT NULL,

    CONSTRAINT PK_DetalleVenta PRIMARY KEY (id),
    CONSTRAINT FK_DetalleVentaVentas
        FOREIGN KEY (ventaId)
        REFERENCES Ventas(id),
    CONSTRAINT FK_DetalleVenta_Productos
        FOREIGN KEY (productoid)
        REFERENCES Productos(id),
    CONSTRAINT CK_DetalleVentaCantidad
        CHECK (cantidad > 0),
    CONSTRAINT CK_DetalleVentaPrecioUnitario
        CHECK (precioUnitario >= 0)
)

------------- Inserci�n de datos -----------
INSERT INTO Productos (nombre, precio, stock)
VALUES
    ('Laptop Lenovo IdeaPad', 6500.00, 10),
    ('Mouse Logitech M185', 150.00, 25),
    ('Teclado Logitech K120', 180.00, 20),
    ('Monitor LG 24 pulgadas', 1250.00, 8),
    ('Aud�fonos Sony WH-CH520', 650.00, 15);

INSERT INTO Clientes (nombre, email)
VALUES
    ('Angel Ayala', 'angel.ayala@gmail.com'),
    ('Daniel Samayoa', 'svdan@gmail.com'),
    ('M�nica Hern�ndez', 'monik.hdz@gmail.com');

INSERT INTO Ventas (fecha, clienteId, total)
VALUES
    ('2026-07-01 10:30:00', 1, 6800.00),
    ('2026-08-22 14:15:00', 2, 1900.00),
    ('2026-09-30 09:45:00', 1, 1300.00);

INSERT INTO DetalleVenta(ventaId, productoId, cantidad, precioUnitario)
VALUES
    (1, 1, 1, 6500.00),
    (1, 2, 2, 150.00),
    (2, 3, 1, 350.00),
    (2, 4, 1, 1250.00),
    (2, 2, 2, 150.00),
    (3, 4, 1, 1120.00),
    (3, 3, 1, 180.00);

------- Consultas solicitadas ---------
--- 1. Total vendido por cliente.
SELECT c.nombre Cliente, SUM(total) [TOTAL VENDIDO] FROM Ventas v
JOIN Clientes c ON v.clienteId = c.id
GROUP BY c.nombre

---- 2. Productos mas vendidos
SELECT p.nombre Producto, ISNULL(SUM(cantidad), 0) [TOTAL VENDIDO] FROM Productos p
LEFT JOIN DetalleVenta dv ON p.id = dv.productoId
GROUP BY p.nombre 
ORDER BY [TOTAL VENDIDO] DESC

------ 3. Stock actual, asumiendo que las los datos ingresados anteriormente de las ventas no afectaron
------ Se considera actualizar stock en la funci�n de venta.

SELECT nombre Producto, stock Disponible FROM Productos 

-------- Tabla temporal para almacear detalles al insertar venta
CREATE TYPE TipoDetalleVenta AS TABLE
(
    productoId INT,
    cantidad INT,
    precioUnitario DECIMAL(18,2)
)

------ Procedimiento almacenado para ingresar ventas 

CREATE PROCEDURE SP_RegistrarVenta
    @clienteId INT,
    @fecha DATETIME,
    @detalles TipoDetalleVenta READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        ----- Validar cliente 
        IF NOT EXISTS (
            SELECT 1 
            FROM Clientes 
            WHERE id = @clienteId
        )
        BEGIN
            THROW 50001, 'El cliente no existe', 1;
        END
        ------- Validar que se hayan enviado productos 
        IF NOT EXISTS (
            SELECT 1 
            FROM @detalles
        )
        BEGIN
            THROW 50002, 'Debe agregar al menos un producto a la venta', 1;
        END
        ----- Validar que todos los productos existan
        IF EXISTS (
            SELECT 1
            FROM @detalles d
            LEFT JOIN Productos p ON d.productoId = p.id
            WHERE p.id IS NULL
        )
        BEGIN
            THROW 50003, 'Uno o m�s productos no existen', 1;
        END
        ----- Validar stock ----
        IF EXISTS (
            SELECT 1
            FROM (
                SELECT productoId, SUM(cantidad) cantidad
                FROM @detalles
                GROUP BY productoId
            ) d
            JOIN Productos p ON d.productoId = p.id
            WHERE d.cantidad > p.stock
        )
        BEGIN
            THROW 50004, 'Stock insuficiente para uno o m�s productos', 1;
        END
        ---- Descontar stock 
        UPDATE p
        SET p.stock = p.stock - d.cantidad
        FROM Productos p
        JOIN (
            SELECT productoId, SUM(cantidad) cantidad
            FROM @detalles
            GROUP BY productoId
        ) d ON p.id = d.productoId;
        ----- Calcular total 
        DECLARE @total DECIMAL(18,2);

        SELECT @total = SUM(cantidad * precioUnitario)
        FROM @detalles;
        ---- Crear encabezado de venta -----
        INSERT INTO Ventas(fecha, clienteId, total)
        VALUES (@fecha, @clienteId, @total);
        ----- Obtener Id generado ---
        DECLARE @ventaId INT;
        SET @ventaId = SCOPE_IDENTITY();

        ----- Crear detalles de venta 
        INSERT INTO DetalleVenta
        (ventaId, productoId,cantidad, precioUnitario)
        SELECT
            @ventaId,
            productoId,
            cantidad,
            precioUnitario
        FROM @detalles;

        COMMIT TRANSACTION;
        ----- Retornar venta creada 
        SELECT
            @ventaId VentaId,
            @total Total;
    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END



