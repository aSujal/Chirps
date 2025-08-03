# ExampleBlazor

## How to Run

1. Clone the repo.
2. Setup your MySQL connection string in `appsettings.json`.
3. Run EF migrations (`dotnet ef database update`).
4. Install npm dependencies: `npm install`
5. Build Tailwind CSS: `npm run build:css`
6. Run the Blazor app: `dotnet run`

## Tailwind CSS Setup

1. Install Tailwind & PostCSS:
   ```bash
   npm install -D tailwindcss postcss autoprefixer
   npx tailwindcss init -p
   ```

2. Create `wwwroot/tailwind.css` with:
   ```css
   @tailwind base;
   @tailwind components;
   @tailwind utilities;
   ```

3. Add script to `package.json`:
   ```json
   "build:css": "tailwindcss -i ./wwwroot/tailwind.css -o ./wwwroot/css/app.css --watch"
   ```

4. Run `npm run build:css` to build and watch for changes.

5. Link CSS in `index.html` or `_Host.cshtml`:
   ```html
   <link href="css/app.css" rel="stylesheet" />
   ```

## Database (EF Core + MySQL)

- Install NuGet packages:
  ```bash
  dotnet add package Pomelo.EntityFrameworkCore.MySql
  dotnet add package Microsoft.EntityFrameworkCore.Design
  ```

- Define `AppDbContext`:
  ```csharp
  public class AppDbContext : DbContext
  {
      public DbSet<User> Users { get; set; }
      
      protected override void OnConfiguring(DbContextOptionsBuilder options)
          => options.UseMySql("your-connection-string", 
                             ServerVersion.AutoDetect("your-connection-string"));
  }
  ```

- Add migrations:
  ```bash
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```

## Authentication

- Custom user class with:
  - Id, Username (unique), PasswordHash, PasswordSalt, IsAdmin, CreatedAt
- Password hashing using PBKDF2 with SHA-256 and 100,000 iterations
- AuthService with:
  - RegisterUserAsync(username, password)
  - LoginAsync(username, password)  
  - VerifyPassword
- Cookie-based authentication configured in Program.cs:
  ```csharp
  builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
      .AddCookie(options => {
          options.Cookie.Name = "auth_token";
          options.LoginPath = "/login";
          options.Cookie.MaxAge = TimeSpan.FromDays(2);
          options.AccessDeniedPath = "/access-denied";
      });
  ```

## UI Components

FontAwesome icons installed locally via npm for offline use:
  ```bash
  npm install @fortawesome/fontawesome-free
  ```
  Import it. 
  ```css
  @import "@fortawesome/fontawesome-free/css/all.min.css";
  ```

## Notes
