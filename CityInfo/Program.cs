using CityInfo.API;
using CityInfo.API.DbContexts;
using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Serilog;

//This is cusom logging using a third party logging system serilog.aspnet and serilog.sinks.console and serilog.sinks.file
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

//builder.Logging.ClearProviders();                     //Stop all logging
//builder.Logging.AddConsole();                         // add console logging

builder.Host.UseSerilog();                              // tell the builder to use serilog instead

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;         //if the client wants anything except JSON

})
    .AddNewtonsoftJson()                            //This is from adding the dependencies mvc.newtonsoft.json and .jsonpatch 
    .AddXmlDataContractSerializerFormatters();      //Accepts xml requests

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();      //using all types of file extensions for downloading files

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

//adding the data store dependency
builder.Services.AddSingleton<CitiesDataStore>();

//adding the database through dependency injection
builder.Services.AddDbContext<CityInfoContext>(
    dbContextOptions =>
        dbContextOptions.UseSqlite(
            builder.Configuration["ConnectionString:CityInfoDBConnectionString"]));

builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapControllers();

app.Run();

