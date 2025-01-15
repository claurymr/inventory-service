namespace InventoryService.Application.Contracts;
public record InventoryHistoryResponse
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public Guid InventoryId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal OldQuantity { get; init; }
    public decimal NewQuantity { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}