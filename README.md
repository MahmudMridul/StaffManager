
---

## Database Context

### RbacContext Class

**Namespace**: `RBAC_API.Database`

**Inheritance**: Inherits from `IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>`

**Description**: Entity Framework Core database context that manages all RBAC entities and their relationships.

#### DbSet Properties

| Property | Type | Description |
|----------|------|-------------|
| `Permissions` | `DbSet<Permission>` | Database set for Permission entities |
| `RolePermissions` | `DbSet<RolePermission>` | Database set for RolePermission junction entities |

#### Key Configuration Features

- **Schema**: All tables are created in the `rbac` schema
- **Unique Constraints**: 
  - User emails must be unique
  - Role names must be unique
  - Permission Resource+Action combinations must be unique
  - Role+Permission combinations must be unique
- **Indexes**: Optimized queries with strategic indexing
- **Cascade Deletes**: Proper cleanup of related data
- **Default Values**: Automatic timestamp generation using PostgreSQL's NOW() function

#### Override Methods

##### `SaveChanges()`
**Description**: Overrides the base SaveChanges to automatically update timestamps before saving.

**Returns**: Number of affected records

##### `SaveChangesAsync(CancellationToken cancellationToken = default)`
**Description**: Asynchronous version of SaveChanges with automatic timestamp updates.

**Returns**: Task<int> representing the number of affected records

##### `UpdateTimestamps()` (Private)
**Description**: Internal method that automatically updates the `UpdatedAt` property for User and Role entities when they are modified.

**Key Features**:
- Automatically tracks entity state changes
- Updates timestamps only for modified entities
- Supports both User and Role entities

---

## Configuration

### Program.cs

**Description**: Application entry point and service configuration.

#### Configured Services
- **Controllers**: ASP.NET Core Web API controllers
- **Database Context**: PostgreSQL connection with Entity Framework Core
- **Connection String**: Uses "DefaultConnection" from configuration

#### Pipeline Configuration
- HTTPS redirection
- Authorization middleware
- Controller routing

#### Key Features
- Clean architecture setup
- Database connection configuration
- Minimal API configuration

---

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- PostgreSQL database
- Visual Studio 2022 or compatible IDE

### Installation

1. Clone the repository
2. Update the connection string in `appsettings.json`:
