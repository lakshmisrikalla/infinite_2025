create database Assignments;

use Assignments;

-- creating  clients table

Create Table Clients (
    Client_ID VARCHAR(4) PRIMARY KEY,
    Cname VARCHAR(40) NOT NULL,
    Address VARCHAR(30),
    Email VARCHAR(30) UNIQUE,
    Phone VARCHAR(10),
    Business VARCHAR(20) NOT NULL
);

-- insert values into

Insert Into clients ( Client_ID , Cname, Address, Email, Phone, Business) Values
(1001, 'ACME Utilities', 'Noida', 'contact@acmeutil.com', '9567880032', 'Manufacturing'),
(1002, 'Trackon Consultants', 'Mumbai', 'consult@trackon.com', '8734210090', 'Consultant'),
(1003, 'MoneySaver Distributors','Kolkata','save@moneysaver.com', '7799886655', 'Reseller'),
(1004, 'MoneySaver Distributors', 'Chennai', 'justice@lawful.com', '9210342219', 'Professional');

-- show clients table

Select * From Clients;





-- create Employee table 

Create Table Employees (
    Empno VARCHAR(4) PRIMARY KEY,
    Ename VARCHAR(20) NOT NULL,
    Job VARCHAR(15),
    Salary VARCHAR(7) CHECK (Salary > 0),
    Deptno VARCHAR(2)
);


-- insert values into employees table

Insert Into Employees (Empno, Ename, Job, Salary, Deptno)  Values 
(7001, 'Sandeep', 'Analyst', 25000, 10),
(7002, 'Rajesh', 'Designer', 30000, 10),
(7003, 'Madhav', 'Developer', 40000, 20),
(7004, 'Manoj', 'Developer', 40000, 20),
(7005, 'Abhay', 'Designer', 35000, 10),
(7006, 'Uma', 'Tester', 30000, 30),
(7007, 'Gita', 'Tech. writer', 30000, 40),
(7008, 'Priya', 'Tester', 35000, 30),
(7009, 'Nutan', 'Developer', 45000, 20),
(7010, 'Smita', 'Analyst', 20000, 10),
(7011, 'Anand', 'Project mgr', 65000, 10);

-- show employees table

Select * From Employees;






-- create departments table

Create Table Departments (
    Deptno VARCHAR(2) PRIMARY KEY,
    Dname VARCHAR(15) NOT NULL,
    Loc VARCHAR(20)
);

-- insert values into departments table

Insert Into departments ( Deptno, Dname, Loc) Values
(10,'Design','Pune'),
(20,'Development','Pune'),
(30,'Testing','Mumbai'),
(40,'Document','Mumbai');

-- show departments table

Select * From Departments;

-- Adding foreign key
alter table Employees
add constraint fkey1 foreign key (Deptno) references Departments(Deptno)
 





-- create projects table

Create Table Projects (
    Project_ID INT PRIMARY KEY,
    Descr VARCHAR(30) NOT NULL,   -- Description 
    Start_Date DATE,
    Planned_End_Date DATE,
    Actual_End_Date DATE,
    Budget DECIMAL(10, 2) CHECK (Budget > 0),
    Client_ID INT
);

-- insert values into projects table

Insert Into Projects (Project_ID, Descr, Start_Date, Planned_End_Date, Actual_End_Date, Budget, Client_ID) Values
(401, 'Inventory',      '2011-04-01', '2011-10-01', '2011-10-31', 150000, 1001),
(402, 'Accounting',     '2011-08-01', '2012-01-01', NULL,         500000, 1002),
(403, 'Payroll',        '2011-10-01', '2011-12-31', NULL,          75000, 1003),
(404, 'Contact Mgmt',   '2011-11-01', '2011-12-31', NULL,          50000, 1004);


-- show projects table

Select * From Projects;






-- create EmpProjectTasks

Create Table EmpProjectTasks (
    Project_ID INT,
    Empno INT,
    Start_Date DATE,
    End_Date DATE,
    Task VARCHAR(50),
    Status VARCHAR(20)
);

-- insert values in EmpProjectTasks table

INSERT INTO EmpProjectTasks (Project_ID, Empno, Start_Date, End_Date, Task, Status) VALUES
(401, 7001, '2011-04-01', '2011-04-20', 'System Analysis', 'Completed'),
(401, 7002, '2011-04-21', '2011-05-30', 'System Design', 'Completed'),
(401, 7003, '2011-06-01', '2011-07-15', 'Coding', 'Completed'),
(401, 7004, '2011-07-18', '2011-09-01', 'Coding', 'Completed'),
(401, 7006, '2011-09-03', '2011-09-15', 'Testing', 'Completed'),
(401, 7009, '2011-09-18', '2011-10-05', 'Code Change', 'Completed'),
(401, 7008, '2011-10-06', '2011-10-16', 'Testing', 'Completed'),
(401, 7007, '2011-10-06', '2011-10-22', 'Documentation', 'Completed'),
(401, 7011, '2011-10-22', '2011-10-31', 'Sign off', 'Completed'),
(402, 7010, '2011-08-01', '2011-08-20', 'System Analysis', 'Completed'),
(402, 7002, '2011-08-22', '2011-09-30', 'System Design', 'Completed'),
(402, 7004, '2011-10-01', NULL,        'Coding', 'In Progress');

-- show EmpProjectTasks table

Select * From EmpProjectTasks;



