using FastEndpoints;
using InventoryService.Application.Contracts;
using InventoryService.Application.InventoryHistories.GetInventoryHistories;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InventoryService.Api.Endpoints.InventoryHistories;
public class GetInventoryHistoryByProductIdEndpoint(IMediator mediator) : Endpoint<GetInventoryHistoryByProductIdQuery>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get("/inventorieshistories/{productId}");

        Options(x =>
        {
            x.RequireAuthorization();
            x.WithDisplayName("Get Inventory History By Product Id");
            x.Produces<Ok<IEnumerable<InventoryHistoryResponse>>>(StatusCodes.Status200OK);
            x.Accepts<GetInventoryHistoryByProductIdQuery>();
            x.WithOpenApi();
        });
    }

    public override async Task HandleAsync(GetInventoryHistoryByProductIdQuery req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        await SendOkAsync(result, ct);
    }
}