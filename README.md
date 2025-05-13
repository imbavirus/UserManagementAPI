# UserManagementAPI
A RESTful API for managing user profiles and roles.

## About This Project

This project is the backend API for a user management system. It provides a comprehensive set of RESTful endpoints for:
- Create, Read, Update, and Delete (CRUD) operations on user profiles and roles.
- Assign and manage roles for users.

This API is designed to be consumed by the frontend application: [UserManagementWeb](https://github.com/imbavirus/UserManagementWeb) project.

## Live Demo

A live demo of the API can be found here: [Demo](https://user-management-api.home.infernos.co.za/swagger)

## Technology Stack

- **Framework:** [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) on [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- **Programming Language:** [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)
- **Database:** [SQLite](https://www.sqlite.org/index.html) (via [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/))
- **API Documentation:** [Swagger](https://swagger.io/) integrated for easy testing and exploration.
- **Version Control:** [Git](https://git-scm.com/)

## Requirements

Before you begin, ensure you have the following installed:
- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- A code editor of your choice (e.g., [Visual Studio](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/)).
- [Git](https://git-scm.com/)
- (Optional) [SQLite Browser](https://sqlitebrowser.org/) or a similar tool for database inspection.
- (Optional, if managing EF Core migrations from CLI) [.NET EF Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)

## Getting Started

Follow these steps to get the API up and running on your local machine.

**1. Clone the Repository:**

   ```bash
   git clone https://github.com/imbavirus/UserManagementAPI.git
   ```

**2. Navigate to the Project Directory:**

   ```bash
   cd UserManagementAPI
   ```

**3. Restore Dependencies:**

   Run the following command from the root directory of the repository (`UserManagementAPI`):
   ```bash
   dotnet restore
   ```
   If you open the solution (`.sln`) file in Visual Studio, it typically restores dependencies automatically.

**4. Database Setup (Entity Framework Core Migrations):**
   This project uses Entity Framework Core for database management. Migrations are used to evolve the database schema over time.

   *   **Applying Existing Migrations:**
       To apply existing migrations and create/update your database schema (e.g., when setting up the project for the first time or after pulling changes), run the following command from the root directory of the repository:

       ```bash
       dotnet ef database update --project ./Application --startup-project ./API
       ```

        Alternatively use the provided setup-database.ps1 script in powershell

   *   **Creating New Migrations (when making model changes):**
       If you make changes to your EF Core models (entities) that require database schema changes, you'll need to create a new migration.
       1.  Ensure your `DbContext` and entity changes are saved.
       2.  Run the following command from the root directory, replacing `YourMigrationName` with a descriptive name for the changes (e.g., `AddUserEmailVerification`):

           ```bash
           dotnet ef migrations add YourMigrationName --project ./Application --startup-project ./API
           ```

           Alternatively use the provided run-migrations.ps1 script

       3.  After the migration is created, apply it to your database using the `dotnet ef database update` command shown above.

       Migrations are typically stored in a `Migrations` folder within the project specified by the `--project` argument (e.g., `./Application/Migrations`).


## Running the Application

You can run the API using the .NET CLI. From the root directory of the repository:

```bash
dotnet run --project ./API
```

Alternatively use the provided run.ps1 script in powershell

You can also run the project directly from Visual Studio (by opening the `.sln` file and pressing F5 or the "Start" button) or VS Code (using its launch configurations).

By default, the API should be accessible at `http://127.0.0.1:5000`

## Running Tests

This project includes automated tests to ensure code quality and functionality. To run the tests:

1.  Navigate to the root directory of the repository.
2.  Execute the following command:
   ```bash
   dotnet test
   ```
This command will discover and run all test projects within the solution. Test results will be displayed in the console.

## API Endpoints & Documentation

This API provides RESTful endpoints for managing user profiles and roles. Once the application is running, Swagger UI is available for interactive API documentation, exploration, and testing. Access it via your browser at:

- `http://127.0.0.1:5000/swagger`

Available endpoints:
- `GET /api/UserProfiles` - Retrieve all user profiles
- `GET /api/UserProfiles/{id}` - Retrieve a specific user profile
- `POST /api/UserProfiles` - Create a new user profile
- `PUT /api/UserProfiles` - Update an existing user profile

- `GET /api/Roles` - Retrieve all roles
- `GET /api/Roles/{id}` - Retrieve a specific role
- `POST /api/Roles` - Create a new role
- `PUT /api/Roles` - Update an existing role

Refer to the Swagger UI for a complete list of endpoints, request/response models, and to try them out.

## Interacting with the Frontend

This API serves as the backend for the UserManagementWeb frontend application.

When setting up and running the frontend:
- Ensure this UserManagementAPI is running and accessible.
- Configure the frontend's `API_BASE_URL` environment variable to point to the address where this API is hosted (e.g., `http://127.0.0.1:5000`).

## Previews

The primary way to "preview" and interact with this API is through its Swagger UI documentation.

*   **Swagger UI**
![Swagger](https://i.gyazo.com/797babcd5ba72bea00a23fafec8b280c.png)
