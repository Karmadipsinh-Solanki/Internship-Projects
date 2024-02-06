ASSIGNMENT-2

use karmadipsolanki_db;
create table salesman
(salesman_id int primary key,
name varchar(200) not null,
city varchar(200) not null,
commission decimal(14,2) not null);

create table customer(
customer_id int primary key,
cust_name varchar(200) not null,
city varchar(200) not null,
grade int not null,
salesman_id int,
foreign key(salesman_id) references salesman(salesman_id));

create table orders(
ord_no int primary key,
purch_amt decimal(17,2) not null,
ord_date DateTime not null,
salesman_id int,
customer_id int,
foreign key(customer_id) references customer(customer_id),
foreign key(salesman_id) references salesman(salesman_id));
alter table orders 
alter column salesman_id int;
alter table orders 
alter column customer_id int;

insert into salesman values(11,'Pranav','Karwar',200);
insert into salesman values(24,'Prasanna','Bengalore',300);
insert into salesman values(39,'Prajwal','Kodagu',100);
insert into salesman values(44,'Pooja','Hubli',500.5);
insert into salesman values(15,'Prokta','Mysore',200.2);

insert into customer values(101,'Bhargav','Mysore',1,15);
insert into customer values(206,'Ramya','Bengalore',3,24);
insert into customer values(225,'Rajesh','Hubli',2,39);
insert into customer values(324,'Ravi','Mangalore',5,44);
insert into customer values(456,'Rajdeep','Belagavi',3,15);
insert into customer values(501,'Raghu','Dharavad',4,39);
insert into customer values(300,'Bhavya','Bengalore',1,15);
insert into customer values(400,'Amar','Goa',1,NULL);

insert into orders values(5,10000,CAST('2020-03-25' as DateTime),11,101);
insert into orders values(10,5000,CAST('2020-03-25' as DateTime),15,456);
insert into orders values(7,9500,CAST('2020-04-30' as DateTime),44,225);
insert into orders values(11,8700,CAST('2020-07-07' as DateTime),24,324);
insert into orders values(17,1500,CAST('2020-07-07' as DateTime),39,206);
insert into orders values(25,1200,CAST('2020-09-07' as DateTime),NULL,400);

1. write a SQL query to find the salesperson and customer who reside in the same city. Return Salesman, cust_name and city
select s.name as salesman_name,c.cust_name as customer_name,c.city
from salesman s
inner join customer c on c.city = s.city;
 
 2.write a SQL query to find those orders where the order amount exists between 500 and 2000. Return ord_no, purch_amt, cust_name, city
 select o.ord_no,o.purch_amt,c.cust_name,c.city 
 from orders o
 inner join customer c on c.customer_id = o.customer_id
 where o.purch_amt between 500 and 2000;

 3. write a SQL query to find the salesperson(s) and the customer(s) he represents. Return Customer Name, city, Salesman, commission

 4. write a SQL query to find salespeople who received commissions of more than 12 percent from the company.
 Return Customer Name, customer city, Salesman, commission.
 select c.cust_name,c.city,s.name,s.commission
 from customer c
 inner join salesman s on s.salesman_id = c.salesman_id
 where s.commission >= 12;

5. write a SQL query to locate those salespeople who do not live in the same city where their customers live and have received a commission of 
more than 12% from the company. Return Customer Name, customer city, Salesman, salesman city, commission
select c.cust_name,c.city as cust_city,s.name as salesman_name,s.city as salesman_city,s.commission
from customer c
inner join salesman s on s.salesman_id = c.salesman_id
where s.commission >= 12
and c.city != s.city;

6. write a SQL query to find the details of an order. Return ord_no, ord_date,purch_amt, Customer Name, grade, Salesman, commission
select o.ord_no,o.ord_date,o.purch_amt,c.cust_name,c.grade,s.name,s.commission
from orders o
inner join salesman s on o.salesman_id = s.salesman_id 
inner join customer c on o.customer_id = c.customer_id 

7. Write a SQL statement to join the tables salesman, customer and orders so that the 
same column of each table appears once and only the relational rows are returned.
select * from orders inner join customer on orders.customer_id = customer.customer_id inner join salesman on customer.salesman_id = salesman.salesman_id;

8. write a SQL query to display the customer name, customer city, grade, salesman, 
salesman city. The results should be sorted by ascending customer_id.
select c.cust_name,c.city as cust_city,s.name as salesman_name,s.city as salesman_city,s.commission
from customer c
inner join salesman s on s.salesman_id = c.salesman_id
where s.commission >= 12
and c.city != s.city;

9. write a SQL query to find those customers with a grade less than 300. Return cust_name, customer city, grade, Salesman, salesmancity. 
The result should be ordered by ascending customer_id. 
select c.cust_name, c.city as cust_name, c.grade, s.name as salesman, s.city
from customer c
inner join salesman s on s.salesman_id = c.salesman_id
where c.grade < 300
order by customer_id;

10. Write a SQL statement to make a report with customer name, city, order number, order date, and order amount in ascending order according to 
the order date to determine whether any of the existing customers have placed an order or not
select c.cust_name,c.city,o.ord_no,o.ord_date,
CASE when o.ord_no is not null then 'Placed' else 'Not Placed' end as Order_placed 
from customer as c 
left join orders o on c.customer_id = o.customer_id 
order by o.ord_date;

11. Write a SQL statement to generate a report with customer name, city, order number, order date, order amount, salesperson name, and commission 
to determine if any of the existing customers have not placed orders or if they have placed orders through their salesman or by themselves
select c.cust_name, c.city, o.ord_no, o.ord_date, o.purch_amt, s.name as salesman, s.commission,
CASE when o.ord_no is not null then 'Placed' else 'Not Placed' end as Order_placed ,
CASE when s.name is not null then 'Salesman' else 'Themselves' end as Order_Placed_By
from customer c 
left join orders o on o.customer_id = c.customer_id 
left join salesman s on s.salesman_id = c.salesman_id;

12. Write a SQL statement to generate a list in ascending order of salespersons who 
work either for one or more customers or have not yet joined any of the customers
select s.name as salesman, c.cust_name,
CASE when c.salesman_id is not null then 'Customer_joined' else 'CustomerNotjoined' end as "Joined or not"
from salesman s
left join customer c on c.salesman_id = s.salesman_id 

13. write a SQL query to list all salespersons along with customer name, city, grade, order number, date, and amount.
select s.name as Salesman,c.cust_name,c.city,c.grade,o.ord_no,o.ord_date,o.purch_amt
from salesman s
left join customer c on s.salesman_id = c.salesman_id 
left join orders o on o.salesman_id = s.salesman_id 

14. Write a SQL statement to make a list for the salesmen who either work for one or more customers or yet to join any of the customers. 
The customer may have placed, either one or more orders on or above order amount 2000 and must have a grade, or 
he may not have placed any order to the associated supplier.
select * from salesman as s 
left join customer as c on s.salesman_id = c.salesman_id 
left join orders as o on c.customer_id = o.customer_id 
where (o.ord_no is NULL) or (o.ord_no is not NULL and o.purch_amt > 2000 and c.grade is not NULL);

15. Write a SQL statement to generate a list of all the salesmen who either work for one 
or more customers or have yet to join any of them. The customer may have placed 
one or more orders at or above order amount 2000, and must have a grade, or he 
may not have placed any orders to the associated supplier.
select * from salesman as s 
left join customer as c on s.salesman_id = c.salesman_id 
left join orders as o on c.customer_id = o.customer_id 
where (o.ord_no is NULL) or (o.ord_no is not NULL and o.purch_amt > 2000 and c.grade is not NULL);

16. Write a SQL statement to generate a report with the customer name, city, order no. order date, purchase amount for only those customers 
on the list who must have a grade and placed one or more orders or which order(s) have been placed by the customer who neither is 
on the list nor has a grade.
select c.cust_name,c.city,o.ord_no,o.ord_date,o.purch_amt
from customer c
left join orders o on c.customer_id = o.customer_id
where ((c.grade is  not NULL) and (o.customer_id is not NULL)) or ((o.customer_id is NULL) and (c.grade is NULL));

17. Write a SQL query to combine each row of the salesman table with each row of the customer table
select* from salesman cross join customer

18. Write a SQL statement to create a Cartesian product between salesperson and customer, i.e. each salesperson will appear 
for all customers and vice versa for that salesperson who belongs to that city
select* from salesman cross join customer

19. Write a SQL statement to create a Cartesian product between salesperson and customer, i.e. each salesperson will appear for every customer 
and vice versa for those salesmen who belong to a city and customers who require a grade
select* from salesman s cross join customer c where s.city is not NULL and c.grade is not NULL;

20. Write a SQL statement to make a Cartesian product between salesman and customer i.e. each salesman will appear for all customers and vice versa 
for those salesmen who must belong to a city which is not the same as his customer and the customers should have their own grade
select* from salesman s cross join customer c where s.city is not NULL and s.city!= c.city and c.grade is not NULL;