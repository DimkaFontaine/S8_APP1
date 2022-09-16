using SondageApi.Controllers;
using SondageApi.Models;
using SondageApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DataBaseFileUri>(builder.Configuration.GetSection(DataBaseFileUri.Section));

builder.Services.AddSingleton<ITextDataBase,TextDataBase>();

builder.Services.AddSingleton<SondageController, SondageController>();

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

app.Run();
