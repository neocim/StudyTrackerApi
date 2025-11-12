using Api.Dto.Requests.Task;
using Api.Dto.Responses.Task;
using Application.Cqrs.Commands.Task;
using Application.Cqrs.Queries.Task;
using Application.Dto.Task.ReadModels;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// [Authorize]
[Route("users/{userId:guid}")]
public class TasksController(IMediator mediator, IMapper mapper)
    : ApiController
{
    [HttpPost("tasks")]
    public async Task<ActionResult<TaskResponse>> CreateTask(Guid userId,
        CreateTaskRequest request)
    {
        var taskId = Guid.NewGuid();

        var command = new CreateTaskCommand(taskId, userId, request.BeginDate,
            request.EndDate, request.Name, request.Description, request.Success);
        var result = await mediator.Send(command);

        var response = new TaskResponse(taskId, userId, null, request.BeginDate,
            request.EndDate, request.Name, request.Description, request.Success);
        var routeValues = new { userId, taskId };

        return result.Match(_ => CreatedAtAction(nameof(GetTask), routeValues, response),
            Error);
    }

    [HttpPost("tasks/{parentTaskId:guid}/subtasks")]
    public async Task<ActionResult<TaskResponse>> CreateSubTask(Guid userId,
        Guid parentTaskId,
        CreateSubTaskRequest request)
    {
        var taskId = Guid.NewGuid();

        var command = new CreateSubTaskCommand(taskId, parentTaskId, userId,
            request.BeginDate, request.EndDate, request.Name, request.Description,
            request.Success);
        var result = await mediator.Send(command);

        var response = new TaskResponse(taskId, userId, parentTaskId, request.BeginDate,
            request.EndDate, request.Name, request.Description, request.Success);
        var routeValues = new { userId, taskId };

        return result.Match(_ => CreatedAtAction(nameof(GetTask), routeValues, response),
            Error);
    }

    [HttpGet("tasks/{taskId:guid}")]
    public async Task<ActionResult<TaskResponse>> GetTask(Guid taskId)
    {
        var query = new GetTaskByIdQuery(taskId);
        var result = await mediator.Send(query);

        return result.Match(task => Ok(mapper.Map<TaskReadModel, TaskResponse>(task)),
            Error);
    }

    [HttpGet("tasks")]
    public async Task<ActionResult<IEnumerable<TaskNodeResponse>>> GetTasks(Guid userId)
    {
        var query = new GetTasksByUserIdQuery(userId);
        var result = await mediator.Send(query);

        return result.Match(
            tasks => Ok(mapper
                .Map<IEnumerable<TaskNodeReadModel>,
                    IEnumerable<TaskNodeResponse>>(tasks)), Error);
    }

    [HttpGet("tasks/{parentTaskId:guid}/subtasks")]
    public async Task<ActionResult<IEnumerable<TaskNodeResponse>>> GetSubTasks(
        Guid parentTaskId)
    {
        var query = new GetSubTasksByParentIdQuery(parentTaskId);
        var result = await mediator.Send(query);

        return result.Match(
            tasks => Ok(mapper
                .Map<IEnumerable<TaskNodeReadModel>,
                    IEnumerable<TaskNodeResponse>>(tasks)), Error);
    }

    [HttpPatch("tasks/{taskId:guid}")]
    public async Task<ActionResult> UpdateTask(Guid taskId, UpdateTaskRequest request)
    {
        var command = new UpdateTaskCommand(taskId, request.Name, request.Description,
            request.Success, request.BeginDate, request.EndDate);
        var result = await mediator.Send(command);

        return result.Match(_ => NoContent(), Error);
    }

    [HttpDelete("tasks/{taskId:guid}")]
    public async Task<ActionResult> DeleteTask(Guid taskId)
    {
        var command = new RemoveTaskCommand(taskId);
        var result = await mediator.Send(command);

        return result.Match(_ => NoContent(), Error);
    }
}