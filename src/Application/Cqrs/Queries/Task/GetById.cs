using Application.Dto.Task.ReadModels;
using Application.Security;
using AutoMapper;
using Domain.Readers;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Entity = Domain.Entities;

namespace Application.Cqrs.Queries.Task;

public record GetTaskByIdQuery(Guid TaskId) : IRequest<ErrorOr<TaskReadModel>>;

public class GetTaskByIdQueryHandler(
    ITaskReader taskReader,
    IMapper mapper,
    ISecurityContext securityContext,
    ILogger<GetTaskByIdQueryHandler> logger)
    : IRequestHandler<GetTaskByIdQuery, ErrorOr<TaskReadModel>>
{
    public async Task<ErrorOr<TaskReadModel>> Handle(GetTaskByIdQuery request,
        CancellationToken cancellationToken)
    {
        var task = await taskReader.GetByIdAsync(request.TaskId);

        if (task is null)
            return Error.NotFound(
                description: $"Task with ID `{request.TaskId}` doesn't exist");

        logger.LogInformation($"Get the task `{request.TaskId}`");

        return mapper.Map<Entity.Task, TaskReadModel>(task);
    }
}