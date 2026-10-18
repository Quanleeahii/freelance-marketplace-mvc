using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FreelanceMarketplace.Models;

namespace FreelanceMarketplace.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Dùng ViewBag truyền dữ liệu sang View, hoàn toàn không dính lỗi thuộc tính Model
            ViewBag.JobId = 94521;
            ViewBag.JobTitle = "Tuyển Leader Team Marketing cho dự án Bán hàng online sản phẩm đồ ăn cao cấp";
            ViewBag.CategoryName = "Marketing & Bán hàng Online";
            ViewBag.LocationCity = "TP. Hồ Chí Minh";
            ViewBag.WorkMode = "Làm Online";
            ViewBag.WorkType = "Bán thời gian";
            ViewBag.BudgetMin = "3.000.000đ";
            ViewBag.BudgetMax = "8.000.000đ";
            ViewBag.Deadline = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
            ViewBag.CreatedAt = "2 giờ trước";
            ViewBag.ClientName = "Lê Thị Oanh";
            ViewBag.ClientAddress = "Hà Nội";
            ViewBag.ClientJoined = "10/04/2024";
            ViewBag.Skills = new List<string> { "Bán hàng", "Marketing Lan Truyền", "TikTok Shop", "Shopee", "Digital Marketing", "Zalo OA" };
            ViewBag.Description = @"<p><strong>NextHub Client</strong> cần tuyển đội Marketing mới tại TP. Hồ Chí Minh gồm <strong>1 Trưởng nhóm (Leader)</strong> và <strong>2 Sinh viên năm 3–4</strong>, để triển khai kế hoạch marketing mới, tập trung kênh online và định vị phân khúc cao cấp.</p>
<p><strong>Thông tin sản phẩm:</strong> Thương hiệu ngô nếp Mochi (trắng và tím) từ giống cổ truyền Nhật Bản, hấp chín và hút chân không. Sản phẩm không biến đổi gen (Non-GMO), không chất phụ gia bảo quản.</p>
<p><strong>Nơi làm việc & Kênh triển khai:</strong></p>
<ul>
    <li>Làm việc Online / Remote, yêu cầu ứng viên ưu tiên tại khu vực miền Nam.</li>
    <li>Kênh cần xây dựng: Website, Facebook, TikTok & TikTok Shop, Shopee, Lazada, YouTube, Instagram, Threads, Zalo OA.</li>
    <li>Định vị: Thực phẩm sạch cao cấp, quà tặng gia đình và doanh nghiệp.</li>
    <li>Khách hàng mục tiêu: Chủ yếu phụ nữ 30–50 tuổi, thu nhập khá trở lên.</li>
</ul>";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}