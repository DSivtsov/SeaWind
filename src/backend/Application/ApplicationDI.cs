using Application.Abstractions.Services;
using Application.Abstractions.Services.ChatExercise;
using Application.UseCases;
using Application.UseCases.ChatExercise;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationDI
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddScoped<IThreadService, ThreadService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IAttachmentService, AttachmentService>();
        services.AddScoped<ILockService, LockService>();
        services.AddScoped<CleanMongoService>();

        return services;
    }
}
