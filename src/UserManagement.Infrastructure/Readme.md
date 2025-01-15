How to run migration

1. Ensure EF Core Tools Are Installed
Run the following command to ensure you have the EF Core CLI tools:
dotnet tool install --global dotnet-ef


If already installed, update it to the latest version:
dotnet tool update --global dotnet-ef


2. Add a Migration
Run the following command in the terminal within the project directory containing your DbContext:
dotnet ef migrations add InitialMigration



Replace InitialMigration with a descriptive name if you're adding subsequent migrations.
Parameters:
--context: If you have multiple DbContext classes, specify the context:
dotnet ef migrations add InitialMigration --context AppDbContext

--project: If the DbContext is in a different project, specify the project:
dotnet ef migrations add InitialMigration --project Infrastructure


3. Apply the Migration
To update your database schema, run:
dotnet ef database update



4. Verify Database Changes
Check your database to ensure the migration was applied correctly:

Inspect the new tables.
Verify the __EFMigrationsHistory table for the applied migration.



Common Issues and Fixes

No provider has been configured for this DbContext Ensure the database provider is configured in Program.cs:

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


Build errors during migration Ensure your application builds successfully before adding a migration. Run:
    dotnet build
   
   
Migration conflicts If you have unresolved changes, resolve conflicts or clear previous migrations (if safe):
    dotnet ef migrations remove
