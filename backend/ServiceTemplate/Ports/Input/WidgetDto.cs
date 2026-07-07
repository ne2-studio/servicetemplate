namespace ServiceTemplate.Ports.Input;

public record WidgetDto
(
    string Id,
    string Name,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
