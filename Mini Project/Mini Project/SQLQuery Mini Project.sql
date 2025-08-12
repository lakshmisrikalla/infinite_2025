create database Miniproject;

use Miniproject;
-- Admin Table
CREATE TABLE Admin (
    AdminID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE,
    Password NVARCHAR(50) 
);

-- inserting values in admin
INSERT INTO Admin (Username, Password) VALUES ('lucky', 'lucky@123');

-- Customer Table
CREATE TABLE Customer (
    CustID INT PRIMARY KEY IDENTITY(1,1),
    CustName NVARCHAR(100),
    Phone NVARCHAR(15),
    MailID NVARCHAR(100)
);
 
ALTER TABLE Customer ADD Username NVARCHAR(50) UNIQUE;
ALTER TABLE Customer ADD Password NVARCHAR(50);

-- Train Table
CREATE TABLE Train (
    TrainNo INT PRIMARY KEY IDENTITY(1001,1),
    TrainName NVARCHAR(100),
    Source NVARCHAR(100),
    Destination NVARCHAR(100),
    SleeperSeats INT,
    SecondACSeats INT,
    ThirdACSeats INT,
    SleeperFare DECIMAL(10,2),
    SecondACFare DECIMAL(10,2),
    ThirdACFare DECIMAL(10,2),
    IsActive BIT DEFAULT 1
);

-- Reservation Table
CREATE TABLE Reservation (
    BookingID INT PRIMARY KEY IDENTITY(5001,1),
    CustID INT FOREIGN KEY REFERENCES Customer(CustID),
    PassengerName NVARCHAR(100),
    TravelDate DATE,
    Class NVARCHAR(20), -- Sleeper / 2nd AC / 3rd AC
    BerthAllotment NVARCHAR(50),
    TotalCost DECIMAL(10,2),
    BookingDate DATETIME DEFAULT GETDATE(),
    IsCancelled BIT DEFAULT 0
);

-- Cancellation Table
CREATE TABLE Cancellation (
    BookingID INT PRIMARY KEY,
    TicketCancelled BIT DEFAULT 1,
    RefundAmount DECIMAL(10,2),
    CancellationDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (BookingID) REFERENCES Reservation(BookingID)
);
 

-- Stored Procedure: Admin Login
CREATE PROCEDURE sp_AdminLogin
    @Username NVARCHAR(50),
    @Password NVARCHAR(50)
AS
BEGIN
    SELECT * FROM Admin WHERE Username = @Username AND Password = @Password;
END;

-- Stored Procedure: Customer Login
CREATE PROCEDURE sp_CustomerLogin
    @Username NVARCHAR(50),
    @Password NVARCHAR(50)
AS
BEGIN
    SELECT * FROM Customer WHERE Username = @Username AND Password = @Password;
END;
--


-- sample trains 
USE Miniproject;

INSERT INTO Train (TrainName, Source, Destination, SleeperSeats, SecondACSeats, ThirdACSeats,SleeperFare, SecondACFare, ThirdACFare, IsActive)VALUES

('Godavari Express', 'Visakhapatnam', 'Hyderabad',120, 40, 60,350.00, 950.00, 700.00,1),
('Konark Express', 'Mumbai', 'Bhubaneswar',150, 50, 80,400.00, 1000.00, 750.00,1),
('Coromandel Express', 'Kolkata', 'Chennai',180, 60, 90,420.00, 1100.00, 800.00,1),
('Vande Bharat Express', 'Delhi', 'Varanasi',100, 30, 50,600.00, 1500.00, 1100.00,1),
('Palnadu Express', 'Guntur', 'Vijayawada',80, 20, 30,150.00, 500.00, 350.00,1);

-- To view 

SELECT *FROM Admin;

SELECT *FROM Customer;

SELECT *FROM Train;

SELECT *FROM Reservation;

SELECT *FROM Cancellation;

--
ALTER TABLE Reservation ADD TrainNo INT FOREIGN KEY REFERENCES Train(TrainNo);

ALTER TABLE Reservation ADD Status NVARCHAR(20) DEFAULT 'Confirmed';

SELECT AdminID FROM Admin WHERE Username = 'lucky' AND Password = 'lucky@123'
