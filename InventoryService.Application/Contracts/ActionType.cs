namespace InventoryService.Application.Contracts;


/// <summary>
/// Represents the type of action that can be performed in the inventory service.
/// </summary>
public enum ActionType
{
    /// <summary>
    /// Represents an entry action in the inventory.
    /// </summary>
    Entry,

    /// <summary>
    /// Represents an exit action in the inventory.
    /// </summary>
    Exit
}