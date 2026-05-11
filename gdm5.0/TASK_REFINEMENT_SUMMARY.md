# Task Refinement Summary: .NET 5 to .NET 8 Migration

## Quick Status

**DoR Score:** ?? 9/10 - ALMOST READY  
**Critical Blocker:** ?? Identity Server 4 migration decision needed  
**When Resolved:** Task ready for AI agent execution

---

## ?? Refined Task for Agent

### Objective
Migrate GDM 5.0 ERP from .NET 5 (end-of-life) to .NET 8 LTS, updating all dependencies while maintaining functionality.

### Why This Matters
- .NET 5 has been EOL since May 2022 (no security patches)
- .NET 8 is LTS (supported until November 2026)
- Security, performance, and maintainability improvements

---

## ?? What's IN Scope

? Update project files to target `net8.0`  
? Update all NuGet packages to .NET 8 compatible versions  
? Fix breaking changes in ASP.NET Core, EF Core  
? Update Program.cs/Startup.cs to .NET 8 patterns  
? Ensure all tests pass  
? Maintain API compatibility with Angular frontend  
? Update documentation  

## ? What's OUT of Scope

? Angular frontend changes (separate task)  
? New feature development  
? Architectural refactoring  
? UI/UX changes  
? Cloud migration  
? Performance optimization (beyond framework improvements)  

---

## ?? CRITICAL BLOCKER

### Identity Server 4 Migration Decision Needed

**Problem:** Identity Server 4 is end-of-life and not compatible with .NET 8

**Must Choose:**

#### Option A: Duende Identity Server
- ? Direct IS4 successor
- ? Full feature parity
- ? Commercial license required for production ($1,500+/year)
- **Best for:** If you need all IS4 features

#### Option B: OpenIddict ? RECOMMENDED
- ? Open source and actively maintained
- ? Compatible with .NET 8
- ? Similar features to IS4
- ? Free for commercial use
- **Best for:** Most scenarios

#### Option C: Plain JWT Bearer Authentication
- ? Simplest migration
- ? No third-party dependencies
- ? Need to implement OAuth/OpenID manually if needed
- **Best for:** If you only use JWT tokens, not full OAuth

---

## ?? Investigation Needed

Before choosing, check your codebase:

```csharp
// Search for Identity Server usage:
// 1. Check Startup.cs or Program.cs
services.AddIdentityServer()
// 2. Check for OAuth/OpenID endpoints
// 3. Check if external apps connect via OAuth
```

**Questions to answer:**
1. Do you have external clients using OAuth 2.0?
2. Do you use Identity Server for SSO (Single Sign-On)?
3. Do you only use JWT tokens for your Angular app?

**If answer to 1-2 is NO:** ? Go with Option C (Plain JWT)  
**If answer to 1-2 is YES:** ? Go with Option B (OpenIddict)

---

## ?? Files That Will Change

### Core Project Files (High Priority)
```
gdm5.0/
??? gdm5.0.csproj                    ? Change target to net8.0
??? Program.cs                        ? Update hosting model
??? Startup.cs                        ? Update middleware
??? appsettings.json                 ??  May need updates
??? Properties/
    ??? launchSettings.json          ??  Update runtime
```

### Areas with Breaking Changes
```
Models/DataContext.cs                 ?? EF Core 8 changes
Data/ApplicationDbContext.cs          ?? Identity changes
Controllers/                          ?? Check for deprecated APIs
Services/                             ?? DI pattern updates
```

### Migration Files
```
Migrations/                           ??  May need regeneration
```

---

## ? Acceptance Criteria (Must Pass)

### Build & Compilation
- [ ] Project targets `net8.0`
- [ ] All packages updated
- [ ] Zero compilation errors
- [ ] Zero warnings (or documented exceptions)

### Functionality
- [ ] User login works
- [ ] JWT tokens generate and validate
- [ ] Product CRUD operations work
- [ ] Order management works
- [ ] Price lists work
- [ ] Reports generate (PDF)
- [ ] All API endpoints respond

### Testing
- [ ] All unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing completed

### Performance
- [ ] API response time ? .NET 5 performance
- [ ] Startup time ? .NET 5 performance
- [ ] Memory usage similar or better

### Documentation
- [ ] README updated
- [ ] Migration notes documented
- [ ] Breaking changes listed

---

## ?? Testing Strategy

### Automated Tests
```bash
# Must all pass
dotnet test

# Must build without errors
dotnet build

# Check for warnings
dotnet build /warnaserror
```

### Manual Testing Checklist
1. **Authentication** ??
   - [ ] User login
   - [ ] Token generation
   - [ ] Token validation
   - [ ] Authorization checks

2. **Core Features** ??
   - [ ] Product management
   - [ ] Order processing
   - [ ] Customer management
   - [ ] Price list operations
   - [ ] Warehouse operations

3. **Reports** ??
   - [ ] PDF generation
   - [ ] Data export

---

## ?? Prerequisites

### Before Starting
- [ ] .NET 8 SDK installed
- [ ] Visual Studio 2022 (v17.8 or later)
- [ ] Database backup created
- [ ] Testing environment ready
- [ ] **Identity Server decision made** ??

### Tools Needed
```bash
# Install .NET 8 SDK
winget install Microsoft.DotNet.SDK.8

# Verify installation
dotnet --version  # Should show 8.0.x
```

---

## ?? Recommended Timeline

### Phase 1: Decision & Assessment (2 days)
- **Day 1:** Make Identity Server decision
- **Day 2:** Audit packages, create migration checklist

### Phase 2: Migration (3-5 days)
- **Days 3-5:** Update project, fix breaking changes
- **Days 6-7:** Update authentication (if IS4 migration needed)

### Phase 3: Testing (3-5 days)
- **Days 8-10:** Automated testing
- **Days 11-12:** Manual testing, bug fixes

### Phase 4: Deployment (2 days)
- **Day 13:** Documentation, staging deployment
- **Day 14:** Validation, production deployment prep

**Total Estimate:** 2-3 weeks

---

## ?? Agent-Ready Prompt

Once Identity Server decision is made, use this prompt:

```markdown
Migrate the GDM 5.0 ERP application from .NET 5 to .NET 8.

Files to modify:
- gdm5.0/gdm5.0.csproj (target framework)
- Program.cs (hosting model)
- Startup.cs (middleware configuration)
- All NuGet packages

Requirements:
1. Update target framework to net8.0
2. Update all NuGet packages to latest .NET 8 compatible versions
3. Fix all breaking changes
4. Maintain API compatibility (Angular frontend must still work)
5. Ensure authentication continues to work
6. No functionality regression

Identity Server approach: [DECISION NEEDED]
- Option: OpenIddict / Plain JWT / Duende

Constraints:
- No breaking API changes
- Database schema must remain compatible
- Must pass all existing tests

Output:
- Updated project files
- List of package updates
- Breaking changes fixed
- Migration notes

Acceptance criteria:
- Project builds successfully
- All tests pass
- Application runs in development
- Authentication works
- All CRUD operations functional

See DoR_CHECK_ISSUE_2.md for complete specification.
```

---

## ?? Next Immediate Action

### YOU MUST DO THIS FIRST:

1. **Check Identity Server Usage** (15 minutes)
   ```bash
   # Search for Identity Server in code
   code --goto Startup.cs
   # Look for: services.AddIdentityServer()
   ```

2. **Make Identity Server Decision** (30 minutes)
   - Review current authentication code
   - Answer: Do you use OAuth/OpenID features?
   - Choose: OpenIddict vs Plain JWT vs Duende
   - Document decision in Issue #2

3. **Update Issue #2 with Decision**
   - Comment on issue with chosen approach
   - Link to this DoR document

### THEN:

4. **Proceed with Migration**
   - Use the agent-ready prompt above
   - Follow the phase timeline
   - Check off acceptance criteria

---

## ?? Key Resources

### Microsoft Documentation
- [.NET 5 ? 6 Migration](https://learn.microsoft.com/en-us/aspnet/core/migration/50-to-60)
- [.NET 6 ? 7 Migration](https://learn.microsoft.com/en-us/aspnet/core/migration/60-to-70)
- [.NET 7 ? 8 Migration](https://learn.microsoft.com/en-us/aspnet/core/migration/70-to-80)
- [EF Core 5 ? 8](https://learn.microsoft.com/en-us/ef/core/what-is-new/)

### Authentication Alternatives
- [OpenIddict Documentation](https://documentation.openiddict.com/)
- [Duende IdentityServer](https://duendesoftware.com/products/identityserver)
- [JWT Bearer in .NET 8](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/)

### Project Documentation
- `PROJECT_MAP.md` - Architecture overview
- `DoR_CHECK_ISSUE_2.md` - Complete DoR assessment

---

## ?? Risks & Mitigation

| Risk | Impact | Mitigation |
|------|--------|------------|
| Identity Server migration breaks auth | ?? High | Test thoroughly; have rollback plan |
| Package incompatibility | ?? Medium | Research packages early; find alternatives |
| Breaking API changes | ?? Medium | Test frontend integration; maintain contracts |
| Performance regression | ?? Low | Benchmark before/after |

---

## ? Definition of Done

Task is complete when:
- ? Application runs on .NET 8
- ? All tests pass
- ? Authentication works
- ? No functionality regression
- ? Documentation updated
- ? Successfully deployed to staging
- ? Team trained on changes

---

## ?? Need Help?

### Questions to Resolve
1. **Identity Server:** Which approach should we use?
2. **Timeline:** When must this be complete?
3. **Testing:** Is staging environment ready?

### Ask in Issue #2
- Tag relevant team members
- Share this DoR document
- Get decision on Identity Server

---

**Status:** ?? PAUSED - Awaiting Identity Server decision  
**Next Action:** Investigate current authentication setup  
**Then:** Make Identity Server choice and proceed  
**DoR Score:** 9/10 ? Will be 10/10 after decision

---

**Created:** $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")  
**For Issue:** #2 - Update .NET 5 to .NET 8  
**Repository:** garin87/gdm_v5
