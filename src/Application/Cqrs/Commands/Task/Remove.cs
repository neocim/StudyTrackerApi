using Application.Data;
using Application.Security;
using Domain.Readers;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Cqrs.Commands.Task;

public record RemoveTaskCommand(Guid TaskId) : IRequest<ErrorOr<Deleted>>;

public class RemoveTaskCommandHandler(
    IDataContext dataContext,
    ITaskReader taskReader,
    ISecurityContext securityContext,
    ILogger<RemoveTaskCommandHandler> logger)
    : IRequestHandler<RemoveTaskCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(RemoveTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await taskReader.GetByIdAsync(request.TaskId);

        if (task is null)
            return Error.NotFound(
                description: $"Task with ID `{request.TaskId}` doesn't exist");

        await dataContext.TaskRepository.Remove(task);
        await dataContext.SaveChangesAsync();

        logger.LogInformation($"Task `{request.TaskId}` was removed");

        return Result.Deleted;
    }
}