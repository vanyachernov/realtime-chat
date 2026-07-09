
## EF Core Migrations

To manage database migrations, ensure you run the `dotnet ef` commands from the root directory specifying both the infrastructure project (where the DbContext lives) and the WebApi project (the startup project that provides the configuration):

**Add a new migration:**
```bash
dotnet ef migrations add <MigrationName> --project src/RealtimeChat.Infrastructure --startup-project src/RealtimeChat.WebApi
```
*Example:* `dotnet ef migrations add InitialCreate --project src/RealtimeChat.Infrastructure --startup-project src/RealtimeChat.WebApi`

**Update the database:**
```bash
dotnet ef database update --project src/RealtimeChat.Infrastructure --startup-project src/RealtimeChat.WebApi
```
