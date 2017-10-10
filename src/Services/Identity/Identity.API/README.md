dotnet ef migrations add initial  -c PersistedGrantDbContext -o Migrations/PersistedGrantDb
dotnet ef database update -c PersistedGrantDbContext

dotnet ef migrations add initial  -c ConfigurationDbContext -o Migrations/ConfigurationDb
dotnet ef database update -c ConfigurationDbContext

dotnet ef migrations add initial  -c ApplicationDbContext -o Migrations/ApplicationDb
dotnet ef database update -c ApplicationDbContext

