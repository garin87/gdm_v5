# Definition of Ready Check: Update .NET 5 to .NET 8 Migration

## Task Reference
**Issue:** #2 - "Update .NET 5 to version 8 and its dependencies"  
**Repository:** garin87/gdm_v5  
**Branch:** AI-tooling-setup  
**Date:** $(Get-Date -Format "yyyy-MM-dd")

---

## DoR Assessment

### 1. ? Objective Clarity

**Primary Goal:**
Migrate the GDM 5.0 ERP system from .NET 5 (end-of-life since May 2022) to .NET 8 LTS to ensure:
- Security patches and support
- Performance improvements
- Modern framework features
- Long-term maintainability (LTS support until November 2026)

**Problem Solved:**
- ?? .NET 5 is out of support and no longer receives security updates
- Risk of security vulnerabilities
- Missing out on performance improvements
- Difficulty hiring developers for outdated tech

**Expected Outcome:**
- Application runs on .NET 8 LTS
- All dependencies updated to .NET 8 compatible versions
- No functionality regression
- Build and tests pass
- Application successfully deploys

**Completion Criteria:**
- Project targets .NET 8.0
- All NuGet packages updated to compatible versions
- Application builds without errors
- All existing tests pass
- Application runs successfully in dev/staging

---

### 2. ? Scope Definition

#### IN SCOPE:
1. **Backend Migration:**
   - Update target framework from `net5.0` to `net8.0`
   - Update all NuGet packages to .NET 8 compatible versions
   - Fix breaking changes in:
     - ASP.NET Core 6, 7, 8 breaking changes
     - Entity Framework Core 6, 7, 8 breaking changes
     - Identity Server ? OpenIddict/Duende migration (if needed)
   - Update `Program.cs` and `Startup.cs` to .NET 8 patterns
   - Update middleware configuration

2. **Project Files:**
   - Update `.csproj` files
   - Update `global.json` (if exists)
   - Update `launchSettings.json`

3. **Testing:**
   - Ensure all unit tests pass
   - Ensure integration tests pass
   - Manual testing of critical features

4. **Configuration:**
   - Update `appsettings.json` if needed
   - Update deployment configurations

5. **Documentation:**
   - Update README with .NET 8 requirements
   - Update setup instructions
   - Document breaking changes

#### OUT OF SCOPE:
- ? Angular frontend updates (separate task if needed)
- ? Database schema changes (unless required by EF Core 8)
- ? New feature development
- ? UI/UX changes
- ? Performance optimization (beyond what .NET 8 provides)
- ? Architectural refactoring
- ? Cloud migration or deployment changes
- ? Third-party API changes

#### Related Tasks (to be done separately):
- Evaluate and migrate from Identity Server 4 to Duende/OpenIddict (if not already done)
- Review and update deployment pipelines for .NET 8
- Performance benchmarking and optimization
- Update monitoring/logging for .NET 8 improvements

---

### 3. ? Technical Context

#### Files/Directories to Modify:

**Core Project Files:**
- `gdm5.0/gdm5.0.csproj` ? (main project file)
- `gdm5.0/Program.cs` (hosting configuration)
- `gdm5.0/Startup.cs` (middleware, services)

**Configuration:**
- `gdm5.0/Properties/launchSettings.json`
- `gdm5.0/appsettings.json`
- `gdm5.0/appsettings.Development.json`

**Potential Breaking Change Areas:**
- `Models/DataContext.cs` - EF Core changes
- `Data/ApplicationDbContext.cs` - Identity changes
- `Startup.cs` - Middleware order changes
- Authentication/Authorization setup
- JSON serialization (System.Text.Json changes)

#### Architectural Patterns to Follow:
- **Layered Architecture:** Controllers ? Services ? Domain ? Data
- **Service Layer Pattern:** Keep existing service interfaces
- **DTO Pattern:** Maintain existing request/response patterns
- **Dependency Injection:** Use built-in .NET 8 DI
- **Minimal API consideration:** Optional - evaluate for new endpoints only

#### Dependencies:
**Current Stack (needs updating):**
- .NET 5.0 ? .NET 8.0
- Entity Framework Core 5.0.4 ? 8.0.x
- Identity Server 4 ? Evaluate migration path
- System.Linq.Dynamic.Core ? Check compatibility
- MigraDoc/PdfSharp ? Check compatibility

**Angular Frontend:**
- Angular 16.2.3 (no changes needed for this task)
- Should remain compatible with .NET 8 backend

#### APIs/Frameworks:
- ASP.NET Core Web API
- Entity Framework Core
- ASP.NET Core Identity
- JWT Bearer Authentication
- SQL Server (no driver changes expected)

---

### 4. ? Constraints

#### Technical Constraints:
1. **Compatibility:**
   - Must maintain backward compatibility with existing database schema
   - API contracts must remain unchanged (no breaking changes for frontend)
   - Existing authentication tokens should remain valid (if possible)

2. **Performance:**
   - Application must perform at least as well as .NET 5 version
   - No increase in startup time
   - No degradation in API response times

3. **Database:**
   - No breaking database changes
   - EF Core migrations must be compatible
   - Existing data must remain accessible

4. **Dependencies:**
   - All third-party packages must have .NET 8 compatible versions
   - If packages are incompatible, find alternatives

#### Business Constraints:
1. **Timeline:** TBD (recommend 2-3 weeks for thorough testing)
2. **Zero Downtime:** Plan phased rollout if possible
3. **Testing:** Comprehensive testing required before production
4. **Budget:** Use existing tools/licenses only

#### Security Requirements:
1. Authentication must continue to work
2. Authorization rules must be preserved
3. No security regressions
4. Update to latest security patches

#### Tool Restrictions:
- No new paid tools/services
- Use existing development environment
- Visual Studio 2022 (supports .NET 8)
- SQL Server (existing version)

#### Client-Specific Restrictions:
- Must maintain data privacy compliance
- No changes to data storage/handling
- Audit logs must continue working

---

### 5. ? Acceptance Criteria

#### Must Pass:
1. ? **Project builds successfully** targeting .NET 8.0
2. ? **All NuGet packages updated** to .NET 8 compatible versions
3. ? **Zero compilation errors** or warnings (aim for warning-free)
4. ? **All unit tests pass** (existing test suite)
5. ? **Integration tests pass** (if exist)
6. ? **Application starts successfully** in development
7. ? **Database migrations work** correctly
8. ? **Authentication works** (login, JWT tokens)
9. ? **Authorization works** (role-based access)
10. ? **CRUD operations work** for all main entities:
    - Products
    - Orders
    - Customers
    - Price Lists
11. ? **API endpoints respond** correctly
12. ? **Frontend can communicate** with backend (no CORS issues)
13. ? **File uploads work** (if applicable)
14. ? **PDF generation works** (MigraDoc/PdfSharp)
15. ? **Dynamic LINQ queries work**

#### Documentation Updated:
- [ ] README.md reflects .NET 8 requirements
- [ ] Setup instructions updated
- [ ] Breaking changes documented
- [ ] Migration guide created (if complex changes)

#### Testing Checklist:
- [ ] User authentication flow
- [ ] Product search and parametric filtering
- [ ] Order creation and status workflow
- [ ] Price list operations
- [ ] Customer management
- [ ] Report generation
- [ ] Currency conversion

#### Performance Benchmarks:
- API response time ? existing .NET 5 performance
- Startup time ? existing .NET 5 performance
- Memory usage ? existing .NET 5 usage

---

### 6. ? Context & References

#### Issue Link:
- **GitHub Issue:** #2 - "Update .NET 5 to version 8 and its dependencies"
- **Repository:** https://github.com/garin87/gdm_v5

#### Project Context:
- **Project:** GDM 5.0 ERP System
- **Current State:** .NET 5 (EOL May 2022)
- **Target State:** .NET 8 LTS (supported until Nov 2026)
- **Branch:** AI-tooling-setup

#### Relevant Documentation:
- **PROJECT_MAP.md** - Complete architecture overview
- **.NET 5 ? 8 Migration Guide:** https://learn.microsoft.com/en-us/aspnet/core/migration/50-to-60
- **.NET 6 ? 7 Changes:** https://learn.microsoft.com/en-us/aspnet/core/migration/60-to-70
- **.NET 7 ? 8 Changes:** https://learn.microsoft.com/en-us/aspnet/core/migration/70-to-80
- **EF Core 5 ? 8:** https://learn.microsoft.com/en-us/ef/core/what-is-new/

#### Similar Examples:
- Search for ".NET 5 to .NET 8 migration" examples
- Check GitHub for similar migrations in ERP systems

#### Key Decisions:
- ?? **Identity Server 4 End of Life** - Need to decide migration path:
  - Option 1: Duende Identity Server (commercial for production)
  - Option 2: OpenIddict (open source)
  - Option 3: Plain JWT without Identity Server
  - **Decision needed:** Which path to take?

---

### 7. ?? Dependencies & Blockers

#### Current Blockers:
1. **Identity Server 4 Decision** ??
   - Identity Server 4 is deprecated
   - Need to choose replacement before full migration
   - **Recommendation:** Evaluate if Identity Server features are needed
   - **Alternative:** Use plain JWT Bearer authentication

2. **Testing Environment** ??
   - Need access to testing database
   - Need staging environment for validation
   - **Action Required:** Confirm test environment availability

#### Dependencies:
1. **External:**
   - SQL Server (already in place)
   - GitHub access (for issue tracking)
   - Development environment with .NET 8 SDK

2. **Internal:**
   - No blocking PRs
   - No conflicting branches
   - Team availability for testing

3. **Package Compatibility:**
   - Need to verify all NuGet packages have .NET 8 versions
   - Some packages may need alternatives

#### Prerequisites:
- [ ] .NET 8 SDK installed on dev machine
- [ ] Visual Studio 2022 (17.8 or later)
- [ ] Database backup before testing
- [ ] Testing environment prepared

---

### 8. ? Risk Assessment

#### High Risks:
1. **Identity Server 4 Migration** ??
   - **Risk:** Complex authentication changes
   - **Impact:** Users can't log in
   - **Mitigation:** Thoroughly test auth flow; have rollback plan
   - **Recommendation:** Tackle this separately if complex

2. **Breaking Changes in EF Core** ??
   - **Risk:** Database queries fail
   - **Impact:** Data access broken
   - **Mitigation:** Review EF Core 8 breaking changes; test all queries

3. **Third-Party Package Incompatibility** ??
   - **Risk:** Packages don't support .NET 8
   - **Impact:** Features broken; need alternatives
   - **Mitigation:** Research packages early; identify alternatives

#### Medium Risks:
1. **JSON Serialization Changes** ??
   - System.Text.Json changes between versions
   - May affect API responses
   - **Mitigation:** Test API contracts thoroughly

2. **Middleware Ordering** ??
   - Changes in middleware pipeline
   - May affect authentication/authorization
   - **Mitigation:** Follow .NET 8 best practices

3. **Performance Regression** ??
   - Unlikely but possible
   - **Mitigation:** Benchmark before/after

#### Rollback Plan:
1. Keep .NET 5 branch intact
2. Test in staging before production
3. Have database backup
4. Phased rollout: dev ? staging ? production
5. Monitor errors closely after deployment

#### Breaking Changes to Watch:
- Minimal hosting model changes
- Authentication middleware changes
- JSON serialization behavior
- Nullability handling
- Async stream changes
- HTTP client changes

---

### 9. ? Testing Strategy

#### Unit Tests:
- Run existing test suite
- All tests must pass
- No skipped tests
- Test coverage should not decrease

#### Integration Tests:
- Database operations (CRUD)
- Authentication flow
- Authorization checks
- API endpoint functionality

#### Manual Testing Checklist:

**Authentication:**
- [ ] User registration
- [ ] User login
- [ ] JWT token generation
- [ ] Token validation
- [ ] Password reset
- [ ] Role assignment

**Core Features:**
- [ ] Product CRUD operations
- [ ] Product parametric search
- [ ] Order creation and updates
- [ ] Order status workflow
- [ ] Customer management
- [ ] Price list operations
- [ ] Currency conversion
- [ ] Warehouse operations

**Reports & Export:**
- [ ] PDF generation
- [ ] Data export
- [ ] Report generation

**Edge Cases:**
- [ ] Large dataset queries
- [ ] Concurrent user access
- [ ] Invalid input handling
- [ ] Permission boundary testing

#### Testing Environments:
1. **Development:** Local machine
2. **Staging:** Pre-production environment
3. **Production:** After successful staging validation

#### Automated Testing:
```bash
# Run all tests
dotnet test

# Build project
dotnet build

# Check for warnings
dotnet build /warnaserror
```

---

### 10. ? Definition of Done

#### Code Complete:
- [ ] All `.csproj` files target `net8.0`
- [ ] All NuGet packages updated
- [ ] Breaking changes resolved
- [ ] Code compiles without errors
- [ ] Code compiles without warnings (or documented exceptions)
- [ ] New code follows project patterns

#### Tests Complete:
- [ ] All unit tests pass
- [ ] All integration tests pass
- [ ] Manual testing completed
- [ ] Edge cases tested
- [ ] Performance validated

#### Documentation Complete:
- [ ] README.md updated with .NET 8 requirements
- [ ] CHANGELOG.md entry added
- [ ] Breaking changes documented
- [ ] Setup instructions updated
- [ ] Migration notes added to PROJECT_MAP.md

#### Review Complete:
- [ ] Code review completed
- [ ] Architecture review (if significant changes)
- [ ] Security review (authentication changes)
- [ ] Performance review

#### Deployment Ready:
- [ ] Application runs in dev
- [ ] Application runs in staging
- [ ] Database migrations tested
- [ ] Rollback plan documented
- [ ] Deployment checklist created

#### Communication Complete:
- [ ] Team notified of changes
- [ ] Stakeholders informed
- [ ] Known issues documented
- [ ] Post-deployment validation plan shared

---

## DoR Score: 9/10

### ? Complete Categories (9):
1. ? Objective Clarity
2. ? Scope Definition
3. ? Technical Context
4. ? Constraints
5. ? Acceptance Criteria
6. ? Context & References
7. ? Risk Assessment
8. ? Testing Strategy
9. ? Definition of Done

### ?? Incomplete Categories (1):
7. ?? Dependencies & Blockers - **Needs attention:**
   - **Identity Server 4 decision needed**
   - **Testing environment confirmation needed**

---

## ?? Critical Blockers

### 1. Identity Server 4 Migration Path ??
**Issue:** Identity Server 4 is end-of-life. Need to decide migration strategy.

**Options:**
- **Option A:** Migrate to Duende Identity Server (commercial license required for production)
- **Option B:** Migrate to OpenIddict (open source)
- **Option C:** Simplify to plain JWT Bearer authentication (remove Identity Server)

**Recommendation:** Evaluate if full Identity Server features are needed:
- If YES ? Use OpenIddict (open source, actively maintained)
- If NO ? Simplify to JWT Bearer (easier migration)

**Action Required:** Decide migration path before proceeding

**Impact:** High - Authentication is critical functionality

---

## ?? Recommended Next Steps

### Immediate (Before Starting):
1. **?? CRITICAL:** Make Identity Server decision
   - Review current Identity Server usage
   - Decide: Duende vs OpenIddict vs Plain JWT
   - Document decision in issue #2

2. **Confirm Testing Environment**
   - Verify staging environment availability
   - Create database backup
   - Ensure test data is available

3. **Install Prerequisites**
   - Install .NET 8 SDK
   - Update Visual Studio to 2022 (17.8+)
   - Verify all team members have tools

### Phase 1: Assessment (1-2 days)
1. Audit all NuGet packages
2. Check .NET 8 compatibility
3. Identify packages needing alternatives
4. Review breaking changes documentation
5. Create detailed migration checklist

### Phase 2: Migration (3-5 days)
1. Update project files to target .NET 8
2. Update NuGet packages (conservative approach)
3. Fix compilation errors
4. Address breaking changes
5. Update Program.cs/Startup.cs

### Phase 3: Testing (3-5 days)
1. Run automated tests
2. Perform manual testing
3. Test authentication flow
4. Test all critical features
5. Performance validation

### Phase 4: Documentation & Deployment (1-2 days)
1. Update documentation
2. Create deployment plan
3. Deploy to staging
4. Validate in staging
5. Deploy to production (when ready)

---

## ?? Agent-Ready Task Specification

### For AI Agent Execution:

```markdown
# Task: Migrate GDM 5.0 from .NET 5 to .NET 8

## Objective
Update the GDM 5.0 ERP application from .NET 5 (EOL) to .NET 8 LTS, 
ensuring all dependencies are updated and application functionality is preserved.

## Scope
- Update all .csproj files to target net8.0
- Update NuGet packages to .NET 8 compatible versions
- Fix all breaking changes
- Maintain API compatibility with Angular frontend
- Ensure zero functionality regression

## Out of Scope
- Frontend changes
- New feature development
- Architectural refactoring
- Performance optimization beyond framework improvements

## Files to Modify
Primary: gdm5.0/gdm5.0.csproj, Program.cs, Startup.cs
Review: All service files, DataContext.cs, authentication configuration

## Constraints
- No breaking API changes
- Database schema must remain compatible
- Must use existing authentication approach
- Zero downtime deployment

## Acceptance Criteria
1. Application builds targeting net8.0
2. All tests pass
3. Authentication works
4. All CRUD operations function
5. No performance regression

## Known Blockers
- Identity Server 4 migration decision (see issue comments)
- Testing environment confirmation needed

## References
- Issue #2: https://github.com/garin87/gdm_v5/issues/2
- PROJECT_MAP.md for architecture details
- Microsoft .NET 5?8 migration guides

## Testing Required
- Automated: All existing tests
- Manual: Authentication, CRUD operations, reports
- Performance: API response times, startup time

## Definition of Done
- Builds without errors/warnings
- All tests pass
- Documentation updated
- Runs successfully in dev/staging
```

---

## Summary

**Status:** ? **ALMOST READY** - One critical decision needed

**DoR Score:** 9/10

**Blocker:** Identity Server 4 migration strategy must be decided first

**Once Blocker Resolved:** Task is fully ready for AI agent execution

**Estimated Effort:** 2-3 weeks (including testing)

**Risk Level:** Medium (manageable with proper testing)

---

**Created:** $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")  
**Task:** Issue #2  
**Repository:** garin87/gdm_v5  
**Reviewer:** [To be assigned]
