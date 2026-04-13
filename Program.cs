using BAU_PROTO.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection(ConstantConfig.Sec).Value;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key)
        ),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {

            context.NoResult();

            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            string message = context.Exception switch
            {
                SecurityTokenExpiredException => "Your token already expired",
                SecurityTokenInvalidSignatureException => "Invalid token signature",
                _ => "Your token already invalid, re login first"
            };

            return context.Response.WriteAsJsonAsync(new
            {
                message = message
            });
        },

        OnChallenge = context =>
        {
            context.HandleResponse();

            // prevent double write
            if (context.Response.HasStarted)
                return Task.CompletedTask;

            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsJsonAsync(new
            {
                message = "Unauthorized access, please login first"
            });
        }
    };
});

builder.Services.AddAuthorization();


// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    }); ;
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString(ConstantConfig.DbConfig),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString(ConstantConfig.DbConfig))
    ));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseDefaultFiles();

app.UseStaticFiles(); // I want use angularjs for handle SPA instead of using razor page

//if (app.Environment.IsDevelopment())
//{
//    app.MapFallbackToFile(ConstantConfig.ViewDevRoot); // I want use angularjs for handle SPA instead of using razor page
//} else
//{
//    app.MapFallbackToFile(ConstantConfig.ViewRoot);
//}

app.MapFallbackToFile(ConstantConfig.ViewDevRoot); // use generated index.html by Vite as default page for SPA

app.MapControllers();

app.Run();
