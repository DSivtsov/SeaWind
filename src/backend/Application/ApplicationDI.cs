using Application.Abstractions.Services;
using Application.UseCasesAdmin;
using Application.UseCasesCourse;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationDI
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        return services;
    }
}
