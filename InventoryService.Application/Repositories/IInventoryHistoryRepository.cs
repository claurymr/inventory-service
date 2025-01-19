using InventoryService.Domain;

namespace InventoryService.Application.Repositories;
/// <summary>
/// Interface for inventory history repository.
/// </summary>
public interface IInventoryHistoryRepository
{
    /// <summary>
    /// Retrieves the inventory history for a specific product by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of inventory history records.</returns>
    Task<IEnumerable<InventoryHistory>> GetInventoryHistoryByProductIdAsync(Guid id);

    /// <summary>
    /// Creates a new inventory history record.
    /// </summary>
    /// <param name="inventoryHistory">The inventory history record to create.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CreateInventoryHistoryAsync(InventoryHistory inventoryHistory);
}