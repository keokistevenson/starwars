# Star Wars Starship Management System 🚀

This is a basic **ASP.NET Core** app that manages Star Wars starships using the **SWAPI API** and supports CRUD operations (Create, Read, Update, Delete). You can also upload images for starships, and some starships are **seeded into the database** on startup.

---

## Features  
- **View Starships**: See all starships stored in the database with their images.  
- **Add Starships**: Select starships from the SWAPI API and upload an image.  
- **Edit Starship Image**: Replace the image for a starship.  
- **Delete Starships**: Remove starships from the database.  
- **Database Seeding**: Two starships with images are added to the database when the app runs for the first time.

---

## Tech Stack  
- **ASP.NET Core 8.0**  
- **Entity Framework Core 8.0**  
- **SQL Server**  
- **SWAPI API** (https://swapi.dev/)  
- **Bootstrap 5** (for UI)

---

## Setup Instructions  

### 1. Clone the Repo  
```bash
git clone <repo-url>
cd StarWars
```
### 2. Configure the Database
Update the connection string in appsettings.json:

```json

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=StarWarsDatabase;Trusted_Connection=True;"
}
```

### 3. Apply Migrations and Create the Database
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
### 4. Add Images
Create a folder called SeedImages in the project root and add:

- 01.png
- 02.png

### How It Works
View Starships: See all stored starships on the home page.
Add New Starship: Go to /Home/Create and select a starship from the API, upload an image, and save.
Update Image: Go to /Home/Edit/{id} to replace the starship’s image.
Delete Starship: Go to /Home/Delete/{id} to remove a starship.
### Database Seeding
On the first run, the app will:

- Add Starship ID 2 (CR90 Corvette) with 01.png
- Add Starship ID 3 (Star Destroyer) with 02.png

### Troubleshooting
EF Tools Not Found:
Run this:

```bash
dotnet tool install --global dotnet-ef
```
Images Not Showing:
Ensure images are placed in SeedImages/ before running the app.

SQL Server Issues:
Make sure SQL Server is running, and the connection string is correct.

### That’s It! 🎉
You’re good to go! Enjoy managing Star Wars starships. 🚀 May the Force be with you.


