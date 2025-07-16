create database Codechallenge;
use Codechallenge;

-- Creating books table
Create Table books (
    id INT PRIMARY KEY,
    title VARCHAR(255),
    author VARCHAR(255),
    isbn VARCHAR(13) UNIQUE,
    published_date DATETIME
);

-- Entering values
Insert Into books (id, title, author, isbn, published_date) Values
(1, 'My First SQL book', 'Mary Parker', '981483029127', '2012-02-22 12:08:17'),
(2, 'My Second SQL book', 'John Mayer', '857300923713', '1972-07-03 09:22:45'),
(3, 'My Third SQL book', 'Cary Flint', '523120967812', '2015-10-18 14:05:44');

-- to show books table
select * From books;

-- create reviews table
Create Table reviews (
    id INT PRIMARY KEY,
    book_id INT,
    reviewer_name VARCHAR(100),
    content VARCHAR(255),
    rating INT,
    published_date DATETIME,
    FOREIGN KEY (book_id) REFERENCES books(id)
);

-- enter values into reviews table
Insert Into reviews (id, book_id, reviewer_name, content, rating, published_date) Values
(1, 1, 'John Smith', 'My first review', 4, '2017-12-10 05:50:11.1'),
(2, 2, 'John Smith', 'My second review', 5, '2017-10-13 15:05:12.6'),
(3, 2, 'Alice Walker', 'Another review', 1, '2017-10-22 23:47:10.7');

-- show reviews table
select * From reviews;


--  Q1 Query1
select * from books Where author like '%er';

-- Q1 Query2

select books.title,books.author,reviews.reviewer_name from books
join reviews on books.id = reviews.book_id;


--Q2 Query 1

Select reviewer_name From reviews
Group By reviewer_name
Having COUNT( reviewer_name ) > 1;

-- creating company table
Create Table company (
    id INT PRIMARY KEY,
    name VARCHAR(100),
    age INT,
    address VARCHAR(100),
    salary DECIMAL(10, 2)
);

-- Enter values into company table
Insert Into company (id, name, age, address, salary) VALUES
(1, 'Ramesh',   32, 'Ahmedabad', 2000.00),
(2, 'Khilan',   25, 'Delhi',     1500.00),
(3, 'kaushik',  23, 'Kota',      2000.00),
(4, 'Chaitali', 25, 'Mumbai',    6500.00),
(5, 'Hardik',   27, 'Bhopal',    8500.00),
(6, 'Komal',    22, 'MP',        4500.00),
(7, 'Muffy',    24, 'Indore',   10000.00);

-- show company table
Select * From  company;

-- Q3 Query 1
Select name From company
Where address LIKE '%o%';

-- create orders table
Create Table orders (
    OID INT PRIMARY KEY,
    DATE DATETIME,
    CUSTOMER_ID INT,
    AMOUNT DECIMAL(10, 2)
);

-- enter values into orders table
Insert Into orders (OID, DATE, CUSTOMER_ID, AMOUNT) VALUES
(102, '2009-10-08 00:00:00', 3, 3000.00),
(100, '2009-10-08 00:00:00', 3, 1500.00),
(101, '2009-11-20 00:00:00', 2, 1560.00),
(103, '2008-05-20 00:00:00', 4, 2060.00);


-- show orders table
Select * From orders;


-- Q4 Query 1

Select DATE,Count(Customer_id) As Total_No_of_Customers From Orders
Group By Date
Having Count(Customer_id) > 1;



--Q5 updating company table
UPDATE company
SET salary = NULL
WHERE id = 6 OR id = 7;

--view updated table company 
Select * From company;


-- create table studentdetails

Create Table Studentdetails (
    RegisterNo INT PRIMARY KEY,
    Name VARCHAR(50),
    Age INT,
    Qualification VARCHAR(50),
    MobileNo VARCHAR(15),
    Mail_id VARCHAR(100),
    Location VARCHAR(50),
    Gender CHAR(1)
);

-- enter values into studentdetails table

Insert Into Studentdetails (RegisterNo, Name, Age, Qualification, MobileNo, Mail_id, Location, Gender) VALUES
(2, 'Sai',        22, 'B.E',    '9952836777', 'Sai@gmail.com',      'Chennai', 'M'),
(3, 'Kumar',      20, 'BSC',    '7890125648', 'Kumar@gmail.com',    'Madurai', 'M'),
(4, 'Selvi',      22, 'B.Tech', '8904567342', 'selvi@gmail.com',    'Selam',   'F'),
(5, 'Nisha',      25, 'M.E',    '7834672310', 'Nisha@gmail.com',    'Theni',   'F'),
(6, 'SaiSaran',   21, 'B.A',    '7890345678', 'saran@gmail.com',    'Madurai', 'F'),
(7, 'Tom',        23, 'BCA',    '8901234675', 'Tom@gmail.com',      'Pune',    'M');

-- view studentdetails
Select * From Studentdetails;



-- Q5 Query 1 of company table
Select Lower(name) As Name_of_the_Employee From company
Where salary is NULL;

-- Q5 Query 2 of studentdetails
Select Gender, COUNT( Gender ) AS TotalGender_Count From Studentdetails
Group By Gender;



