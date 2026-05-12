# GDM 5.0 Project Map

## Document Ownership
**Owner:** Development Team  
**Last Updated:** $(Get-Date -Format "yyyy-MM-dd HH:mm:ss") - Updated for .NET 8 migration  
**Purpose:** Architectural reference for development team and AI-assisted tooling  
**Version:** 2.0 (Post .NET 8 Migration)

---

## 1. Project Overview

**Project Name:** GDM 5.0 (gdm5._0)  
**Type:** Full-stack web application  
**Architecture:** ASP.NET Core 8.0 Backend + Angular 16 Frontend (SPA)  
**Repository:** https://github.com/garin87/gdm_v5  
**Branch:** feature/dotnet8-migration (migration in progress)

### Business Domain
Enterprise Resource Planning (ERP) system focused on:
- Product inventory management with parametric search
- Order processing and tracking
- Customer relationship management
- Multi-currency price list management
- Warehouse operations

---

## 2. Technology Stack

### Backend (.NET 8.0)
- **Framework:** ASP.NET Core 8.0
- **Language:** C# 10+
- **ORM:** Entity Framework Core 8.0.0
- **Database:** SQL Server
- **Authentication:** 
  - ASP.NET Core Identity
  - JWT Bearer Tokens (Microsoft.AspNetCore.Authentication.JwtBearer 8.0.0)
  - ~~IdentityServer4~~ (Removed in .NET 8 migration - using pure JWT Bearer)
- **Key Libraries:**
  - EntityFramework.DynamicLinq 1.7.2 (dynamic querying)
  - System.Linq.Dynamic.Core 1.7.2 (parametric search)
  - MigraDoc/PdfSharp 1.3.67 (PDF generation)
  - Newtonsoft.Json 13.0.4
  - Realm 11.7.0 (major version update)

### Frontend (Angular 16)
- **Framework:** Angular 16.2.3
- **UI Components:** Angular Material 16.2.2
- **Styling:** Bootstrap 5.3.1
- **Authentication:** @auth0/angular-jwt 5.1.2
- **State Management:** RxJS 7.8.1

### Development Tools
- Angular CLI 16.2.1
- TypeScript
- Karma (testing)
- Jasmine (testing framework)

---

## 3. Project Structure

### 3.1 Backend Structure (C#)

```
gdm5.0/
??? Controllers/                    # API Controllers
?   ??? Base/                      # Base controllers
?   ??? Customer/                  # Customer management endpoints
?   ??? Currency/                  # Currency management endpoints
?   ??? Order/                     # Order processing endpoints
?   ??? PriceList/                 # Price list endpoints
?   ??? Product/                   # Product catalog endpoints
?   ??? WareHouse/                 # Warehouse management endpoints
?   ??? AuthController.cs          # Authentication endpoints
?   ??? MetadataController.cs      # Metadata endpoints
?   ??? TokenController.cs         # Token management
?
??? Models/                         # Domain Models (Database Entities)
?   ??? Product/                   # Product domain
?   ?   ??? Product.cs
?   ?   ??? ProductType.cs
?   ?   ??? ProductParameter.cs
?   ?   ??? Parameter.cs
?   ?   ??? ProductHistory.cs
?   ?   ??? ParameterHistory.cs
?   ?   ??? ProductParameterHistory.cs
?   ??? Order/                     # Order domain
?   ?   ??? Order.cs
?   ?   ??? OrderProduct.cs
?   ?   ??? OrderStatus.cs
?   ?   ??? OrderProductHistory.cs
?   ??? PriceList/                 # Price management
?   ?   ??? PriceList.cs
?   ?   ??? PriceListValue.cs
?   ?   ??? ComplexPriceList.cs
?   ??? Currency/                  # Multi-currency support
?   ?   ??? Currency.cs
?   ?   ??? CurrencyRate.cs
?   ??? Label/                     # Labeling system
?   ?   ??? Label.cs
?   ?   ??? LabelCategory.cs
?   ?   ??? Dictionary.cs
?   ??? UserManagement/            # User & authentication
?   ?   ??? ApplicationUser.cs
?   ?   ??? User.cs
?   ?   ??? Role.cs
?   ??? Customer.cs
?   ??? WareHouse.cs
?   ??? BaseObject.cs              # Base entity class
?   ??? DataContext.cs             # EF Core DbContext
?
??? Domain/                         # Domain Logic & DTOs
?   ??? Models/                    # Domain-specific models
?   ?   ??? Order/                 # Order domain logic
?   ?   ??? Product/               # Product domain logic
?   ?   ??? PriceList/             # Price list domain logic
?   ?   ??? Filters/               # Query filtering models
?   ?   ??? Base/                  # Base domain models
?   ??? Interfaces/                # Domain interfaces
?
??? Services/                       # Business Logic Layer
?   ??? Interfaces/                # Service contracts
?   ?   ??? IOrderService.cs
?   ?   ??? IProductService.cs
?   ?   ??? IPriceListService.cs
?   ?   ??? ICustomerService.cs
?   ?   ??? IAuthService.cs
?   ?   ??? [etc...]
?   ??? Base/                      # Base service implementations
?   ??? Product/                   # Product services
?   ?   ??? ProductService.cs
?   ?   ??? ProductTypeService.cs
?   ?   ??? ProductParameterService.cs
?   ?   ??? ParameterService.cs
?   ?   ??? ProductBase.cs
?   ??? Order/                     # Order services
?   ?   ??? OrderService.cs
?   ?   ??? OrderProductService.cs
?   ?   ??? BaseOrderService.cs
?   ??? PriceList/                 # Price list services
?   ?   ??? PriceListService.cs
?   ?   ??? PriceListValueService.cs
?   ?   ??? ComplexPriceListService.cs
?   ?   ??? PriceListBase.cs
?   ??? Customer/                  # Customer services
?   ??? Currency/                  # Currency services
?   ??? WareHouse/                 # Warehouse services
?   ??? AuthService.cs
?   ??? TokenService.cs
?   ??? RegistrationService.cs
?   ??? MetadataService.cs
?   ??? PDFGenerator.cs            # PDF report generation
?   ??? UriService.cs              # URI helper service
?
??? DTO/                            # Data Transfer Objects
?   ??? Product/                   # Product DTOs
?   ??? Order/                     # Order DTOs
?   ??? Response/                  # Response DTOs
?   ??? Filters/                   # Filter DTOs
?   ??? Sorting/                   # Sorting DTOs
?
??? Requests/                       # Request Models (API inputs)
?   ??? Product/
?   ??? Order/
?   ??? PriceList/
?   ??? Customer/
?   ??? Currency/
?   ??? WareHouse/
?   ??? Base/
?
??? Responses/                      # Response Models (API outputs)
?   ??? ApplicationResponse.cs
?   ??? ApplicationResponseGeneric.cs
?   ??? ApplicationResponseWithMessage.cs
?
??? Filters/                        # Query Filtering Logic
?   ??? Order/                     # Order filters
?   ??? FilterAssigner.cs
?   ??? ProductsAssigner.cs
?   ??? ProductParametersAssigner.cs
?   ??? ParametersAssigner.cs
?   ??? ProductsHistoryAssigner.cs
?
??? Helpers/                        # Helper Utilities
?   ??? PaginationHelper.cs
?   ??? MathHelper.cs
?   ??? DateTimeHelper.cs
?
??? Extensions/                     # Extension Methods
?   ??? FilterExtensions.cs
?   ??? MathExtensions.cs
?   ??? DateTimeExtensions.cs
?
??? Shared/                         # Shared Resources
?   ??? Constants/                 # Application constants
?   ?   ??? ApiConstants.cs
?   ?   ??? ClaimsConstants.cs
?   ?   ??? OrderStatusConstants.cs
?   ??? Enums/                     # Enumerations
?   ?   ??? StatusCodeEnum.cs
?   ?   ??? OrderStatus.cs
?   ?   ??? SortDirectionType.cs
?   ??? GlobalVariables.cs
?
??? Data/                           # Data Access Layer
?   ??? ApplicationDbContext.cs    # Identity DbContext
?   ??? Migrations/                # EF Core Identity migrations
?
??? Migrations/                     # EF Core Data migrations
?
??? Pages/                          # Razor Pages (minimal usage)
?   ??? Error.cshtml
?
??? Program.cs                      # Application entry point
??? Startup.cs                      # Service configuration
??? appsettings.json               # Configuration

```

### 3.2 Frontend Structure (Angular)

```
ClientApp/
??? src/
?   ??? app/
?   ?   ??? alert/                 # Alert/notification components
?   ?   ??? authorization/         # Auth guards & interceptors
?   ?   ??? common/                # Shared components
?   ?   ??? core/                  # Core services & models
?   ?   ??? formfields/            # Reusable form components
?   ?   ??? home/                  # Home/dashboard pages
?   ?   ??? layout/                # Layout components
?   ?   ??? modeling/              # Data modeling components
?   ?   ??? nav-menu/              # Navigation components
?   ?   ??? order/                 # Order management features
?   ?   ??? product/               # Product management features
?   ?   ??? app.module.ts          # Root module
?   ?
?   ??? assets/                    # Static assets
?   ??? environments/              # Environment configs
?   ??? index.html                 # SPA entry point
?
??? package.json                   # NPM dependencies
??? angular.json                   # Angular configuration
??? tsconfig.json                  # TypeScript configuration
```

---

## 4. Core Domain Models

### 4.1 Product Domain
**Purpose:** Manage product catalog with parametric data

**Key Entities:**
- `Product` - Core product entity with pricing and inventory
- `ProductType` - Product categorization
- `ProductParameter` - Dynamic product attributes
- `Parameter` - Parameter definitions
- `ProductHistory` - Historical tracking
- `ParameterHistory` - Parameter change tracking

**Key Relationships:**
- Product ? ProductType (many-to-one)
- Product ? Currency (many-to-one)
- Product ? WareHouse (many-to-one)
- Product ? ProductParameter (one-to-many)
- Product ? PriceListValue (one-to-many)

### 4.2 Order Domain
**Purpose:** Order processing and fulfillment tracking

**Key Entities:**
- `Order` - Order header with customer and totals
- `OrderProduct` - Order line items
- `OrderStatus` - Order lifecycle states
- `OrderProductHistory` - Historical order changes

**Key Relationships:**
- Order ? Customer (many-to-one)
- Order ? Currency (many-to-one)
- Order ? OrderStatus (many-to-one)
- Order ? User (many-to-one)
- Order ? OrderProduct (one-to-many)

### 4.3 Price List Domain
**Purpose:** Multi-version price management with currency support

**Key Entities:**
- `PriceList` - Price list version container
- `PriceListValue` - Individual price entries
- `ComplexPriceList` - Composite price lists

**Key Relationships:**
- PriceList ? ComplexPriceList (many-to-one, optional)
- PriceList ? PriceListValue (one-to-many)
- PriceListValue ? Product (many-to-one)

### 4.4 Currency Domain
**Purpose:** Multi-currency support with exchange rates

**Key Entities:**
- `Currency` - Currency definitions
- `CurrencyRate` - Exchange rate history

### 4.5 Customer Domain
**Purpose:** Customer/client management

**Key Entities:**
- `Customer` - Customer master data

### 4.6 Warehouse Domain
**Purpose:** Inventory location management

**Key Entities:**
- `WareHouse` - Warehouse/location definitions

### 4.7 User Management Domain
**Purpose:** Authentication and authorization

**Key Entities:**
- `ApplicationUser` - ASP.NET Core Identity user
- `User` - Application-specific user data
- `Role` - User roles

---

## 5. Key Architectural Patterns

### 5.1 Layered Architecture
```
Presentation Layer (Controllers/Angular Components)
        ?
Business Logic Layer (Services)
        ?
Domain Layer (Models/Domain Logic)
        ?
Data Access Layer (Entity Framework Core)
        ?
Database (SQL Server)
```

### 5.2 Design Patterns Used

**Repository Pattern**
- Implemented via EF Core DbContext (`DataContext`, `ApplicationDbContext`)

**Service Layer Pattern**
- All business logic encapsulated in service classes
- Interface-based contracts in `Services/Interfaces/`

**DTO Pattern**
- Separate DTOs for API data transfer
- Request/Response objects for API contracts

**Filter Pattern**
- Dynamic filtering via `Filters/` namespace
- Filter assigners for complex queries

**Base Class Pattern**
- `BaseObject` - Common entity properties (Id, audit fields)
- `BaseService` - Common service functionality
- `BaseOrderService`, `ProductBase`, `PriceListBase` - Domain-specific bases

**History Tracking Pattern**
- Separate history tables for audit trails
- `*History` entities track changes over time

---

## 6. API Structure

### 6.1 Authentication & Authorization
- **Endpoint:** `/api/auth`, `/api/token`
- **Methods:** Login, Register, Token Refresh
- **Security:** JWT Bearer tokens

### 6.2 Product Management
- **Endpoint:** `/api/products`, `/api/producttypes`, `/api/parameters`, `/api/productparameters`
- **Features:** CRUD, filtering, sorting, pagination, parametric search

### 6.3 Order Management
- **Endpoint:** `/api/orders`, `/api/orderproducts`
- **Features:** CRUD, status tracking, order reports

### 6.4 Price List Management
- **Endpoint:** `/api/pricelist`, `/api/pricelistvalue`, `/api/complexpricelist`
- **Features:** Version management, bulk operations

### 6.5 Customer Management
- **Endpoint:** `/api/customers`
- **Features:** CRUD operations

### 6.6 Currency Management
- **Endpoint:** `/api/currencies`
- **Features:** Currency and rate management

### 6.7 Warehouse Management
- **Endpoint:** `/api/warehouses`
- **Features:** CRUD operations

### 6.8 Metadata
- **Endpoint:** `/api/metadata`
- **Features:** System metadata retrieval

---

## 7. Data Access Patterns

### 7.1 Database Contexts
- `DataContext` - Main application data
- `ApplicationDbContext` - Identity/authentication data

### 7.2 Migration Strategy
- EF Core Migrations in `Migrations/` directory
- Separate migrations for Identity in `Data/Migrations/`

### 7.3 Query Patterns
- Dynamic LINQ for flexible filtering
- Pagination via `PaginationHelper`
- Sorting via `SortOptionsDTO`
- Filter assigners for complex WHERE clauses

---

## 8. Frontend Architecture

### 8.1 Module Structure
- Feature-based modules (product, order, etc.)
- Shared module for common components
- Core module for singleton services

### 8.2 Routing
- Lazy-loaded feature modules
- Auth guards for protected routes
- Angular Router

### 8.3 State Management
- RxJS for reactive state
- Services as state containers
- No external state library (NgRx, Akita, etc.)

### 8.4 HTTP Communication
- Angular HttpClient
- JWT interceptor for authentication
- Proxy configuration for local development

---

## 9. Key Features

### 9.1 Dynamic Product Parameters
- Products can have custom parameters defined at runtime
- Parameter values stored in `ProductParameter` junction table
- Enables flexible product catalog without schema changes

### 9.2 Multi-Currency Support
- Multiple currencies with exchange rates
- Currency tracking on Products, Orders, PriceLists
- Automatic conversion calculations

### 9.3 Price List Versioning
- Multiple price list versions
- Complex (composite) price lists
- Product-specific pricing

### 9.4 Order Lifecycle Management
- Status-based order workflow
- Order history tracking
- Multi-user order management

### 9.5 Audit Trail / History Tracking
- Historical records for Products, Orders, Parameters
- Change tracking with timestamps and user info

### 9.6 PDF Report Generation
- `PDFGenerator` service
- Uses MigraDoc/PdfSharp libraries
- Order reports, product reports

### 9.7 Dynamic Filtering & Sorting
- `System.Linq.Dynamic.Core` for runtime query building
- Filter assigners translate filter objects to LINQ
- Pagination support

---

## 10. Configuration & Deployment

### 10.1 Configuration Files
- `appsettings.json` - Backend configuration
- `proxy.conf.json` - Frontend proxy for development
- `angular.json` - Angular build configuration
- `package.json` - NPM scripts and dependencies

### 10.2 Build Process
**Backend:**
```bash
dotnet build
dotnet run
```

**Frontend:**
```bash
npm install --legacy-peer-deps
npm start                    # Development
npm run build                # Production build
npm run build:ssr            # Server-side rendering
```

### 10.3 Hosting Model
- `AspNetCoreHostingModel: OutOfProcess`
- Angular SPA served via `SpaServices`
- Separate backend/frontend build outputs

---

## 11. Development Conventions

### 11.1 Naming Conventions
- **Controllers:** `{Entity}Controller.cs`, `{Entity}sController.cs`
- **Services:** `{Entity}Service.cs`, `I{Entity}Service.cs` (interface)
- **Models:** `{Entity}.cs` in appropriate domain folder
- **DTOs:** `{Entity}DTO.cs`
- **Requests:** `add{Entity}Request.cs`, `update{Entity}Request.cs`
- **Angular Components:** `{feature}-{component}.component.ts`

### 11.2 Namespace Structure
- Root namespace: `gdm5._0`
- Sub-namespaces match folder structure

### 11.3 Response Patterns
- Generic responses: `ApplicationResponseGeneric<T>`
- Simple responses: `ApplicationResponse`
- Message responses: `ApplicationResponseWithMessage`
- Paged responses: `PagedResponseDTO<T>`

---

## 12. Dependencies & Third-Party Libraries

### 12.1 Backend NuGet Packages
| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.EntityFrameworkCore.SqlServer | 5.0.4 | SQL Server provider |
| Microsoft.AspNetCore.Identity | 5.0.4 | Authentication |
| System.Linq.Dynamic.Core | 1.2.15 | Dynamic LINQ |
| MigraDocCore | 1.3.57 | PDF generation |
| PdfSharpCore | 1.3.57 | PDF generation |
| Newtonsoft.Json | 13.0.1 | JSON serialization |
| Realm | 10.4.1 | Mobile database sync |

### 12.2 Frontend NPM Packages
| Package | Version | Purpose |
|---------|---------|---------|
| @angular/core | 16.2.3 | Angular framework |
| @angular/material | 16.2.2 | Material Design UI |
| bootstrap | 5.3.1 | CSS framework |
| @auth0/angular-jwt | 5.1.2 | JWT handling |
| rxjs | 7.8.1 | Reactive programming |

---

## 13. Testing Strategy

### 13.1 Backend Testing
- Unit tests (not currently visible in file structure)
- Integration tests (not currently visible)

### 13.2 Frontend Testing
- Jasmine test framework
- Karma test runner
- Unit tests for components/services
- E2E tests via Protractor (legacy)

---

## 14. Known Technical Debt / Considerations

### 14.1 Framework Versions
- ?? **.NET 5.0 is out of support** (EOL: May 2022)
  - **Recommendation:** Upgrade to .NET 8 LTS
- Angular 16 is current but 17/18 available
- Bootstrap 5.3.1 is current

### 14.2 Authentication
- Mixed authentication approaches (Identity Server + JWT)
- Some commented-out cookie authentication code

### 14.3 Frontend Testing
- Protractor is deprecated
- Should migrate to Playwright or Cypress

---

## 15. Getting Started for New Developers

### 15.1 Prerequisites
- .NET 5.0 SDK (or higher for upgrade)
- Node.js 18+ and NPM
- SQL Server (local or remote)
- Visual Studio 2019+ or VS Code

### 15.2 Initial Setup
1. Clone repository from GitHub
2. Restore NuGet packages: `dotnet restore`
3. Update database connection string in `appsettings.json`
4. Run migrations: `dotnet ef database update`
5. Navigate to `ClientApp/` and run `npm install --legacy-peer-deps`
6. Start backend: `dotnet run`
7. Start frontend: `cd ClientApp && npm start`

### 15.3 Key Files to Review
- `Startup.cs` - Service configuration and middleware
- `DataContext.cs` - Database schema
- `Program.cs` - Application entry point
- `ClientApp/src/app/app.module.ts` - Angular module configuration

---

## 16. AI Agent Guidance

### 16.1 When Adding New Features
1. **Model First:** Define/modify entity in `Models/`
2. **Service Layer:** Create interface in `Services/Interfaces/`, implement in `Services/`
3. **Controller:** Add controller in `Controllers/`
4. **DTOs:** Create request/response objects in `DTO/`, `Requests/`, `Responses/`
5. **Frontend:** Add Angular components/services in `ClientApp/src/app/`

### 16.2 Common Patterns to Follow
- Always use DTOs for API communication
- Implement filtering via `FilterAssigner` pattern
- Use `ApplicationResponseGeneric<T>` for API responses
- Maintain history tables for auditable entities
- Follow existing namespace and folder structure

### 16.3 Database Changes
- Create EF Core migration: `dotnet ef migrations add {MigrationName}`
- Review generated migration before applying
- Update database: `dotnet ef database update`

### 16.4 Code Navigation Tips
- Controllers ? Entry points for API endpoints
- Services ? Business logic implementation
- Models ? Database schema
- Domain/Models ? Complex business rules
- Filters ? Query building logic

---

## 17. Contact & Support

**Repository Owner:** garin87  
**GitHub:** https://github.com/garin87/gdm_v5  
**Branch:** AI-tooling-setup

---

## Document Maintenance

This document should be updated when:
- Major architectural changes are made
- New major features are added
- Technology stack is upgraded
- Project structure changes significantly

**Version Control:** This document is version-controlled in the repository root as `PROJECT_MAP.md`

---

*This project map is a living document intended to provide a comprehensive overview of the GDM 5.0 codebase for both human developers and AI-assisted development tools. It should be treated as the single source of truth for project architecture.*
