using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleTodo.Api;

var builder = WebApplication.CreateBuilder(args);
var credential = new DefaultAzureCredential();
builder.Configuration.AddAzureKeyVault(new Uri(builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"]), credential);

builder.Services.AddScoped<ListsRepository>();
builder.Services.AddDbContext<TodoDb>(options =>
{
    var connectionString = builder.Configuration[builder.Configuration["AZURE_SQL_CONNECTION_STRING_KEY"]];
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
});

builder.Services.AddControllers();
// configure and then enable app insights 
// builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);

builder.Services.AddAuthentication().AddCookie(options => options.LoginPath = "/lists/login"); 

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["AUTH0_DOMAIN"];
    options.ClientId = builder.Configuration["AUTH0_CLIENT_ID"];
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDb>();
    await db.Database.EnsureCreatedAsync();
}

app.UseCors(policy =>
{
    policy.AllowAnyOrigin();
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
});
    
// Swagger UI
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("./openapi.yaml", "v1");
    options.RoutePrefix = "";
});

app.UseStaticFiles(new StaticFileOptions{
    // Serve openapi.yaml file
    ServeUnknownFileTypes = true,
});

app.MapControllers();
app.Run();
