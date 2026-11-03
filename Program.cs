using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using my_pos.Data;
using my_pos.Models.DTOs;
using my_pos.Services;
using my_pos.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"));
});

builder.Services.AddScoped<IUser, UserServices>();
builder.Services.AddScoped<IProduct, ProductServices>();

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Where(e => e.Value?.Errors.Count > 0)
        .SelectMany(e => e.Value!.Errors.Select(x => x.ErrorMessage)).ToList();

        var response = new ResponseDTO
        {
            Success = false,
            Message=string.Join("",errors)
        };

        return new BadRequestObjectResult(response);


    };

});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer =true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience=true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime=true,
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Token"])),
        ValidateActor=true
    };
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
