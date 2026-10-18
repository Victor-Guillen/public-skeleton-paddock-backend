using Paddock.BuildingBlocks.Web;
using Paddock.Modules.Billing;
using Paddock.Modules.Booking;
using Paddock.Modules.Customers;
using Paddock.Modules.Notifications;
using Paddock.Modules.Operations;
using Paddock.Modules.Planning;
using Paddock.Modules.Tenancy;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services
    .AddTenancyModule()
    .AddCustomersModule()
    .AddPlanningModule()
    .AddBookingModule()
    .AddBillingModule()
    .AddOperationsModule()
    .AddNotificationsModule();

var app = builder.Build();

app.UseExceptionHandler(); // necessarily in first place

app.MapDefaultEndpoints();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapTenancyModule()
    .MapCustomersModule()
    .MapPlanningModule()
    .MapBookingModule()
    .MapBillingModule()
    .MapOperationsModule()
    .MapNotificationsModule();

app.Run();
