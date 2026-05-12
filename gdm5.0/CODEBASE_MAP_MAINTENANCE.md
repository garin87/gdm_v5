# ??? Codebase Map Maintenance Guide

**Purpose:** Keep PROJECT_MAP.md fresh and accurate as the codebase evolves

---

## ?? Why Maintain the Map?

### Problems with Stale Maps:
- ? Misleads AI agents with outdated context
- ? Confuses new developers
- ? Documents nonexistent features
- ? Misses new architecture changes

### Benefits of Fresh Maps:
- ? Accurate AI agent assistance
- ? Reliable onboarding documentation
- ? Up-to-date architecture reference
- ? Single source of truth

---

## ?? Three Update Approaches

### Approach 1: Automatic CI Updates (Recommended) ?

**Best for:** Regular, hands-off maintenance

**How it works:**
- Workflow runs automatically:
  - ? On every merge to main branches
  - ? Weekly (Sundays at midnight)
  - ? After significant PR merges
  - ? Manual trigger anytime

**What it updates:**
- Timestamps
- .NET version
- Package versions
- File counts
- Git information

**Setup:**
Already configured in `.github/workflows/update-codebase-map.yml`

**Trigger manually:**
```bash
# Go to GitHub Actions
# Find "Update Codebase Map" workflow
# Click "Run workflow"
```

---

### Approach 2: Manual Updates per PR

**Best for:** Large architectural changes

**When to use:**
- Major framework updates (like .NET 8 migration)
- Architecture changes
- New major features
- Technology stack changes

**How to do it:**

**Option A: Run the script**
```powershell
cd D:\Dev\Ggm\gdm_v5
.\update-project-map.ps1
```

**Option B: Update manually**
1. Open `PROJECT_MAP.md`
2. Update affected sections
3. Update "Last Updated" timestamp
4. Add to PR:
```bash
git add PROJECT_MAP.md
git commit -m "?? Update PROJECT_MAP.md for [feature name]"
```

---

### Approach 3: PR-Triggered Updates

**Best for:** Ensuring map stays in sync with code

**How it works:**
Add map update to PR checklist:

```markdown
## PR Checklist
- [ ] Code changes complete
- [ ] Tests passing
- [ ] Documentation updated
- [ ] **PROJECT_MAP.md updated** ?
```

---

## ?? Recommended Schedule

### Daily (Automatic):
- ? CI checks on every merge

### Weekly (Automatic):
- ? Scheduled Sunday update
- ? Statistics refresh
- ? Version sync

### Per Major Change (Manual):
- ? Framework updates
- ? New features
- ? Architecture changes
- ? Technology additions

### Quarterly (Review):
- ? Full map review
- ? Structure validation
- ? Accuracy check

---

## ??? Update Methods

### Method 1: Automated CI (Zero Effort)

**Setup once, forget forever:**

1. ? Already configured!
2. ? Workflow file: `.github/workflows/update-codebase-map.yml`
3. ? Runs automatically

**When it runs:**
- On push to main, AI-tooling-setup, develop
- On PR merge
- Weekly schedule
- Manual trigger

**What to do:**
Nothing! It's automatic.

**Check results:**
- Go to: https://github.com/garin87/gdm_v5/actions
- Find "Update Codebase Map" workflow
- See last run

---

### Method 2: Local Script (1 minute)

**Quick manual update:**

```powershell
# Run the update script
cd D:\Dev\Ggm\gdm_v5
.\update-project-map.ps1

# It will:
# 1. Analyze your project
# 2. Update PROJECT_MAP.md
# 3. Show you the changes
# 4. Offer to commit
```

**Output:**
```
??? Updating PROJECT_MAP.md...
?? Analyzing project structure...
? Analysis complete!

?? Project Statistics:
  • C# Files: 150
  • TypeScript Files: 85
  • .NET Version: 8.0
  • EF Core Version: 8.0.0
  • Angular Version: ^16.2.3
  • Identity Server: No (JWT Bearer)

?? Updating PROJECT_MAP.md...
? PROJECT_MAP.md updated successfully!

Do you want to commit these changes? (y/n)
```

---

### Method 3: Manual Edit (5-10 minutes)

**For significant changes:**

1. **Open PROJECT_MAP.md**

2. **Update relevant sections:**
   - Technology Stack
   - Architecture description
   - New features
   - Dependencies

3. **Update metadata:**
```markdown
## Document Ownership
**Last Updated:** $(Get-Date -Format "yyyy-MM-dd HH:mm:ss") - [What changed]
```

4. **Commit:**
```bash
git add PROJECT_MAP.md
git commit -m "?? Update PROJECT_MAP.md: [description]"
```

---

## ?? What to Update

### Always Update (Automatic):
- ? Last Updated timestamp
- ? .NET version
- ? Package versions
- ? File counts
- ? Git info (branch, last commit)

### Update on Changes (Manual):
- ? Architecture diagrams
- ? New features documentation
- ? Technology stack additions
- ? Dependency changes
- ? API endpoint lists
- ? Database schema significant changes

### Review Quarterly:
- ? Business domain description
- ? Project structure accuracy
- ? Authentication flow
- ? Deployment architecture
- ? Key patterns and conventions

---

## ?? Update Triggers

### High Priority (Update Immediately):
1. **Framework version change** (like .NET 5 ? 8)
2. **Authentication system change** (like removing Identity Server)
3. **Database technology change**
4. **Major architecture shift**
5. **New major subsystems**

### Medium Priority (Update with PR):
1. **New API endpoints**
2. **New features**
3. **Dependency additions**
4. **Significant refactoring**

### Low Priority (Automatic is fine):
1. **Package version bumps**
2. **Minor bug fixes**
3. **Code cleanup**
4. **Test additions**

---

## ?? Update Checklist

When updating manually:

### Basic Updates:
- [ ] Update "Last Updated" timestamp
- [ ] Update .NET version if changed
- [ ] Update major package versions
- [ ] Update Identity Server status
- [ ] Verify all technology versions

### Architecture Updates:
- [ ] Update architecture diagrams if changed
- [ ] Document new patterns introduced
- [ ] Update authentication flow
- [ ] Update key architectural decisions

### Feature Updates:
- [ ] Document new features
- [ ] Update API endpoint lists
- [ ] Update data models if significant
- [ ] Update business domain if expanded

### Validation:
- [ ] Verify all links work
- [ ] Check formatting
- [ ] Ensure accuracy
- [ ] Review for clarity

---

## ?? Validation

### How to Validate the Map:

**1. Accuracy Check:**
```powershell
# Run analysis
.\update-project-map.ps1

# Compare with existing map
# Are versions correct?
# Are file counts reasonable?
```

**2. Structure Check:**
```powershell
# Check if PROJECT_MAP.md exists
Test-Path "PROJECT_MAP.md"

# Check sections present
Select-String -Path "PROJECT_MAP.md" -Pattern "## [0-9]+\."
```

**3. Freshness Check:**
```powershell
# When was it last updated?
Select-String -Path "PROJECT_MAP.md" -Pattern "Last Updated"
```

---

## ?? Quick Commands

### Update map locally:
```powershell
cd D:\Dev\Ggm\gdm_v5
.\update-project-map.ps1
```

### Trigger CI update:
```bash
# Go to GitHub Actions
# Actions ? Update Codebase Map ? Run workflow
```

### View CI update history:
```bash
# GitHub ? Actions ? Update Codebase Map
# See all runs and results
```

### Compare versions:
```bash
git diff HEAD~1 PROJECT_MAP.md
```

### Revert if needed:
```bash
git checkout HEAD~1 PROJECT_MAP.md
```

---

## ?? Best Practices

### DO:
- ? Update map as part of major PRs
- ? Use automated updates for routine changes
- ? Keep descriptions concise and accurate
- ? Review map quarterly for accuracy
- ? Commit map with meaningful messages
- ? Include map in code review

### DON'T:
- ? Let map go stale for months
- ? Document implementation details (keep high-level)
- ? Include outdated information
- ? Skip updates for major changes
- ? Over-document (keep it useful, not exhaustive)

---

## ?? Training Team

### For New Team Members:
1. Read PROJECT_MAP.md first
2. Understand it's maintained automatically
3. Update it when making significant changes
4. Use it as source of truth

### For Existing Team:
1. Review current map
2. Note automatic update workflow
3. Use manual script for big changes
4. Include map updates in PR reviews

---

## ?? Metrics to Track

### Map Health Indicators:

**Freshness:**
- Last updated < 1 week: ? Excellent
- Last updated < 1 month: ?? Good
- Last updated > 1 month: ?? Needs update

**Accuracy:**
- Versions match csproj: ? Accurate
- Structure reflects code: ? Accurate
- Dead links: ?? Fix needed

**Usefulness:**
- New developers use it: ? Useful
- AI agents reference it: ? Useful
- Team updates it: ? Maintained

---

## ?? Example Update Scenarios

### Scenario 1: .NET 8 Migration (Just completed!)

**What to update:**
```markdown
? Framework version: .NET 5.0 ? 8.0
? Package versions: EF 5.0.4 ? 8.0.0
? Authentication: Identity Server removed
? New packages: JWT Bearer 8.0.0
? Last Updated timestamp
```

**How:**
```powershell
.\update-project-map.ps1
# Or let CI do it automatically
```

---

### Scenario 2: New Feature (Product Reviews)

**What to update:**
```markdown
? Business Domain section
? New API endpoints
? New models
? Last Updated timestamp
```

**How:**
```markdown
## Business Domain
- Product reviews and ratings (NEW)

## API Endpoints
POST /api/products/{id}/reviews (NEW)
GET /api/products/{id}/reviews (NEW)
```

---

### Scenario 3: Technology Addition (Redis Cache)

**What to update:**
```markdown
? Technology Stack
? Architecture section
? Dependencies
? Last Updated timestamp
```

**How:**
```markdown
### Backend
- **Caching:** Redis 7.2 (NEW)

## Dependencies
- Redis client
```

---

## ?? Integration with Development Workflow

### In Your Workflow:

**1. Feature Development:**
```
Code ? Tests ? Docs ? **Map Update** ? PR
```

**2. PR Review:**
```
Code Review ? **Map Review** ? Approve ? Merge
```

**3. After Merge:**
```
Merge ? **CI Updates Map** ? Done
```

---

## ?? Getting Help

### Map Issues:
- Script not working? Check PowerShell version
- CI not running? Check workflow file
- Map inaccurate? Run manual update

### Questions:
- How often to update? ? Let CI handle routine, manual for big changes
- What to document? ? High-level architecture, not implementation
- Who updates? ? Everyone! Part of development process

---

## ? Setup Complete!

You now have:
- [x] ? Automated CI updates (weekly + on merge)
- [x] ? Manual update script (`update-project-map.ps1`)
- [x] ? Updated PROJECT_MAP.md (with .NET 8 info)
- [x] ? Maintenance workflow
- [x] ? Best practices documented

---

## ?? Quick Start

### Right Now:
```powershell
# Update map with .NET 8 migration info
cd D:\Dev\Ggm\gdm_v5
.\update-project-map.ps1
```

### Ongoing:
- Let CI handle routine updates automatically
- Update manually for major changes
- Review quarterly for accuracy

---

**Maintenance Status:** ? Automated  
**Update Frequency:** Weekly (automatic) + per major change (manual)  
**Last Update Tool Run:** [Run the script to see]  
**CI Workflow:** Configured and ready

**Your map will stay fresh automatically!** ????
