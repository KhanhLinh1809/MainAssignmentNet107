USE PolyShoeShopDB;
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Carts') AND name = 'Status')
BEGIN
    ALTER TABLE Carts ADD Status NVARCHAR(50) DEFAULT 'Pending';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Carts') AND name = 'TotalAmount')
BEGIN
    ALTER TABLE Carts ADD TotalAmount DECIMAL(18, 2);
    ALTER TABLE Carts ADD ReceiverAddress NVARCHAR(255);
    ALTER TABLE Carts ADD ReceiverPhone VARCHAR(15);
END
GO
