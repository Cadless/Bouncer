# Bouncer

HTTP licensing service.

# Overview

Bouncer is an HTTP/JSON web API service furnishing configurable, feature-based licensing and license validation.

The following steps outline how Bouncer works at the high level using the Bouncer frontend web interface (Vite, SvelteKit Typescript app).

1) (admin-only) Login via OAuth 2.0

    - Currently uses Microsoft MSAL.

2) (admin-only) Configure a `Bundle` with one or more associated `Feature` definitions.
    
    - A feature can be associated with multiple applications.

3) (admin-only) Create a license instance (HTTP POST).
   
   **Params:**
    
    - ClientKey (unique/auto-assigned, shared with client/principal)
    - PrivateKey (unique/auto-assigned, never shared)
    - Assignee (required in request body)
    - Expiration - Date (or null if license never expires)
    - Features - Array of licensed features.

    When the license is created on the server, a record is inserted into the SQLite database table called License.
    
    Database Tables

    - Bundle 
        
        - Id (unique/auto-assigned)
        - Columns: Name (unique)

    - Feature

        - Id (unique/auto-assigned)
        - Columns: Name (unique)

    - BundleFeatures - Many-to-many relationship table.

        - Id (unique/auto-assigned)
        - Columns: ApplicationId, FeatureId

    - License - Each record is a license instance.

        - Id (unique/auto-assigned)
        - ClientKey (unique/auto-assigned, shared with client/principal)
        - PrivateKey (unique/auto-assigned, never shared)
        - Assignee (required in request body)
        - Expiration - Date (or null if license never expires)
        - Features - Array of licensed features.

    - LicenseFeatures - Many-two-many relationship table.

        - Id (unique/auto-assigned)
        - Columns: LicenseId, FeatureId

    - Principal - License assignee (e.g. license validation requestor).

        - Id (unique/auto-assigned)
        - ExternalId (unique/required) - Externally defined string used to identify the unique license principal/assignee.

The client (shared) key is returned response body.

# Projects

Bouncer consists of two primary components.

- Bouncer.App.csproj - Currently, only functional/permitted for administrator type users to define and assign licenses to principals.

- Bouncer.Api.csproj - Public facing JSON web API for validating licenses.
