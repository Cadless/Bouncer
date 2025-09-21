# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Bouncer is an HTTP/JSON licensing service built with C#/.NET. The system consists of two main components:
- **Bouncer.Api.csproj** - Public-facing JSON web API for license validation and admin operations
- **Bouncer.App.csproj** - Frontend web interface (Vite + SvelteKit TypeScript) for admin management

## Development Setup

- `dotnet build` - Build the project
- `dotnet run` - Run the application
- `dotnet test` - Run tests
- `dotnet restore` - Restore NuGet packages

## Complete Implementation Plan

### Phase 1: Project Structure & Foundation
1. **Create solution file and project structure**
   - Create `Bouncer.sln` solution file in the same folder as this file.
   - Create `Bouncer.Api` web API project in the `.\Bouncer.Api` subfolder.
   - Create `Bouncer.App` frontend project in the `.\Bouncer.App` subfolder.

2. **Database Setup**
   - Configure Entity Framework Core with SQLite
   - Create DbContext with all required entities
   - Set up database migrations
   - Create initial migration

### Phase 2: Data Models & Database
3. **Entity Models**
   - Create `Bundle` entity (Id, Name)
   - Create `Feature` entity (Id, Name)
   - Create `BundleFeature` many-to-many entity
   - Create `License` entity (Id, ClientKey, PrivateKey, Assignee, Expiration, Status)
   - Create `LicenseFeature` many-to-many entity
   - Create `Principal` entity (Id, ExternalId)
   - Create `PrincipalLicense` one-to-many entity

4. **Repository Pattern**
   - Create IRepository<T> interface
   - Implement generic Repository<T> base class
   - Create specific repositories for each entity
   - Set up dependency injection

### Phase 3: Core API Services
5. **Business Logic Services**
   - Create `LicenseService` for license operations
   - Create `PrincipalService` for principal management
   - Create `FeatureService` for feature management
   - Create `BundleService` for bundle management
   - Create `ValidationService` for license validation logic

6. **Authentication & Authorization**
   - Configure OAuth 2.0 with Microsoft MSAL
   - Set up JWT token handling
   - Create admin authorization policies
   - Implement role-based access control

### Phase 4: API Controllers
7. **Admin Controllers (Requires Authentication)**
   - `PrincipalController` - CRUD operations
   - `FeatureController` - CRUD operations
   - `BundleController` - CRUD operations
   - `LicenseController` - CRUD operations

8. **Public Controllers**
   - `LicenseValidationController` - POST endpoint for validation
   - Health check endpoints
   - API documentation endpoints

### Phase 5: License Validation Logic
9. **Validation Engine**
   - Implement license validation algorithm
   - Check license expiration
   - Verify principal-license associations
   - Validate feature permissions
   - Handle license status (Enabled/Disabled)

10. **Security Features**
    - Generate secure ClientKey and PrivateKey
    - Implement request signing/verification
    - Add rate limiting
    - Input validation and sanitization

### Phase 6: Frontend Application
11. **SvelteKit Setup**
    - Initialize Vite + SvelteKit TypeScript project
    - Configure build pipeline
    - Set up routing and layout structure

12. **Admin Interface Components**
    - Login/logout with OAuth 2.0
    - Principal management interface
    - Feature management interface
    - Bundle management interface
    - License creation and management interface

### Phase 7: Testing & Documentation
13. **Unit Tests**
    - Service layer tests
    - Repository tests
    - Validation logic tests
    - Controller tests

14. **Integration Tests**
    - End-to-end API tests
    - Database integration tests
    - Authentication flow tests

15. **Documentation**
    - API documentation (Swagger/OpenAPI)
    - Setup and deployment guides
    - Usage examples

### Phase 8: Deployment & Configuration
16. **Configuration Management**
    - Environment-specific settings
    - Connection strings management
    - OAuth configuration
    - Logging configuration

17. **Production Readiness**
    - Docker containerization
    - Health checks
    - Monitoring and logging
    - Error handling and recovery

## Database Schema

```
Bundle (Id, Name)
Feature (Id, Name)
BundleFeature (Id, BundleId, FeatureId)
License (Id, ClientKey, PrivateKey, Assignee, Expiration, Status)
LicenseFeature (Id, LicenseId, FeatureId)
Principal (Id, ExternalId)
PrincipalLicense (Id, PrincipalId, LicenseId)
```

## API Endpoints

### Admin Endpoints (Authenticated)
- `GET/POST/PUT/DELETE /api/principals`
- `GET/POST/PUT/DELETE /api/features`
- `GET/POST/PUT/DELETE /api/bundles`
- `GET/POST/PUT/DELETE /api/licenses`

### Public Endpoints
- `POST /api/validate` - License validation

## Key Implementation Notes
- Use Entity Framework Core for data access
- Implement proper error handling and logging
- Follow RESTful API conventions
- Use async/await patterns throughout
- Implement proper input validation
- Follow security best practices for key generation
- Use dependency injection for all services