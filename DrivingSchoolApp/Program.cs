using Npgsql;
using Microsoft.EntityFrameworkCore;
using DrivingSchoolApp.Models;
using DrivingSchoolApp.Services;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.MapEnum<EmployeeRole>("employee_role"); 
dataSourceBuilder.MapEnum<GroupStatus>("group_status");
dataSourceBuilder.MapEnum<ExamType>("exam_type");
dataSourceBuilder.MapEnum<TransmissionType>("transmission_type");
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseNpgsql(dataSource)
);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddScoped<DataService>();

builder.Services.AddScoped<ViewsService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} 

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();