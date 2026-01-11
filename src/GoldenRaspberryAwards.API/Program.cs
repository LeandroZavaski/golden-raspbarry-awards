using GoldenRaspberryAwards.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ProjectBootstrap();

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.ConfigurePipeline();

app.Run();

public partial class Program { }