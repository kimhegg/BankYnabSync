
using BankYnabSync.Models.Bank;
using BankYnabSync.Models.Repositories;
using BankYnabSync.Models.Services;
using BankYnabSync.Repository;
using BankYnabSync.Services;
using BankYnabSync.Services.Tools;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Add this line to enable controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add your custom services
builder.Services.AddTransient<IBank, BankService>();
builder.Services.AddTransient<IYnabRepository, YnabRepository>();
builder.Services.AddTransient<IBankRepository, BankRepository>();
builder.Services.AddTransient<IYnabService, YnabService>();
builder.Services.AddTransient<ISecretService, SecretService>();
builder.Services.AddTransient<ISyncService, SyncService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization(); // Add this line if you plan to use authorization

app.MapControllers();

app.Run();