Use Codechallenge;

-- create employee details table

Create Table Employee_Details (
    Empid INT PRIMARY KEY IDENTITY(1001,1),
    Name VARCHAR(50),
    Salary DECIMAL(10,2),
    Gender VARCHAR(10),
    NetSalary AS (Salary - (Salary * 0.1)) 
);


-- Question 1 - Procedure --

CREATE PROCEDURE InsertEmployeeDetails
    @Name VARCHAR(50),
    @GivenSalary DECIMAL(10,2),
    @Gender VARCHAR(10),
    @GeneratedEmpId INT OUTPUT,
    @FinalSalary DECIMAL(10,2) OUTPUT
AS
BEGIN
    SET @FinalSalary = @GivenSalary - (@GivenSalary * 0.1)

    INSERT INTO Employee_Details (Name, Salary, Gender)
    VALUES (@Name, @FinalSalary, @Gender)

    SET @GeneratedEmpId = SCOPE_IDENTITY()
END

Select * From Employee_Details;




-- Question 2 Procedure
CREATE PROCEDURE UpdateSalaryByEmpId
    @EmpId INT,
    @UpdatedSalary DECIMAL(10,2) OUTPUT
AS
BEGIN
    UPDATE Employee_Details
    SET Salary = Salary + 100
    WHERE Empid = @EmpId;

    SELECT @UpdatedSalary = Salary FROM Employee_Details WHERE Empid = @EmpId;
END;

Select * From Employee_Details;
