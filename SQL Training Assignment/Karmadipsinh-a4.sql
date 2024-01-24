use karmadipsolanki_db;

1. Create a stored procedure in the Northwind database that will calculate the average value of Freight for a specified customer.
Then, a business rule will be added that will be triggered before every Update and Insert command in the Orders controller,and will use the stored procedure to verify that the Freight does not exceed the average freight. If it does, a message will be displayed and the command will be cancelled.CREATE PROCEDURE GetAverageFreightOfCustomer
	@CustomerID varchar(100)
AS
BEGIN
	select CustomerID,avg(Freight) from orders_first group by CustomerID having CustomerID = @CustomerID;
END
GO
exec GetAverageFreightOfCustomer @CustomerID = 'commi';

2. write a SQL query to Create Stored procedure in the Northwind database to retrieve Employee Sales by Country
CREATE PROCEDURE [dbo].[GetSalesByCountry]
	@Country varchar(200)
AS
BEGIN	
	select e.FirstName,sum(od.Unitprice*od.Quantity - od.Discount*od.Unitprice*od.Quantity) from employees as e inner join Orders as o_f on e.EmployeeID = o_f.EmployeeID inner join [Order Details] as od on o_f.OrderID = od.OrderID where e.Country = @Country group by e.FirstName ;
END
GO

exec dbo.GetSalesByCountry @Country = 'UK';

3. write a SQL query to Create Stored procedure in the Northwind database to retrieve Sales by Year
create procedure  [dbo].[GetSalesByYear]
@Year varchar(200)
as
begin
select year(o_f.OrderDate),sum(od.Unitprice*od.Quantity - od.Discount*od.Unitprice*od.Quantity) from Orders as o_f inner join [Order Details] as od on o_f.OrderID = od.OrderID group by OrderDate  having YEAR(o_f.[OrderDate] )= @Year;
--select YEAR(o_f.OrderDate),sum(od.Unitprice*od.Quantity - od.Discount*od.Unitprice*od.Quantity) from orders_first as o_f inner join [Order Details] as od on o_f.OrderID = od.OrderID group by YEAR(o_f.OrderDate) having YEAR(o_f.OrderDate) = @Year;
end
go
exec dbo.GetSalesByYear @Year = '1996';
4. write a SQL query to Create Stored procedure in the Northwind database to retrieve Sales By Categorycreate procedure  [dbo].[GetSalesByCategory]
@Category varchar(200)
as
begin
select c.CategoryName,sum(od.Unitprice*od.Quantity - od.Discount*od.Unitprice*od.Quantity) from Products as p inner join Categories as c on p.CategoryID = c.CategoryID inner join [Order Details] od on p.ProductID = od.ProductID group by c.CategoryName  having (c.CategoryName)= @Category;
end
go
exec dbo.GetSalesByCategory @Category = 'Seafood';5. write a SQL query to Create Stored procedure in the Northwind database to retrieve Ten Most Expensive Productscreate procedure  [dbo].[GetTenMostExpensiveProducts]
as
begin
select  top 10 * from Products order by UnitPrice desc;
end
go exec [GetTenMostExpensiveProducts]; 6. write a SQL query to Create Stored procedure in the Northwind database to insert Customer Order Details create procedure  [dbo].[InsertCustomerDetail]
@CustomerID varchar(200),
	@EmployeeID int,
	@OrderDate DateTime,
	@RequiredDate DateTime,
	@ShippedDate DateTime,
	@ShipVia int,
	@Freight decimal(10,2),
	@ShipName varchar(200),
	@ShipAddress varchar(200),
	@ShipCity varchar(200),
	@ShipRegion varchar(200),
	@ShipPostalCode varchar(200),
	@ShipCountry varchar(200),
	@ProductID int,
	@UnitPrice decimal(10,2),
	@Qunatity int,
	@Discount decimal(4,2)
as
begin
declare @order_id int
	insert into Orders values(@CustomerID,@EmployeeID,@OrderDate,@RequiredDate,@ShippedDate,@ShipVia,@Freight,@ShipName,@ShipAddress,@ShipCity,@ShipRegion,@ShipPostalCode,@ShipCountry);
	set @order_id = IDENT_CURRENT('orders_first')
	insert into [Order Details] values(@order_id,@ProductID,@UnitPrice,@Qunatity,@Discount);
end
go
 exec InsertCustomerDetail @CustomerID= 'IDENT_CURRENT(CustomerID)'; 7. write a SQL query to Create Stored procedure in the Northwind database to update Customer Order DetailsCREATE PROCEDURE UpdateCustomerOrder
	@OrderID int,
	@CustomerID varchar(200),
	@EmployeeID int,
	@OrderDate DateTime,
	@RequiredDate DateTime,
	@ShippedDate DateTime,
	@ShipVia int,
	@Freight decimal(10,2),
	@ShipName varchar(200),
	@ShipAddress varchar(200),
	@ShipCity varchar(200),
	@ShipRegion varchar(200),
	@ShipPostalCode varchar(200),
	@ShipCountry varchar(200),
	@ProductID int,
	@UnitPrice decimal(10,2),
	@Qunatity int,
	@Discount decimal(4,2)
AS
BEGIN
	update Orders set CustomerID = @CustomerID,EmployeeID = @EmployeeID,OrderDate = @OrderDate,RequiredDate = @RequiredDate,ShippedDate = @ShippedDate,ShipVia = @ShipVia,Freight = @Freight,ShipName = @ShipName,ShipAddress = @ShipAddress,ShipCity = @ShipCity,ShipRegion = @ShipRegion,ShipPostalCode = @ShipPostalCode,ShipCountry = @ShipCountry where OrderID = @OrderID;
	update [Order Details] set ProductID = @ProductID,UnitPrice = @UnitPrice,Quantity = @Qunatity,Discount = @Discount where OrderID = @OrderID;
END exec UpdateCustomerOrder  @OrderID = 11078, @CustomerID = 'SEVES', @EmployeeID = 1, @OrderDate ='1999-01-09 00:00:00.000' , @RequiredDate = '1999-03-09 00:00:00.000', @ShippedDate = '1999-02-09 00:00:00.000', @ShipVia = 3,@Freight = 22.34, @ShipName = 'Seven Seas Imports', @ShipAddress = '90 Wadhurst Rd.', @ShipCity = 'Surat', @ShipRegion = 'RJ', @ShipPostalCode = '395007', @ShipCountry = 'India', @ProductID = 42, @UnitPrice = 10.00, @Qunatity = 30, @Discount = 0;