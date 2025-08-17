Create database ElectricityProject;

Use ElectricityProject;

CREATE TABLE ElectricityBill (
    consumer_number VARCHAR(20),
    consumer_name VARCHAR(50),
    units_consumed INT,
    bill_amount FLOAT
);



SELECT *FROM ElectricityBill;

ALTER TABLE ElectricityBill
ADD bill_id INT IDENTITY(1,1) PRIMARY KEY;
