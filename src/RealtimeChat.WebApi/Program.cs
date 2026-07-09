using RealtimeChat.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddOpenApi();
    
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddInfrastructure(connectionString);
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();
    app.Run();
}
