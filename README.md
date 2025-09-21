# Bouncer

HTTP licensing service.

# Overview

Bouncer is an HTTP/JSON web API service furnishing configurable, feature-based licensing and license validation.

The following steps outline how Bouncer works at the high level using the Bouncer frontend web interface (Vite, SvelteKit Typescript app).

1) (admin-only) Login (Microsoft MSAL).

2) (admin-only) Configure a `Bundle` with one or more associated `Feature` definitions.
    
    - A feature can be associated with multiple applications.

3) (admin-only) Create a license instance (HTTP POST).
   
   Params:
    
    - Client Key (unique/auto-assigned)
    - Private Key (unique/auto-assigned never shared)
    - Assignee (required in request body)
    - Expiration - Date (or null if license never expires)
    - Features - Array of licensed features.

    When the license is created on the server, a record is inserted into the SQLite database table called License.
    
    Database Tables

    - Bundle 
        - Columns: Name (unique)
    - Feature 
        - Columns: Name (unique)
    - BundleFeatures - Many-to-many relationship table.
        - Columns: ApplicationId, FeatureId
    - License - Each record is a license instance.