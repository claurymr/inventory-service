using FastEndpoints;
using InventoryService.Application.Contracts;
using InventoryService.Application.Inventories.GetInventories;
using InventoryService.Application.Mappings;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InventoryService.Api.Endpoints.Inventories;
/// <summary>
/// Endpoint to get inventory details by product ID.
/// </summary>
/// <param name="mediator">The mediator instance for sending queries.</param>
/// <response code="200">Returns the inventory details.</response>
/// <response code="404">Returns an error response if the inventory is not found.</response>
/// <response code="403">Returns a forbidden response if the user is not authorized.</response>
/// <response code="401">Returns an unauthorized response if the user is not authenticated.</response>
/// <returns>A result indicating the outcome of the operation.</returns>
public class GetInventoryByProductIdEndpoint(IMediator mediator)
    : Endpoint<GetInventoryByProductIdQuery, Results<Ok<InventoryResponse>, NotFound<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Verbs(Http.GET);
        Get("/inventories/products/{productId}");

        Options(x =>
        {
            x.RequireAuthorization("AdminOrUser");
            x.WithDisplayName("Get Inventory By Product Id");
            x.Produces<Ok<InventoryResponse>>(StatusCodes.Status200OK);
            x.Produces<NotFound<OperationFailureResponse>>(StatusCodes.Status404NotFound);
            x.Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden);
            x.Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized);
            x.Accepts<GetInventoryByProductIdQuery>();
            x.WithOpenApi();
        });
    }

    public override async Task<Results<Ok<InventoryResponse>, NotFound<OperationFailureResponse>>>
        ExecuteAsync(GetInventoryByProductIdQuery req, CancellationToken ct)
    {
        var newReq = req with { Id = Route<Guid>("productId") };
        var result = await _mediator.Send(newReq, ct);
        var response = result.Match<IResult>(
                        productResponse => TypedResults.Ok(productResponse),
                        notFound => TypedResults.NotFound(notFound.MapToResponse()));
        return response switch
        {
            Ok<InventoryResponse> success => success,
            NotFound<OperationFailureResponse> notFound => notFound,
            _ => throw new Exception()
        };
    }
}