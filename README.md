# RBAC API Documentation

## Overview

This is a Role-Based Access Control (RBAC) API built with ASP.NET Core 9.0 and Entity Framework Core, using PostgreSQL as the database. The system implements a comprehensive permission-based authorization system with users, roles, and permissions.

## Technology Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core 9.0.6
- **Authentication**: ASP.NET Core Identity with JWT Bearer tokens
- **Language**: C# 13.0

## Database Schema

The system uses a dedicated `rbac` schema in PostgreSQL with the following tables:

- Users
- Roles
- UserRoles
- Permissions
- RolePermissions
- Identity framework tables (UserClaims, UserLogins, UserTokens, RoleClaims)

## Core Models

### User Class

**Namespace**: `RBAC_API.Models`

**Inheritance**: Inherits from `IdentityUser`

**Description**: Represents a user in the RBAC system with extended properties beyond the standard Identity user.

#### Properties

| Property    | Type                    | Description                                     | Constraints               |
| ----------- | ----------------------- | ----------------------------------------------- | ------------------------- |
| `FirstName` | `string?`               | User's first name                               | MaxLength: 100 characters |
| `LastName`  | `string?`               | User's last name                                | MaxLength: 100 characters |
| `CreatedAt` | `DateTime`              | Timestamp when user was created                 | Default: UTC now          |
| `UpdatedAt` | `DateTime`              | Timestamp when user was last updated            | Auto-updated on save      |
| `IsActive`  | `bool`                  | Indicates if user account is active             | Default: true             |
| `UserRoles` | `ICollection<UserRole>` | Navigation property for user-role relationships | Collection                |

#### Key Features

- Automatic timestamp management
- Soft delete capability through `IsActive` flag
- Integration with ASP.NET Core Identity

---

### Role Class

**Namespace**: `RBAC_API.Models`

**Inheritance**: Inherits from `IdentityRole`

**Description**: Represents a role in the RBAC system with additional metadata and permission management.

#### Properties

| Property          | Type                          | Description                                           | Constraints               |
| ----------------- | ----------------------------- | ----------------------------------------------------- | ------------------------- |
| `Description`     | `string?`                     | Detailed description of the role                      | MaxLength: 500 characters |
| `CreatedAt`       | `DateTime`                    | Timestamp when role was created                       | Default: UTC now          |
| `UpdatedAt`       | `DateTime`                    | Timestamp when role was last updated                  | Auto-updated on save      |
| `IsActive`        | `bool`                        | Indicates if role is currently active                 | Default: true             |
| `UserRoles`       | `ICollection<UserRole>`       | Navigation property for user-role relationships       | Collection                |
| `RolePermissions` | `ICollection<RolePermission>` | Navigation property for role-permission relationships | Collection                |

#### Key Features

- Hierarchical role management
- Permission assignment capability
- Audit trail with timestamps
- Unique role names enforced at database level

---

### UserRole Class

**Namespace**: `RBAC_API.Models`

**Inheritance**: Inherits from `IdentityUserRole<string>`

**Description**: Junction table entity managing many-to-many relationships between users and roles with audit information.

#### Properties

| Property     | Type       | Description                                | Constraints               |
| ------------ | ---------- | ------------------------------------------ | ------------------------- |
| `AssignedAt` | `DateTime` | Timestamp when role was assigned to user   | Default: UTC now          |
| `AssignedBy` | `string?`  | ID of the user who assigned this role      | MaxLength: 450 characters |
| `User`       | `User`     | Navigation property to the associated user | Required                  |
| `Role`       | `Role`     | Navigation property to the associated role | Required                  |

#### Key Features

- Audit trail for role assignments
- Composite primary key (UserId, RoleId)
- Cascade delete behavior
- Assignment tracking

---

### Permission Class

**Namespace**: `RBAC_API.Models`

**Description**: Represents a specific permission that can be granted to roles, defining what actions can be performed on resources.

#### Properties

| Property          | Type                          | Description                                           | Constraints              |
| ----------------- | ----------------------------- | ----------------------------------------------------- | ------------------------ |
| `Id`              | `int`                         | Primary key identifier                                | Auto-increment           |
| `Name`            | `string`                      | Human-readable permission name                        | Required, MaxLength: 100 |
| `Description`     | `string?`                     | Detailed description of the permission                | MaxLength: 500           |
| `Resource`        | `string`                      | The resource this permission applies to               | Required, MaxLength: 50  |
| `Action`          | `string`                      | The action that can be performed                      | Required, MaxLength: 50  |
| `CreatedAt`       | `DateTime`                    | Timestamp when permission was created                 | Default: UTC now         |
| `RolePermissions` | `ICollection<RolePermission>` | Navigation property for role-permission relationships | Collection               |

#### Key Features

- Resource-action based permission model
- Unique constraint on Resource + Action combination
- Granular permission control
- Indexed for performance

---

### RolePermission Class

**Namespace**: `RBAC_API.Models`

**Description**: Junction table entity managing many-to-many relationships between roles and permissions with grant tracking.

#### Properties

| Property       | Type         | Description                                      | Constraints              |
| -------------- | ------------ | ------------------------------------------------ | ------------------------ |
| `Id`           | `int`        | Primary key identifier                           | Auto-increment           |
| `RoleId`       | `string`     | Foreign key to the role                          | Required, MaxLength: 450 |
| `PermissionId` | `int`        | Foreign key to the permission                    | Required                 |
| `GrantedAt`    | `DateTime`   | Timestamp when permission was granted            | Default: UTC now         |
| `GrantedBy`    | `string?`    | ID of the user who granted this permission       | MaxLength: 450           |
| `Role`         | `Role`       | Navigation property to the associated role       | Required                 |
| `Permission`   | `Permission` | Navigation property to the associated permission | Required                 |

#### Key Features

- Audit trail for permission grants
- Unique constraint on RoleId + PermissionId combination
- Cascade delete behavior
- Grant tracking and accountability

---

### RbacResponse Class

**Namespace**: `RBAC_API.Models`

**Description**: Standardized response wrapper for all API endpoints, providing consistent response structure across the application.

#### Properties

| Property     | Type             | Description                                    | Default      |
| ------------ | ---------------- | ---------------------------------------------- | ------------ |
| `Success`    | `bool`           | Indicates if the operation was successful      | -            |
| `StatusCode` | `HttpStatusCode` | HTTP status code for the response              | -            |
| `Message`    | `string`         | Descriptive message about the operation result | Empty string |
| `Data`       | `object?`        | Response payload data                          | null         |
| `Errors`     | `List<string>?`  | Collection of error messages if any            | null         |
| `TimeStamp`  | `DateTime`       | Timestamp when response was created            | UTC now      |

#### Static Methods

##### `Create(bool success, HttpStatusCode code, string msg, object? data = null, List<string>? errors = null)`

**Description**: Factory method to create a custom response with all parameters.

**Parameters**:

- `success`: Boolean indicating operation success
- `code`: HTTP status code
- `msg`: Response message
- `data`: Optional response data
- `errors`: Optional error list

**Returns**: `RbacResponse` instance

##### `Ok(object? data = null, string message = "")`

**Description**: Factory method to create a successful response (HTTP 200).

**Parameters**:

- `data`: Optional response data
- `message`: Optional custom message (defaults to "Request was successful")

**Returns**: `RbacResponse` instance with Success=true, StatusCode=OK

##### `Created(object? data = null, string message = "")`

**Description**: Factory method to create a resource creation response (HTTP 201).

**Parameters**:

- `data`: Optional response data
- `message`: Optional custom message (defaults to "Resource created successfully")

**Returns**: `RbacResponse` instance with Success=true, StatusCode=Created

#### Usage Examples

---

## Database Context

### RbacContext Class

**Namespace**: `RBAC_API.Database`

**Inheritance**: Inherits from `IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>`

**Description**: Entity Framework Core database context that manages all RBAC entities and their relationships.

#### DbSet Properties

| Property          | Type                    | Description                                       |
| ----------------- | ----------------------- | ------------------------------------------------- |
| `Permissions`     | `DbSet<Permission>`     | Database set for Permission entities              |
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
