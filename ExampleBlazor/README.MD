# ExampleBlazor

Testing Blazor Server with Tailwind CSS, FontAwesome, and MySQL

## Description
- Custom authentication system with cookie-based sessions
- Modern UI with Tailwind CSS and FontAwesome icons
- MySQL database with Entity Framework Core
- NO Responsive desgin with dark theme
- Password hashing with PBKDF2 + SHA-256

## How to Run

1. **Clone the repository**
   ```bash
   git clone ...
   cd ExampleBlazor
   ```

2. **Setup MySQL Database**
   - Update MySQL connection string in `appsettings.json`
   - Example: 
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ExampleBlazor;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
     ```
   - Run Entity Framework migrations:
     ```bash
     dotnet ef migrations add InitialCreate
     dotnet ef database update
     ```

3. **Install Dependencies**
   ```bash
   npm install
   ```

4. **Build Tailwind CSS**
   ```bash
   npm run build:css
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```

## Project Setup

### Tailwind CSS Configuration

1. **Install Tailwind CSS and dependencies:**
   ```bash
   npm install -D tailwindcss postcss autoprefixer
   npx tailwindcss init -p
   ```

2. **Configure `tailwind.config.js`:**
   ```javascript
   module.exports = {
     content: ["./**/*.{razor,html,cshtml}"],
     theme: {
       extend: {},
     },
     plugins: [],
   }
   ```

3. **Create `wwwroot/tailwind.css`:**
   ```css
   @tailwind base;
   @tailwind components;
   @tailwind utilities;
   ```

4. **Add build script to `package.json`:**
   ```json
   {
     "scripts": {
       "build:css": "tailwindcss -i ./wwwroot/tailwind.css -o ./wwwroot/css/app.css --watch"
     }
   }
   ```

5. **Link CSS in your main HTML file:**
   ```html
   <link href="css/app.css" rel="stylesheet" />
   ```

### Database Setup (Entity Framework Core + MySQL)

1. **Install required NuGet packages:**
   ```bash
   dotnet add package Pomelo.EntityFrameworkCore.MySql
   dotnet add package Microsoft.EntityFrameworkCore.Design
   dotnet add package Microsoft.EntityFrameworkCore.Tools
   ```

2. **Configure `AppDbContext`:**
   ```csharp
   public class AppDbContext : DbContext
   {
       public DbSet<User> Users { get; set; }
       
       public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
       
       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           modelBuilder.Entity<User>()
               .HasIndex(u => u.Username)
               .IsUnique();
       }
   }
   ```

3. **Add to `Program.cs`:**
   ```csharp
   builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
   ```

4. **Create and apply migrations:**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

### Authentication System

**User Model:**
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string PasswordSalt { get; set; } = "";
    public bool IsAdmin { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

**Authentication Configuration in `Program.cs`:**
```csharp
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();
```
**Logout:**
```csharp
var customAuthStateProvider = (CustomAuthenticationStateProvider)AuthStateProvider;
await customAuthStateProvider.UpdateAuthenticationState(null);
```

**Usage:** 
```csharp
<AuthorizeView>
    <Authorized>
        <p>Hello, @context.User.Identity?.Name!</p>
    </Authorized>
    <NotAuthorized>
        <p>You're not logged in.</p>
    </NotAuthorized>
</AuthorizeView>
``` 

**Security Features:**
- Password hashing using PBKDF2 with SHA-256
- 100,000 iterations for enhanced security
- Unique salt per password
- Cookie-based session management
- Sliding expiration for active users

### FontAwesome Icons
Download it from [FontAwesome](https://fontawesome.com/download) and place it in `wwwroot/fontawesome`.

**Direct Link**
```html
<link rel="stylesheet" href="fontawesome/css/all.min.css" />
```

**Usage:**
```html
<i class="fa-solid fa-user"></i>
<i class="fa-solid fa-lock"></i>
<i class="fa-solid fa-eye"></i>
```

## Development Workflow

1. **Start Tailwind CSS watcher:**
   ```bash
   npm run build:css
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **Apply database changes:**
   ```bash
   dotnet ef migrations add <MigrationName>
   dotnet ef database update
   ```
