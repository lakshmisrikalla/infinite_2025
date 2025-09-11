Create Database MainProject;

Use MainProject;




--**********IDENTITY & ACCESS**************

-- One login table for all roles. Clients must also supply CompanyCode at login.
CREATE TABLE LoginCredentials (
  LoginID INT IDENTITY PRIMARY KEY,
  Email VARCHAR(100) NULL,                -- NULL for Clients if you want username-only + code
  Username VARCHAR(50) NULL,              -- allow either email or username
  PasswordHash VARCHAR(255) NOT NULL,
  Role VARCHAR(10) CHECK (Role IN ('Admin','Client','User')) NOT NULL,
  IsActive BIT NOT NULL DEFAULT 1,
  CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  CONSTRAINT uq_login_email UNIQUE (Email),
  CONSTRAINT uq_login_username UNIQUE (Username)
);

CREATE TABLE Admins (
  AdminID INT IDENTITY PRIMARY KEY,
  LoginID INT NOT NULL UNIQUE REFERENCES LoginCredentials(LoginID),
  FullName VARCHAR(100) NOT NULL,
  ContactNumber VARCHAR(15) NULL
);

CREATE TABLE Clients (
  ClientID INT IDENTITY PRIMARY KEY,
  LoginID INT NOT NULL UNIQUE REFERENCES LoginCredentials(LoginID),
  CompanyName VARCHAR(150) NOT NULL,
  CompanyCode VARCHAR(32) NOT NULL,                -- required at login (store hashed if needed)
  ContactEmail VARCHAR(100) NULL,
  ContactPhone VARCHAR(20) NULL,
  Status VARCHAR(12) CHECK (Status IN ('Pending','Approved','Blocked')) NOT NULL DEFAULT 'Pending',
  RegisteredAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  CONSTRAINT uq_clients_company UNIQUE (CompanyName),
  CONSTRAINT uq_clients_code UNIQUE (CompanyCode)
);

CREATE TABLE Users (
  UserID INT IDENTITY PRIMARY KEY,
  LoginID INT NOT NULL UNIQUE REFERENCES LoginCredentials(LoginID),
  FullName VARCHAR(100) NOT NULL,
  PhoneNumber VARCHAR(15) NULL,
  Address NVARCHAR(MAX) NULL,
  DateOfBirth DATE NULL,
  Gender VARCHAR(10) NULL,
  Status VARCHAR(10) CHECK (Status IN ('Active','Blocked')) NOT NULL DEFAULT 'Active',
  CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);


--************DOCUMENT************
-- Polymorphic documents with strict visibility
CREATE TABLE Documents (
  DocumentID INT IDENTITY PRIMARY KEY,
  OwnerType VARCHAR(10) CHECK (OwnerType IN ('Client','User','UserPolicy')) NOT NULL,
  OwnerID INT NOT NULL,                   -- FK enforced via app/service layer by OwnerType
  DocumentType VARCHAR(50) NOT NULL,
  FilePath NVARCHAR(500) NOT NULL,
  UploadedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  IsVerified BIT NOT NULL DEFAULT 0,
  VerifiedByRole VARCHAR(10) NULL CHECK (VerifiedByRole IN ('Admin','Client')),
  VerifiedByID INT NULL,                  -- AdminID or ClientID depending on role
  VerifiedAt DATETIME2 NULL,
  -- Admin can only verify Client docs; Client verifies User / UserPolicy docs
  Visibility VARCHAR(30) CHECK (Visibility IN ('ClientOnly','AdminOnly','ClientAndUser')) NOT NULL
);

-- Optional helper views for enforcement (not shown):
-- v_ClientDocuments (OwnerType='Client'); v_UserDocuments (OwnerType in ('User','UserPolicy'))


--**************ProductModel(Policy)***************

CREATE TABLE PolicyTypes (
  PolicyTypeID INT IDENTITY PRIMARY KEY,
  TypeName VARCHAR(20) CHECK (TypeName IN ('Motor','Travel')) NOT NULL,
  Description NVARCHAR(255) NULL,
  IsActive BIT NOT NULL DEFAULT 1,
  CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Policies are created by Clients; go live only after Admin approves
CREATE TABLE Policies (
  PolicyID INT IDENTITY PRIMARY KEY,
  ClientID INT NOT NULL REFERENCES Clients(ClientID),
  PolicyTypeID INT NOT NULL REFERENCES PolicyTypes(PolicyTypeID),
  PolicyName VARCHAR(120) NOT NULL,
  PlanKind VARCHAR(30) CHECK (PlanKind IN ('ThirdParty','Comprehensive')) NULL,
  Description NVARCHAR(MAX) NULL,
  CoverageDetails NVARCHAR(MAX) NULL,
  DurationMonths INT NOT NULL,
  BasePremium DECIMAL(18,2) NOT NULL,
  Status VARCHAR(15) CHECK (Status IN ('Draft','Pending','Approved','Rejected','Inactive','Active'))
          NOT NULL DEFAULT 'Pending',
  AdminNotes NVARCHAR(MAX) NULL,
  CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Explicit admin approval trail for client policies
CREATE TABLE PolicyApprovals (
  ApprovalID INT IDENTITY PRIMARY KEY,
  PolicyID INT NOT NULL REFERENCES Policies(PolicyID),
  AdminID INT NOT NULL REFERENCES Admins(AdminID),
  Decision VARCHAR(10) CHECK (Decision IN ('Approved','Rejected')) NOT NULL,
  DecisionAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  Notes NVARCHAR(MAX) NULL
);


--*********User Purchases,Renewals,Claims***********

CREATE TABLE UserPolicies (
  UserPolicyID INT IDENTITY PRIMARY KEY,
  UserID INT NOT NULL REFERENCES Users(UserID),
  PolicyID INT NOT NULL REFERENCES Policies(PolicyID),
  InsuranceType VARCHAR(50) NULL,
  StartDate DATE NOT NULL,
  EndDate DATE NOT NULL,
  Status VARCHAR(12) CHECK (Status IN ('Pending','Active','Expired','Cancelled')) NOT NULL DEFAULT 'Pending',
  PaymentStatus VARCHAR(12) CHECK (PaymentStatus IN ('Unpaid','Paid','Refunded')) NOT NULL DEFAULT 'Unpaid',
  CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  NomineeName VARCHAR(100) NULL
);
 
CREATE TABLE TravelDetails (
  TravelDetailsID INT IDENTITY PRIMARY KEY,
  UserPolicyID INT NOT NULL REFERENCES UserPolicies(UserPolicyID),
  PersonName VARCHAR(100) NOT NULL,
  Age INT NOT NULL,
  Gender VARCHAR(12) NOT NULL,
  DOB DATE NOT NULL,
  HealthIssues NVARCHAR(MAX) NULL
);
 
CREATE TABLE VehicleDetails (
  VehicleDetailsID INT IDENTITY PRIMARY KEY,
  UserPolicyID INT NOT NULL REFERENCES UserPolicies(UserPolicyID),
  VehicleType VARCHAR(50) NOT NULL,         -- e.g., Car, Bike, Truck
  VehicleName VARCHAR(100) NOT NULL,        -- e.g., Honda, Maruti
  Model VARCHAR(100) NOT NULL,              -- e.g., Activa 5G, Swift
  DrivingLicense VARCHAR(50) NOT NULL,      -- License of driver/owner
  RegistrationNumber VARCHAR(50) NOT NULL,  -- Registration/Number Plate
  RCNumber VARCHAR(100) NOT NULL,           -- RC document number
  RegistrationDate DATE NULL,           -- Original registration date 
  ExpiryDate DATE NOT NULL,                 -- Registration expiry
  EngineNumber VARCHAR(100) NOT NULL,       -- Engine number
  ChassisNumber VARCHAR(100) NOT NULL,      -- Chassis number
  HolderName VARCHAR(100) NOT NULL          -- Owner's name
);

-- Client must verify user/userpolicy docs before activation
CREATE TABLE UserPolicyApprovals (
 UserPolicyApprovalID INT IDENTITY PRIMARY KEY,
 UserPolicyID INT NOT NULL REFERENCES UserPolicies(UserPolicyID),
 ClientID INT NOT NULL REFERENCES Clients(ClientID),
 Decision VARCHAR(10) CHECK (Decision IN ('Approved','Rejected')) NOT NULL,
 DecisionAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
 Notes NVARCHAR(MAX) NULL
);

CREATE TABLE Payments (
 PaymentID INT IDENTITY PRIMARY KEY,
 UserPolicyID INT NOT NULL REFERENCES UserPolicies(UserPolicyID),
 Amount DECIMAL(18,2) NOT NULL,
 Mode VARCHAR(20) CHECK (Mode IN ('Card','UPI','NetBanking','Wallet')) NOT NULL,
 PaidAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
 Status VARCHAR(12) CHECK (Status IN ('Success','Failed','Pending','Refunded')) NOT NULL,
 GatewayRef VARCHAR(100) NULL
);

CREATE TABLE Renewals (
 RenewalID INT IDENTITY PRIMARY KEY,
 UserPolicyID INT NOT NULL REFERENCES UserPolicies(UserPolicyID),
 RequestedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
 ApprovedAt DATETIME2 NULL,
 Amount DECIMAL(18,2) NOT NULL,
 Status VARCHAR(12) CHECK (Status IN ('Pending','Approved','Rejected','Paid')) NOT NULL DEFAULT 'Pending'
);

CREATE TABLE Claims (
 ClaimID INT IDENTITY PRIMARY KEY,
 UserPolicyID INT NOT NULL REFERENCES UserPolicies(UserPolicyID),
 ClaimDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
 ClaimType VARCHAR(50) NOT NULL,
 Reason NVARCHAR(MAX) NULL,
 ClaimedAmount DECIMAL(18,2) NOT NULL,
 Status VARCHAR(15) CHECK (Status IN ('Submitted','UnderReview','Approved','Rejected','Settled')) NOT NULL DEFAULT 'Submitted',
 ApprovedAmount DECIMAL(18,2) NULL,
 DecisionAt DATETIME2 NULL
);


--****************Messaging, Emails,Clients-AdminPayments-Calculations-Due Dates*************************

-- Strictly user<->client (no admin in chat)
CREATE TABLE Conversations (
  ConversationID INT IDENTITY PRIMARY KEY,
  UserID INT NOT NULL REFERENCES Users(UserID),
  ClientID INT NOT NULL REFERENCES Clients(ClientID),
  CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  CONSTRAINT uq_user_client UNIQUE (UserID, ClientID)   -- one thread per pair (optional)
);

CREATE TABLE Messages (
  MessageID INT IDENTITY PRIMARY KEY,
  ConversationID INT NOT NULL REFERENCES Conversations(ConversationID),
  SenderRole VARCHAR(6) CHECK (SenderRole IN ('User','Client')) NOT NULL,
  SenderID INT NOT NULL,                                  -- UserID or ClientID as per role
  Body NVARCHAR(MAX) NOT NULL,
  SentAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Admin emails (welcome, reminders, activations, deactivations)
CREATE TABLE EmailLogs (
  EmailID INT IDENTITY PRIMARY KEY,
  RecipientType VARCHAR(6) CHECK (RecipientType IN ('User','Client')) NOT NULL,
  RecipientID INT NOT NULL,
  Subject NVARCHAR(255) NOT NULL,
  Body NVARCHAR(MAX) NOT NULL,
  EmailType VARCHAR(30) NULL,
  SentAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Client pays admin for platform/maintenance
CREATE TABLE ClientPayments (
  ClientPaymentID INT IDENTITY PRIMARY KEY,
  ClientID INT NOT NULL REFERENCES Clients(ClientID),
  Amount DECIMAL(18,2) NOT NULL,
  PaidAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  Status VARCHAR(12) CHECK (Status IN ('Success','Failed','Pending')) NOT NULL,
  Description NVARCHAR(255) NULL
);

-- Premium estimate history (user-side calculators)
CREATE TABLE PremiumCalculations (
  CalculationID INT IDENTITY PRIMARY KEY,
  UserID INT NOT NULL REFERENCES Users(UserID),
  PolicyTypeID INT NOT NULL REFERENCES PolicyTypes(PolicyTypeID),
  VehicleModel VARCHAR(100) NULL,
  VehicleAge INT NULL,
  TravelDetails NVARCHAR(MAX) NULL,
  EstimatedPremium DECIMAL(18,2) NOT NULL,
  CalculatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Reminders for unpaid dues / renewals
CREATE TABLE PolicyDueDates (
  DueDateID INT IDENTITY PRIMARY KEY,
  UserPolicyID INT NOT NULL REFERENCES UserPolicies(UserPolicyID),
  DueDate DATETIME2 NOT NULL,
  IsPaid BIT NOT NULL DEFAULT 0,
  ReminderSent BIT NOT NULL DEFAULT 0
);

Select *FROM Payments;
Select *FROM PolicyDueDates;
Select *FROM Users;
SELECT *FROM Clients;
Select *FROM LoginCredentials;
Select *FROM Documents;
Select *FROM TravelDetails;

SELECT *FROM VehicleDetails;
SELECT *FROM UserPolicyApprovals;
Select *FROM PolicyTypes;
Select *FROM Policies;
Select *FROM Clients;
Select *FROM UserPolicies;
Select *FROM Claims;

-- 1. Add Pending to the check constraint (example for SQL Server)
ALTER TABLE UserPolicyApprovals DROP CONSTRAINT IF EXISTS CK_UserPolicyApprovals_Decision;
 
ALTER TABLE UserPolicyApprovals
ADD CONSTRAINT CK_UserPolicyApprovals_Decision
CHECK (Decision IN ('Pending','Approved','Rejected'));
 
-- 2. (Optional) set default to 'Pending' so inserts without Decision get Pending automatically
ALTER TABLE UserPolicyApprovals
ADD CONSTRAINT DF_UserPolicyApprovals_Decision DEFAULT 'Pending' FOR Decision;

ALTER TABLE dbo.Policies
ADD IsActive bit NULL;

ALTER TABLE UserPolicyApprovals
DROP CONSTRAINT CK__UserPolic__Decis__797309D9;

ALTER TABLE UserPolicyApprovals
ADD CONSTRAINT CK_UserPolicyApprovals_Decision_Valid
CHECK (Decision IN ('Approved', 'Rejected', 'Pending'));


UPDATE Documents SET IsVerified='1' WHERE OwnerID=9;

INSERT INTO LoginCredentials (Username, Email, PasswordHash, Role)
VALUES 
('ApolloSecure', 'apollo@secure.com', 'HashApollo123', 'Client'),
('TataTravel', 'tata@travel.com', 'HashTata456', 'Client'),
('HDFCProtect', 'hdfc@protect.com', 'HashHDFC789', 'Client');

INSERT INTO Clients (LoginID, CompanyName, CompanyCode, ContactEmail, ContactPhone, Status)
VALUES 
(1, 'Apollo Secure Ltd', 'APL001', 'contact@apollo.com', '9876543210', 'Approved'),
(2, 'Tata Travel Guard', 'TTG002', 'support@tatatravel.com', '9123456780', 'Approved'),
(3, 'HDFC Protect Pvt Ltd', 'HDFC003', 'info@hdfcprotect.com', '9988776655', 'Approved');


INSERT INTO PolicyTypes (TypeName, Description)
VALUES 
('Motor', 'Insurance for cars, bikes, and commercial vehicles'),
('Travel', 'Insurance for domestic and international travel');



INSERT INTO Policies (ClientID, PolicyTypeID, PolicyName, PlanKind, Description, CoverageDetails, DurationMonths, BasePremium, Status)
VALUES 
(1, 1, 'Apollo Motor Shield', 'Comprehensive', 'Full coverage for private vehicles', 'Covers theft, damage, third-party liability', 12, 8200.00, 'Approved'),
(1, 1, 'Apollo Bike Basic', 'ThirdParty', 'Basic protection for two-wheelers', 'Third-party liability only', 12, 3100.00, 'Approved'),
(2, 1, 'Tata Car Protect', 'Comprehensive', 'Car insurance with roadside assistance', 'Covers accidents, theft, fire, and towing', 12, 8900.00, 'Approved'),
(3, 1, 'HDFC Motor Max', 'Comprehensive', 'Premium motor insurance for SUVs', 'Includes engine protection, zero depreciation', 12, 10500.00, 'Approved');

INSERT INTO Policies (ClientID, PolicyTypeID, PolicyName, PlanKind, Description, CoverageDetails, DurationMonths, BasePremium, Status)
VALUES 
(3, 2, 'HDFC Motor Max 2', 'Comprehensive', 'Premium motor insurance for XUVs', 'Includes engine protection, zero depreciation', 12, 10500.00, 'Pending');

INSERT INTO Policies (ClientID, PolicyTypeID, PolicyName, PlanKind, Description, CoverageDetails, DurationMonths, BasePremium, Status)
VALUES 
(2, 2, 'Tata Travel Basic', 'ThirdParty', 'Travel insurance for short domestic trips', 'Trip cancellation, lost baggage, medical emergencies', 6, 4200.00, 'Approved'),
(2, 2, 'Tata Travel Premium', 'Comprehensive', 'Extended travel insurance with global coverage', 'COVID protection, evacuation, hotel coverage', 12, 7800.00, 'Approved'),
(3, 2, 'HDFC Travel Secure', 'Comprehensive', 'Travel insurance for international business travelers', 'Medical, legal, and hotel protection', 12, 9600.00, 'Approved');




INSERT INTO UserPolicies (UserID, PolicyID, InsuranceType, StartDate, EndDate, Status, CreatedAt)
 
VALUES (1, 3, 'Travel', GETDATE(), DATEADD(year, 1, GETDATE()), 'Active', GETDATE());
 
 GO
 
SET NOCOUNT ON;
PRINT '--- Start: Ensure UserPolicyApprovals supports Pending ---';
 
IF OBJECT_ID('dbo.UserPolicyApprovals', 'U') IS NULL
BEGIN
    PRINT 'Table dbo.UserPolicyApprovals does not exist. Creating table...';
    CREATE TABLE dbo.UserPolicyApprovals (
        UserPolicyApprovalID INT IDENTITY PRIMARY KEY,
        UserPolicyID INT NOT NULL REFERENCES dbo.UserPolicies(UserPolicyID),
        ClientID INT NOT NULL REFERENCES dbo.Clients(ClientID),
        Decision VARCHAR(10) NOT NULL CONSTRAINT DF_UserPolicyApprovals_Decision DEFAULT('Pending'),
        DecisionAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        Notes NVARCHAR(MAX) NULL
    );
    PRINT 'Table created. Adding CHECK constraint...';
 
    DECLARE @newName SYSNAME = N'CK_UserPolicyApprovals_Decision_' + LEFT(REPLACE(CONVERT(varchar(36), NEWID()), '-', ''), 8);
    DECLARE @sqlCreateConstraint NVARCHAR(MAX) =
        N'ALTER TABLE dbo.UserPolicyApprovals ADD CONSTRAINT [' + @newName + '] CHECK (Decision IN (''Approved'',''Rejected'',''Pending''));';
    EXEC sp_executesql @sqlCreateConstraint;
 
    PRINT 'Done: table created with Decision default Pending and CHECK constraint.';
    RETURN;
END
 
PRINT 'Table exists. Searching & dropping Decision-related check constraints...';
 
-- Find constraints to drop
CREATE TABLE #ConstraintsToDrop (
    ConstraintName SYSNAME,
    Definition NVARCHAR(MAX)
);
 
INSERT INTO #ConstraintsToDrop (ConstraintName, Definition)
SELECT cc.name, cc.definition
FROM sys.check_constraints cc
WHERE cc.parent_object_id = OBJECT_ID('dbo.UserPolicyApprovals')
  AND cc.definition LIKE '%Decision%';
 
IF EXISTS(SELECT 1 FROM #ConstraintsToDrop)
BEGIN
    PRINT 'Found these Decision-related CHECK constraints:';
    SELECT ConstraintName, Definition FROM #ConstraintsToDrop;
 
    DECLARE @cname SYSNAME;
    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
    SELECT ConstraintName FROM #ConstraintsToDrop;
 
    OPEN cur;
    FETCH NEXT FROM cur INTO @cname;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        DECLARE @dropSql NVARCHAR(MAX) = N'ALTER TABLE dbo.UserPolicyApprovals DROP CONSTRAINT [' + @cname + N'];';
        PRINT 'Dropping: ' + @cname;
        EXEC sp_executesql @dropSql;
        FETCH NEXT FROM cur INTO @cname;
    END
    CLOSE cur;
    DEALLOCATE cur;
END
ELSE
BEGIN
    PRINT 'No Decision-related CHECK constraints found.';
END
 
DROP TABLE IF EXISTS #ConstraintsToDrop;
 
PRINT 'Ensuring Decision column exists and is VARCHAR(10) NOT NULL...';
IF COL_LENGTH('dbo.UserPolicyApprovals', 'Decision') IS NULL
BEGIN
    PRINT 'Decision column missing. Adding Decision column with default Pending...';
    ALTER TABLE dbo.UserPolicyApprovals
    ADD Decision VARCHAR(10) NOT NULL CONSTRAINT DF_UserPolicyApprovals_Decision DEFAULT('Pending');
END
ELSE
BEGIN
    -- Alter column to VARCHAR(10) NOT NULL if necessary
    DECLARE @colType nvarchar(200);
    SELECT @colType = DATA_TYPE + CASE WHEN CHARACTER_MAXIMUM_LENGTH IS NOT NULL THEN '(' + CAST(CHARACTER_MAXIMUM_LENGTH AS NVARCHAR(10)) + ')' ELSE '' END
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'UserPolicyApprovals' AND COLUMN_NAME = 'Decision';
 
    PRINT 'Current Decision column type: ' + ISNULL(@colType, '(unknown)');
 
    -- If not varchar(10) or nullable, alter it (this will fail if existing data cannot be converted)
    BEGIN TRY
        ALTER TABLE dbo.UserPolicyApprovals
        ALTER COLUMN Decision VARCHAR(10) NOT NULL;
        PRINT 'Altered Decision column to VARCHAR(10) NOT NULL (if different).';
    END TRY
    BEGIN CATCH
        PRINT 'Warning: could not alter Decision column. Error: ' + ERROR_MESSAGE();
    END CATCH
END
 
-- Ensure a default constraint exists; if not, add one (safe add inside TRY)
DECLARE @hasDefault INT = (
    SELECT COUNT(1)
    FROM sys.default_constraints dc
    JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE c.object_id = OBJECT_ID('dbo.UserPolicyApprovals') AND c.name = 'Decision'
);
 
IF @hasDefault = 0
BEGIN
    BEGIN TRY
        ALTER TABLE dbo.UserPolicyApprovals
        ADD CONSTRAINT DF_UserPolicyApprovals_Decision DEFAULT('Pending') FOR Decision;
        PRINT 'Added default DF_UserPolicyApprovals_Decision (Pending).';
    END TRY
    BEGIN CATCH
        PRINT 'Could not add default constraint: ' + ERROR_MESSAGE();
    END CATCH
END
ELSE
BEGIN
    PRINT 'Default for Decision already exists.';
END
 
-- Add new unique-named CHECK constraint allowing Pending
DECLARE @uniqueSuffix NVARCHAR(12) = LEFT(REPLACE(CONVERT(varchar(36), NEWID()), '-', ''), 8);
DECLARE @newConstraintName SYSNAME = N'CK_UserPolicyApprovals_Decision_' + @uniqueSuffix;
DECLARE @addCheckSql NVARCHAR(MAX) = N'ALTER TABLE dbo.UserPolicyApprovals ADD CONSTRAINT [' + @newConstraintName + N'] CHECK (Decision IN (''Approved'',''Rejected'',''Pending''));';
 
BEGIN TRY
    EXEC sp_executesql @addCheckSql;
    PRINT 'Added CHECK constraint: ' + @newConstraintName;
END TRY
BEGIN CATCH
    PRINT 'Failed to add CHECK constraint: ' + ERROR_MESSAGE();
END CATCH
 
PRINT 'Verification: list current check constraints on table:';
SELECT cc.name AS ConstraintName, cc.definition
FROM sys.check_constraints cc
WHERE cc.parent_object_id = OBJECT_ID('dbo.UserPolicyApprovals');
 
PRINT '--- Done ---';
GO


