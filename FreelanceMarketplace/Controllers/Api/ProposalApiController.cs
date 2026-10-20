using System.Security.Claims;
using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Controllers.Api;

[ApiController]
[Route("api/proposals")]
public class ProposalApiController : ControllerBase
{
    private readonly IProposalService _proposalService;

    public ProposalApiController(IProposalService proposalService)
    {
        _proposalService = proposalService;
    }

    private int? GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idClaim, out int currentUserId) && currentUserId > 0)
        {
            return currentUserId;
        }

        var sessionUserId = HttpContext.Session.GetString("UserId");
        if (int.TryParse(sessionUserId, out currentUserId) && currentUserId > 0)
        {
            return currentUserId;
        }

        return null;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Submit([FromForm] SubmitProposalRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState
            });
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "Vui lòng đăng nhập để gửi báo giá"
            });
        }

        var (success, errorMessage) = await _proposalService.SubmitProposalAsync(model, userId.Value);
        if (!success)
        {
            return BadRequest(new
            {
                success = false,
                message = errorMessage
            });
        }

        return StatusCode(StatusCodes.Status201Created, new
        {
            success = true,
            message = "Gửi báo giá thành công"
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "Vui lòng đăng nhập"
            });
        }

        var proposal = await _proposalService.GetProposalForEditAsync(id, userId.Value);
        if (proposal == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Bài chào giá không tồn tại hoặc bạn không có quyền truy cập"
            });
        }

        return Ok(new
        {
            success = true,
            data = proposal
        });
    }

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(int id, [FromForm] SubmitProposalRequest model)
    {
        model.Id = id;

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errors = ModelState
            });
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "Vui lòng đăng nhập để chỉnh sửa báo giá"
            });
        }

        var (success, errorMessage) = await _proposalService.UpdateProposalAsync(model, userId.Value);
        if (!success)
        {
            return BadRequest(new
            {
                success = false,
                message = errorMessage
            });
        }

        return Ok(new
        {
            success = true,
            message = "Cập nhật báo giá thành công"
        });
    }
}