# GitHub Copilot Instructions for GDM 5.0

## Project Context

This is a full-stack ERP system with:
- **Backend:** ASP.NET Core 5.0 + Entity Framework Core + SQL Server
- **Frontend:** Angular 16 + Angular Material + Bootstrap 5
- **Architecture:** Layered (Controllers ? Services ? Domain ? Data Access)

## Required Reading

**For detailed architecture, see [PROJECT_MAP.md](../PROJECT_MAP.md)**

This document is the single source of truth for project structure, patterns, and conventions.

## Code Generation Guidelines

### 1. Architectural Patterns

#### Service Layer Pattern
```csharp
// Always create interface first
public interface IEntityService
{
    Task<ApplicationResponseGeneric<EntityDTO>> GetEntityAsync(int id);
    Task<ApplicationResponseGeneric<List<EntityDTO>>> GetAllAsync();
    Task<ApplicationResponse> CreateAsync(AddEntityRequest request);
    Task<ApplicationResponse> UpdateAsync(int id, UpdateEntityRequest request);
    Task<ApplicationResponse> DeleteAsync(int id);
}

// Then implement in Services/{Domain}/
public class EntityService : IEntityService
{
    private readonly DataContext _context;

    public EntityService(DataContext context)
    {
        _context = context;
    }

    // Implementation...
}
```

#### Controller Pattern
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Add for protected endpoints
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

#### DTO Pattern
```csharp
// Always separate API contracts from domain models
public class EntityDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    // ... other properties
}

public class AddEntityRequest
{
    [Required]
    public string Name { get; set; }
    // ... other properties
}

public class UpdateEntityRequest
{
    [Required]
    public string Name { get; set; }
    // ... other properties
}
```

#### Response Pattern
```csharp
// Use ApplicationResponseGeneric<T> for data responses
public async Task<ApplicationResponseGeneric<EntityDTO>> GetEntityAsync(int id)
{
    try
    {
        var entity = await _context.Entities.FindAsync(id);

        if (entity == null)
        {
            return new ApplicationResponseGeneric<EntityDTO> 
            { 
                Success = false,
                Message = "Entity not found"
            };
        }

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
        return new ApplicationResponseGeneric<EntityDTO> 
        { 
            Success = false,
            Message = "An error occurred"
        };
    }
}

// Use ApplicationResponse for simple operations
public async Task<ApplicationResponse> DeleteAsync(int id)
{
    try
    {
        var entity = await _context.Entities.FindAsync(id);

        if (entity == null)
        {
            return new ApplicationResponse 
            { 
                Success = false,
                Message = "Entity not found"
            };
        }

        // Soft delete pattern
        entity.IsDeleted = true;
        await _context.SaveChangesAsync();

        return new ApplicationResponse { Success = true };
    }
    catch (Exception ex)
    {
        // Log exception
        return new ApplicationResponse 
        { 
            Success = false,
            Message = "An error occurred"
        };
    }
}
```

### 2. Entity Conventions

#### Base Entity
```csharp
// All domain entities should inherit from BaseObject
public class Entity : BaseObject
{
    // BaseObject provides: Id, CreatedDate, ModifiedDate, IsDeleted

    public string Name { get; set; }

    [ForeignKey("RelatedEntity")]
    public int? RelatedEntityId { get; set; }
    public virtual RelatedEntity RelatedEntity { get; set; }

    public ICollection<ChildEntity> ChildEntities { get; set; } = new List<ChildEntity>();
}
```

#### History Tracking
```csharp
// Create parallel history entity for audit trail
public class EntityHistory : BaseObject
{
    public int EntityId { get; set; }
    public string Name { get; set; }
    public string ChangedBy { get; set; }
    public DateTime ChangedDate { get; set; }
    public string ChangeType { get; set; } // Created, Updated, Deleted
    // ... copy all relevant fields from main entity
}
```

### 3. Filtering & Pagination

```csharp
// Define filter in Domain/Models/Filters/
public class EntityFilter : PagingFilter
{
    public string Name { get; set; }
    public int? EntityTypeId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

// Create assigner in Filters/
public class EntityAssigner : IFilterAssigner<Entity>
{
    public IQueryable<Entity> Assign(IQueryable<Entity> query, dynamic filter)
    {
        EntityFilter entityFilter = filter;

        if (!string.IsNullOrEmpty(entityFilter.Name))
        {
            query = query.Where(e => e.Name.Contains(entityFilter.Name));
        }

        if (entityFilter.EntityTypeId.HasValue)
        {
            query = query.Where(e => e.EntityTypeId == entityFilter.EntityTypeId.Value);
        }

        if (entityFilter.FromDate.HasValue)
        {
            query = query.Where(e => e.CreatedDate >= entityFilter.FromDate.Value);
        }

        if (entityFilter.ToDate.HasValue)
        {
            query = query.Where(e => e.CreatedDate <= entityFilter.ToDate.Value);
        }

        return query;
    }
}

// Use in service
public async Task<PagedResponseDTO<EntityDTO>> GetPagedAsync(EntityFilter filter)
{
    var query = _context.Entities
        .Include(e => e.RelatedEntity)
        .AsQueryable();

    // Apply filters
    var assigner = new EntityAssigner();
    query = assigner.Assign(query, filter);

    // Get total count
    var totalRecords = await query.CountAsync();

    // Apply pagination
    query = query
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize);

    var entities = await query.ToListAsync();
    var dtos = entities.Select(MapToDTO).ToList();

    return PaginationHelper.CreatePagedResponse(
        dtos,
        filter,
        totalRecords,
        _uriService,
        "api/entities"
    );
}
```

### 4. Angular Service Pattern

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ApplicationResponseGeneric<T> {
  success: boolean;
  data?: T;
  message?: string;
}

export interface EntityDTO {
  id: number;
  name: string;
  // ... other properties
}

export interface AddEntityRequest {
  name: string;
  // ... other properties
}

@Injectable({
  providedIn: 'root'
})
export class EntityService {
  private apiUrl = '/api/entities';

  constructor(private http: HttpClient) { }

  getEntity(id: number): Observable<ApplicationResponseGeneric<EntityDTO>> {
    return this.http.get<ApplicationResponseGeneric<EntityDTO>>(`${this.apiUrl}/${id}`);
  }

  getAll(): Observable<ApplicationResponseGeneric<EntityDTO[]>> {
    return this.http.get<ApplicationResponseGeneric<EntityDTO[]>>(this.apiUrl);
  }

  create(request: AddEntityRequest): Observable<ApplicationResponseGeneric<any>> {
    return this.http.post<ApplicationResponseGeneric<any>>(this.apiUrl, request);
  }

  update(id: number, request: AddEntityRequest): Observable<ApplicationResponseGeneric<any>> {
    return this.http.put<ApplicationResponseGeneric<any>>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<ApplicationResponseGeneric<any>> {
    return this.http.delete<ApplicationResponseGeneric<any>>(`${this.apiUrl}/${id}`);
  }
}
```

### 5. Angular Component Pattern

```typescript
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { EntityService, EntityDTO } from './entity.service';

@Component({
  selector: 'app-entity',
  templateUrl: './entity.component.html',
  styleUrls: ['./entity.component.css']
})
export class EntityComponent implements OnInit, OnDestroy {
  entities: EntityDTO[] = [];
  loading = false;
  error: string | null = null;

  private destroy$ = new Subject<void>();

  constructor(private entityService: EntityService) { }

  ngOnInit(): void {
    this.loadEntities();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadEntities(): void {
    this.loading = true;
    this.error = null;

    this.entityService.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.entities = response.data;
          } else {
            this.error = response.message || 'Failed to load entities';
          }
          this.loading = false;
        },
        error: (err) => {
          this.error = 'An error occurred';
          this.loading = false;
          console.error(err);
        }
      });
  }

  deleteEntity(id: number): void {
    if (!confirm('Are you sure?')) return;

    this.entityService.delete(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.loadEntities();
          } else {
            this.error = response.message || 'Failed to delete entity';
          }
        },
        error: (err) => {
          this.error = 'An error occurred';
          console.error(err);
        }
      });
  }
}
```

## Important Conventions

### Naming
- **Backend:**
  - Controllers: `{Entity}Controller.cs` or `{Entity}sController.cs`
  - Services: `{Entity}Service.cs` with `I{Entity}Service.cs`
  - DTOs: `{Entity}DTO.cs`
  - Requests: `add{Entity}Request.cs`, `update{Entity}Request.cs`
  - Filters: `{Entity}Filter.cs` with `{Entity}Assigner.cs`

- **Frontend:**
  - Components: `{entity}.component.ts`, `{entity}.component.html`
  - Services: `{entity}.service.ts`
  - Use kebab-case for file names

### Code Style
- **C#:** PascalCase for classes, methods, properties; camelCase for parameters, local variables
- **TypeScript:** camelCase for variables, methods; PascalCase for classes, interfaces
- Use async/await for asynchronous operations
- Always handle errors with try-catch or error callbacks
- Use dependency injection via constructor

### Security
- Apply `[Authorize]` to protected controllers
- Validate all user input
- Use `[Required]`, `[StringLength]`, etc. on request models
- Never trust client-side validation alone

### Performance
- Use `.AsNoTracking()` for read-only queries
- Include related entities explicitly with `.Include()`
- Implement pagination for list endpoints
- Unsubscribe from RxJS observables to prevent memory leaks

## Don't Do

? Don't expose EF entities directly via API (use DTOs)  
? Don't skip service layer (controllers should call services)  
? Don't mix authentication strategies (use JWT Bearer)  
? Don't ignore the filter/assigner pattern  
? Don't hard-code configuration values  
? Don't forget to register services in `Startup.cs`  
? Don't create new architectural patterns when existing ones work

## Do

? Follow the layered architecture  
? Use dependency injection  
? Create DTOs for all API contracts  
? Use `ApplicationResponseGeneric<T>` for responses  
? Apply pagination for list endpoints  
? Create history entities for audit trails  
? Implement soft deletes (IsDeleted flag)  
? Use base classes (`BaseObject`, `BaseService`)  
? Test your changes with `dotnet build`

## Quick Reference

### File Locations
- **Models:** `Models/{Domain}/`
- **Services:** `Services/{Domain}/` + `Services/Interfaces/`
- **Controllers:** `Controllers/{Domain}/`
- **DTOs:** `DTO/{Domain}/`
- **Requests:** `Requests/{Domain}/`
- **Filters:** `Filters/` + `Domain/Models/Filters/`
- **Config:** `appsettings.json`, `Startup.cs`
- **Frontend:** `ClientApp/src/app/{feature}/`

### Common Tasks
```bash
# Backend
dotnet ef migrations add MigrationName
dotnet ef database update
dotnet build
dotnet run

# Frontend
cd ClientApp
npm install --legacy-peer-deps
npm start
npm run build
```

---

**For comprehensive documentation, see PROJECT_MAP.md**
