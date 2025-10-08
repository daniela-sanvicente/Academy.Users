using Academy.Users.Application;
using Academy.Users.Infrastructure;
using Academy.Users.Presentation;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Academy Users API",
        Version = "v1",
        Description = "API mínima para administrar datos personales de usuarios dentro del ecosistema Academy. " +
                      "Expone operaciones para actualizar información de contacto y sirve como punto de integración con la capa Application basada en CQRS.",
        Contact = new OpenApiContact
        {
            Name = "Daniela Sanvicente",
            Url = new Uri("https://github.com/daniela-sanvicente")
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPresentationEndpoints();

app.Run();
