namespace ProductService.Application.Contracts
{
    public record InventoryResponse
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public int Quantity { get; init; }
    }
}
