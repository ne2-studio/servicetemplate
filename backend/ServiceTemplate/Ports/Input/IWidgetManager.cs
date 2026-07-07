using CSharpFunctionalExtensions;

namespace ServiceTemplate.Ports.Input;

public interface IWidgetManager
{
    /// <summary>
    /// Creates a new widget with the given name.
    /// </summary>
    /// <param name="name">The unique name for the widget.</param>
    /// <returns>A result containing the created widget if successful, or an error message if the name is already taken.</returns>
    Task<Result<WidgetDto>> CreateAsync(string name);

    /// <summary>
    /// Retrieves a widget by its id.
    /// </summary>
    /// <param name="id">The unique identifier for the widget.</param>
    /// <returns>A result containing the widget if it exists, or an error message if it does not.</returns>
    Task<Result<WidgetDto>> GetAsync(Guid id);

    /// <summary>
    /// Lists all widgets with pagination support.
    /// </summary>
    /// <param name="skip">The number of items to skip (for pagination).</param>
    /// <param name="take">The maximum number of items to return (for pagination).</param>
    /// <returns>A result containing a collection of widgets.</returns>
    Task<Result<IEnumerable<WidgetDto>>> ListAsync(int skip = 0, int take = 10);

    /// <summary>
    /// Renames an existing widget identified by its id.
    /// </summary>
    /// <param name="id">The unique identifier for the widget to update.</param>
    /// <param name="name">The new name for the widget.</param>
    /// <returns>A result indicating success or an error message if the id does not exist or the name is taken.</returns>
    Task<Result> UpdateAsync(Guid id, string name);

    /// <summary>
    /// Deletes a widget identified by its id.
    /// </summary>
    /// <param name="id">The unique identifier for the widget to delete.</param>
    /// <returns>A result indicating success or an error message if the id does not exist.</returns>
    Task<Result> DeleteAsync(Guid id);
}
