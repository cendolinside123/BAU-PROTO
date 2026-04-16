Instaled NuGet packages:
- Microsoft.AspNetCore.Authentication.JwtBearer
- Pomelo.EntityFrameworkCore.MySql

Installed Nodejs lib:
- Vite (Use for minify and bundle js and css files, run npm run build before deploying)

For database using Mysql/Mariadb, you can change the connection string in appsettings.json file.
My please import file is bau_storage.sql to database befor run the project, this file contains the database schema and some sample data.

Front end page:
- Login page: /login
- Register page: /register
- Home page: / (note aonly can be access after login, all process CRUD products happen in here)
Front end using angularjs, can be found on wwwroot folder, you can run the project and access the pages using browser.

.env file:
all sensitive information such as database connection string, JWT secret key, and other configuration settings are stored in the .env file. Make sure to create a .env file in the root directory of the project and add the necessary environment variables before running the application.
add all .env to root folder project

For .env you can contact me throught Whatsapp (fast response), office email, MSTeams, or create github ticket issue

List of API endpoints:

- POST /api/Configuration/configPrivate: Get private configuration data (use before login & register, does not require authentication)

- POST /api/auth/login: Authenticate user and return JWT token
input:
{
    "Email": "Jan1@gmail.com",
    "Password": "Encrypt pasword usng  AES 128 - CBC - PKCS7"
}
- POST /api/auth/register: Register a new user
input:
	{
    "Email": "Jan1@gmail.com",
    "Role": "admin",
    "Password": "Encrypt pasword usng  AES 128 - CBC - PKCS7"
}
- POST /api/auth/logout: Logout the user, clear the token (requires authentication)
header: {
	"Authorization: "access token from login",
	"Refreshtoken" "refresh token from login"
	}
- POST /api/auth/refresh: Refresh the JWT token (requires authentication)
 header: {
	"Authorization: "access token from login",
	"Refreshtoken" "refresh token from login"
	}
- POST /api/GetListProduct: Get a list of all products
input:
	{
		"page": 1,
		"pageSize": 10,
		"Name": "Product Name" // optional, filter by product name
	}
	header: {
	"Authorization: "access token from login",
	"Refreshtoken" "refresh token from login"
	}
- POST /api/GetProduct: Get details of a specific product by ID
 input:
	{
		"Id": "1"
	}
	header: {
	"Authorization: "access token from login",
	"Refreshtoken" "refresh token from login"
	}
- POST /api/AddProduct: Create a new product (requires authentication)
	input:
	{
		"Name": "New Product",
		"Description": "Product description",
		"Price": "9.99"
	}
	header: {
	"Authorization: "access token from login",
	"Refreshtoken" "refresh token from login"
	}
- POST /api/UpdateProduct: Update an existing product by ID (requires authentication)
input:
	{
		"Id": "1",
		"Name": "Updated Product Name",
		"Description": "Updated description",
		"Price": "19.99"
	}
	header: {
	"Authorization: "access token from login",
	"Refreshtoken" "refresh token from login"
	}
- POST /api/DeleteProduct: Delete a product by ID (requires authentication)
	input:
	{
		"id": "1"
	}
	header: {
	"Authorization: "access token from login",
	"Refreshtoken" "refresh token from login"
	}
