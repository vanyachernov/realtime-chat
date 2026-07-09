using RealtimeChat.Application;
using RealtimeChat.Infrastructure;
using RealtimeChat.WebApi.Hubs;
using RealtimeChat.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddOpenApi();
    builder.Services.AddControllers();
    builder.Services.AddSignalR();
    builder.Services.AddApplication();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddInfrastructure(connectionString);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("WpfClient", policy =>
        {
            policy.WithOrigins("http://localhost", "https://localhost")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });
}

var app = builder.Build();
{
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseCors("WpfClient");
    app.UseHttpsRedirection();
    app.MapControllers();
    app.MapHub<ChatHub>("/chat");
    app.Run();
}
