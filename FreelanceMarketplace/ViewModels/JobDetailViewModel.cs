using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using FreelanceMarketplace.Models;

namespace FreelanceMarketplace.ViewModels;

public class JobDetailViewModel
{
    public Job Job { get; set; } = null!;
    public List<Proposal> Proposals { get; set; } = new();
    public Proposal? CurrentUserProposal { get; set; }

    public int TotalBids => Proposals.Count;
    public decimal MinBidAmount => Proposals.Any() ? Proposals.Min(p => p.BidAmount) : 0;
    public decimal MaxBidAmount => Proposals.Any() ? Proposals.Max(p => p.BidAmount) : 0;
    public decimal AvgBidAmount => Proposals.Any() ? Proposals.Average(p => p.BidAmount) : 0;
    public double AvgDeliveryDays => Proposals.Any() ? Math.Round(Proposals.Average(p => p.DeliveryDays), 1) : 0;

    public SubmitProposalRequest ProposalForm { get; set; } = new();
}

public class SubmitProposalRequest
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Dự án không hợp lệ")]
    public int JobId { get; set; }

    [Display(Name = "Chi phí đề xuất (VNĐ)")]
    [Required(ErrorMessage = "Vui lòng nhập mức chi phí bạn đề xuất")]
    [Range(100000, 1000000000, ErrorMessage = "Chi phí đề xuất tối thiểu 100.000 VNĐ")]
    public decimal BidAmount { get; set; }

    [Display(Name = "Dự kiến hoàn thành (ngày)")]
    [Required(ErrorMessage = "Vui lòng chọn thời gian hoàn thành")]
    [Range(1, int.MaxValue, ErrorMessage = "Thời gian hoàn thành phải lớn hơn 0 ngày")]
    public int DeliveryDays { get; set; }

    [Display(Name = "Đề xuất thuyết phục")]
    [MinLength(20, ErrorMessage = "Nội dung đề xuất quá ngắn")]
    [MaxLength(2000, ErrorMessage = "Nội dung đề xuất quá dài")]
    public string? CoverLetter { get; set; }

    [Display(Name = "Tài liệu đính kèm")]
    public IFormFile? AttachmentFile { get; set; }
    public string? CurrentAttachmentUrl { get; set; }
    public bool RemoveCurrentAttachment { get; set; } = false;

    [Display(Name = "Số điện thoại liên hệ")]
    [Required(ErrorMessage = "Vui lòng cung cấp số điện thoại liên hệ")]
    [RegularExpression(@"^0[35789]\d{8}$", ErrorMessage = "Số điện thoại Việt Nam không hợp lệ (gồm 10 số, bắt đầu bằng 03, 05, 07, 08, 09)")]
    public string PhoneNumber { get; set; } = string.Empty;
}