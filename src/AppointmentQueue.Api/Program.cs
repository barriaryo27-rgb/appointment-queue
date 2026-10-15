using AppointmentQueue.Api.Middleware;
using AppointmentQueue.Business.Interfaces;
using AppointmentQueue.Business.Repositories;
using AppointmentQueue.Business.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Use Autofac as the dependency injection container.
// Every class in the Business project only asks for interfaces in its constructor;
// this is the one place that decides which real class is used for each interface.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    container.RegisterType<SystemClock>().As<IClock>().SingleInstance();
    container.RegisterType<AgencySettings>().As<IAgencySettings>().SingleInstance();

    // Repositories are singletons because the data lives in memory.
    container.RegisterType<InMemoryAppointmentRepository>().As<IAppointmentRepository>().SingleInstance();
    container.RegisterType<InMemoryOffDayRepository>().As<IOffDayRepository>().SingleInstance();

    // Services are created once per web request.
    container.RegisterType<AppointmentService>().As<IAppointmentService>().InstancePerLifetimeScope();
    container.RegisterType<OffDayService>().As<IOffDayService>().InstancePerLifetimeScope();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Appointment Queue API",
        Version = "v1",
        Description = "Agency appointment booking with queue tokens, off days and a daily limit."
    });
});

var app = builder.Build();

// First in the pipeline so it catches errors from everything after it.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger is always on (also on Azure) so the API can be tried from the browser.
app.UseSwagger();
app.UseSwaggerUI();

// Opening the site root goes straight to Swagger.
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapControllers();

app.Run();
