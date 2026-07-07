namespace ServiceTemplate.Ports.Output;

public record Widget
(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
