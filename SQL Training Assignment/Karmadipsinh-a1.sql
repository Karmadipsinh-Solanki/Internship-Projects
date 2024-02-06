--CREATE TABLE Products (
--ProductID INT PRIMARY  KEY IDENTITY(1,1),
--ProductName VARCHAR(30) NOT NULL,
--SupplierID  INT,
--CategoryID INT,
--QuantityPerUnit INT,
--UnitPrice INT,
--UnitsInStock INT,
--UnitsOnOrder INT,
--ReorderLevel  INT,
--Discontinued  bit
--);

--SET IDENTITY_INSERT [Products] ON
-- INSERT INTO Products (ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued) 
-- VALUES (6,'Pen',21,8,5,4,10,4,7,0);
-- INSERT INTO Products (ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued)
-- VALUES (7,'Cup',27,1,6,21,32,4,9,1);
-- INSERT INTO Products (ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued)
-- VALUES (5,'Bat',25,2,7,15,30,6,5,0);
-- INSERT INTO Products (ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued)
-- VALUES (4,'Ball',2,3,10,4,50,2,2,1);
-- INSERT INTO Products (ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued)
-- VALUES (3,'Bucket',12,10,6,9,7,9,12,0);
-- INSERT INTO Products (ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued)
-- VALUES (8,'Shirt',1,2,100,21,32,10,3,1);
-- INSERT INTO Products (ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued)
-- VALUES (2,'Shoes',8,8,6,13,56,3,3,1);
-- INSERT INTO Products (ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued)
-- VALUES (1,'Candy',27,8,6,2,95,9,4,1);
-- INSERT INTO Products (ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued)
-- VALUES (9,'Perfume',1,14,16,80,3,5,5,1);
-- INSERT INTO Products (ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued)
-- VALUES (10,'Sciesor',2,3,7,17,34,6,3,0);
-- INSERT INTO Products (ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued)
-- VALUES (11,'Knife',23,5,7,23,3,8,7,1);
-- INSERT INTO Products (ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued)
-- VALUES (12,'Fruits',29,3,2,20,13,1,5,1);

-- 1. Write a query to get a Product list (id, name, unit price) where current products cost less than $20.
 SELECT ProductID,ProductName,UnitPrice FROM Products WHERE UnitPrice < 20;
--2. Write a query to get Product list (id, name, unit price) where products cost between $15 and $25
 SELECT ProductID,ProductName,UnitPrice FROM Products WHERE (((UnitPrice) >= 15 AND (UnitPrice)<=25));
--3. Write a query to get Product list (name, unit price) of above average price. 
 SELECT ProductID,ProductName,UnitPrice FROM Products WHERE UnitPrice > (SELECT avg(UnitPrice) FROM Products);
--4. Write a query to get Product list (name, unit price) of ten most expensive products
 SELECT ProductID,ProductName,UnitPrice FROM Products AS a WHERE 10 >= (SELECT COUNT(DISTINCT UnitPrice) FROM Products AS b WHERE b.UnitPrice >= a.UnitPrice)
  ORDER BY UnitPrice desc;
 --5. Write a query to count current and discontinued products
 SELECT Count(ProductName) FROM Products GROUP BY Discontinued;
 --6. Write a query to get Product list (name, units on order , units in stock) of stock is less than the quantity on order
 SELECT ProductName,UnitPrice FROM Products WHERE (((Discontinued )=0) AND ((UnitsInStock) < UnitsOnOrder));

--select * from products;