# Machine Industry Investment Fund - Full-Stack Technical Assessment

A full-stack request management application developed using ASP.NET Core and React.

The system allows employees to submit requests, automatically routes them to the appropriate reviewer based on a configurable amount threshold, and enables authorized managers or finance users to approve or reject requests.

## Technology Stack

**Backend**

- ASP.NET Core 10 Web API
- C# / Clean Architecture
- Entity Framework Core 10
- PostgreSQL
- ASP.NET Core Identity
- JWT Authentication

**Frontend**

- React + TypeScript
- Vite 8
- Material UI v9
- React Router v7
- Axios

## Architecture

The backend follows Clean Architecture principles:

- **Domain:** Core business entities and rules
- **Application:** DTOs, interfaces, validation, and application logic
- **Infrastructure:** Database persistence, Identity, and service implementations
- **API:** HTTP endpoints, authentication, and application configuration

The frontend is located in the `UI` directory and communicates with the backend through REST APIs.

## Main Features

- JWT-based authentication
- Role-based authorization (Employee, Manager, Finance)
- Dynamic request forms driven by JSON configuration
- Client-side and server-side validation
- Automatic Manager/Finance routing based on request amount
- Request listing with server-side pagination, sorting, and filtering
- Request search and status filtering
- Role-restricted request visibility
- Request approval and rejection
- Reviewer identification and review timestamps
- PostgreSQL persistence
- Protected frontend routes
- Confirmation dialogs and operation feedback

## Prerequisites

- .NET 10 SDK
- Node.js and npm compatible with Vite 8
- PostgreSQL
- Visual Studio 2022 or later, or another compatible IDE

## Database Configuration

1. Install and start PostgreSQL.
2. Create a local database.
3. Configure the backend connection string in the appropriate ASP.NET Core configuration source.
4. Configure the required JWT settings.
5. Apply the project's existing EF Core migrations before running the application.

Database credentials and JWT signing secrets should be stored securely and should not be committed to source control.

## Running the Backend

1. Open the .NET solution in Visual Studio.
2. Configure the PostgreSQL connection and JWT settings.
3. Apply any pending EF Core migrations.
4. Set the Web API project as the startup project.
5. Run the backend using the HTTPS development profile.

Development API URL:

`https://localhost:7009/api`

## Running the Frontend

Open a terminal in the repository root:

```powershell
cd UI
npm install
npm.cmd run dev
```

Frontend development URL:

`http://localhost:5173`

To generate a production build:

```powershell
npm.cmd run build
```

The frontend API configuration must point to the running backend.

## Authentication and Roles

The application uses ASP.NET Core Identity and JWT authentication.

**Employee**

- Create requests
- View personal requests

**Manager**

- View requests assigned to the Manager role
- Approve or reject assigned requests

**Finance**

- View requests assigned to the Finance role
- Approve or reject assigned requests

Request routing is controlled by a configurable amount threshold on the backend.

Test accounts should be created or configured locally with their respective roles. Credentials are intentionally not included in this repository documentation.

## Core API Endpoints

| Method | Endpoint                     | Purpose                     |
| ------ | ---------------------------- | --------------------------- |
| POST   | `/api/requests/{id}/approve` | Approve an assigned request |
| POST   | `/api/requests/{id}/reject`  | Reject an assigned request  |

Additional authentication, request creation, and listing endpoints are implemented in the backend API.

## Verification

The primary application flows have been tested:

- Real JWT login
- Employee request creation and database persistence
- Manager and Finance request review
- Approval and rejection
- Role-based navigation and authorization
- Request filtering and pagination
- Frontend production build

## Known Limitations

This project was developed under a limited technical-assessment timeframe.

The following noncritical improvements remain:

- Some frontend pages do not consistently use the shared sidebar/header layout.
- The Finance review page may refresh automatically following a review operation, which could be improved for a smoother user experience.
- Additional automated tests and UI refinements could be implemented.

These limitations do not prevent the core request submission and review workflows from functioning.

## Project Status

**Functional MVP completed.**

The implemented scope prioritizes authentication, role-based workflows, request management, API integration, and persistence.

Further UI refinements and expanded automated test coverage are potential future improvements.
