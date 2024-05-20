using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using MyBusiness.DepartmentMicroservice.Services;
using MyBusiness.EmployeeMicroservice.Services;
using MyBusiness.ProductMicroservice.Services;
using MyBusiness.RelationData;
using MyBusiness.ReportMicroservice.Services;
using MyBusiness.SupplierMicroservice.Services;
using MyBusiness.TransactionMicroservice.Services;
using MyBusiness.UserMicroservice.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MySQLDataContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("MySQL")));

builder.Services.AddSingleton<MongoDBDataContext>(provider =>
    new MongoDBDataContext(builder.Configuration.GetConnectionString("MongoDB"), "mybusinessdb"));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();

// Register MongoDB collections using a scoped approach
builder.Services.AddScoped(provider =>
{
    var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDB"));
    var database = mongoClient.GetDatabase("mybusinessdb");
    return database.GetCollection<BsonDocument>("employees");
});
builder.Services.AddScoped(provider =>
{
    var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDB"));
    var database = mongoClient.GetDatabase("mybusinessdb");
    return database.GetCollection<BsonDocument>("products");
});
builder.Services.AddScoped(provider =>
{
    var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDB"));
    var database = mongoClient.GetDatabase("mybusinessdb");
    return database.GetCollection<BsonDocument>("reports");
});
builder.Services.AddScoped(provider =>
{
    var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDB"));
    var database = mongoClient.GetDatabase("mybusinessdb");
    return database.GetCollection<BsonDocument>("transactions");
});
builder.Services.AddScoped(provider =>
{
    var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDB"));
    var database = mongoClient.GetDatabase("mybusinessdb");
    return database.GetCollection<BsonDocument>("suppliers");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
