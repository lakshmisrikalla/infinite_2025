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
Select *FROM LoginCredentials;

Select *FROM PolicyTypes;
Select *FROM Policies;
Select *FROM Clients;
Select *FROM UserPolicies;
Select *FROM Claims;


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
(2, 2, 'Tata Travel Basic', 'ThirdParty', 'Travel insurance for short domestic trips', 'Trip cancellation, lost baggage, medical emergencies', 6, 4200.00, 'Approved'),
(2, 2, 'Tata Travel Premium', 'Comprehensive', 'Extended travel insurance with global coverage', 'COVID protection, evacuation, hotel coverage', 12, 7800.00, 'Approved'),
(3, 2, 'HDFC Travel Secure', 'Comprehensive', 'Travel insurance for international business travelers', 'Medical, legal, and hotel protection', 12, 9600.00, 'Approved');
