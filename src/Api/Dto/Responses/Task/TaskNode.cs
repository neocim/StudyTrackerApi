namespace Api.Dto.Responses.Task;

public record TaskNodeResponse(
    Guid Id,
    Guid OwnerId,
    Guid? ParentId,
    ICollection<TaskNodeResponse> SubTasks,
    DateOnly BeginDate,
    DateOnly EndDate,
    string Name,
    string? Description,
    bool? Success);