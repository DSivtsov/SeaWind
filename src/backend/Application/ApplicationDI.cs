using Application.Abstractions.Services;
using Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationDI
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<IExerciseService, ExerciseService>();
        return services;
    }
}
