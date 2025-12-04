using Application.Dto.Task.ReadModels;
using Application.Security;
using AutoMapper;
using Domain.Readers;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Entity = Domain.Entities;

namespace Application.Cqrs.Queries.Task;

public record GetTasksByUserIdQuery(Guid UserId)
    : IRequest<ErrorOr<IEnumerable<TaskNodeReadModel>>>;

public class GetTasksByUserIdQueryHandler(
    ITaskReader taskReader,
    IMapper mapper,
    ISecurityContext securityContext,
    ILogger<GetTasksByUserIdQueryHandler> logger)
    : IRequestHandler<GetTasksByUserIdQuery, ErrorOr<IEnumerable<TaskNodeReadModel>>>
{
    public async Task<ErrorOr<IEnumerable<TaskNodeReadModel>>> Handle(
        GetTasksByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var tasks = await taskReader.GetByUserIdAsync(request.UserId);

        logger.LogInformation($"User `{request.UserId}` gets the tasks");

        return mapper
            .Map<IEnumerable<Entity.Task>, IEnumerable<TaskNodeReadModel>>(tasks)
            .ToErrorOr();
    }
}