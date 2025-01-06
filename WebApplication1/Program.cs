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


//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin", policy =>
//    {
//        policy.WithOrigins("https://molczane.github.io")
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOriginLocal", policy =>
//    {
//        policy.WithOrigins("http://localhost:8090")
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAnyOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApiContext>
    ( opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                            provOpt => provOpt.EnableRetryOnFailure()));


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

var app = builder.Build();

//app.UseCors("AllowSpecificOrigin");
//app.UseCors("AllowSpecificOriginLocal");
app.UseCors("AllowAnyOrigins");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
