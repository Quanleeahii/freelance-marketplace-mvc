using FreelanceMarketplace.Models;
using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreelanceMarketplace.Controllers;

[Route("jobs")]
public class JobSearchController : Controller
{
    private readonly IJobSearchService _jobSearchService;
    private readonly IJobService _jobService;
    public JobSearchController(
        IJobSearchService jobSearchService,
        IJobService jobService)
    {
        _jobSearchService = jobSearchService;
        _jobService = jobService;
    }

    [HttpGet("", Name = "JobList")]
    public async Task<IActionResult> Index([FromQuery] JobFilterRequest request)
    {
        var result = await _jobSearchService.GetJobsAsync(request);
        await LoadSidebarDataAsync(request.CategoryId);
        ViewBag.Filter = request;
        return View("~/Views/Job/SearchJob.cshtml", result);
    }

    [HttpGet("{id:int}", Name = "JobDetails")]
    public async Task<IActionResult> Details(int id)
    {
        var job = await _jobSearchService.GetJobByIdAsync(id);
        if (job == null)
        {
            return NotFound();
        }

        // 1. Lấy Id User đăng nhập (tương thích cả Claims và Session từ SignInService)
        int? currentUserId = GetCurrentUserId();

        // 2. Tìm proposal của chính user đó (nếu có)
        var proposalList = job.Proposals?.OrderByDescending(p => p.CreatedAt).ToList() ?? new List<Proposal>();
        Proposal? currentProposal = null;

        if (currentUserId.HasValue)
        {
            currentProposal = proposalList.FirstOrDefault(p => p.FreelancerProfile?.UserId == currentUserId.Value);
        }

        // 3. Đóng gói đầy đủ vào JobDetailViewModel cho View JobDetail.cshtml
        var viewModel = new JobDetailViewModel
        {
            Job = job,
            Proposals = proposalList,
            CurrentUserProposal = currentProposal,
            ProposalForm = new SubmitProposalRequest
            {
                JobId = job.Id,
                Id = currentProposal?.Id,
                BidAmount = currentProposal?.BidAmount ?? 0,
                DeliveryDays = currentProposal?.DeliveryDays ?? 0,
                PhoneNumber = currentProposal?.PhoneNumber ?? string.Empty,
                CoverLetter = currentProposal?.CoverLetter,
                CurrentAttachmentUrl = currentProposal?.AttachmentUrl
            }
        };

        return View("~/Views/Job/JobDetail.cshtml", viewModel);
    }

    private int? GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idClaim, out int uid) && uid > 0)
        {
            return uid;
        }

        var sessionUid = HttpContext.Session.GetString("UserId");
        if (int.TryParse(sessionUid, out uid) && uid > 0)
        {
            return uid;
        }

        return null;
    }

    private async Task LoadSidebarDataAsync(int? selectedCategoryId = null)
    {
        var parentCategories = await _jobService.GetParentCategoriesAsync();
        var skills = await _jobService.GetAllSkillsAsync();
        ViewBag.Categories = parentCategories;
        ViewBag.Skills = skills;
    }
}