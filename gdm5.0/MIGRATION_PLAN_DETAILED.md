# Migration Plan: .NET 5 to .NET 8

## ?? Executive Summary

**Task:** Migrate GDM 5.0 ERP from .NET 5 (EOL) to .NET 8 LTS  
**Complexity:** ?? LOW (simplified approach - no complex Identity Server migration)  
**Risk Level:** ?? LOW  
**Estimated Time:** 2-3 days  
**Confidence:** ?? HIGH (well-defined path)  

**Key Decision:** Remove Identity Server 4 (only used for SPA template wrapper), use existing JWT Bearer authentication.

---

## ?? Phase-by-Phase Implementation Plan

### Phase 0: Preparation & Backup (30 minutes)

#### 0.1. Create Feature Branch
```bash
cd D:\Dev\Ggm\gdm_v5
git checkout AI-tooling-setup
git pull origin AI-tooling-setup
git checkout -b feature/dotnet8-migration
```

#### 0.2. Create Backup Point
```bash
# Commit current state
git add .
git commit -m "Checkpoint: Before .NET 8 migration"
git push origin feature/dotnet8-migration
```

#### 0.3. Backup Database
- Create database backup before any testing
- Document connection string
- Verify backup is restorable

#### 0.4. Verify Prerequisites
```powershell
# Check .NET 8 SDK
dotnet --version  # Should be 8.0.x

# Check Visual Studio version
# Ensure VS 2022 v17.8 or later

# Verify current build works
cd D:\Dev\Ggm\gdm_v5\gdm5.0
dotnet restore
dotnet build
dotnet test
```

**Deliverables:**
- ? Feature branch created
- ? Current state backed up
- ? Database backup created
- ? Prerequisites verified

---

### Phase 1: Update Project Files (1-2 hours)

#### 1.1. Update Target Framework

**File:** `D:\Dev\Ggm\gdm_v5\gdm5.0\gdm5.0.csproj`

**Change Line 3:**
```xml
<!-- FROM: -->
<TargetFramework>net5.0</TargetFramework>

<!-- TO: -->
<TargetFramework>net8.0</TargetFramework>
```

#### 1.2. Remove Identity Server Package

**File:** `D:\Dev\Ggm\gdm_v5\gdm5.0\gdm5.0.csproj`

**Remove Line 19:**
```xml
<!-- REMOVE THIS LINE: -->
<PackageReference Include="Microsoft.AspNetCore.ApiAuthorization.IdentityServer" Version="5.0.4" />
```

#### 1.3. Update Microsoft Packages

**File:** `D:\Dev\Ggm\gdm_v5\gdm5.0\gdm5.0.csproj`

**Update Lines 18-29:**
```xml
<ItemGroup>
  <!-- Keep unchanged for now -->
  <PackageReference Include="EntityFramework.DynamicLinq" Version="1.2.15" />

  <!-- Update to 8.0.0 -->
  <PackageReference Include="Microsoft.AspNetCore.SpaServices.Extensions" Version="8.0.0" />
  <!-- Identity Server line REMOVED -->
  <PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="8.0.0" />
  <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
  <PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="8.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
  <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="8.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />

  <!-- Keep PDF generation packages (check compatibility in Phase 2) -->
  <PackageReference Include="MigraDocCore.DocumentObjectModel" Version="1.3.57" />
  <PackageReference Include="MigraDocCore.Rendering" Version="1.3.57" />

  <!-- Update Newtonsoft.Json -->
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />

  <!-- Keep PdfSharp -->
  <PackageReference Include="PdfSharpCore" Version="1.3.57" />

  <!-- Update Realm (CAUTION: Major version change) -->
  <PackageReference Include="Realm" Version="11.7.0" />

  <!-- Update System packages -->
  <PackageReference Include="System.Drawing.Common" Version="8.0.0" />
  <PackageReference Include="System.Linq.Dynamic.Core" Version="1.3.7" />

  <!-- Remove if not needed (included in .NET 8) -->
  <!-- <PackageReference Include="System.Linq.Queryable" Version="4.3.0" /> -->
</ItemGroup>
```

#### 1.4. Test Initial Changes
```powershell
cd D:\Dev\Ggm\gdm_v5\gdm5.0

# Restore packages
dotnet restore

# Expected: Some warnings about package conflicts
# Expected: Possible errors about Identity Server
```

**Commit Point:**
```bash
git add gdm5.0.csproj
git commit -m "Phase 1: Update target framework to net8.0 and packages"
```

**Deliverables:**
- ? Target framework changed to net8.0
- ? Identity Server package removed
- ? All Microsoft packages updated to 8.0.0
- ? Third-party packages updated
- ? Changes committed

---

### Phase 2: Update Startup Configuration (30 minutes)

#### 2.1. Remove Identity Server from Startup.cs

**File:** `D:\Dev\Ggm\gdm_v5\gdm5.0\Startup.cs`

**Change Lines 66-68:**
```csharp
// FROM:
services.AddIdentityServer()        
    //.AddSigningCredential(cert)
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

// TO:
// Identity Server removed - using JWT Bearer only (configured below at line 92-107)
```

**Change Line 108:**
```csharp
// FROM:
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = false,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection("securityKey").Value))
    };
}).AddIdentityServerJwt();

// TO:
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = false,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection("securityKey").Value))
    };
});
// .AddIdentityServerJwt() removed
```

#### 2.2. Remove Identity Server Using Statements (if exists)

**File:** `D:\Dev\Ggm\gdm_v5\gdm5.0\Startup.cs` (top of file)

Check for and remove if exists:
```csharp
// Remove if present:
using IdentityServer4;
using IdentityServer4.Models;
```

#### 2.3. Test Build
```powershell
cd D:\Dev\Ggm\gdm_v5\gdm5.0
dotnet build

# Expected: Build should succeed or have fixable errors
# Check for errors related to Identity Server
```

**Commit Point:**
```bash
git add Startup.cs
git commit -m "Phase 2: Remove Identity Server configuration"
```

**Deliverables:**
- ? Identity Server configuration removed
- ? JWT Bearer authentication kept (already configured)
- ? Unnecessary using statements removed
- ? Changes committed

---

### Phase 3: Fix Breaking Changes (2-4 hours)

#### 3.1. Resolve Compilation Errors

**Priority 1: Build Errors**

Run and address errors:
```powershell
dotnet build 2>&1 | Tee-Object -FilePath build-errors.txt
```

**Common Breaking Changes to Fix:**

##### 3.1.1. Program.cs Hosting Model
If using `Program.cs` with `CreateHostBuilder`:

**Current (.NET 5):**
```csharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

**May need to update to (.NET 8):**
```csharp
// If using minimal hosting model
var builder = WebApplication.CreateBuilder(args);
// Or keep existing if it still works
```

**Action:** Check if Program.cs needs updates (likely minimal changes)

##### 3.1.2. JSON Serialization Changes

If you see JSON-related errors:

**File:** `Startup.cs` (in `ConfigureServices`)

Add if needed:
```csharp
services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Keep System.Text.Json settings compatible
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        // Add other required settings
    });
```

##### 3.1.3. EF Core Breaking Changes

**Check:** `Models/DataContext.cs` or `Data/ApplicationDbContext.cs`

Common issues:
- `OnModelCreating` method changes
- Navigation property changes
- Cascade delete behavior changes

**Reference:** https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/breaking-changes

##### 3.1.4. Middleware Order Changes

**File:** `Startup.cs` (in `Configure` method)

Ensure correct order:
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // 1. Exception handling first
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    // 2. HTTPS redirection
    app.UseHttpsRedirection();

    // 3. Static files
    app.UseStaticFiles();
    app.UseSpaStaticFiles();

    // 4. Routing
    app.UseRouting();

    // 5. CORS (before authentication!)
    app.UseCors("EnableCORS");

    // 6. Authentication (before authorization!)
    app.UseAuthentication();

    // 7. Authorization
    app.UseAuthorization();

    // 8. Endpoints
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapRazorPages();
    });

    // 9. SPA
    app.UseSpa(spa => { /* ... */ });
}
```

#### 3.2. Fix Warnings

```powershell
# Build with warnings as errors to catch issues
dotnet build /warnaserror 2>&1 | Tee-Object -FilePath build-warnings.txt
```

**Common Warnings:**
- Nullable reference types warnings
- Deprecated API usage
- Obsolete method calls

**Action:** Fix critical warnings, document acceptable warnings

#### 3.3. Check Third-Party Package Compatibility

##### 3.3.1. Test Realm Database (if used)

```powershell
# Search for Realm usage
cd D:\Dev\Ggm\gdm_v5\gdm5.0
findstr /s /i "Realm" *.cs
```

If Realm is used:
- Review breaking changes: https://www.mongodb.com/docs/realm/sdk/dotnet/
- Test data access
- May need code updates for v11.x

##### 3.3.2. Test PDF Generation

```powershell
# Search for MigraDoc/PdfSharp usage
findstr /s /i "MigraDoc\|PdfSharp" *.cs
```

**Action:** 
- Build the project
- If PDF generation fails, consider alternatives (QuestPDF)
- Document in MIGRATION_NOTES.md

##### 3.3.3. Test Dynamic LINQ

```powershell
# Search for dynamic LINQ usage
findstr /s /i "Dynamic" *.cs
```

**Action:**
- Verify System.Linq.Dynamic.Core works with .NET 8
- Test parametric search functionality

#### 3.4. Verify Build Success

```powershell
dotnet clean
dotnet restore
dotnet build --configuration Release

# Build should succeed with zero errors
# Document any remaining warnings
```

**Commit Point:**
```bash
git add .
git commit -m "Phase 3: Fix breaking changes and build errors"
```

**Deliverables:**
- ? All compilation errors fixed
- ? Critical warnings resolved
- ? Third-party packages tested
- ? Build succeeds (Release mode)
- ? Breaking changes documented

---

### Phase 4: Database & Migrations (1-2 hours)

#### 4.1. Check EF Core Migrations

```powershell
cd D:\Dev\Ggm\gdm_v5\gdm5.0

# List migrations
dotnet ef migrations list

# Check for any issues
dotnet ef database update --dry-run
```

#### 4.2. Test Database Connection

**File:** `appsettings.Development.json`

Verify connection string is correct:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;..."
  }
}
```

#### 4.3. Run Migrations (if needed)

```powershell
# Backup database first!

# Apply migrations
dotnet ef database update

# Verify database structure
```

**If migrations fail:**
- Check EF Core 8 breaking changes
- May need to regenerate migrations
- Document any schema changes

#### 4.4. Test Data Access

**Create test script:** `test-data-access.ps1`
```powershell
# Test basic CRUD
# 1. Start application
# 2. Test GET /api/products (or similar)
# 3. Verify data retrieval works
```

**Commit Point:**
```bash
git add Migrations/
git commit -m "Phase 4: Update migrations for .NET 8"
```

**Deliverables:**
- ? Migrations compatible with EF Core 8
- ? Database connection tested
- ? Data access verified
- ? No data loss confirmed

---

### Phase 5: Authentication Testing (2-3 hours)

#### 5.1. Test User Registration

**Manual Test:**
1. Start application: `dotnet run`
2. Navigate to registration endpoint
3. Create test user
4. Verify user created in database

**Expected Result:** Registration works

#### 5.2. Test User Login

**Manual Test:**
1. Login with test user credentials
2. Verify JWT token is generated
3. Check token structure and claims

**Expected Result:** Login returns valid JWT token

**Token Validation:**
```powershell
# Decode JWT token at https://jwt.io
# Verify:
# - Correct claims present
# - Expiration time set
# - Signature valid
```

#### 5.3. Test Token Validation

**Manual Test:**
1. Call protected endpoint with token
2. Verify access granted
3. Call protected endpoint without token
4. Verify access denied (401)

**Test Endpoints:**
- `GET /api/products` (should require auth)
- `GET /api/orders` (should require auth)
- Any other protected endpoints

#### 5.4. Test Authorization

**Manual Test:**
1. Test role-based access
2. Test user with different roles
3. Verify permissions enforced

**Expected Result:** Authorization rules work correctly

#### 5.5. Test Token Expiration

**Manual Test:**
1. Wait for token to expire (or set short expiration)
2. Try to use expired token
3. Verify access denied

**Expected Result:** Expired tokens rejected

**Commit Point:**
```bash
git add .
git commit -m "Phase 5: Authentication verified working"
```

**Deliverables:**
- ? User registration works
- ? User login works
- ? JWT tokens generated correctly
- ? Token validation works
- ? Authorization enforced
- ? Token expiration handled

---

### Phase 6: Functional Testing (4-8 hours)

#### 6.1. Automated Tests

```powershell
cd D:\Dev\Ggm\gdm_v5\gdm5.0

# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Generate coverage report (if configured)
dotnet test /p:CollectCoverage=true
```

**Expected Result:** All tests pass

**If tests fail:**
- Review test failures
- Check for .NET 8 compatibility issues
- Update tests if needed
- Document any intentional changes

#### 6.2. Manual Testing Checklist

##### 6.2.1. Product Management
- [ ] List products (GET /api/products)
- [ ] Get single product (GET /api/products/{id})
- [ ] Create product (POST /api/products)
- [ ] Update product (PUT /api/products/{id})
- [ ] Delete product (DELETE /api/products/{id})
- [ ] Search products (parametric search)
- [ ] Filter by parameters

##### 6.2.2. Order Management
- [ ] List orders
- [ ] Create order
- [ ] Update order status
- [ ] Process order workflow
- [ ] Calculate totals
- [ ] Apply discounts

##### 6.2.3. Customer Management
- [ ] List customers
- [ ] Create customer
- [ ] Update customer
- [ ] Delete customer
- [ ] View customer orders

##### 6.2.4. Price Lists
- [ ] List price lists
- [ ] Get active price list
- [ ] Apply price rules
- [ ] Currency conversion
- [ ] Multi-version support

##### 6.2.5. Reports & Export
- [ ] Generate PDF report
- [ ] Export data
- [ ] Custom reports

##### 6.2.6. Warehouse Operations
- [ ] Inventory tracking
- [ ] Stock movements
- [ ] Location management

#### 6.3. Performance Testing

**Test API Response Times:**
```powershell
# Use PowerShell to measure response time
Measure-Command {
    Invoke-WebRequest -Uri "http://localhost:5000/api/products" -Headers @{Authorization="Bearer $token"}
}
```

**Benchmarks:**
- API response time ? .NET 5 performance
- No memory leaks
- CPU usage reasonable

#### 6.4. Frontend Integration Testing

##### 6.4.1. Build Angular App
```powershell
cd D:\Dev\Ggm\gdm_v5\gdm5.0\ClientApp
npm install
npm run build
```

##### 6.4.2. Test Frontend-Backend Integration
- [ ] Frontend loads successfully
- [ ] Login from UI works
- [ ] API calls from frontend work
- [ ] No CORS errors
- [ ] No console errors
- [ ] All features functional from UI

#### 6.5. Edge Cases

**Test Edge Cases:**
- [ ] Large dataset queries
- [ ] Concurrent user access
- [ ] Invalid input handling
- [ ] Database connection failure
- [ ] Missing required data
- [ ] Null reference scenarios

**Commit Point:**
```bash
git add .
git commit -m "Phase 6: All functional tests passing"
```

**Deliverables:**
- ? All automated tests pass
- ? All manual tests completed
- ? Performance benchmarks met
- ? Frontend integration verified
- ? Edge cases handled
- ? Test results documented

---

### Phase 7: Documentation (2 hours)

#### 7.1. Create Migration Notes

**File:** `MIGRATION_NOTES.md`

```markdown
# .NET 8 Migration Notes

## Changes Made
- Updated from .NET 5 to .NET 8
- Removed Identity Server 4 (replaced with JWT Bearer only)
- Updated all packages to .NET 8 compatible versions
- [List other significant changes]

## Breaking Changes
- Identity Server removed (SPA authentication simplified)
- [List any API changes]
- [List any behavior changes]

## Package Updates
- Microsoft.AspNetCore.* : 5.0.4 ? 8.0.0
- Entity Framework Core: 5.0.4 ? 8.0.0
- Realm: 10.4.1 ? 11.7.0
- [List other updates]

## Testing Results
- All unit tests: PASS
- All integration tests: PASS
- Manual testing: COMPLETE
- Performance: [Results]

## Known Issues
- [List any known issues or limitations]

## Rollback Plan
- [Document rollback procedure if needed]
```

#### 7.2. Update README.md

**File:** `README.md`

Update Prerequisites section:
```markdown
## Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 (v17.8 or later) or VS Code
- SQL Server 2016 or later
- Node.js 16.x or later (for Angular frontend)
```

Update Build Instructions:
```markdown
## Building the Project

```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run
```
```

#### 7.3. Update PROJECT_MAP.md

**File:** `PROJECT_MAP.md`

Update Technology Stack section:
```markdown
## Technology Stack

### Backend
- **Framework:** ASP.NET Core 8.0
- **Language:** C# 10
- **ORM:** Entity Framework Core 8.0
- **Database:** SQL Server
- **Authentication:** JWT Bearer (ASP.NET Core Identity)
  - ~~Identity Server 4~~ (removed in .NET 8 migration)
  - Using built-in JWT Bearer authentication
```

#### 7.4. Create CHANGELOG.md Entry

**File:** `CHANGELOG.md` (create if doesn't exist)

```markdown
## [Unreleased] - .NET 8 Migration

### Changed
- Migrated from .NET 5 to .NET 8 LTS
- Updated all Microsoft packages to 8.0.0
- Replaced Identity Server 4 with JWT Bearer authentication
- Updated third-party packages to latest compatible versions

### Removed
- Microsoft.AspNetCore.ApiAuthorization.IdentityServer package
- Identity Server 4 configuration

### Fixed
- [List any bugs fixed during migration]

### Security
- Now using .NET 8 with active security support (until Nov 2026)
```

#### 7.5. Create Deployment Guide

**File:** `DEPLOYMENT.md`

```markdown
# .NET 8 Deployment Guide

## Prerequisites
- Server with .NET 8 Runtime installed
- [List other requirements]

## Deployment Steps
1. [Step-by-step deployment instructions]
2. [Environment configuration]
3. [Database migration steps]
4. [Verification steps]

## Rollback Procedure
1. [How to rollback if needed]
```

**Commit Point:**
```bash
git add *.md
git commit -m "Phase 7: Documentation complete"
```

**Deliverables:**
- ? MIGRATION_NOTES.md created
- ? README.md updated
- ? PROJECT_MAP.md updated
- ? CHANGELOG.md entry added
- ? Deployment guide created
- ? All documentation committed

---

### Phase 8: Final Validation (1-2 hours)

#### 8.1. Clean Build Verification

```powershell
cd D:\Dev\Ggm\gdm_v5\gdm5.0

# Clean everything
dotnet clean
Remove-Item -Recurse -Force bin, obj

# Full rebuild
dotnet restore
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release

# Verify no errors or warnings
```

**Expected Result:** Clean build with all tests passing

#### 8.2. Run Application End-to-End

```powershell
# Start application
dotnet run --configuration Release

# Test critical workflows:
# 1. User registration/login
# 2. Product CRUD operations
# 3. Order processing
# 4. Report generation
# 5. Any other critical features
```

#### 8.3. Performance Validation

**Compare .NET 5 vs .NET 8:**

Test and document:
- Application startup time
- API response times
- Memory usage
- CPU usage

**Expected:** .NET 8 should be equal or better

#### 8.4. Code Quality Check

```powershell
# Check for code issues (if using analyzer)
dotnet build /p:RunAnalyzers=true

# Check for security issues
# (optional: use security scanning tools)
```

#### 8.5. Final Checklist

**Pre-Merge Checklist:**
- [ ] All tests pass
- [ ] No compilation errors
- [ ] No critical warnings
- [ ] Documentation updated
- [ ] Code reviewed
- [ ] Performance validated
- [ ] Security verified
- [ ] Ready for staging deployment

**Commit Point:**
```bash
git add .
git commit -m "Phase 8: Final validation complete - ready for merge"
```

**Deliverables:**
- ? Clean build verified
- ? End-to-end testing complete
- ? Performance validated
- ? Code quality checked
- ? Final checklist complete
- ? Ready for PR/merge

---

### Phase 9: Staging Deployment (Optional - 2-3 hours)

#### 9.1. Prepare Staging Environment

- [ ] Staging server has .NET 8 Runtime
- [ ] Database backup created
- [ ] Configuration updated
- [ ] Environment variables set

#### 9.2. Deploy to Staging

```powershell
# Publish application
dotnet publish --configuration Release --output ./publish

# Deploy to staging server
# (method depends on your infrastructure)
```

#### 9.3. Smoke Testing in Staging

- [ ] Application starts
- [ ] Health check endpoint responds
- [ ] Login works
- [ ] Critical features work
- [ ] No errors in logs

#### 9.4. Monitor Staging

**Monitor for:**
- Application errors
- Performance issues
- Memory leaks
- Unexpected behavior

**Duration:** Run for 24-48 hours minimum

#### 9.5. Get Stakeholder Approval

- [ ] Demo to stakeholders
- [ ] Get approval for production
- [ ] Document any issues found
- [ ] Create production deployment plan

**Deliverables:**
- ? Deployed to staging
- ? Smoke tests passed
- ? Monitoring complete
- ? Stakeholder approval obtained
- ? Ready for production (when scheduled)

---

## ?? Risk Management Plan

### High-Risk Areas

#### 1. Authentication Changes (Risk: ?? Medium)
**Mitigation:**
- Thorough testing of auth flow
- Test with multiple user types
- Verify token validation
- Test token expiration
- Have rollback plan ready

#### 2. Realm Database Update (Risk: ?? Medium)
**Mitigation:**
- Check if Realm is actually used
- Review v11.x breaking changes
- Test data access thoroughly
- Consider alternatives if issues

#### 3. PDF Generation (Risk: ?? Medium)
**Mitigation:**
- Test MigraDoc/PdfSharp compatibility
- Have QuestPDF as backup plan
- Test all report types
- Document any changes needed

#### 4. Third-Party Package Compatibility (Risk: ?? Low)
**Mitigation:**
- Test each package individually
- Have alternatives identified
- Document compatibility issues
- Update packages incrementally

### Rollback Plan

**If Critical Issues Found:**

1. **Immediate Rollback:**
   ```bash
   git checkout AI-tooling-setup
   git branch -D feature/dotnet8-migration
   # Restore database backup
   # Redeploy .NET 5 version
   ```

2. **Investigate Issues:**
   - Document what went wrong
   - Identify root cause
   - Plan fix or alternative approach

3. **Retry Migration:**
   - Address issues found
   - Create new migration branch
   - Proceed with updated plan

---

## ?? Success Metrics

### Technical Success Criteria

- ? Application runs on .NET 8
- ? Zero compilation errors
- ? Zero critical warnings
- ? All tests pass (100%)
- ? Authentication works flawlessly
- ? All CRUD operations functional
- ? Performance ? .NET 5
- ? No memory leaks
- ? No security regressions

### Business Success Criteria

- ? Zero user-facing issues
- ? No data loss
- ? No downtime (or minimal planned)
- ? All features working
- ? Stakeholder approval
- ? Documentation complete

### Quality Metrics

- ? Code quality maintained/improved
- ? Test coverage maintained
- ? Technical debt not increased
- ? Architecture improved (IS4 removed)

---

## ?? Time Estimates

### By Phase

| Phase | Task | Estimated Time |
|-------|------|----------------|
| 0 | Preparation & Backup | 30 minutes |
| 1 | Update Project Files | 1-2 hours |
| 2 | Update Startup Config | 30 minutes |
| 3 | Fix Breaking Changes | 2-4 hours |
| 4 | Database & Migrations | 1-2 hours |
| 5 | Authentication Testing | 2-3 hours |
| 6 | Functional Testing | 4-8 hours |
| 7 | Documentation | 2 hours |
| 8 | Final Validation | 1-2 hours |
| 9 | Staging Deployment (Optional) | 2-3 hours |
| **Total** | | **16-27 hours** |

### Realistic Schedule

**Day 1:**
- Phases 0-3 (Prep, Updates, Breaking Changes)
- ~6-8 hours

**Day 2:**
- Phases 4-6 (Database, Auth, Testing)
- ~7-13 hours

**Day 3:**
- Phases 7-8 (Documentation, Validation)
- ~3-4 hours

**Day 4 (if needed):**
- Phase 9 (Staging deployment)
- Buffer for unexpected issues

---

## ?? Next Immediate Actions

### Right Now (15 minutes)

1. ? Review this plan
2. ? Ensure .NET 8 SDK installed
3. ? Create database backup
4. ? Create feature branch

### Then Start Phase 1 (1-2 hours)

1. Update `gdm5.0.csproj`
2. Test `dotnet restore`
3. Commit changes
4. Proceed to Phase 2

---

## ?? Questions & Clarifications

### Before Starting, Confirm:

1. **Is staging environment ready?**
   - If no, skip Phase 9

2. **Is Realm database actually used?**
   - Check code, may be removable

3. **When is production deployment window?**
   - Plan accordingly

4. **Who approves production deployment?**
   - Include in process

---

## ?? References

### Documentation Links
- [.NET 5 ? 6 Migration](https://learn.microsoft.com/en-us/aspnet/core/migration/50-to-60)
- [.NET 6 ? 7 Migration](https://learn.microsoft.com/en-us/aspnet/core/migration/60-to-70)
- [.NET 7 ? 8 Migration](https://learn.microsoft.com/en-us/aspnet/core/migration/70-to-80)
- [EF Core Breaking Changes](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/breaking-changes)

### Project Documentation
- `IDENTITY_SERVER_DECISION.md` - Authentication migration rationale
- `DoR_CHECK_ISSUE_2.md` - Complete specification
- `PROJECT_MAP.md` - Architecture reference

---

## ? Plan Summary

**Complexity:** ?? LOW (simplified approach)  
**Time Required:** 2-3 days focused work  
**Risk Level:** ?? LOW (well-defined path)  
**Confidence:** ?? HIGH  

**Key Success Factor:** Identity Server removal simplifies migration significantly!

**Ready to Start:** ? YES - All prerequisites met

---

**Plan Created:** $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")  
**For Issue:** #2 - Update .NET 5 to .NET 8  
**Repository:** garin87/gdm_v5  
**Branch:** feature/dotnet8-migration (to be created)  
**Status:** ?? READY TO EXECUTE

---

## ?? Start Command

When ready to begin:

```powershell
# Navigate to repository
cd D:\Dev\Ggm\gdm_v5

# Create feature branch
git checkout AI-tooling-setup
git checkout -b feature/dotnet8-migration

# Commit checkpoint
git add .
git commit -m "Checkpoint before .NET 8 migration"

# Start Phase 1
code gdm5.0\gdm5.0.csproj
```

**Then follow Phase 1 instructions above!** ??
