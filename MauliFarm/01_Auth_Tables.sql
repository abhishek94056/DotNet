-- ============================================================
--  Mauli Farm Management System
--  Authentication Module - SQL Script
--  Database: MauliFarmDB
--  Schema:   dbo
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MauliFarmDB')
BEGIN
    CREATE DATABASE MauliFarmDB;
    PRINT 'Database MauliFarmDB created.';
END
GO

USE MauliFarmDB;
GO

-- ============================================================
--  TABLE: MF_Roles (ASP.NET Identity Roles)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MF_Roles')
BEGIN
    CREATE TABLE dbo.MF_Roles (
        Id               NVARCHAR(450)    NOT NULL,
        [Name]           NVARCHAR(256)    NULL,
        NormalizedName   NVARCHAR(256)    NULL,
        ConcurrencyStamp NVARCHAR(MAX)    NULL,
        [Description]    NVARCHAR(300)    NULL,
        CreatedOn        DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
        IsActive         BIT              NOT NULL DEFAULT 1,

        CONSTRAINT PK_MF_Roles PRIMARY KEY (Id)
    );

    CREATE UNIQUE INDEX UIX_MF_Roles_NormalizedName
        ON dbo.MF_Roles (NormalizedName)
        WHERE NormalizedName IS NOT NULL;

    PRINT 'Table MF_Roles created.';
END
GO

-- ============================================================
--  TABLE: MF_Users (ASP.NET Identity Users - Extended)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MF_Users')
BEGIN
    CREATE TABLE dbo.MF_Users (
        Id                   NVARCHAR(450)   NOT NULL,
        UserName             NVARCHAR(256)   NULL,
        NormalizedUserName   NVARCHAR(256)   NULL,
        Email                NVARCHAR(256)   NULL,
        NormalizedEmail      NVARCHAR(256)   NULL,
        EmailConfirmed       BIT             NOT NULL DEFAULT 0,
        PasswordHash         NVARCHAR(MAX)   NULL,
        SecurityStamp        NVARCHAR(MAX)   NULL,
        ConcurrencyStamp     NVARCHAR(MAX)   NULL,
        PhoneNumber          NVARCHAR(MAX)   NULL,
        PhoneNumberConfirmed BIT             NOT NULL DEFAULT 0,
        TwoFactorEnabled     BIT             NOT NULL DEFAULT 0,
        LockoutEnd           DATETIMEOFFSET(7) NULL,
        LockoutEnabled       BIT             NOT NULL DEFAULT 1,
        AccessFailedCount    INT             NOT NULL DEFAULT 0,

        -- Extended Fields for Mauli Farm
        FullName             NVARCHAR(100)   NOT NULL,
        EmployeeCode         NVARCHAR(20)    NULL,
        Designation          NVARCHAR(50)    NULL,
        ProfilePicturePath   NVARCHAR(500)   NULL,
        IsActive             BIT             NOT NULL DEFAULT 1,
        CreatedOn            DATETIME2(7)    NOT NULL DEFAULT GETUTCDATE(),
        LastLogin            DATETIME2(7)    NULL,
        [Address]            NVARCHAR(200)   NULL,
        Notes                NVARCHAR(500)   NULL,

        CONSTRAINT PK_MF_Users PRIMARY KEY (Id)
    );

    CREATE UNIQUE INDEX UIX_MF_Users_NormalizedUserName
        ON dbo.MF_Users (NormalizedUserName)
        WHERE NormalizedUserName IS NOT NULL;

    CREATE INDEX IX_MF_Users_NormalizedEmail
        ON dbo.MF_Users (NormalizedEmail);

    CREATE UNIQUE INDEX UIX_MF_Users_EmployeeCode
        ON dbo.MF_Users (EmployeeCode)
        WHERE EmployeeCode IS NOT NULL;

    PRINT 'Table MF_Users created.';
END
GO

-- ============================================================
--  TABLE: MF_UserRoles
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MF_UserRoles')
BEGIN
    CREATE TABLE dbo.MF_UserRoles (
        UserId NVARCHAR(450) NOT NULL,
        RoleId NVARCHAR(450) NOT NULL,

        CONSTRAINT PK_MF_UserRoles PRIMARY KEY (UserId, RoleId),
        CONSTRAINT FK_MF_UserRoles_Users FOREIGN KEY (UserId) REFERENCES dbo.MF_Users(Id) ON DELETE CASCADE,
        CONSTRAINT FK_MF_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES dbo.MF_Roles(Id) ON DELETE CASCADE
    );

    PRINT 'Table MF_UserRoles created.';
END
GO

-- ============================================================
--  TABLE: MF_UserClaims
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MF_UserClaims')
BEGIN
    CREATE TABLE dbo.MF_UserClaims (
        Id         INT             NOT NULL IDENTITY(1,1),
        UserId     NVARCHAR(450)   NOT NULL,
        ClaimType  NVARCHAR(MAX)   NULL,
        ClaimValue NVARCHAR(MAX)   NULL,

        CONSTRAINT PK_MF_UserClaims PRIMARY KEY (Id),
        CONSTRAINT FK_MF_UserClaims_Users FOREIGN KEY (UserId) REFERENCES dbo.MF_Users(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_MF_UserClaims_UserId ON dbo.MF_UserClaims (UserId);

    PRINT 'Table MF_UserClaims created.';
END
GO

-- ============================================================
--  TABLE: MF_UserLogins
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MF_UserLogins')
BEGIN
    CREATE TABLE dbo.MF_UserLogins (
        LoginProvider       NVARCHAR(128)  NOT NULL,
        ProviderKey         NVARCHAR(128)  NOT NULL,
        ProviderDisplayName NVARCHAR(MAX)  NULL,
        UserId              NVARCHAR(450)  NOT NULL,

        CONSTRAINT PK_MF_UserLogins PRIMARY KEY (LoginProvider, ProviderKey),
        CONSTRAINT FK_MF_UserLogins_Users FOREIGN KEY (UserId) REFERENCES dbo.MF_Users(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_MF_UserLogins_UserId ON dbo.MF_UserLogins (UserId);

    PRINT 'Table MF_UserLogins created.';
END
GO

-- ============================================================
--  TABLE: MF_UserTokens
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MF_UserTokens')
BEGIN
    CREATE TABLE dbo.MF_UserTokens (
        UserId        NVARCHAR(450)  NOT NULL,
        LoginProvider NVARCHAR(128)  NOT NULL,
        [Name]        NVARCHAR(128)  NOT NULL,
        [Value]       NVARCHAR(MAX)  NULL,

        CONSTRAINT PK_MF_UserTokens PRIMARY KEY (UserId, LoginProvider, [Name]),
        CONSTRAINT FK_MF_UserTokens_Users FOREIGN KEY (UserId) REFERENCES dbo.MF_Users(Id) ON DELETE CASCADE
    );

    PRINT 'Table MF_UserTokens created.';
END
GO

-- ============================================================
--  TABLE: MF_RoleClaims
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MF_RoleClaims')
BEGIN
    CREATE TABLE dbo.MF_RoleClaims (
        Id         INT             NOT NULL IDENTITY(1,1),
        RoleId     NVARCHAR(450)   NOT NULL,
        ClaimType  NVARCHAR(MAX)   NULL,
        ClaimValue NVARCHAR(MAX)   NULL,

        CONSTRAINT PK_MF_RoleClaims PRIMARY KEY (Id),
        CONSTRAINT FK_MF_RoleClaims_Roles FOREIGN KEY (RoleId) REFERENCES dbo.MF_Roles(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_MF_RoleClaims_RoleId ON dbo.MF_RoleClaims (RoleId);

    PRINT 'Table MF_RoleClaims created.';
END
GO

-- ============================================================
--  TABLE: MF_UserActivityLogs
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MF_UserActivityLogs')
BEGIN
    CREATE TABLE dbo.MF_UserActivityLogs (
        Id           INT              NOT NULL IDENTITY(1,1),
        UserId       NVARCHAR(450)    NOT NULL,
        ActivityType NVARCHAR(100)    NOT NULL,
        [Description] NVARCHAR(500)  NULL,
        IpAddress    NVARCHAR(50)     NULL,
        UserAgent    NVARCHAR(300)    NULL,
        [Timestamp]  DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
        IsSuccess    BIT              NOT NULL DEFAULT 1,

        CONSTRAINT PK_MF_UserActivityLogs PRIMARY KEY (Id),
        CONSTRAINT FK_MF_ActivityLogs_Users FOREIGN KEY (UserId) REFERENCES dbo.MF_Users(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_MF_ActivityLogs_UserId       ON dbo.MF_UserActivityLogs (UserId);
    CREATE INDEX IX_MF_ActivityLogs_Timestamp    ON dbo.MF_UserActivityLogs ([Timestamp]);
    CREATE INDEX IX_MF_ActivityLogs_ActivityType ON dbo.MF_UserActivityLogs (ActivityType);

    PRINT 'Table MF_UserActivityLogs created.';
END
GO

-- ============================================================
--  SEED: Default Roles
-- ============================================================
MERGE dbo.MF_Roles AS target
USING (VALUES
    ('superadmin',    'SuperAdmin',    'SUPERADMIN',    'Full system access — owner / developer level'),
    ('admin',         'Admin',         'ADMIN',         'Full operational access across all modules'),
    ('farmmanager',   'FarmManager',   'FARMMANAGER',   'Manage labour, harvest, expenses, and reports'),
    ('supervisor',    'Supervisor',    'SUPERVISOR',    'Manage daily field operations and labour attendance'),
    ('accountsstaff', 'AccountsStaff', 'ACCOUNTSSTAFF', 'Access to expenses, payroll, and financial reports only'),
    ('viewonly',      'ViewOnly',      'VIEWONLY',      'Read-only access to reports and dashboards')
) AS source (Id, [Name], NormalizedName, [Description])
ON target.Id = source.Id
WHEN NOT MATCHED THEN
    INSERT (Id, [Name], NormalizedName, [Description], CreatedOn, IsActive, ConcurrencyStamp)
    VALUES (source.Id, source.[Name], source.NormalizedName, source.[Description],
            '2025-01-01 00:00:00', 1, NEWID());
GO

-- ============================================================
--  SEED: Default SuperAdmin User
--  Password: Admin@123  (bcrypt hash below — regenerate in prod)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM dbo.MF_Users WHERE Email = 'admin@maulifarm.com')
BEGIN
    DECLARE @userId NVARCHAR(450) = 'a1b2c3d4-e5f6-7890-abcd-ef1234567890';

    INSERT INTO dbo.MF_Users
        (Id, UserName, NormalizedUserName, Email, NormalizedEmail,
         EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
         TwoFactorEnabled, LockoutEnabled, AccessFailedCount,
         FullName, EmployeeCode, Designation, IsActive, CreatedOn)
    VALUES
        (@userId, 'admin', 'ADMIN', 'admin@maulifarm.com', 'ADMIN@MAULIFARM.COM',
         1,
         -- Password hash for Admin@123 (ASP.NET Identity v3 format)
         'AQAAAAIAAYagAAAAEPlaceholderHashReplaceWithMigrationGeneratedHash==',
         NEWID(), NEWID(),
         0, 0, 0,
         'Farm Administrator', 'MF-001', 'System Administrator', 1, GETUTCDATE());

    INSERT INTO dbo.MF_UserRoles (UserId, RoleId)
    VALUES (@userId, 'superadmin');

    PRINT 'SuperAdmin user seeded. IMPORTANT: Run EF migrations to get correct password hash.';
END
GO

PRINT '=== Mauli Farm Authentication Tables ready ===';
GO
