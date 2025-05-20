using ScrapperEy.DataAccess;
using ScrapperEy.Services;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

services.AddSingleton(configuration);

services.AddTransient<DatabaseManager>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<UiPathService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["UiPath:BaseUrl"]);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("ScrapperEy",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

services.AddScoped<OffShoreService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("ScrapperEy");

app.UseAuthorization();

app.MapControllers();

app.Run();
