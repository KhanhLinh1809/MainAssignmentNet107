USE master;
GO

-- Create Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PolyShoeShopDB')
BEGIN
    CREATE DATABASE PolyShoeShopDB;
END
GO

USE PolyShoeShopDB;
GO

-- Create Categories Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
BEGIN
    CREATE TABLE Categories (
        CategoryId INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(255)
    );
END
GO

-- Create Customers Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Customers' AND xtype='U')
BEGIN
    CREATE TABLE Customers (
        CustomerId INT IDENTITY(1,1) PRIMARY KEY,
        Username VARCHAR(50) UNIQUE NOT NULL,
        Password VARCHAR(100) NOT NULL, -- In real app, store hash
        FullName NVARCHAR(100),
        Email VARCHAR(100),
        Address NVARCHAR(200),
        Role VARCHAR(20) DEFAULT 'Customer' -- 'Admin', 'Staff', 'Customer'
    );
END
GO

-- Create Products Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Products' AND xtype='U')
BEGIN
    CREATE TABLE Products (
        ProductId INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Price DECIMAL(18, 2) NOT NULL,
        Image VARCHAR(255),
        Description NVARCHAR(MAX),
        Color NVARCHAR(50),
        Size NVARCHAR(20),
        CategoryId INT,
        FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
    );
END
GO

-- Create Carts Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Carts' AND xtype='U')
BEGIN
    CREATE TABLE Carts (
        CartId INT IDENTITY(1,1) PRIMARY KEY,
        CustomerId INT,
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
    );
END
GO

-- Create CartDetails Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CartDetails' AND xtype='U')
BEGIN
    CREATE TABLE CartDetails (
        DetailId INT IDENTITY(1,1) PRIMARY KEY,
        CartId INT,
        ProductId INT,
        Quantity INT DEFAULT 1,
        FOREIGN KEY (CartId) REFERENCES Carts(CartId),
        FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
    );
END
GO

-- Seed Categories (6 items)
INSERT INTO Categories (Name, Description) VALUES
(N'Giày Thể Thao', N'Giày thể thao năng động, thoải mái'),
(N'Giày Tây', N'Giày tây lịch lãm cho nam'),
(N'Giày Cao Gót', N'Giày cao gót quyến rũ cho nữ'),
(N'Giày Boots', N'Giày boots cá tính'),
(N'Giày Sandal', N'Dép sandal thoáng mát'),
(N'Giày Lười', N'Giày lười tiện lợi');
GO

-- Seed Users
INSERT INTO Customers (Username, Password, FullName, Email, Role) VALUES
('admin', '123456', N'Quản Trị Viên', 'admin@polyshop.com', 'Admin'),
('staff', '123456', N'Nhân Viên 1', 'staff@polyshop.com', 'Staff'),
('customer', '123456', N'Khách Hàng A', 'customer@gmail.com', 'Customer');
GO

-- Seed Products (60 items loops)
-- Note: We will use a dynamic insert approach to generate data if needed, but here are explicit inserts for clarity and control.
-- Category 1: Sneakers
INSERT INTO Products (Name, Price, Image, Description, Color, Size, CategoryId) VALUES
(N'Nike Air Max 1', 2500000, 'sneaker01.jpg', N'Giày thể thao Nike Air Max thỏa mái', N'Trắng/Đỏ', '40', 1),
(N'Adidas Ultraboost', 3200000, 'sneaker02.jpg', N'Giày chạy bộ cao cấp', N'Đen', '41', 1),
(N'Puma Suede', 1800000, 'sneaker03.jpg', N'Phong cách cổ điển', N'Xanh', '39', 1),
(N'Reebok Classic', 1500000, 'sneaker04.jpg', N'Thiết kế đơn giản tinh tế', N'Trắng', '42', 1),
(N'New Balance 574', 2100000, 'sneaker05.jpg', N'Biểu tượng của sự thoải mái', N'Xám', '40', 1),
(N'Vans Old Skool', 1200000, 'sneaker06.jpg', N'Giày trượt ván kinh điển', N'Đen/Trắng', '38', 1),
(N'Converse Chuck 70', 1400000, 'sneaker07.jpg', N'Cao cổ, vải canvas bền bỉ', N'Vàng', '37', 1),
(N'Asics Gel-Lyte III', 2800000, 'sneaker08.jpg', N'Công nghệ Gel êm ái', N'Xanh rêu', '43', 1),
(N'Fila Disruptor II', 1600000, 'sneaker09.jpg', N'Phong cách chunky hầm hố', N'Trắng', '39', 1),
(N'Jordan 1 Low', 3500000, 'sneaker10.jpg', N'Huyền thoại bóng rổ', N'Đỏ/Đen', '42', 1);

-- Category 2: Formal (Giày Tây) (CategoryId 2)
INSERT INTO Products (Name, Price, Image, Description, Color, Size, CategoryId) VALUES
(N'Oxford Black Classic', 1500000, 'formal01.jpg', N'Giày Oxford đen lịch lãm', N'Đen', '40', 2),
(N'Derby Brown Leather', 1600000, 'formal02.jpg', N'Giày Derby da nâu sang trọng', N'Nâu', '41', 2),
(N'Monk Strap Premium', 1800000, 'formal03.jpg', N'Khóa gài kim loại nổi bật', N'Đen', '42', 2),
(N'Borgue Wingtip', 1700000, 'formal04.jpg', N'Họa tiết đục lỗ cổ điển', N'Nâu đậm', '40', 2),
(N'Chelsea Boot Formal', 1900000, 'formal05.jpg', N'Bốt da cổ thấp công sở', N'Đen', '43', 2),
(N'Loafer Penny', 1400000, 'formal06.jpg', N'Giày lười công sở tiện lợi', N'Nâu bò', '39', 2),
(N'Oxford Cap Toe', 1550000, 'formal07.jpg', N'Mũi giày cắt ngang tinh tế', N'Đen', '41', 2),
(N'Wholecut Oxford', 2200000, 'formal08.jpg', N'Làm từ một miếng da duy nhất', N'Đỏ rượu', '42', 2),
(N'Derby Suede', 1300000, 'formal09.jpg', N'Da lộn mềm mại', N'Xám', '40', 2),
(N'Tassel Loafer', 1450000, 'formal10.jpg', N'Có chuông da trang trí', N'Đen', '41', 2);

-- Category 3: Heels (Giày Cao Gót) (CategoryId 3)
INSERT INTO Products (Name, Price, Image, Description, Color, Size, CategoryId) VALUES
(N'Stiletto Red Pump', 900000, 'heels01.jpg', N'Gót nhọn quyến rũ', N'Đỏ', '36', 3),
(N'Block Heel Sandal', 850000, 'heels02.jpg', N'Gót vuông dễ đi', N'Kem', '37', 3),
(N'Kitten Heel Pointed', 750000, 'heels03.jpg', N'Gót thấp thanh lịch', N'Đen', '38', 3),
(N'Platform High Heel', 1100000, 'heels04.jpg', N'Đế đúp tôn dáng', N'Bạc', '36', 3),
(N'Slingback Beige', 800000, 'heels05.jpg', N'Quai hậu thoáng mát', N'Be', '37', 3),
(N'Peep Toe Classic', 880000, 'heels06.jpg', N'Hở mũi cổ điển', N'Đen', '39', 3),
(N'Ankle Strap Heel', 950000, 'heels07.jpg', N'Quai cổ chân nữ tính', N'Hồng', '36', 3),
(N'Mule High Heel', 820000, 'heels08.jpg', N'Sục cao gót thời trang', N'Trắng', '38', 3),
(N'Wedge Sandal', 700000, 'heels09.jpg', N'Đế xuồng vững chãi', N'Nâu', '37', 3),
(N'Glitter Party Heel', 1200000, 'heels10.jpg', N'Lấp lánh dự tiệc', N'Vàng Gold', '36', 3);

-- Category 4: Boots (Giày Boots) (CategoryId 4)
INSERT INTO Products (Name, Price, Image, Description, Color, Size, CategoryId) VALUES
(N'Timberland Yellow', 3500000, 'boot01.jpg', N'Bốt vàng huyền thoại', N'Vàng', '41', 4),
(N'Dr. Martens 1460', 3800000, 'boot02.jpg', N'Bốt da cổ cao 8 lỗ', N'Đỏ Cherry', '40', 4),
(N'Chelsea Boot Suede', 1200000, 'boot03.jpg', N'Bốt Chelsea da lộn', N'Nâu', '42', 4),
(N'Combat Boot Military', 1500000, 'boot04.jpg', N'Phong cách quân đội', N'Rằn ri', '43', 4),
(N'Chukka Boot Leather', 1400000, 'boot05.jpg', N'Bốt cổ lửng đơn giản', N'Nâu đất', '41', 4),
(N'Hiking Boot Waterproof', 2200000, 'boot06.jpg', N'Chống nước leo núi', N'Xám/Đen', '42', 4),
(N'Winter Snow Boot', 1800000, 'boot07.jpg', N'Lót lông giữ ấm', N'Trắng', '39', 4),
(N'Biker Boot Studded', 1900000, 'boot08.jpg', N'Đính đinh tán cá tính', N'Đen', '40', 4),
(N'Cowboy Boot', 2500000, 'boot09.jpg', N'Phong cách miền viễn tây', N'Nâu da bò', '41', 4),
(N'Rain Boot Rubber', 500000, 'boot10.jpg', N'Ủng cao su đi mưa', N'Xanh làm', '40', 4);

-- Category 5: Sandals (Giày Sandal) (CategoryId 5)
INSERT INTO Products (Name, Price, Image, Description, Color, Size, CategoryId) VALUES
(N'Teva Original', 900000, 'sandal01.jpg', N'Sandal quai dù bền bỉ', N'Đen', '40', 5),
(N'Birkenstock Arizona', 2200000, 'sandal02.jpg', N'Đế trấu êm chân', N'Nâu', '41', 5),
(N'Crocs Classic', 800000, 'sandal03.jpg', N'Nhựa nhẹ tênh thoáng khí', N'Xanh lá', '42', 5),
(N'Adidas Adilette', 600000, 'sandal04.jpg', N'Dép quai ngang thể thao', N'Trắng/Đen', '40', 5),
(N'Chaco Z1 Classic', 2100000, 'sandal05.jpg', N'Dây đai điều chỉnh được', N'Đen', '41', 5),
(N'Slide Sandal Leather', 1100000, 'sandal06.jpg', N'Dép lê da cao cấp', N'Nâu', '42', 5),
(N'Gladiator Sandal', 850000, 'sandal07.jpg', N'Dây buộc cao cổ chân', N'Vàng kim', '37', 5),
(N'Wedge Sandal Cork', 950000, 'sandal08.jpg', N'Đế xuồng giả gỗ', N'Be', '38', 5),
(N'Sport Sandal Velcro', 750000, 'sandal09.jpg', N'Dán xé tiện lợi', N'Xám', '40', 5),
(N'Fisherman Sandal', 1300000, 'sandal10.jpg', N'Đan dây kín mũi', N'Nâu', '41', 5);

-- Category 6: Loafers (Giày Lười) (CategoryId 6)
INSERT INTO Products (Name, Price, Image, Description, Color, Size, CategoryId) VALUES
(N'Penny Loafer Classic', 1400000, 'loafer01.jpg', N'Giày lười Penny cổ điển', N'Đen', '40', 6),
(N'Driving Shoe Moccasin', 1200000, 'loafer02.jpg', N'Đế hạt nhẹ nhàng', N'Xanh Navy', '41', 6),
(N'Gucci Jordaan (Fake)', 500000, 'loafer03.jpg', N'Mô phỏng kiểu dáng cao cấp', N'Đen', '42', 6),
(N'Tassel Loafer Suede', 1300000, 'loafer04.jpg', N'Da lộn có chuông', N'Nâu', '40', 6),
(N'Boat Shoe Leather', 1500000, 'loafer05.jpg', N'Giày đi biển đế trắng', N'Nâu/Trắng', '41', 6),
(N'Slip-on Canvas', 800000, 'loafer06.jpg', N'Vải canvas không dây', N'Caro', '39', 6),
(N'Espadrille Flat', 700000, 'loafer07.jpg', N'Đế cói nhẹ nhàng', N'Be', '38', 6),
(N'Venetian Loafer', 1600000, 'loafer08.jpg', N'Thiết kế trơn đơn giản', N'Đỏ đô', '42', 6),
(N'Horsebit Loafer', 1550000, 'loafer09.jpg', N'Khóa ngựa kim loại', N'Đen', '41', 6),
(N'Belgian Loafer', 1700000, 'loafer10.jpg', N'Đính nơ nhỏ sang trọng', N'Nhung đen', '40', 6);
GO
