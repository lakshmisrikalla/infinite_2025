


-- 1) Query 1

Select DATENAME(dw, '2004-02-13') As 'Birthday Day';




-- 2) Query 2
Select DATEDIFF(DAY, '2004-02-13', GETDATE()) As 'Age In Days';


-- 3) Query 3
Use Codechallenge;

-- creating new Testemp table and inserting values
Create Table TestEmp (
    empno INT,
    ename VARCHAR(50),
    job VARCHAR(20),
    sal INT,
    doj DATE,
    deptno INT
);

Insert Into TestEmp Values
(101, 'Alisa', 'Clerk', 1200, '2018-07-10', 10), 
(102, 'Banu', 'Manager', 3000, '2015-07-22', 20), 
(103, 'Teja', 'Analyst', 2600, '2021-03-05', 30), 
(104, 'Luci', 'Salesman', 2000, '2019-07-15', 30), 
(105, 'Elina', 'Engineer', 2800, '2024-07-01', 40); 

Select * From TestEmp;

-- to show the doj before 5 years people
Select * From TestEmp 
Where MONTH(doj) = MONTH(GETDATE()) AND DATEDIFF(YEAR, doj, GETDATE()) >= 5;



-- 4) Query 4
use Codechallenge;

BEGIN TRANSACTION;

-- a. Insert 3 rows
Insert Into TestEmp Values 
(106, 'Pari', 'Clerk', 1200,'2022-04-22', 10),
(107, 'Dhruv', 'Salesman', 1400,'2021-09-23', 20),
(108, 'Tara', 'Analyst', 1600, '2009-06-01', 30);

-- b. Update second row's salary by 15%
UPDATE TestEmp SET sal = sal * 1.15 WHERE empno = 102;

Select * From TestEmp;

-- c. Delete first row
SAVE TRANSACTION AFTERDELETE
DELETE FROM TestEmp WHERE empno = 101;
ROLLBACK TRANSACTION AFTERDELETE
COMMIT;



-- 5) Query 5
CREATE OR ALTER FUNCTION fn_CalculateBonus(@deptno INT, @salary FLOAT)
RETURNS FLOAT
AS
BEGIN
    DECLARE @bonus FLOAT;

    IF @deptno = 10
        SET @bonus = @salary * 0.15;
    ELSE IF @deptno = 20
        SET @bonus = @salary * 0.20;
    ELSE
        SET @bonus = @salary * 0.05;

    RETURN @bonus;
END;

-- 
SELECT empno, ename, sal, deptno, dbo.fn_CalculateBonus(deptno, sal) AS Bonus 
FROM Testemp;



-- 6) Query 6

Use Assignments;

UPDATE emp SET sal = 1200 WHERE empno = 7844;  


CREATE OR ALTER PROCEDURE sp_UpdateSalesSalary
AS
BEGIN
    UPDATE emp
    SET sal = sal + 500
    WHERE deptno IN (
        SELECT deptno FROM dept WHERE dname = 'Sales'
    ) AND sal < 1500;
END;


-- 
EXEC sp_UpdateSalesSalary;

Select * From Emp;


