using Application.Data;
using Application.Security;
using Domain.Readers;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Cqrs.Commands.Task;

public record UpdateTaskCommand(
    Guid Id,
    string? Name,
    string? Description,
    bool? Success,
    DateOnly? BeginDate,
    DateOnly? EndDate)
    : IRequest<ErrorOr<Updated>>;

public class UpdateTaskCommandHandler(
    IDataContext dataContext,
    ITaskReader taskReader,
    ISecurityContext securityContext,
    ILogger<UpdateTaskCommandHandler> logger)
    : IRequestHandler<UpdateTaskCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateTaskCommand request,
        CancellationToken cancellationToken)
    {
        if (request.BeginDate >= request.EndDate)
            return Error.Validation(
                description: "`beginDate` must be less than `endDate`");

        var task = await taskReader.GetByIdAsync(request.Id);

        if (task is null)
            return Error.NotFound(
                description: $"Task with ID `{request.Id}` doesn't exist");

        task.Success = request.Success ?? task.Success;
        task.Name = request.Name ?? task.Name;
        task.Description = request.Description ?? task.Description;
        task.BeginDate = request.BeginDate ?? task.BeginDate;
        task.EndDate = request.EndDate ?? task.EndDate;

        await dataContext.TaskRepository.Update(task);
        await dataContext.SaveChangesAsync();

        logger.LogInformation($"Task `{request.Id}` was updated");

        return Result.Updated;
    }
}