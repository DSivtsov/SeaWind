using Application.Abstractions.Repositories.ChatExercise;
using Infrastructure.Mongo.ExerciseChat;
using Infrastructure.Mongo.ExerciseChat.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Mongo;

public static class ExerciseChatRepositoriesDI
{
    internal static IServiceCollection AddExerciseChatRepositories(this IServiceCollection services)
    {
        // Для интеграционных тестов (CI).
        // При включённом WC_USE_TEST_SETTINGS вместо Mongo-репозиториев
        // регистрируются FakeRepository.
        if (Environment.GetEnvironmentVariable("WC_USE_TEST_SETTINGS") != "true")
        {
            services.AddScoped<IThreadRepository, MongoThreadRepository>();
            services.AddScoped<IMessageRepository, MongoMessageRepository>();
            services.AddScoped<IAttachmentRepository, MongoAttachmentRepository>();
            services.AddScoped<ILockRepository, MongoLockRepository>();
        }
        else
        {
            services.AddScoped<IMessageRepository, FakeExerciseChatRepository>();
            services.AddScoped<IThreadRepository, FakeExerciseChatRepository>();
            services.AddScoped<IAttachmentRepository, FakeExerciseChatRepository>();
            services.AddScoped<ILockRepository, FakeExerciseChatRepository>();
        }

        return services;
    }
}
