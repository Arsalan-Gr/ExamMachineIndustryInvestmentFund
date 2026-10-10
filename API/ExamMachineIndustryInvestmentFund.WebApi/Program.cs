using ExamMachineIndustryInvestmentFund.Application.Configurations;
using ExamMachineIndustryInvestmentFund.Application.Interfaces;
using ExamMachineIndustryInvestmentFund.Application.Interfaces.Repositories;
using ExamMachineIndustryInvestmentFund.Application.Interfaces.Services;
using ExamMachineIndustryInvestmentFund.Application.Services;
using ExamMachineIndustryInvestmentFund.Infrastructure;
using ExamMachineIndustryInvestmentFund.Infrastructure.Identity;
using ExamMachineIndustryInvestmentFund.Infrastructure.Repositories;
using ExamMachineIndustryInvestmentFund.Infrastructure.Services;
using ExamMachineIndustryInvestmentFund.WebApi.ExceptionHandling;
using Microsoft.OpenApi;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IRequestService ,RequestService>();


builder.Services.AddJwtConfiguration(builder.Configuration);


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

builder.Services
    .AddOptions<RequestRoutingOptions>()
    .Bind(builder.Configuration.GetSection(RequestRoutingOptions.SectionName))
    .Validate
    (
        options => options.ApprovalThreshold > 0,
        "Approval Threshold must be greater than zero."
    )
    .ValidateOnStart()
    ;

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactUI", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("ReactUI");

await IdentitySeeder.SeedRolesAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
