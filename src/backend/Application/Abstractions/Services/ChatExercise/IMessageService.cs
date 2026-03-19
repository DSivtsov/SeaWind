using Application.Dto.ChatExercise;

namespace Application.Abstractions.Services.ChatExercise
{
    public interface IMessageService
    {
        Task<CreateMessageResponse> ValidateAndSaveMessageAsync(CreateMessageCommand cmd, CancellationToken ct);

    }
}
