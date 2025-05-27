using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Endpoints;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Models.Modelos;
using System.Data.SqlTypes;
using System.Text.Json.Serialization;




var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddDbContext<ScreenSoundContext>();
builder.Services.AddTransient<DAL<Artista>>();
builder.Services.AddTransient<DAL<Musica>>();
builder.Services.AddTransient<DAL<Genero>>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(
    options => options.AddPolicy(
        "_myAllowSpecificOrigins",
        policy => policy.WithOrigins([builder.Configuration["BackendUrl"] ?? "https://localhost:7017",
            builder.Configuration["FrontendUrl"] ?? "https://localhost:7222"])
            .AllowAnyMethod()
            .SetIsOriginAllowed(pol => true)
            .AllowAnyHeader()
            .AllowCredentials()));

var app = builder.Build();

app.UseCors("_myAllowSpecificOrigins");

app.UseStaticFiles();
app.AddEndpointsArtistas();
app.AddEndpointsMusicas();
app.AddEndpointsGeneros();
app.UseSwagger();
app.UseSwaggerUI();



app.Run();
