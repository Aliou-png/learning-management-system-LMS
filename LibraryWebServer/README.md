# Library Web Server - CS 5530

## Author
Aliou Tippett

## Date
March 2026

## Project Overview
An ASP.NET MVC web application that connects to a MySQL Library database.
Practices scaffolding model code, querying with LINQ, and returning JSON 
responses to a web client.

## How to Run
1. Open the solution in Visual Studio
2. If you get a framework error, make sure your project is targeting .NET 9.0
   - Right click the project → Properties → Target Framework → .NET 9.0
   - The handout defaults to .NET 9.5 which may not be installed, switch it to 9.0
3. Ensure the database connection string is configured in the DbContext
4. Register the DbContext in Program.cs
5. Run the project with IIS Express

## Features
- **Login** — Verifies a Patron's name and card number against the database
- **My Books** — Displays all books currently checked out by the logged in user
- **All Titles** — Displays the full library inventory with checkout status

## Technologies Used
- ASP.NET Core MVC
- Entity Framework Core (Scaffolded)
- Pomelo.EntityFrameworkCore.MySql
- LINQ
- JSON

## Notes
- All queries are written using LINQ against the scaffolded Team3LibraryContext
- This server is not secure and is intended for learning purposes only
- One user can be logged in at a time via a static session variable