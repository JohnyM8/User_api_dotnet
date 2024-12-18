using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.Design;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.Owin.Cors;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Controllers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://molczane.github.io")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddHttpClient();
// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddDbContext<ApiContext>
//    (opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApiContext>
    ( opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                            provOpt => provOpt.EnableRetryOnFailure()));
/*
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("jsonCopy:Jwt:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("jsonCopy:Jwt:Audience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("jsonCopy:Jwt:Key").Value!))
    };
});
*/
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["jsonCopy:Jwt:Issuer"],
                ValidAudience = builder.Configuration["jsonCopy:Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["jsonCopy:Jwt:Key"]!)
                )
            };
        });

// Dodaj serwisy
//builder.Services.AddHttpClient<GoogleAuthService>();
//builder.Services.AddScoped<GoogleAuthService>();

// Konfiguracja Identity
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
        policy =>
        {
            policy.AllowAnyOrigin();
        });
});
/*
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(jwtBearerOptions =>
    {
        jwtBearerOptions.RequireHttpsMetadata = false;
        jwtBearerOptions.SaveToken = true;
        jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TokenManager._secret)),
            ValidateLifetime = true, //validate the expiration and not before values in the token
            ClockSkew = TimeSpan.FromMinutes(1) //1 minute tolerance for the expiration date
        };
    });*/
//builder.Services.AddDbContext<CarContext>
//    (opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionCars"),
//                            provOpt => provOpt.EnableRetryOnFailure()));



var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("MyAllowSpecificOrigins");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
