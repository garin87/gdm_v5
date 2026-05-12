# Cartograph Skill

## Purpose
Automatically provide repository structure and architectural context to AI agents working with this codebase.

## Trigger
This skill is **always active** and provides context for all AI interactions with the repository.

## Behavior
When invoked (automatically or manually), this skill:
1. References the authoritative project map at `PROJECT_MAP.md`
2. Provides high-level architectural understanding
3. Guides code navigation and feature development
4. Ensures consistency with established patterns

## Context Provided

### Repository Overview
- **Project Name:** GDM 5.0 (gdm5._0)
- **Type:** Full-stack ERP system
- **Backend:** ASP.NET Core 5.0 + Entity Framework Core + SQL Server
- **Frontend:** Angular 16 + Angular Material + Bootstrap 5
- **Architecture:** Layered (Controllers ? Services ? Domain ? Data Access)

### Key Entry Points

#### Backend Entry Points
1. **`Program.cs`** - Application startup and host configuration
2. **`Startup.cs`** - Service registration, middleware pipeline, authentication setup
3. **`Models/DataContext.cs`** - Main database context with all entity sets
4. **`Data/ApplicationDbContext.cs`** - Identity/authentication database context

#### Frontend Entry Points
1. **`ClientApp/src/main.ts`** - Angular application bootstrap
2. **`ClientApp/src/app/app.module.ts`** - Root module with all feature imports
3. **`ClientApp/src/app/app-routing.module.ts`** - Application routing configuration

### Core Domain Modules

#### 1. Product Management
- **Location:** `Models/Product/`, `Services/Product/`, `Controllers/ProductsController.cs`
- **Purpose:** Product catalog with dynamic parameters
- **Key Entities:** Product, ProductType, ProductParameter, Parameter
- **Features:** Parametric search, history tracking, multi-currency pricing

#### 2. Order Management
- **Location:** `Models/Order/`, `Services/Order/`, `Controllers/OrdersController.cs`
- **Purpose:** Order processing and fulfillment
- **Key Entities:** Order, OrderProduct, OrderStatus
- **Features:** Status workflow, order history, customer association

#### 3. Price List Management
- **Location:** `Models/PriceList/`, `Services/PriceList/`, `Controllers/PriceList/`
- **Purpose:** Multi-version price management
- **Key Entities:** PriceList, PriceListValue, ComplexPriceList
- **Features:** Version control, bulk operations, currency support

#### 4. Customer Management
- **Location:** `Models/Customer.cs`, `Services/Customer/`, `Controllers/Customer/`
- **Purpose:** Customer/client data management
- **Key Entities:** Customer
- **Features:** CRUD operations, order association

#### 5. Authentication & Authorization
- **Location:** `Models/UserManagement/`, `Services/AuthService.cs`, `Controllers/AuthController.cs`
- **Purpose:** User authentication and JWT token management
- **Key Entities:** ApplicationUser, User, Role
- **Features:** Identity Server 4, JWT Bearer tokens, role-based access

#### 6. Currency Management
- **Location:** `Models/Currency/`, `Services/Currency/`, `Controllers/Currency/`
- **Purpose:** Multi-currency support with exchange rates
- **Key Entities:** Currency, CurrencyRate
- **Features:** Currency conversion, rate history

#### 7. Warehouse Management
- **Location:** `Models/WareHouse.cs`, `Services/WareHouse/`, `Controllers/WareHouse/`
- **Purpose:** Inventory location tracking
- **Key Entities:** WareHouse
- **Features:** Location-based inventory

### Important Architectural Patterns

#### 1. Service Layer Pattern
```
Interface Definition: Services/Interfaces/I{Entity}Service.cs
Implementation: Services/{Domain}/{Entity}Service.cs
Base Classes: Services/Base/BaseService.cs
```

#### 2. DTO Pattern
```
Request Models: Requests/{Domain}/add{Entity}Request.cs
Response Models: Responses/ApplicationResponseGeneric.cs
Data Transfer: DTO/{Domain}/{Entity}DTO.cs
```

#### 3. Filter Pattern
```
Filter Models: Domain/Models/Filters/{Entity}Filter.cs
Filter Assigners: Filters/{Entity}Assigner.cs
Purpose: Dynamic LINQ query building
```

#### 4. History Tracking Pattern
```
History Entities: Models/{Domain}/{Entity}History.cs
Purpose: Audit trail and change tracking
Pattern: Parallel history table for auditable entities
```

#### 5. Base Entity Pattern
```
Base Class: Models/BaseObject.cs
Common Properties: Id, CreatedDate, ModifiedDate, IsDeleted
Usage: All domain entities inherit from BaseObject
```

### Key Relationships

#### Product Relationships
- Product ? ProductType (categorization)
- Product ? Currency (pricing currency)
- Product ? WareHouse (storage location)
- Product ? ProductParameter (dynamic attributes)
- Product ? PriceListValue (pricing)
- Product ? OrderProduct (order line items)

#### Order Relationships
- Order ? Customer (buyer)
- Order ? Currency (transaction currency)
- Order ? OrderStatus (lifecycle state)
- Order ? User (order owner/creator)
- Order ? OrderProduct (line items)

#### Price List Relationships
- PriceList ? ComplexPriceList (composite lists)
- PriceList ? PriceListValue (price entries)
- PriceListValue ? Product (product pricing)

### Technology Stack Quick Reference

#### Backend
- **Framework:** .NET 5.0 (?? EOL - consider upgrading to .NET 8)
- **ORM:** Entity Framework Core 5.0.4
- **Database:** SQL Server
- **Auth:** Identity Server 4 + JWT Bearer
- **Dynamic Queries:** System.Linq.Dynamic.Core
- **PDF Generation:** MigraDoc + PdfSharp

#### Frontend
- **Framework:** Angular 16.2.3
- **UI Library:** Angular Material 16.2.2
- **Styling:** Bootstrap 5.3.1
- **State:** RxJS 7.8.1
- **HTTP:** Angular HttpClient + JWT Interceptor

### Development Workflow

#### Adding a New Feature
1. **Define Model:** Add/modify entity in `Models/{Domain}/`
2. **Create Service Interface:** Define contract in `Services/Interfaces/`
3. **Implement Service:** Create service in `Services/{Domain}/`
4. **Add Controller:** Create API endpoints in `Controllers/`
5. **Create DTOs:** Add request/response objects in `DTO/`, `Requests/`, `Responses/`
6. **Frontend Component:** Add Angular component in `ClientApp/src/app/{feature}/`
7. **Wire Up Service:** Create/update Angular service for API calls

#### Database Changes
```bash
# Add migration
dotnet ef migrations add {MigrationName}

# Review generated migration in Migrations/ folder

# Apply migration
dotnet ef database update
```

#### Frontend Development
```bash
# Install dependencies
cd ClientApp
npm install --legacy-peer-deps

# Run dev server (with proxy to backend)
npm start

# Build for production
npm run build -- --configuration production
```

### Common Code Patterns

#### Service Method Pattern
```csharp
public async Task<ApplicationResponseGeneric<EntityDTO>> GetEntityAsync(int id)
{
    try
    {
        var entity = await _context.Entities
            .Include(e => e.RelatedEntity)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entity == null)
            return new ApplicationResponseGeneric<EntityDTO> { Success = false };

        var dto = MapToDTO(entity);
        return new ApplicationResponseGeneric<EntityDTO> 
        { 
            Success = true, 
            Data = dto 
        };
    }
    catch (Exception ex)
    {
        // Log exception
        return new ApplicationResponseGeneric<EntityDTO> { Success = false };
    }
}
```

#### Controller Pattern
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EntitiesController : ControllerBase
{
    private readonly IEntityService _service;

    public EntitiesController(IEntityService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var response = await _service.GetEntityAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }
}
```

#### Angular Service Pattern
```typescript
@Injectable({
  providedIn: 'root'
})
export class EntityService {
  private apiUrl = '/api/entities';

  constructor(private http: HttpClient) { }

  getEntity(id: number): Observable<ApplicationResponseGeneric<EntityDTO>> {
    return this.http.get<ApplicationResponseGeneric<EntityDTO>>(`${this.apiUrl}/${id}`);
  }
}
```

### Navigation Tips

#### Finding Functionality
- **API Endpoints:** Start in `Controllers/` - methods map to HTTP verbs
- **Business Logic:** Look in `Services/{Domain}/` - organized by feature
- **Data Models:** Check `Models/{Domain}/` - database entities
- **Database Schema:** Review `Models/DataContext.cs` - all DbSets listed
- **Filtering Logic:** See `Filters/` - complex query building
- **Request/Response Contracts:** Browse `Requests/` and `Responses/`

#### Common File Locations
- Configuration: `appsettings.json`, `Startup.cs`
- Database Contexts: `Models/DataContext.cs`, `Data/ApplicationDbContext.cs`
- Migrations: `Migrations/` (data), `Data/Migrations/` (identity)
- Frontend Config: `ClientApp/angular.json`, `ClientApp/package.json`
- Routing: `ClientApp/src/app/app-routing.module.ts`

### Important Conventions

#### Naming
- Controllers: `{Entity}Controller.cs` or `{Entity}sController.cs`
- Services: `{Entity}Service.cs` with `I{Entity}Service.cs` interface
- DTOs: `{Entity}DTO.cs`
- Requests: `add{Entity}Request.cs`, `update{Entity}Request.cs`
- Filters: `{Entity}Filter.cs` with `{Entity}Assigner.cs`

#### Response Handling
- Use `ApplicationResponseGeneric<T>` for data responses
- Use `ApplicationResponse` for simple success/failure
- Use `PagedResponseDTO<T>` for paginated results
- Always include `Success` boolean flag

#### Filtering & Pagination
- Filters: Define in `Domain/Models/Filters/`
- Assigners: Implement in `Filters/` to build LINQ queries
- Pagination: Use `PaginationHelper` and `PagingFilter`
- Sorting: Use `SortOptionsDTO` and dynamic LINQ

### Security Considerations
- **Authentication:** JWT Bearer tokens required for most endpoints
- **Authorization:** `[Authorize]` attribute on controllers
- **Identity:** ASP.NET Core Identity + Identity Server 4
- **Token Management:** `TokenService` and `TokenController`
- **User Context:** Available via `HttpContext.User`

### Testing
- **Backend:** Unit tests should go in separate test project
- **Frontend:** Jasmine + Karma for unit tests
- **E2E:** Consider migrating from deprecated Protractor to Playwright/Cypress

### Known Issues & Technical Debt
- ?? **.NET 5.0 is out of support** (EOL: May 2022) - should upgrade to .NET 8 LTS
- Mixed authentication strategies (Identity Server + JWT)
- Protractor (E2E testing) is deprecated
- Some commented-out authentication code needs cleanup

## Detailed Documentation

For comprehensive architectural details, entity relationships, and development guidelines, see:

**? [PROJECT_MAP.md](../../../PROJECT_MAP.md)**

This is the **single source of truth** for project architecture and should be consulted for:
- Complete project structure
- Detailed domain model documentation
- Full technology stack specifications
- Development setup instructions
- AI agent guidance

## Usage

This skill is automatically invoked by Claude Code to provide context. You can also explicitly reference it by mentioning:
- "Use the Cartograph skill"
- "Check the project map"
- "What's the architecture?"
- "How is {feature} organized?"

## Maintenance

Update this skill when:
- Major architectural changes occur
- New major features are added
- Project structure changes significantly
- `PROJECT_MAP.md` is updated with significant changes

**Last Updated:** [Auto-generated on creation]
