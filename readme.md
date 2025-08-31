# Task Management Project

## Description
This project is a Blazor web application designed to assist users with Task Management. It provides functionalities for user authentication, task management, and user administration, built on a .NET backend with Entity Framework Core for data persistence.

## Features
-   **User Authentication**: Secure user registration, login, logout, and password reset functionalities.
-   **User Management**: Administration features to manage user accounts.
-   **Task Management**: Create, view, update, and delete tasks related to Hajj and Umrah.
-   **Email Services**: Integration for sending emails (e.g., for account activation, password resets).
-   **Responsive UI**: Built with Blazor components for an interactive and modern user experience.

## Technologies Used
-   **Frontend**: Blazor (Razor Components)
-   **Backend**: .NET (C#)
-   **Database**: Entity Framework Core (with SQLite/SQL Server, depending on configuration)
-   **Styling**: CSS, Bootstrap (via wwwroot/lib/bootstrap)

## Installation

### Prerequisites
-   .NET SDK version 9.0 or later
-   An IDE that you love
-   **MySQL Server**: Ensure you have a running MySQL server instance.

### Steps
1.  **Clone the Repository**:
    ```bash
    git clone <repository-url>
    cd "Tasks Project"
    ```
2.  **Navigate to the Project Directory**:
    ```bash
    cd hua
    ```
3.  **Configure Database Connection**:
    Open `appsettings.json` and `appsettings.Development.json` and update the `DefaultConnection` string under `ConnectionStrings` to match your MySQL server configuration. An example connection string is:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "server=<your_server_name>;database=<your_database_name>;user=<your_database_user_name>;password=<your_database_password>"
    }
    ```
    Replace `<your_server_name>`, `<your_database_name>`, `<your_database_user_name>`, and `<your_database_password>` with your MySQL server's address, database name, username, and password, respectively.

4.  **Restore Dependencies**:
    ```bash
    dotnet restore
    ```
5.  **Apply Migrations / Update Database**:
    If this is a new setup, you might need to apply existing migrations. If you make changes to the data model, you'll need to create new migrations.
    ```bash
    # To add a new migration (only if you've made changes to the data model)
    dotnet ef migrations add [MigrationName]

    # To apply existing migrations to the database
    dotnet ef database update
    ```
6.  **Run the Application**:
    ```bash
    dotnet watch run
    ```
    The application will typically be accessible at `https://localhost:7192` or `http://localhost:5192`.

## Usage
-   **Authentication**: Users can sign up, log in, and manage their profiles via the `/Auth` components.
-   **Task Management**: Access and manage tasks through the relevant sections of the application interface, likely under `AppPages/Tasks`.
-   **User Administration**: If authorized, administrative users can manage other users through `AppPages/Users`.

## Contributing
We welcome contributions to the Hajj and Umra Project! To contribute:

1.  Fork the repository.
2.  Create a new branch for your feature or bug fix.
3.  Make your changes and ensure tests pass.
4.  Commit your changes with a descriptive message.
5.  Push your branch and open a pull request.

## License
This project is licensed under the MIT License. See the `LICENSE` file (if present) for full details.