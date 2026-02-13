using Application.Abstractions.Services;
using Application.AbstractionsTime.Services;
using Application.UseCasesAdmin;
using Application.UseCasesCourse;
using Application.UseCasesTime;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationDI
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITesterService, TesterService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        return services;
    }
}
