namespace ServiceTemplate.Ports.Output;

public record TaskItem
(
    Guid Id,
    string Title,
    DateTime CreatedAt
);
