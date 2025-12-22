namespace Application.Dto.Task.ReadModels;

public record TaskNodeReadModel(
    Guid Id,
    Guid OwnerId,
    Guid? ParentId,
    ICollection<TaskNodeReadModel> SubTasks,
    DateOnly BeginDate,
    DateOnly EndDate,
    string Name,
    string? Description,
    bool? Success);