using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebAPIApplication;
// using Auth0.AspNetCore.Authentication;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
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

/*builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["AUTH0_DOMAIN"];
    options.ClientId = builder.Configuration["AUTH0_CLIENT_ID"];
});
builder.Services.AddAuthorization();*/

var domain = $"https://{builder.Configuration["AUTH0_DOMAIN"]}/";
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
.AddJwtBearer(options =>
{
    options.Authority = domain;
    options.Audience = builder.Configuration["AUTH0_AUDIENCE"];
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("read:lists", policy => policy.RequireClaim("permissions", "read:lists"));
});


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

/* await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDb>();
    await db.Database.EnsureCreatedAsync();
} */

app.UseCors(policy =>
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );
    
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
