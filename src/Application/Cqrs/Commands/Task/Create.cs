using Application.Data;
using Application.Security;
using Domain.Readers;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Entity = Domain.Entities;

namespace Application.Cqrs.Commands.Task;

public record CreateTaskCommand(
    Guid Id,
    Guid OwnerId,
    DateOnly BeginDate,
    DateOnly EndDate,
    string Name,
    string? Description,
    bool? Success)
    : IRequest<ErrorOr<Created>>;

public class CreateTaskCommandHandler(
    IDataContext dataContext,
    ITaskReader taskReader,
    ISecurityContext securityContext,
    ILogger<CreateTaskCommandHandler> logger)
    : IRequestHandler<CreateTaskCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        if (request.BeginDate >= request.EndDate)
            return Error.Validation(
                description: "`beginDate` must be less than `endDate`");

        if (await taskReader.GetByIdAsync(request.Id) is not null)
            return Error.Conflict(
                description: $"Task with ID `{request.Id}` is already exists");

        await dataContext.TaskRepository.Add(new Entity.Task(request.Id, request.OwnerId,
            request.BeginDate, request.EndDate, request.Name, request.Description,
            request.Success));

        await dataContext.SaveChangesAsync();

        logger.LogInformation($"User `{request.OwnerId}` created a task `{request.Id}`");

        return Result.Created;
    }
}