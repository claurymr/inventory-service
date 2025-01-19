using InventoryService.Domain;
using InventoryService.Domain.Enums;

namespace InventoryService.Application.Repositories;
/// <summary>
/// Interface for inventory repository operations.
/// </summary>
public interface IInventoryRepository
{
    /// <summary>
    /// Creates a new inventory record asynchronously.
    /// </summary>
    /// <param name="inventory">The inventory object to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unique identifier of the created inventory.</returns>
    Task<Guid> CreateInventoryAsync(Inventory inventory);

    /// <summary>
    /// Retrieves an inventory record by product ID asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the inventory record.</returns>
    Task<Inventory> GetInventoryByProductIdAsync(Guid id);

    /// <summary>
    /// Adjusts the inventory quantity asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the inventory.</param>
    /// <param name="actionType">The type of action to perform (e.g., add or remove).</param>
    /// <param name="quantity">The quantity to adjust.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated inventory and the old quantity.</returns>
    Task<(Inventory Inventory, int OldQuantity)> AdjustInventoryAsync(Guid id, ActionType actionType, int quantity);

    /// <summary>
    /// Resets the inventory quantity to its initial value asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the inventory.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated inventory and the old quantity.</returns>
    Task<(Inventory Inventory, int OldQuantity)> UpdateInventoryToInitialAsync(Guid id);

    /// <summary>
    /// Updates the product details in the inventory by its ID asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the inventory.</param>
    /// <param name="inventory">The inventory object with updated product details.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateProductInInventoryByIdAsync(Guid id, Inventory inventory);
}