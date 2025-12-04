using Application.Dto.Task.ReadModels;
using Application.Security;
using AutoMapper;
using Domain.Readers;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Entity = Domain.Entities;

namespace Application.Cqrs.Queries.Task;

public record GetSubTasksByParentIdQuery(Guid ParentTaskId)
    : IRequest<ErrorOr<IEnumerable<TaskNodeReadModel>>>;

public class GetSubTasksByParentIdQueryHandler(
    ITaskReader taskReader,
    IMapper mapper,
    ISecurityContext securityContext,
    ILogger<GetSubTasksByParentIdQueryHandler> logger)
    : IRequestHandler<GetSubTasksByParentIdQuery, ErrorOr<IEnumerable<TaskNodeReadModel>>>
{
    public async Task<ErrorOr<IEnumerable<TaskNodeReadModel>>> Handle(
        GetSubTasksByParentIdQuery request,
        CancellationToken cancellationToken)
    {
        var tasks = await taskReader.GetSubTasksByParentIdAsync(request.ParentTaskId);

        logger.LogInformation($"Get subtasks of task `{request.ParentTaskId}`");

        return mapper
            .Map<IEnumerable<Entity.Task>, IEnumerable<TaskNodeReadModel>>(tasks)
            .ToErrorOr();
    }
}