Use Assignments;

-- 
Select *From Emp;
Select *From Dept;

-- Query 1

Select Ename From Emp Where Job = 'MANAGER';

-- Query 2
Select Ename, Sal From Emp Where Sal > 1000;

-- Query 3
Select Ename, Sal From Emp Where Ename <> 'JAMES';

-- Query 4
Select * From Emp Where Ename LIKE 'S%';

-- Query 5
Select Ename From Emp Where Ename LIKE '%A%';

-- Query 6
Select * From Emp Where Ename LIKE '__L%'

-- Query 7
Select Ename, Sal / 30 As 'Daily Salary' From Emp Where Ename = 'JONES';

-- Query 8
Select Sum(Sal) As 'Total Monthly Salary' From Emp;

-- Query 9
Select Avg(Sal) * 12 As 'Average salary of the month' From Emp;

-- Query 10
Select Ename, Job, Sal, Deptno From Emp 
Where Empno In (
    Select Empno From Emp Where Deptno = 30
)
And Job Not In ('SALESMAN');


-- Query 11
Select Distinct(e.Deptno),d.Dname From Emp e 
Join Dept d On e.Deptno=d.Deptno

-- Query 12
Select Ename As Employee, Sal As 'Monthly Salary' From Emp
Where Sal > 1500 And Deptno In (10, 30);

-- Query 13
Select Ename, Job, Sal From Emp
Where Job In ('MANAGER', 'ANALYST') And Sal Not In (1000, 3000, 5000);

-- Query 14
Select Ename, Sal, Comm FROM Emp
Where Comm > Sal * 1.10;

-- Query 15
Select Ename From Emp
Where Ename Like '%L%L%' And (Deptno = 30 Or Mgr_id = 7782);

-- Query 16

Select Ename From Emp 
Where Datediff(Year, Hire_date, Getdate()) Between 31 And 39;

Select Count(*) As 'Total Experienced Employees' 
From Emp 
Where Datediff(Year, Hire_date, Getdate()) Between 31 And 39;



-- Query 17
Select Deptno, Ename From Emp
Order By Deptno ASC, Ename DESC;

-- Query 18

Select Ename, Hire_date,
Datediff(YEAR, Hire_date, GETDATE()) AS 'Experience Years' From Emp
Where Ename = 'MILLER';
