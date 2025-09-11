using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace LTI.Infrastructure
{
    public static class DbBootstrapper
    {
        public static void EnsureAuditAndBlacklist()
        {
            var cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AdminAuditLogs')
BEGIN
    CREATE TABLE dbo.AdminAuditLogs(
        AuditID     INT IDENTITY PRIMARY KEY,
        AdminID     INT NOT NULL,
        ActionType  NVARCHAR(100) NOT NULL,
        TargetType  NVARCHAR(50) NULL,
        TargetID    INT NULL,
        Details     NVARCHAR(MAX) NULL,
        CreatedAt   DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
    ALTER TABLE dbo.AdminAuditLogs
    ADD CONSTRAINT FK_AdminAuditLogs_Admins_AdminID
    FOREIGN KEY (AdminID) REFERENCES dbo.Admins(AdminID);
END;
 
IF OBJECT_ID('dbo.BlacklistedLogins','U') IS NULL
BEGIN
    CREATE TABLE dbo.BlacklistedLogins(
        BlacklistID INT IDENTITY(1,1) PRIMARY KEY,
        LoginID INT NOT NULL REFERENCES LoginCredentials(LoginID),
        Reason NVARCHAR(255) NULL,
        BlacklistedAt DATETIME2 NOT NULL CONSTRAINT DF_BlacklistedLogins_BlacklistedAt DEFAULT SYSUTCDATETIME()
    );
    IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE [name] = 'UQ_BlacklistedLogins_Login')
        ALTER TABLE dbo.BlacklistedLogins ADD CONSTRAINT UQ_BlacklistedLogins_Login UNIQUE(LoginID);
END
", con))
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
