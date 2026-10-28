# BAU-PROTO

An ASP.NET Core (.NET 10.0) web application with an AngularJS single-page application (SPA) frontend bundled using Vite, JWT authentication (Access & Refresh tokens), AES-256-GCM encryption, and MySQL/MariaDB persistence via Entity Framework Core.

---

## Tech Stack & Dependencies

### Backend (.NET 10.0)
Installed NuGet packages:
- `DotNetEnv` (`3.2.0`) — Load environment variables from `.env` files
- `Microsoft.AspNetCore.Authentication.JwtBearer` (`10.0.13`) — JWT authentication (`AccessToken` & `RefreshToken` schemes)
- `Microsoft.AspNetCore.OpenApi` (`10.0.13`) — OpenAPI specification support
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (`1.23.0`) — Docker/Container tooling support
- `Pomelo.EntityFrameworkCore.MySql` (`9.0.0`) — MySQL/MariaDB Entity Framework Core provider

### Frontend (`wwwroot`)
- **Framework**: AngularJS (`1.8.2`), Angular Material (`1.2.1`), Bootstrap (`3.4.1`), jQuery (`3.7.1`)
- **Build & Bundling (Node.js)**:
  - `vite` (`^8.0.8`) — Minifies and bundles frontend JS/CSS/HTML assets into `wwwroot/dist`
  - `concurrently` (`^9.2.1`)

---

## Getting Started

### 1. Database Setup (MySQL / MariaDB)
1. Import [`bau_storage.sql`](bau_storage.sql) into your MySQL/MariaDB server before running the project. This file contains the database schema (`users` and `products` tables) and sample data.
2. Configure the database connection string (`DefaultConnection`) in the appropriate `appsettings.<Environment>.json` file (`appsettings.json`, `appsettings.Development.json`, `appsettings.BVT.json`, or `appsettings.Production.json`).

### 2. Environment Variables (`.env`)
Sensitive configuration values such as JWT secret keys and AES encryption keys/IVs are stored in `.env` files (`DotNetEnv` loads `.env.<ASPNETCORE_ENVIRONMENT>` automatically at startup):
- Root directory: `.env`, `.env.Development`, `.env.BVT`, `.env.Production`
- Frontend directory (`wwwroot/`): `.env`, `.env.Development`, `.env.BVT`, `.env.Production`

Make sure the required `.env` files are placed in the root folder (and `wwwroot/` if needed) before building and running the application.

> **Need the `.env` files?**
> Contact the maintainer via **WhatsApp** (fastest response), **office email**, **MS Teams**, or by creating a **GitHub issue**.

### 3. Build Frontend Assets (Vite)
Install Node.js dependencies and run the Vite build for your target environment before deploying or running the app (since the server serves the bundled SPA from `wwwroot/dist/index.html`):

```bash
npm install

# Build for Development (SIT)
npm run build:dev

# Build for BVT
npm run build:bvt

# Build for Production
npm run build:prod
```

### 4. Run the Application
You can run the application using Visual Studio launch profiles (`SIT`, `BVT`, `Production`, or Container profiles) or via the .NET CLI:

```bash
dotnet run --launch-profile SIT
```

---

## Frontend Pages

The AngularJS SPA is located in the `wwwroot` folder and uses HTML5 routing:
- `/login` — User login page
- `/register` — User registration page
- `/` — Home page (requires authentication; all product CRUD operations happen here)

---

## API Endpoints

### Configuration
#### `POST /api/Configuration/configPrivate`
Get private encryption configuration data (`S0VZ` and `SVY`) used by the frontend before login & registration. Does not require authentication.

---

### Authentication

#### `POST /api/auth/login`
Authenticate a user and return JWT access & refresh tokens along with user info.
- **Request Body**:
  ```json
  {
    "Email": "Jan1@gmail.com",
    "Password": "<Password encrypted using AES-256-GCM>"
  }
  ```

#### `POST /api/auth/register`
Register a new user.
- **Request Body**:
  ```json
  {
    "Email": "Jan1@gmail.com",
    "Role": "admin",
    "Password": "<Password encrypted using AES-256-GCM>"
  }
  ```

#### `POST /api/auth/refresh`
Refresh an expired JWT access token (requires authentication via `RefreshToken` scheme).
- **Headers**:
  ```json
  {
    "Authorization": "Bearer <access_token_from_login>",
    "Refreshtoken": "<refresh_token_from_login>"
  }
  ```

#### `POST /api/auth/logout`
Logout the user and clear the active session token.
- **Headers**:
  ```json
  {
    "Authorization": "Bearer <access_token_from_login>"
  }
  ```

---

### Products
> All Product endpoints require authentication (`AccessToken` scheme) via the `Authorization` header.

#### `POST /api/Product/GetListProduct`
Get a paginated list of products with optional name filtering.
- **Headers**:
  ```json
  {
    "Authorization": "Bearer <access_token_from_login>"
  }
  ```
- **Request Body**:
  ```json
  {
    "page": 1,
    "pageSize": 10,
    "Name": "Product Name"
  }
  ```
  *(Note: `Name` is optional)*

#### `POST /api/Product/GetProduct`
Get details of a specific product by ID.
- **Headers**:
  ```json
  {
    "Authorization": "Bearer <access_token_from_login>"
  }
  ```
- **Request Body**:
  ```json
  {
    "Id": "1"
  }
  ```

#### `POST /api/Product/AddProduct`
Create a new product.
- **Headers**:
  ```json
  {
    "Authorization": "Bearer <access_token_from_login>"
  }
  ```
- **Request Body**:
  ```json
  {
    "Name": "New Product",
    "Description": "Product description",
    "Price": "9.99"
  }
  ```

#### `POST /api/Product/UpdateProduct`
Update an existing product by ID.
- **Headers**:
  ```json
  {
    "Authorization": "Bearer <access_token_from_login>"
  }
  ```
- **Request Body**:
  ```json
  {
    "Id": "1",
    "Name": "Updated Product Name",
    "Description": "Updated description",
    "Price": "19.99"
  }
  ```

#### `POST /api/Product/DeleteProduct`
Delete a product by ID.
- **Headers**:
  ```json
  {
    "Authorization": "Bearer <access_token_from_login>"
  }
  ```
- **Request Body**:
  ```json
  {
    "Id": "1"
  }
  ```
