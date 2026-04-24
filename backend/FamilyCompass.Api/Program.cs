using FamilyCompass.Api.Extensions;
using FamilyCompass.Application.Families.Interfaces;
using FamilyCompass.Application.Persistence;
using FamilyCompass.Application.Transactions.Interfaces;
using FamilyCompass.Application.Families.Services;
using FamilyCompass.Infrastructure.Persistence.Families;
using FamilyCompass.Application.Transactions.Services;
using FamilyCompass.Infrastructure.Persistence.Transactions;
using FamilyCompass.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
    );
});
builder.Services.AddOpenApi();
builder.Services.AddFamilyCompassAuthentication(configuration);

// DB
builder.Services.AddDbContext<FamilyCompassDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// DI
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IFamilyRepository, FamilyRepository>();
builder.Services.AddScoped<IFamilyMembershipRepository, FamilyMembershipRepository>();
builder.Services.AddScoped<IFamilyService, FamilyService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FamilyCompassDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.MapFallbackToFile("index.html");
app.Run();
