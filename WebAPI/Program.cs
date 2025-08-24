using CommonLibrary;
using CommonLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Reflection;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy => policy
        .WithOrigins("http://localhost:63443")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new() { Title = "ICON: Task Management System - API", Version = "v1" });

    x.IncludeXmlComments(xmlPath);

    foreach (var enumTypeItem in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsEnum))
    {
        x.MapType<TaskItemStatusType>(() => new OpenApiSchema
        {
            Type = "string",
            Enum = [.. Enum.GetValues(enumTypeItem)
                .Cast<Enum>()
                .Select(x => new OpenApiString($"{Convert.ToInt32(x)} = {x}"))
                .Cast<IOpenApiAny>()],
        });
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<MiddlewareErrorHandling>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();
