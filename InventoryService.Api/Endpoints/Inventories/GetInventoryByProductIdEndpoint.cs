using FastEndpoints;
using InventoryService.Application.Contracts;
using InventoryService.Application.Inventories.GetInventories;
using InventoryService.Application.Mappings;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductService.Application.Contracts;

namespace InventoryService.Api.Endpoints.Inventories;
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