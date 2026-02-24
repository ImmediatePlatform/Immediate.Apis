using Immediate.Apis.FunctionalTests.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddImmediateApisFunctionalTestsSwaggerHandlers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
	o.CustomSchemaIds(t => t.FullName?.Replace('+', '.'))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapImmediateApisFunctionalTestsSwaggerEndpoints();

app.Run();

public sealed partial class Program;
