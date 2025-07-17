use Assignments;

-- create table Emp

Create Table Emp(
Empno int primary key,
Ename varchar(30) not null,
Job varchar(30) not null,
Mgr_id int,
Hire_date varchar(30),
Sal int,
Comm int,
Deptno int references Dept(deptno)
)

-- insert values into Emp table

Insert Into Emp Values 
(7369, 'SMITH', 'CLERK', 7902, '17-DEC-80', 800, NULL, 20),
(7499, 'ALLEN', 'SALESMAN', 7698, '20-FEB-81', 1600, 300, 30),
(7521, 'WARD', 'SALESMAN', 7698, '22-FEB-81', 1250, 500, 30),
(7566, 'JONES', 'MANAGER', 7839, '02-APR-81', 2975, NULL, 20),
(7654, 'MARTIN', 'SALESMAN', 7698, '28-SEP-81', 1250, 1400, 30),
(7698, 'BLAKE', 'MANAGER', 7839, '01-MAY-81', 2850, NULL, 30),
(7782, 'CLARK', 'MANAGER', 7839, '09-JUN-81', 2450, NULL, 10),
(7788, 'SCOTT', 'ANALYST', 7566, '19-APR-87', 3000, NULL, 20),
(7839, 'KING', 'PRESIDENT', NULL, '17-NOV-81', 5000, NULL, 10),
(7844, 'TURNER', 'SALESMAN', 7698, '08-SEP-81', 1500, 0, 30),
(7876, 'ADAMS', 'CLERK', 7788, '23-MAY-87', 1100, NULL, 20),
(7900, 'JAMES', 'CLERK', 7698, '03-DEC-81', 950, NULL, 30),
(7902, 'FORD', 'ANALYST', 7566, '03-DEC-81', 3000, NULL, 20),
(7934, 'MILLER', 'CLERK', 7782, '23-JAN-82', 1300, NULL, 10)

-- to view Emp table
Select * From Emp;




-- create table Dept 
Create Table Dept(
Deptno int primary key,
Dname varchar(30),
Loc varchar(30)
)

-- insert values into dept table
Insert Into Dept Values
(10,'ACCOUNTING','NEW YORK'),
(20,'RESEARCH','DALLAS'),
(30,'SALES','CHICAGO' ),
(40,'OPERATIONS','BOSTON')
 
 -- to show dept table
 Select * From Dept;


-- Query 1
Select * From Emp Where Ename Like 'A%';


--Query 2
Select * From Emp Where Mgr_id Is NULL;

--Query 3
Select Empno, Ename, Sal From Emp Where Sal BETWEEN 1200 AND 1400;


-- Query 4
-- before raise
Select * From Emp Where Deptno = 20;

-- applying raise
Update Emp SET Sal = Sal * 1.10 Where Deptno = 20;

-- after raise
Select * From Emp Where Deptno = 20;


--Query 5
Select Count(Job) As No_of_clerks From Emp Where Job = 'CLERK';


--Query 6
Select Job, Count(Job) As Total_employees, Avg(sal) As Avg_Salary From Emp
Group By Job;


-- Query 7
-- For lowest salary and the highest salaryy
Select * From Emp 
Where Sal = (Select MIN(Sal) From Emp) OR  Sal = (Select MAX(Sal) From Emp);


--Query 8
Select * From Dept
Where Deptno NOT IN (Select Deptno From Emp);


-- Query 9
Select Ename, Sal From Emp
Where Job = 'Analyst' AND Sal > 1200 AND Deptno = 20
Order By Ename ASC;


--Query 10
Select D.Dname, D.Deptno, SUM(E.Sal) As [Total Salary]
From Dept D
LEFT JOIN Emp E ON D.Deptno = E.Deptno
GROUP BY D.Dname,D.Deptno;


--Query 11
Select Ename, Sal From Emp Where Ename IN ('MILLER', 'SMITH');


--Query 12
Select Ename From Emp Where Ename LIKE 'A%' OR Ename LIKE 'M%';


--Query 13
Select Ename, Sal * 12 AS Yearly_salary From Emp Where Ename = 'SMITH';


--Query 14
Select Ename, Sal From Emp Where Sal NOT BETWEEN 1500 AND 2850;


--Query 15
Select Mgr_id, COUNT(*) As Reportees From Emp
Where Mgr_id IS NOT NULL 
GROUP BY Mgr_id
HAVING COUNT(*) > 2;





















