using Npgsql;
using Microsoft.EntityFrameworkCore;
using DrivingSchoolApp.Models;
using DrivingSchoolApp.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Настройка маппинга enum для PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.MapEnum<GroupStatus>("group_status");
dataSourceBuilder.MapEnum<ExamResult>("exam_result");
dataSourceBuilder.MapEnum<ExamType>("exam_type");
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


//builder.Services.AddScoped<GroupService>();

//builder.Services.AddScoped<LicenseCategoryService>();
//builder.Services.AddScoped<StudentService>();
//builder.Services.AddScoped<EmployeeService>();

//builder.Services.AddScoped<TransportService>();
//builder.Services.AddScoped<TheoryLessonService>();
//builder.Services.AddScoped<DrivingLessonService>();
//builder.Services.AddScoped<ExamService>();

//builder.Services.AddScoped<ExamResultService>();
builder.Services.AddScoped<DataService>();

builder.Services.AddScoped<ViewsService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); 

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();