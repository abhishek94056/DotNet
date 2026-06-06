# Mauli Farm Management System — Authentication Module Setup

## NuGet Packages Required

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore"                     Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer"           Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools"               Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design"              Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Identity.UI"                  Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="8.0.0" />
```

---

## Connection String (appsettings.json)

```json
"ConnectionStrings": {
  "MauliFarmConnection": "Server=(localdb)\\mssqllocaldb;Database=MauliFarmDB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

For production SQL Server:
```
Server=YOUR_SERVER;Database=MauliFarmDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True
```

---

## EF Core Migration Commands

### Package Manager Console (Visual Studio)
```powershell
# Step 1 — Add initial migration
Add-Migration InitialAuthSetup -Context ApplicationDbContext

# Step 2 — Apply migration to database
Update-Database -Context ApplicationDbContext

# Step 3 — Verify (optional)
Get-Migration -Context ApplicationDbContext
```

### .NET CLI (Terminal)
```bash
# Step 1 — Add initial migration
dotnet ef migrations add InitialAuthSetup --context ApplicationDbContext

# Step 2 — Apply migration
dotnet ef database update --context ApplicationDbContext

# Step 3 — View migration SQL (optional, without applying)
dotnet ef migrations script --context ApplicationDbContext
```

---

## Default Login Credentials

| Field    | Value                  |
|----------|------------------------|
| Username | `admin`                |
| Email    | `admin@maulifarm.com`  |
| Password | `Admin@123`            |
| Role     | SuperAdmin             |

> ⚠️ Change the password immediately after first login in production.

---

## Database Tables Created

| Table Name              | Purpose                              |
|-------------------------|--------------------------------------|
| `MF_Users`              | Extended Identity users              |
| `MF_Roles`              | Custom farm roles                    |
| `MF_UserRoles`          | User-role mappings                   |
| `MF_UserClaims`         | User claims                          |
| `MF_UserLogins`         | External login providers             |
| `MF_UserTokens`         | Password reset / 2FA tokens          |
| `MF_RoleClaims`         | Role-based claims                    |
| `MF_UserActivityLogs`   | Login/logout and audit trail         |

---

## Seeded Roles

| Role           | Description                                         |
|----------------|-----------------------------------------------------|
| SuperAdmin     | Full system access — owner / developer level        |
| Admin          | Full operational access across all modules          |
| FarmManager    | Manage labour, harvest, expenses, and reports       |
| Supervisor     | Manage daily field operations and labour attendance |
| AccountsStaff  | Expenses, payroll, and financial reports only       |
| ViewOnly       | Read-only access to reports and dashboards          |

---

## Project Structure (Auth Module Files)

```
MauliFarm/
├── Data/
│   ├── ApplicationDbContext.cs     ← EF Core context + seed
│   └── DbInitializer.cs            ← Startup runtime init
├── Models/
│   ├── ApplicationUser.cs          ← Extended Identity user
│   ├── ApplicationRole.cs          ← Custom role + FarmRoles constants
│   ├── UserActivityLog.cs          ← Login/activity audit model
│   └── ViewModels/
│       └── AuthViewModels.cs       ← Login, Register, Profile VMs
├── SQL/
│   └── 01_Auth_Tables.sql          ← Pure SQL script (alternate setup)
├── Program.cs                      ← DI + middleware pipeline
└── appsettings.json                ← Config + connection string
```
