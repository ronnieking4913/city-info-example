using System.Text;
using CityInfo.API;
using CityInfo.API.DbContexts;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

//This is custom logging using a third party logging system serilog.aspnet and serilog.sinks.console and serilog.sinks.file
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

//add auto mapper to the project
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//add authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))

        };
    });

builder.Services.AddAuthorization(options =>
{
    //give the policy a name
    options.AddPolicy("MustBeFromAntwerp", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "Antwerp");
    });
});

//add versioning support
builder.Services.AddApiVersioning(setupAction =>
{
    // to not request a version to access apis
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    
    // make the default version v1.0
    setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1,0);

    //report back what versions the apis support
    setupAction.ReportApiVersions = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
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

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapControllers();

app.Run();

