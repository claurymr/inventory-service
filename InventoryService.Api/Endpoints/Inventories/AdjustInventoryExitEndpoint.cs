using FastEndpoints;
using InventoryService.Application.Contracts;
using InventoryService.Application.Inventories.AdjustInventories;
using InventoryService.Application.Mappings;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InventoryService.Api.Endpoints.Inventories;
/// <summary>
/// Endpoint to adjust the inventory exit for a specific product.
/// </summary>
/// <param name="mediator">The mediator instance used to send commands.</param>
/// <response code="204">The inventory exit was successfully adjusted.</response>
/// <response code="400">The request was invalid, typically due to validation failures.</response>
/// <response code="404">The specified product was not found.</response>
/// <response code="403">The user is not authorized to perform this action.</response>
/// <response code="401">The user is not authenticated.</response>
/// <returns>A result indicating the outcome of the operation.</returns>
public class AdjustInventoryExitEndpoint(IMediator mediator)
    : Endpoint<AdjustInventoryExitCommand, Results<NoContent, BadRequest<ValidationFailureResponse>, NotFound<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Verbs(Http.PUT);
        Put("/inventories/products/{productId}/exit");

        Options(x =>
        {
            x.RequireAuthorization("AdminOnly");
            x.WithDisplayName("Adjust Inventory Exit");
            x.Produces<NoContent>(StatusCodes.Status204NoContent);
            x.Produces<BadRequest<ValidationFailureResponse>>(StatusCodes.Status400BadRequest);
            x.Produces<NotFound<OperationFailureResponse>>(StatusCodes.Status404NotFound);
            x.Produces<ForbidHttpResult>(StatusCodes.Status403Forbidden);
            x.Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized);
            x.Accepts<AdjustInventoryExitCommand>();
            x.WithOpenApi();
        });
    }

    
    public override async Task<Results<NoContent, BadRequest<ValidationFailureResponse>, NotFound<OperationFailureResponse>>>
        ExecuteAsync(AdjustInventoryExitCommand req, CancellationToken ct)
    {
        var newReq = req with { ProductId = Route<Guid>("productId") };
        var result = await _mediator.Send(newReq, ct);
        var response = result
            .Match<IResult>(
            noContent => TypedResults.NoContent(),
            failed => TypedResults.BadRequest(failed.MapToResponse()),
            notFound => TypedResults.NotFound(notFound.MapToResponse())
        );
        return response switch
        {
            NoContent noContent => noContent,
            BadRequest<ValidationFailureResponse> badRequest => badRequest,
            NotFound<OperationFailureResponse> notFound => notFound,
            _ => throw new Exception()
        };
    }
}