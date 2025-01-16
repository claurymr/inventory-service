using FastEndpoints;
using InventoryService.Application.Contracts;
using InventoryService.Application.Inventories.AdjustInventories;
using InventoryService.Application.Mappings;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InventoryService.Api.Endpoints.Inventories;
public class AdjustInventoryExitEndpoint(IMediator mediator)
    : Endpoint<AdjustInventoryExitCommand, Results<NoContent, BadRequest<ValidationFailureResponse>, NotFound<OperationFailureResponse>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Put("/inventories/products/{productId}/exit");

        Options(x =>
        {
            x.RequireAuthorization();
            x.WithDisplayName("Adjust Inventory Exit");
            x.Produces<NoContent>(StatusCodes.Status204NoContent);
            x.Produces<BadRequest<ValidationFailureResponse>>(StatusCodes.Status400BadRequest);
            x.Produces<NotFound<OperationFailureResponse>>(StatusCodes.Status404NotFound);
            x.Accepts<AdjustInventoryExitCommand>();
            x.WithOpenApi();
        });
    }

    public override async Task<Results<NoContent, BadRequest<ValidationFailureResponse>, NotFound<OperationFailureResponse>>>
        ExecuteAsync(AdjustInventoryExitCommand req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
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