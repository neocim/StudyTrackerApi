using Entity = Domain.Entities;
using Application.Data;
using Application.Security;
using Application.Security.Permissions;
using Domain.Readers;
using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Cqrs.Commands.Task;

public record CreateSubTaskCommand(
    Guid Id,
    Guid ParentTaskId,
    Guid OwnerId,
    DateOnly BeginDate,
    DateOnly EndDate,
    string Name,
    string? Description,
    bool? Success)
    : IRequest<ErrorOr<Created>>;

public class CreateSubTaskCommandHandler(
    IDataContext dataContext,
    ITaskReader taskReader,
    ISecurityContext securityContext,
    ILogger<CreateSubTaskCommandHandler> logger)
    : IRequestHandler<CreateSubTaskCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateSubTaskCommand request,
        CancellationToken cancellationToken)
    {
        if (!securityContext.HasPermission(Permission.Task.Create))
            return Error.Forbidden(description: "Access denied");

        if (request.BeginDate >= request.EndDate)
            return Error.Validation(
                description: $"`beginDate` must be less than `endDate`");

        var task = await taskReader.GetByIdAsync(request.ParentTaskId);

        if (task is null)
            return Error.NotFound(
                description:
                $"Task with ID `{request.ParentTaskId}` doesn't exist");

        await dataContext.TaskRepository.Add(new Entity.Task(request.Id, request.OwnerId,
            request.BeginDate, request.EndDate, request.Name, request.Description,
            request.Success, task));

        await dataContext.SaveChangesAsync();

        logger.LogInformation(
            $"User `{request.OwnerId}` created a subtask `{request.Id}` of the `{request.ParentTaskId}`");

        return Result.Created;
    }
}