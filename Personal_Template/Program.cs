using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Personal.Template.Web.Api.AutoFac;
using System.Text.Json.Serialization;
using Personal.Repository.Extensions;
using Personal.Service.AutoMapper;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Personal.Repository.Share;

var builder = WebApplication.CreateBuilder(args);

//builder.Host.AddApolloService(builder.Configuration["Apollo:MetaAddress"], "LDP", "ComplianceDatabase");

var serviceProvider = new AutofacServiceProviderFactory();
var _container = default(ILifetimeScope);

IdentityModelEventSource.ShowPII = true;
builder.Host.UseServiceProviderFactory(serviceProvider).ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule<AutofacModule>();
    containerBuilder.RegisterBuildCallback(container =>
    {
        _container = container;
    });
});

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

//add cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        configurePolicy =>
        {
            configurePolicy.WithOrigins("*")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
            //.AllowCredentials();
        });
});

builder.Services.AddDatabaseSupport(builder.Configuration);
builder.Services.AddAutoMapper(e =>
{
    e.AddProfile<AutoMapperProfile>();
});

builder.Services.AddLogging(configure =>
{
    Serilog.Log.Logger = new LoggerConfiguration()
    .WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(builder.Configuration["ElasticSearch:Url"]))
    {
        TypeName = null,
        AutoRegisterTemplate = true,
        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
        IndexFormat = $"log-{builder.Environment?.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.Now:yyyy-MM-dd}"
    }).CreateLogger();
});

var app = builder.Build();
app.UseExceptionHandler(new ExceptionHandlerOptions
{
    ExceptionHandler = async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>();
        if (exception != null)
        {
            Serilog.Log.Logger.Fatal(exception.Error, exception.Error?.Message ?? "SystemError");
        }
    }
});
app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
await _container.Resolve<AppDbContext>().ApplyDatabaseMigrationsAsync();

app.Run();