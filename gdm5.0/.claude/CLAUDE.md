# GDM 5.0 - Always-On Project Guidance

## Project Identity
**Name:** GDM 5.0 (gdm5._0)  
**Type:** Enterprise Resource Planning (ERP) System  
**Architecture:** Full-stack web application with ASP.NET Core net8.0 backend and Angular 16 frontend; IdentityServer is not part of the current authentication architecture

---

## Quick Reference

### For Detailed Architecture
**? See [PROJECT_MAP.md](../PROJECT_MAP.md)** - The single source of truth for project architecture, structure, and patterns.

### Active Skills
This project has the following Claude Code skills configured:

#### ??? **Cartograph Skill** (Always Active)
- **Location:** `.claude/skills/Cartograph/SKILL.md`
- **Purpose:** Provides automatic repository mapping and architectural context
- **Triggers:** Always active for all AI interactions
- **Use:** Reference this for quick navigation and architectural understanding

---

## Core Principles for AI-Assisted Development

### 1. **Architectural Consistency**
- Follow the established layered architecture (Controllers ? Services ? Domain ? Data)
- Maintain separation of concerns
- Use existing patterns (Service Layer, DTO, Filter, Base Class patterns)

### 2. **Single Source of Truth**
- **Architecture:** `PROJECT_MAP.md`
- **Code Structure:** `.claude/skills/Cartograph/SKILL.md`
- **Configuration:** `appsettings.json`, `angular.json`
- Always consult these before making architectural decisions

### 3. **Code Navigation Strategy**
When working with this codebase:
1. **Start with the domain** - Understand the business entity first
2. **Find the model** - Check `Models/{Domain}/`
3. **Locate the service** - Look in `Services/{Domain}/` or `Services/Interfaces/`
4. **Check the controller** - Find API endpoints in `Controllers/`
5. **Review DTOs** - Examine data contracts in `DTO/`, `Requests/`, `Responses/`

### 4. **Pattern Adherence**
Always use established patterns:
- **Services:** Interface in `Services/Interfaces/`, implementation in `Services/{Domain}/`
- **Responses:** Use `ApplicationResponseGeneric<T>` for data, `ApplicationResponse` for simple operations
- **Filtering:** Define filters in `Domain/Models/Filters/`, implement assigners in `Filters/`
- **History:** Create parallel `*History` entities for audit trails
- **Base Classes:** Extend `BaseObject` for all domain entities

---

## Technology Context

### Backend Stack
- **.NET 5.0** (?? Out of support - consider upgrade to .NET 8 LTS)
- **Entity Framework Core 5.0.4** (SQL Server)
- **ASP.NET Core Identity + Identity Server 4** (Authentication)
- **JWT Bearer Tokens** (API Authorization)
- **System.Linq.Dynamic.Core** (Dynamic querying)
- **MigraDoc/PdfSharp** (PDF generation)

### Frontend Stack
- **Angular 16.2.3** (TypeScript framework)
- **Angular Material 16.2.2** (UI components)
- **Bootstrap 5.3.1** (Styling framework)
- **RxJS 7.8.1** (Reactive state management)
- **@auth0/angular-jwt** (JWT handling)

---

## Development Workflow Guidance

### Adding New Features
```
1. Define/Update Model      ? Models/{Domain}/{Entity}.cs
2. Create Service Interface ? Services/Interfaces/I{Entity}Service.cs
3. Implement Service        ? Services/{Domain}/{Entity}Service.cs
4. Register in Startup      ? Startup.cs (ConfigureServices)
5. Create Controller        ? Controllers/{Domain}/{Entity}Controller.cs
6. Define DTOs              ? DTO/, Requests/, Responses/
7. Frontend Service         ? ClientApp/src/app/{feature}/{entity}.service.ts
8. Frontend Component       ? ClientApp/src/app/{feature}/{entity}.component.ts
```

### Database Changes
```bash
# Create migration
dotnet ef migrations add MigrationName

# Review in Migrations/ folder

# Apply to database
dotnet ef database update
```

### Common Tasks

#### Build & Run Backend
```bash
dotnet restore
dotnet build
dotnet run
```

#### Build & Run Frontend
```bash
cd ClientApp
npm install --legacy-peer-deps
npm start                    # Dev server with proxy
npm run build                # Production build
```

---

## Key Domain Areas

### 1. Product Management
- **Path:** `Models/Product/`, `Services/Product/`, `Controllers/ProductsController.cs`
- **Features:** Dynamic parameters, multi-currency pricing, history tracking
- **Key Entities:** Product, ProductType, ProductParameter, Parameter

### 2. Order Management
- **Path:** `Models/Order/`, `Services/Order/`, `Controllers/OrdersController.cs`
- **Features:** Status workflow, order history, customer association
- **Key Entities:** Order, OrderProduct, OrderStatus

### 3. Price List Management
- **Path:** `Models/PriceList/`, `Services/PriceList/`, `Controllers/PriceList/`
- **Features:** Version control, complex price lists, bulk operations
- **Key Entities:** PriceList, PriceListValue, ComplexPriceList

### 4. Customer Management
- **Path:** `Models/Customer.cs`, `Services/Customer/`, `Controllers/Customer/`
- **Features:** Customer CRUD, order association
- **Key Entities:** Customer

### 5. Authentication & Authorization
- **Path:** `Models/UserManagement/`, `Services/AuthService.cs`
- **Features:** JWT tokens, Identity Server, role-based access
- **Key Entities:** ApplicationUser, User, Role

### 6. Currency Management
- **Path:** `Models/Currency/`, `Services/Currency/`
- **Features:** Multi-currency support, exchange rates
- **Key Entities:** Currency, CurrencyRate

### 7. Warehouse Management
- **Path:** `Models/WareHouse.cs`, `Services/WareHouse/`
- **Features:** Inventory location tracking
- **Key Entities:** WareHouse

---

## Code Quality Standards

### For AI Agents
When generating or modifying code:

1. **Match Existing Style**
   - Follow C# and TypeScript conventions used in the codebase
   - Maintain consistent indentation and formatting
   - Use existing naming patterns

2. **Minimal Changes**
   - Make only the changes necessary to achieve the goal
   - Don't refactor unrelated code
   - Preserve existing functionality

3. **Pattern Consistency**
   - Use established architectural patterns
   - Follow existing service/controller/DTO structures
   - Maintain response format consistency

4. **Error Handling**
   - Use try-catch blocks in services
   - Return appropriate `ApplicationResponse*` objects
   - Log exceptions appropriately

5. **Security**
   - Apply `[Authorize]` attribute to protected endpoints
   - Validate user input
   - Use parameterized queries (EF Core handles this)

6. **Testing**
   - Ensure changes don't break existing tests
   - Build the project to verify compilation
   - Test API endpoints after changes

---

## Common Pitfalls to Avoid

### ? Don't
- Create new architectural patterns when existing ones work
- Mix authentication strategies (use JWT Bearer)
- Skip DTO mapping (always use DTOs for API communication)
- Directly expose EF entities via API
- Ignore the filter/assigner pattern for complex queries
- Hard-code configuration values (use `appsettings.json`)

### ? Do
- Follow the layered architecture
- Use dependency injection
- Implement service interfaces
- Create DTOs for all API contracts
- Use `ApplicationResponseGeneric<T>` for responses
- Apply pagination for list endpoints
- Create history entities for audit trails
- Use base classes (`BaseObject`, `BaseService`)

---

## Important Files & Locations

### Configuration
- `appsettings.json` - Backend configuration (connection strings, JWT settings)
- `Startup.cs` - Service registration and middleware pipeline
- `Program.cs` - Application entry point
- `ClientApp/angular.json` - Angular configuration
- `ClientApp/proxy.conf.json` - Development proxy settings

### Database Contexts
- `Models/DataContext.cs` - Main application data
- `Data/ApplicationDbContext.cs` - Identity/authentication data

### Entry Points
- **Backend API:** `https://localhost:5001/api/`
- **Frontend Dev:** `http://localhost:4200/`
- **Angular Proxy:** Routes `/api` to backend

---

## Security Context

### Authentication Flow
1. User logs in via `POST /api/auth/login`
2. Server validates credentials via ASP.NET Core Identity
3. JWT token generated by `TokenService`
4. Token returned to client
5. Client includes token in `Authorization: Bearer {token}` header
6. Backend validates token on protected endpoints

### Authorization
- **Controllers:** Protected with `[Authorize]` attribute
- **Roles:** Defined in `Models/UserManagement/Role.cs`
- **Claims:** Constants in `Shared/Constants/ClaimsConstants.cs`

---

## Performance Considerations

### Backend
- Use `.AsNoTracking()` for read-only queries
- Implement pagination for large result sets (`PaginationHelper`)
- Use `Include()` judiciously to avoid N+1 queries
- Cache frequently accessed data where appropriate

### Frontend
- Lazy load feature modules
- Use `OnPush` change detection strategy
- Unsubscribe from observables to prevent memory leaks
- Implement virtual scrolling for large lists

---

## Debugging & Troubleshooting

### Backend Issues
- Check `Output` window in Visual Studio
- Review exception logs
- Verify database connection string in `appsettings.json`
- Ensure migrations are up to date (`dotnet ef database update`)

### Frontend Issues
- Check browser console for errors
- Verify proxy configuration in `ClientApp/proxy.conf.json`
- Ensure backend is running before starting frontend
- Check Angular dev server output for compilation errors

### Authentication Issues
- Verify JWT token is being sent in requests
- Check token expiration
- Ensure `[Authorize]` attributes are correctly applied
- Review Identity Server configuration in `Startup.cs`

---

## Repository Information

**GitHub:** https://github.com/garin87/gdm_v5  
**Current Branch:** AI-tooling-setup  
**Owner:** garin87

---

## Document Maintenance

### Update This File When:
- Project architecture changes significantly
- New major features are added
- Technology stack is upgraded
- Development workflow changes

### Related Documentation
- **PROJECT_MAP.md** - Comprehensive architecture documentation
- **.claude/skills/Cartograph/SKILL.md** - Cartograph skill configuration
- **ClientApp/README.md** - Angular-specific documentation

---

## Quick Commands Reference

### Backend
```bash
# Restore packages
dotnet restore

# Build project
dotnet build

# Run application
dotnet run

# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Run specific migration
dotnet ef database update MigrationName
```

### Frontend
```bash
# Navigate to frontend
cd ClientApp

# Install dependencies
npm install --legacy-peer-deps

# Start dev server
npm start

# Build for production
npm run build -- --configuration production

# Run tests
npm test

# Lint code
npm run lint
```

### Git
```bash
# Check status
git status

# Create feature branch
git checkout -b feature/your-feature-name

# Commit changes
git add .
git commit -m "Description of changes"

# Push to remote
git push origin branch-name
```

---

## AI Agent Instructions

When working with this codebase:

1. **Always reference** `PROJECT_MAP.md` for architectural decisions
2. **Use the Cartograph skill** for navigation and context
3. **Follow existing patterns** - don't invent new ones
4. **Maintain consistency** with established code style
5. **Test changes** by building the project
6. **Document significant changes** in commit messages
7. **Preserve backward compatibility** unless explicitly asked to break it

### Before Making Changes
- ? Understand the domain area you're working in
- ? Review existing code in that area
- ? Identify the pattern being used
- ? Check for similar implementations

### After Making Changes
- ? Build the project (`dotnet build`)
- ? Fix any compilation errors
- ? Verify the change achieves the goal
- ? Check that related functionality still works

---

**This document provides always-on guidance for AI-assisted development. For detailed architectural information, always refer to PROJECT_MAP.md.**
