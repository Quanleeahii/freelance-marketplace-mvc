/**
 * profile.js - Xử lý tương tác giao diện và sự kiện người dùng cho trang Profile
 */

document.addEventListener("DOMContentLoaded", function () {
    // 1. Tham chiếu các phần tử HTML
    const avatarFileInput = document.getElementById("avatarFileInput");
    const avatarUrlInput = document.getElementById("avatarUrlInput");
    const avatarPreview = document.getElementById("avatarPreview");
    const summaryAvatar = document.getElementById("summaryAvatar");
    const btnResetAvatar = document.getElementById("btnResetAvatar");
    const btnCancel = document.getElementById("btnCancel");
    const profileForm = document.getElementById("profileForm");

    // Lưu lại ảnh gốc ban đầu
    const defaultAvatarUrl = "https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?auto=format&fit=crop&w=300&q=80";
    const initialAvatarSrc = (avatarPreview && avatarPreview.src) ? avatarPreview.src : defaultAvatarUrl;

    // 2. Xử lý xem trước ảnh khi người dùng chọn file từ máy tính
    if (avatarFileInput) {
        avatarFileInput.addEventListener("change", function (e) {
            const file = e.target.files[0];
            if (file) {
                // Kiểm tra dung lượng file (tối đa 2MB)
                if (file.size > 2 * 1024 * 1024) {
                    alert("Kích thước file ảnh không được vượt quá 2MB!");
                    avatarFileInput.value = "";
                    return;
                }

                const reader = new FileReader();
                reader.onload = function (event) {
                    const result = event.target.result;
                    if (avatarPreview) avatarPreview.src = result;
                    if (summaryAvatar) summaryAvatar.src = result;
                };
                reader.readAsDataURL(file);
            }
        });
    }

    // 3. Xử lý xem trước ảnh khi người dùng dán hoặc nhập URL trực tiếp
    if (avatarUrlInput) {
        avatarUrlInput.addEventListener("input", function () {
            const url = avatarUrlInput.value.trim();
            if (url) {
                if (avatarPreview) avatarPreview.src = url;
                if (summaryAvatar) summaryAvatar.src = url;
            }
        });
    }

    // 4. Xử lý nút khôi phục ảnh gốc
    if (btnResetAvatar) {
        btnResetAvatar.addEventListener("click", function () {
            if (avatarFileInput) avatarFileInput.value = "";
            if (avatarUrlInput) avatarUrlInput.value = initialAvatarSrc;
            if (avatarPreview) avatarPreview.src = initialAvatarSrc;
            if (summaryAvatar) summaryAvatar.src = initialAvatarSrc;
        });
    }

    // 5. Xử lý nút Hủy bỏ (Reset lại form về dữ liệu ban đầu)
    if (btnCancel && profileForm) {
        btnCancel.addEventListener("click", function (e) {
            if (confirm("Bạn có chắc chắn muốn hủy bỏ các thay đổi chưa lưu?")) {
                profileForm.reset();
                if (avatarPreview) avatarPreview.src = initialAvatarSrc;
                if (summaryAvatar) summaryAvatar.src = initialAvatarSrc;
            } else {
                e.preventDefault();
            }
        });
    }

    // 6. Xử lý chuyển đổi Tab trên thanh điều hướng ngang
    const tabs = document.querySelectorAll(".profile-tabs .nav-link");
    tabs.forEach(tab => {
        tab.addEventListener("click", function (e) {
            e.preventDefault();
            tabs.forEach(t => t.classList.remove("active"));
            this.classList.add("active");
        });
    });

    // 7. Xử lý chuyển đổi liên kết trong Menu Sidebar bên trái
    const sidebarItems = document.querySelectorAll(".profile-sidebar .list-group-item-action");
    sidebarItems.forEach(item => {
        item.addEventListener("click", function (e) {
            e.preventDefault();
            document.querySelectorAll(".profile-sidebar .list-group-item").forEach(i => i.classList.remove("active"));
            this.classList.add("active");
        });
    });
});
