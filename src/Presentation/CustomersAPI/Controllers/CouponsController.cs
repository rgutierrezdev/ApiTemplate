using ApiTemplate.Application.Features.Coupons;
using ApiTemplate.Application.Features.Coupons.Dtos;

namespace ApiTemplate.CustomersAPI.Controllers;

public class CouponsController : BaseApiController
{
    [HttpGet("page")]
    [AuthPermission(Permissions.CouponsRead)]
    public Task<PaginationResponse<PageCouponDto>> PageAsync(
        [FromQuery] GetCouponsPage.Request request,
        CancellationToken cancellationToken)
    {
        return Mediator.Send(request, cancellationToken);
    }

    [HttpGet("{id:guid}")]
    [AuthPermission(Permissions.CouponsRead)]
    [OperationErrors(ErrorCodes.CouponNotFound)]
    public Task<CouponDto> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetCoupon.Request(id), cancellationToken);
    }

    [HttpGet("{id:guid}/users")]
    [AuthPermission(Permissions.CouponsRead)]
    [OperationErrors(ErrorCodes.CouponNotFound)]
    public Task<List<CouponUserDto>> GetUsersAsync(Guid id, CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetCouponUsers.Request(id), cancellationToken);
    }

    [HttpPost("")]
    [AuthPermission(Permissions.CouponsRead)]
    [OperationErrors(ErrorCodes.CouponNotFound, ErrorCodes.CouponAlreadyApplied)]
    public Task<Guid> SaveAsync(SaveCoupon.Request request, CancellationToken cancellationToken)
    {
        return Mediator.Send(request, cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    [AuthPermission(Permissions.CouponsWrite)]
    [OperationErrors(ErrorCodes.CouponNotFound, ErrorCodes.CouponAlreadyApplied)]
    public Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return Mediator.Send(new DeleteCoupon.Request(id), cancellationToken);
    }

    [HttpPost("{id:guid}/disable")]
    [AuthPermission(Permissions.CouponsWrite)]
    [OperationErrors(ErrorCodes.CouponNotFound)]
    public Task<Guid> DisabledAsync(Guid id, CancellationToken cancellationToken)
    {
        return Mediator.Send(new DisableCoupon.Request(id), cancellationToken);
    }
}
