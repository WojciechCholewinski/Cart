using Cart.Application.Carts.Commands.AddProductToCart;
using Cart.Application.Carts.Commands.CheckoutCart;
using Cart.Application.Carts.Commands.CreateCart;
using Cart.Application.Carts.Commands.RemoveProductFromCart;
using Cart.Application.Carts.Queries.GetCart;
using Cart.Application.DTO.Carts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CartsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Tworzy koszyk dla użytkownika.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateCartResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateCartResponse>> Create(
        [FromBody] CreateCartRequest request,
        CancellationToken ct)
    {
        try
        {
            var cartId = await mediator.Send(new CreateCartCommand(request.UserId), ct);

            return CreatedAtAction(
                nameof(GetById),
                new { cartId },
                new CreateCartResponse(cartId));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Title = ex.Message });
        }
    }

    /// <summary>
    /// Dodaje produkt do koszyka lub zwiększa ilość, jeśli już istnieje.
    /// </summary>
    [HttpPost("{cartId:guid}/items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem(
        Guid cartId,
        [FromBody] AddCartItemRequest request,
        CancellationToken ct)
    {
        try
        {
            await mediator.Send(new AddProductToCartCommand(cartId, request.ProductId, request.Quantity), ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Title = ex.Message });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new ProblemDetails { Title = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // np. cart not active
            return Conflict(new ProblemDetails { Title = ex.Message });
        }
    }

    /// <summary>
    /// Zmniejsza ilość produktu w koszyku. Gdy spadnie do 0 - usuwa pozycję.
    /// </summary>
    [HttpDelete("{cartId:guid}/items/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveItem(
        Guid cartId,
        Guid productId,
        [FromQuery] int quantity,
        CancellationToken ct)
    {
        try
        {
            await mediator.Send(new RemoveProductFromCartCommand(cartId, productId, quantity), ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Title = ex.Message });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new ProblemDetails { Title = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails { Title = ex.Message });
        }
    }

    /// <summary>
    /// Podgląd koszyka - dociąga nazwę i cenę z ProductService przez HTTP.
    /// </summary>
    [HttpGet("{cartId:guid}")]
    [ProducesResponseType(typeof(CartViewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CartViewDto>> GetById(Guid cartId, CancellationToken ct)
    {
        try
        {
            var cart = await mediator.Send(new GetCartQuery(cartId), ct);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Title = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // np. produkt już nie istnieje w ProductService
            return Conflict(new ProblemDetails { Title = ex.Message });
        }
    }

    /// <summary>
    /// Checkout koszyka - tworzy zamówienie i zamyka koszyk.
    /// </summary>
    [HttpPost("{cartId:guid}/checkout")]
    [ProducesResponseType(typeof(CheckoutCartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CheckoutCartResponse>> Checkout(
        Guid cartId,
        [FromBody] CheckoutCartRequest request,
        CancellationToken ct)
    {
        try
        {
            var orderId = await mediator.Send(new CheckoutCartCommand(cartId, request.UserId), ct);
            return Ok(new CheckoutCartResponse(orderId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Title = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Title = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails { Title = ex.Message });
        }
    }

    // --------- Request/Response models ---------

    public sealed record CreateCartRequest(string UserId);
    public sealed record CreateCartResponse(Guid CartId);

    public sealed record AddCartItemRequest(Guid ProductId, int Quantity);

    public sealed record CheckoutCartRequest(string UserId);
    public sealed record CheckoutCartResponse(Guid OrderId);
}
