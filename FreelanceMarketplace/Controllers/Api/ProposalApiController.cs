using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        int currentUserId = 0;
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idClaim, out currentUserId) && currentUserId > 0)
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
    public async Task<IActionResult> Submit([FromForm] SubmitProposalViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Vui lòng đăng nhập để gửi báo giá" });
        }

        var (success, errorMessage) = await _proposalService.SubmitProposalAsync(model, userId.Value);
        if (!success)
        {
            return BadRequest(new { message = errorMessage });
        }

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Gửi báo giá thành công"
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Vui lòng đăng nhập." });
        }

        var proposal = await _proposalService.GetProposalForEditAsync(id, userId.Value);
        if (proposal == null)
        {
            return NotFound(new { message = "Bài chào giá không tồn tại hoặc bạn không có quyền truy cập." });
        }

        return Ok(proposal);
    }

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(int id, [FromForm] SubmitProposalViewModel model)
    {
        model.Id = id;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Vui lòng đăng nhập để chỉnh sửa báo giá" });
        }

        var (success, errorMessage) = await _proposalService.UpdateProposalAsync(model, userId.Value);
        if (!success)
        {
            return BadRequest(new { message = errorMessage });
        }

        return Ok(new { message = "Cập nhật báo giá thành công" });
    }
}