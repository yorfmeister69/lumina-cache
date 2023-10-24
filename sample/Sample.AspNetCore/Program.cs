using LuminaCache.AspNetCore;
using LuminaCache.Core;
using Microsoft.AspNetCore.Mvc;
using Sample.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLuminaCache()
    .AddCollection<Employee, string>(x => x.Id, config =>
    {
        config.SetExpiration(TimeSpan.FromHours(1));
        config.AddSecondaryIndex(x => x.Department);
    }).AddBackingStore<EmployeeBackingStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/employees/all",
        async (ICacheClient<Employee, string> cacheClient, CancellationToken cancellationToken)
            => await cacheClient.Query()
                .MaterializeAsync(cacheClient, cancellationToken))
    .WithName("GetAllEmployees")
    .WithTags("Employees")
    .WithOpenApi();

app.MapGet("/employees/{id}",
        async (ICacheClient<Employee, string> cacheClient, string id, CancellationToken cancellationToken)
            => await cacheClient.GetAsync(id))
    .WithName("GetEmployee")
    .WithTags("Employees")
    .WithOpenApi();

app.MapPost("/employees/many",
        async (ICacheClient<Employee, string> cacheClient, [FromBody]IEnumerable<string> ids, CancellationToken cancellationToken)
            => await cacheClient.GetManyAsync(ids))
    .WithName("GetManyEmployees")
    .WithTags("Employees")
    .WithOpenApi();

app.MapGet("/employees/by-department/{department}",
        async (ICacheClient<Employee, string> cacheClient, string department, CancellationToken cancellationToken)
            => await cacheClient.GetByIndexAsync(x => x.Department, department))
    .WithName("EmployeesByDepartment")
    .WithTags("Employees")
    .WithOpenApi();


app.Run();