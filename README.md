# Employee Leave Management System

A full-stack leave management application built with .NET 8 Web API, PostgreSQL, and React.

## Tech Stack

- **Backend:** .NET 8 Web API, Entity Framework Core 8 (Database First)
- **Database:** PostgreSQL
- **Frontend:** React 18 (Vite)
- **Auth:** JWT

## Project Structure

```

leave-management-system/
├── Backend/
│ └── LeaveManagement/
│ ├── LeaveManagement.API/ .NET 8 Web API
│ └── LeaveManagement.Tests/ xUnit tests
├── Frontend/
│ └── leave-management-ui/ React + Vite
├── SQL/
│ ├── 01_schema.sql
│ ├── 02_seed.sql
│ └── 03_views.sql
└── README.md

```

## Setup

### Database

1. Open pgAdmin and create a database named `LeaveManagementDB`
2. Open Query Tool and run each file in order:
   - `SQL/01_schema_index.sql`
   - `SQL/02_insert.sql`
   - `SQL/03_view.sql`

### Backend

1. Open `Backend/LeaveManagement/LeaveManagement.sln` in Visual Studio 2022
2. In `LeaveManagement.API/appsettings.json`, update the `DefaultConnection` password
3. Press **F5** — API starts at `https://localhost:7083`
4. Swagger: `https://localhost:7083/swagger`

### Frontend

```bash
cd Frontend/leave-management-ui
npm install
npm run dev
```

Opens at `http://localhost:5173`

## Default Credentials

| Role  | Email             | Password  |
| ----- | ----------------- | --------- |
| Admin | admin@company.com | Admin@123 |

## API Endpoints

| Method | URL                             | Access        | Description               |
| ------ | ------------------------------- | ------------- | ------------------------- |
| POST   | /api/auth/login                 | Public        | Login, returns JWT        |
| GET    | /api/employees                  | Admin         | List all employees        |
| GET    | /api/employees/{id}             | Authenticated | Get one employee          |
| POST   | /api/employees                  | Admin         | Add new employee          |
| DELETE | /api/employees/{id}             | Admin         | Remove employee           |
| GET    | /api/leave-requests             | Authenticated | Admin: all, Employee: own |
| POST   | /api/leave-requests             | Employee      | Submit leave request      |
| PUT    | /api/leave-requests/{id}/review | Admin         | Approve or reject         |

## Business Logic

- Leave `ToDate` must be on or after `FromDate`
- `Reason` is required
- Overlapping leave requests (Pending or Approved) are blocked
- Only Admin can approve or reject
- Once approved or rejected, a request cannot be changed
