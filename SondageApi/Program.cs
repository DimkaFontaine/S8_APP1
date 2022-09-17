using Microsoft.OpenApi.Models;
using SondageApi.Middleware;
using SondageApi.Models;
using SondageApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(ApiKeySettings.ApiKeyHeader, new OpenApiSecurityScheme
    {
        Description = "Api key needed to access the endpoints. X-Api-Key: My_API_Key",
        In = ParameterLocation.Header,
        Name = ApiKeySettings.ApiKeyHeader,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = ApiKeySettings.ApiKeyHeader,
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = ApiKeySettings.ApiKeyHeader
                },
             },
             new string[] {}
         }
    });
});

builder.Services.Configure<DataBaseFileUri>(builder.Configuration.GetSection(DataBaseFileUri.Section));

builder.Services.Configure<ApiKeySettings>(builder.Configuration.GetSection(ApiKeySettings.Section));

builder.Services
    .AddSingleton<IAnswer, SurveyAnswerSaver>()
    .AddSingleton<ISurveyReader, SurveyReader>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApiKeyMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
