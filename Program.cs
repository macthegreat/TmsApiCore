using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;



var builder = WebApplication.CreateBuilder(args);





// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();


builder.Services.AddAuthentication("Training").AddScheme<AuthenticationSchemeOptions,TrainingAuthHandler>("Training", null);

builder.Services.AddAuthorization();
//builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddSingleton<EnrollmentWorker>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

builder.Services.AddOptions<paymentOptions>().BindConfiguration("payments").ValidateDataAnnotations().ValidateOnStart();
 builder.Services.AddProblemDetails();

 builder.Services.AddDbContext<TmsDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("TmsDatabase")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    
}

 app.UseExceptionHandler();
 
 app.UseStatusCodePages();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/assesments/results", () => Results.Ok(new
{
    courseCode ="cs-101",
    studentId="s-001",
    letterGrade="A"
})).RequireAuthorization();

app.MapGet("/api/enrollments/worker-smoke", async (EnrollmentWorker worker) =>
{
    await worker.processBatch();
    return Results.Ok("processed");});

    app.MapGet("/api/error", () =>
{
throw new TmsDatabaseException("Simulated database failure for ProblemDetails testing");
});

// Configure the HTTP request pipeline.



app.Run();
