using NexusPay.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ApplyConfigurations(builder.Configuration);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();
app.ApplyServices();

app.Run();
