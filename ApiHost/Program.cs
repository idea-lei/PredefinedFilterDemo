using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using PredefinedFilterDemo;
using PredefinedFilterDemo.Data;

var builder = WebApplication.CreateBuilder(args);

var currentDir = Environment.CurrentDirectory;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=../DB/app.db"));

// Add services to the container.

builder.Services.AddControllers().AddOData(o =>
{
    o.EnableQueryFeatures();
    o.AddRouteComponents(EdmModelCreator.Create());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.OperationFilter<ODataOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.SeedDb();

app.Run();
