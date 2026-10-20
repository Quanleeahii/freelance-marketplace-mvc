# 📖 TÀI LIỆU KỸ THUẬT: HỆ THỐNG FREELANCE MARKETPLACE

## 1. Giới thiệu chung
Tài liệu này cung cấp kiến trúc kỹ thuật và hướng dẫn cài đặt, khởi chạy dự án **FreelanceMarketplace**.
- **Mục tiêu dự án:** Xây dựng nền tảng giao dịch việc làm, kết nối người có nhu cầu thuê nhân sự (Client) và người lao động tự do (Freelancer).
- **Vị trí mã nguồn:** Toàn bộ code được lưu tại thư mục `freelance-marketplace-mvc`. Để xem mã nguồn và chạy dự án, vui lòng mở file Solution **`freelance-marketplace-mvc.slnx`** (Project chính: `FreelanceMarketplace/FreelanceMarketplace.csproj`).

## 2. Công nghệ sử dụng

| Thành phần | Công nghệ | Phiên bản |
| :--- | :--- | :--- |
| **Framework backend** | ASP.NET Core MVC (.NET) | NET 10.0 |
| **Frontend** | Razor View (.cshtml), Bootstrap 5, jQuery, jQuery Validation | — |
| **Cơ sở dữ liệu** | PostgreSQL | 17 (alpine), chạy trong Docker |
| **Driver CSDL** | Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.3 |
| **ORM** | Entity Framework Core (Code First) | 10.0.12 |
| **Băm mật khẩu** | BCrypt.Net-Next | 4.2.0 |
| **Xác thực** | Cookie Authentication (thủ công) + Session | — |
| **Môi trường triển khai** | Docker Desktop, Docker Compose | — |
| **IDE** | Visual Studio 2022, pgAdmin 4 (v9.15) | — |

## 3. Kiến trúc & Cấu trúc mã nguồn

Dự án được thiết kế theo mô hình **MVC (Model-View-Controller)** kết hợp với **Service Layer** nhằm tách biệt phần xử lý logic nghiệp vụ khỏi giao diện, giúp dễ bảo trì và mở rộng. Cấu trúc mã nguồn chi tiết:

```text
freelance-marketplace-mvc/
├── FreelanceMarketplace.slnx
├── docker-compose.yml           # File cấu hình khởi chạy CSDL PostgreSQL
└── FreelanceMarketplace/
    ├── Program.cs               # File cấu hình gốc, đăng ký Dependency Injection
    ├── appsettings.json         # Chứa chuỗi kết nối CSDL và các cấu hình hệ thống
    ├── test.http                # File hỗ trợ test nhanh các endpoint API trên IDE
    ├── Areas/                   # Chứa các module/phân hệ độc lập 
    ├── Controllers/             # Chứa các MVC Controller (trả về View)
    ├── Controllers/Api/         # Chứa các API Controller (trả về JSON cho AJAX)
    ├── Services/                # Chứa toàn bộ Business Logic 
    ├── Data/                    # Chứa AppDbContext (EF Core) và DbInitializer
    ├── Models/                  # Các Entity ánh xạ với bảng trong CSDL
    ├── Enums/                   # Các kiểu dữ liệu liệt kê 
    ├── ViewModels/              # Dữ liệu giao tiếp với View 
    ├── Views/                   # Giao diện người dùng 
    ├── Migrations/              # Lưu trữ lịch sử các bản cập nhật CSDL 
    └── wwwroot/                 # Nơi lưu trữ file tĩnh
```

## 4. Các chức năng và Hệ thống Route

Dự án áp dụng kiến trúc phân tách rõ ràng giữa **Web MVC** (trả về giao diện) và **RESTful API** (trả về dữ liệu JSON) nhằm tối ưu hóa trải nghiệm người dùng thông qua gọi AJAX bất đồng bộ.

### 4.1. Web Routes (MVC Controllers)
Tiếp nhận Request từ người dùng và trả về giao diện HTML.

| Controller | Chức năng chính | Route hiển thị |
| :--- | :--- | :--- |
| **[Area] Admin** | Các Controller quản trị hệ thống (Duyệt job, quản lý User, Category...) | `/Admin/...` |
| `AccountController` | Đăng nhập, Đăng ký, Đăng xuất | `/login`, `/register` |
| `HomeController` | Trang chủ hệ thống | `/` |
| `JobController` | Form đăng tin, Xem chi tiết dự án | `/jobs/create`, `/jobs/{id}` |
| `JobSearchController` | Giao diện tìm kiếm việc làm | `/jobs` |
| `ProfileController` | Giao diện quản lý hồ sơ cá nhân | `/profile` |
| `ProposalController` | Giao diện quản lý các báo giá đã gửi/nhận | `/proposals` |

### 4.2. RESTful API Endpoints (Controllers/Api)
Tuân thủ các phương thức HTTP chuẩn (GET, POST, PUT), xử lý các thao tác bất đồng bộ (Client-side).

| API Controller | Endpoint gốc | Vai trò chính |
| :--- | :--- | :--- |
| `AuthApiController` | `/api/auth` | Kiểm tra tính hợp lệ của email khi đăng ký (Live Validation). |
| `JobPostingAPIController` | `/api/job-posting` | Xử lý logic đăng tin tuyển dụng, xác minh ngân sách dự án. |
| `JobSearchAPIController` | `/api/job-search` | Trả về danh sách việc làm (JSON), hỗ trợ bộ lọc trực tiếp không cần tải lại trang. |
| `ProposalApiController` | `/api/proposal` | Gửi báo giá mới (POST), Cập nhật trạng thái báo giá (PUT). |
| `UserProfileApiController`| `/api/user-profile` | Cập nhật Avatar và thông tin cá nhân (Freelancer/Client). |

## 5. Hướng dẫn cấu hình (appsettings.json)

Hệ thống quản lý cấu hình thông qua file `FreelanceMarketplace/appsettings.Development.json`. 

Trước khi chạy dự án, bạn cần đảm bảo chuỗi kết nối (`ConnectionStrings`) khớp với cấu hình của Docker:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5433;Database=nexthub_dev;Username=postgres;Password=nexthub_secret"
}
```

## 6. Hướng dẫn khởi tạo Cơ sở dữ liệu 

**Bước 1: Tải mã nguồn về máy**
Mở Terminal (hoặc Git Bash) và chạy lệnh sau:
```bash
git clone https://github.com/Quanleeahii/freelance-marketplace-mvc.git
cd freelance-marketplace-mvc
```

**Bước 2: Khởi chạy PostgreSQL bằng Docker**
Tại thư mục gốc của dự án, chạy lệnh:
```bash
docker compose up -d
```

**Bước 3: Khôi phục thư viện**
Di chuyển vào thư mục chứa project chính và tải lại các thư viện:
```bash
cd FreelanceMarketplace
dotnet restore
```

**Bước 4: Tạo bảng**
Chạy lệnh Migration để khởi tạo các bảng vào database:
```bash
dotnet ef database update
```

## 7. Hướng dẫn khởi chạy dự án

Chạy ứng dụng:

**Chạy bằng dòng lệnh CLI**
Đứng ở trong thư mục `FreelanceMarketplace`, gõ lệnh sau:
```bash
dotnet run
```

## 8. Tài khoản kiểm thử 

Khi chạy ứng dụng lần đầu, hệ thống (thông qua `DbInitializer`) sẽ tự động tạo sẵn một số dữ liệu danh mục và kỹ năng mẫu. 
Để thuận tiện cho quá trình kiểm thử các chức năng phân quyền, vui lòng sử dụng các tài khoản sau:

- **Tài khoản Admin**
  - Email: `admin@freelance.com` 
  - Mật khẩu: `Admin@123`

- **Tài khoản User**
  - Để trải nghiệm thực tế luồng phân quyền và đăng tin/nhận việc, vui lòng sử dụng chức năng Đăng ký (`/register`) trên giao diện trang chủ để tạo mới tài khoản với vai trò tương ứng.
