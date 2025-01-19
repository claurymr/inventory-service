using FastEndpoints;
using InventoryService.Application.Contracts;
using InventoryService.Application.InventoryHistories.GetInventoryHistories;
using InventoryService.Application.Mappings;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InventoryService.Api.Endpoints.InventoryHistories;
/// <summary>
/// Endpoint to get the inventory history by product ID.
/// </summary>
/// <param name="mediator">The mediator instance for sending queries.</param>
/// <response code="200">Returns a list of inventory history responses.</response>
/// <response code="403">If the user is not authorized.</response>
/// <response code="401">If the user is not authenticated.</response>
/// <response code="500">If there is an internal server error.</response>
/// <returns>A result indicating the outcome of the operation.</returns>
public class GetInventoryHistoryByProductIdEndpoint(IMediator mediator)
: Endpoint<GetInventoryHistoryByProductIdQuery, Results<Ok<IEnumerable<InventoryHistoryResponse>>, JsonHttpResult<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Verbs(Http.GET);
        Get("/inventoryhistories/products/{productId}");

        Options(x =>
        {
            x.RequireAuthorization("AdminOrUser");
            x.WithDisplayName("Get Inventory History By Product Id");
            x.Produces<Ok<IEnumerable<InventoryHistoryResponse>>>(StatusCodes.Status200OK);
            x.Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden);
            x.Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized);
            x.Accepts<GetInventoryHistoryByProductIdQuery>();
            x.WithOpenApi();
        });
    }

    public override async Task<Results<Ok<IEnumerable<InventoryHistoryResponse>>, JsonHttpResult<OperationFailureResponse>>>
        ExecuteAsync(GetInventoryHistoryByProductIdQuery req, CancellationToken ct)
    {
        var newReq = req with { ProductId = Route<Guid>("productId") };
        var result = await _mediator.Send(newReq, ct);
        var response = result.Match<IResult>(
                        inventoryHistories => TypedResults.Ok(inventoryHistories),
                        failed => TypedResults.Json(failed.MapToResponse(), statusCode: StatusCodes.Status500InternalServerError));
        return response switch
        {
            Ok<IEnumerable<InventoryHistoryResponse>> success => success,
            JsonHttpResult<OperationFailureResponse> badRequest => badRequest,
            _ => throw new Exception()
        };
    }
}