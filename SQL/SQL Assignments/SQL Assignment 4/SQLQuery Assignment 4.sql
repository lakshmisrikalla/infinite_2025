use Assignments;


-- 1) factorial of given number
Begin
    Declare @num Int = 2
    Declare @fact Int = 1
    Declare @var Int = 1

    While @var <= @num
    Begin
        Set @fact = @fact * @var
        Set @var = @var + 1
   
   End
    Print 'Factorial of ' + CAST(@num As Varchar) + ' is: ' + CAST(@fact As Varchar)
End



--2) multiplication table 

Create OR Alter Procedure sp_MultiplicationTable @number Int, @upto Int
As
Begin
    Declare @var INT = 1
    While @var <= @upto
    Begin
        Print CAST(@number As Varchar) + ' x ' + CAST(@var As Varchar) + ' = ' + CAST(@number * @var As Varchar)
        Set @var = @var + 1
    End
End

-- Example execution
sp_MultiplicationTable 4, 25



--3)

--create student table
Create Table Student (
    Sid INT PRIMARY KEY,
    Sname VARCHAR(50)
);

-- insert values into student table
Insert Into Student Values 
(1, 'Jack'), 
(2, 'Rithvik'), 
(3, 'Jaspreeth'),
(4, 'Praveen'), 
(5, 'Bisa'), 
(6, 'Suraj');

--show students table
Select * From Student;


-- create marks table
Create Table Marks (
    Mid INT PRIMARY KEY,
    Sid INT,
    Score INT
);

-- insert values into marks table
Insert Into Marks Values 
(1, 1, 23), 
(2, 6, 95), 
(3, 4, 98),
(4, 2, 17), 
(5, 3, 53), 
(6, 5, 13);

-- Show marks table
Select * From Marks;

Create OR Alter Function fn_GetStatus (@score INT)
Returns Varchar(10) As
Begin
    Declare @result Varchar(10)

    If @score >= 50
        Set @result = 'Pass'
    Else
        Set @result = 'Fail'

    Return @result  
End


Select stu.Sid, stu.Sname, m.Score, dbo.fn_GetStatus(m.Score) As Status 
From Student stu 
Join Marks m ON stu.Sid = m.Sid
