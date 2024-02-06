use karmadipsolanki_db;
create table department(
dept_id int primary key,
dept_name varchar(100) not NULL);

create table employee(
emp_id int primary key,
dept_id int,
mngr_id int,
emp_name varchar(100) not NULL,
salary int,
foreign key(dept_id) references department(dept_id));

insert into department values(12,'Education');
insert into department values(15,'Health');
insert into department values(10,'Sports');
insert into department values(17,'Sanitary');
insert into department values(5,'HR');
insert into department values(22,'Police');
insert into department values(2,'Forest');

insert into employee values(101,10,1001,'Vraj',1000);
insert into employee values(102,10,1005,'Raj',2000);
insert into employee values(201,12,2001,'Ramesh',10000);
insert into employee values(203,12,2001,'Mahesh',15000);
insert into employee values(205,12,2003,'Suresh',20000);
insert into employee values(207,12,2003,'Rajesh',30000);
insert into employee values(209,12,2005,'Mukesh',5000);
insert into employee values(301,15,3001,'Ram',10000);
insert into employee values(302,15,3004,'Bharat',10000);
insert into employee values(303,15,3009,'Lakhan',10000);
insert into employee values(401,17,4001,'Jeet',100);
insert into employee values(501,5,5002,'Deep',100000);
insert into employee values(502,5,5003,'Rajdeep',150000);
insert into employee values(503,5,5004,'Kuldeep',170000);
insert into employee values(507,5,5002,'Dharmadeep',100000);
insert into employee values(611,22,6001,'Harishchandrasinh',1000009);
insert into employee values(612,22,6001,'Virbhadrasinh',2000009);
insert into employee values(613,22,6001,'Kiritsinh',3000009);
insert into employee values(701,2,7002,'Yuvraj',18000);
insert into employee values(702,2,7002,'Digvijay',19000);
insert into employee values(703,2,7001,'Dharmpal',20000);

1. write a SQL query to find Employees who have the biggest salary in their Department
select e.dept_id,e.emp_name,e.salary from employee e
where e.salary in
(select max(salary) from employee
group by dept_id);

2. write a SQL query to find Departments that have less than 3 people in it
select d.dept_name from department d inner join employee e on e.dept_id = d.dept_id group by d.dept_name having count(e.emp_id) <3;

3. write a SQL query to find All Department along with the number of people there
select d.dept_name,count(e.dept_id) from department d inner join employee e on e.dept_id = d.dept_id group by d.dept_name,e.dept_id ;

4. write a SQL query to find All Department along with the total salary thereselect d.dept_name,sum(e.salary) from department d inner join employee e on e.dept_id = d.dept_id group by d.dept_name;