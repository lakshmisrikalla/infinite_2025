Use Assignments;

-- 1) T-SQL


CREATE OR ALTER PROCEDURE sp_ShowPayslip
    @EmpID INT
AS
BEGIN
    
	DECLARE @hra INT, @da INT, @pf INT, @it INT;
    DECLARE @deductions INT, @grossSalary INT, @netSalary INT;
	DECLARE @Ename varchar(20),@Salary INT;
    SELECT @Ename = ename, @Salary = salary
    FROM employees
    WHERE empno = @EmpID;
    IF @Ename IS NULL
    BEGIN
        PRINT 'Employee not found. Please enter a valid EmpID.';
        RETURN;
    END

    
    SET @hra = @salary * 10 / 100;
    SET @da  = @salary * 20 / 100;
    SET @pf  = @salary * 8 / 100;
    SET @it  = @salary * 5 / 100;
    SET @deductions = @pf + @it;
    SET @grossSalary = @salary + @hra + @da;
    SET @netSalary = @grossSalary - @deductions;
	 PRINT 'PAYSLIP ';
	PRINT 'Employee ID        : ' + CAST(@EmpID AS VARCHAR);
    PRINT 'Employee Name      : ' + @Ename;
    PRINT 'Basic Salary     : ' + CAST(@salary AS VARCHAR);
    PRINT 'HRA (10%)        : ' + CAST(@hra AS VARCHAR);
    PRINT 'DA  (20%)        : ' + CAST(@da AS VARCHAR);
    PRINT 'PF  (8%)         : ' + CAST(@pf AS VARCHAR);
    PRINT 'IT  (5%)         : ' + CAST(@it AS VARCHAR);
    PRINT 'Deductions       : ' + CAST(@deductions AS VARCHAR);
    PRINT 'Gross Salary     : ' + CAST(@grossSalary AS VARCHAR);
    PRINT 'Net Salary       : ' + CAST(@netSalary AS VARCHAR);
END
-- To execute

Exec sp_ShowPayslip @EmpID = 7010




-- 2) Trigger

-- create holiday table and insert values
CREATE TABLE Holiday (
    Holiday_Date DATE PRIMARY KEY,
    Holiday_Name VARCHAR(50)
)

-- Insert values holiday table
INSERT INTO Holiday VALUES
('2025-08-15', 'Independence Day'),
('2025-10-20', 'Diwali'),
('2025-01-26', 'Republic Day'),
('2025-12-25', 'Christmas');

--to show holiday table
Select * From Holiday;

-- create trigger 
CREATE OR ALTER TRIGGER trg_HolidayBlock
ON Employees
INSTEAD Of Insert,Update,Delete
AS
BEGIN
    DECLARE @today DAte
	SET @today='2025-08-15'
    DECLARE @holiday_name VARCHAR(50);

    SELECT @holiday_name = holiday_name 
    FROM Holiday 
    WHERE holiday_date = @today;

    IF @holiday_name IS NOT NULL
    BEGIN
        RAISERROR(' Due to %s, you cannot manipulate data on this day.', 16, 1, @holiday_name);
    END
END;

-- try inserting value on holiday

Insert Into Employees Values (7016, 'Rosy', 'Engineer', 68800, 30);




