using Immediate.Apis.FunctionalTests.Scalar;
using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddImmediateApisFunctionalTestsScalarHandlers();

builder.Services.AddEndpointsApiExplorer();
builder.Services
	.AddOpenApi(o => o.CreateSchemaReferenceId = t =>
		t.Type.IsNested
			? $"{t.Type.DeclaringType!.Name}+{t.Type.Name}"
			: OpenApiOptions.CreateDefaultSchemaReferenceId(t));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi().CacheOutput();
app.MapScalarApiReference();

app.MapImmediateApisFunctionalTestsScalarEndpoints();

await app.RunAsync();

public sealed partial class Program;
