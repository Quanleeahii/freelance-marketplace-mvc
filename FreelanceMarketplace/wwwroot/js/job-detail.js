$(document).ready(function () {
    // Hàm format VNĐ
    function formatVND(amount) {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
    }

    // Hàm tính toán lại Thanh Thống Kê vLance động
    function recalculateStats() {
        let prices = [];
        let daysList = [];

        $(".bid-item-card").each(function () {
            const price = parseFloat($(this).attr("data-price"));
            const days = parseFloat($(this).attr("data-days"));

            if (!isNaN(price)) prices.push(price);
            if (!isNaN(days)) daysList.push(days);
        });

        const totalBids = prices.length;
        $("#bidCount").text(totalBids);

        if (totalBids > 0) {
            const minPrice = Math.min(...prices);
            const maxPrice = Math.max(...prices);
            const sumPrice = prices.reduce((a, b) => a + b, 0);
            const avgPrice = Math.round(sumPrice / totalBids);

            const sumDays = daysList.reduce((a, b) => a + b, 0);
            const avgDays = Math.round(sumDays / totalBids);

            $("#statMinPrice").text(formatVND(minPrice));
            $("#statMaxPrice").text(formatVND(maxPrice));
            $("#statAvgPrice").text(formatVND(avgPrice));
            $("#statAvgDays").text(avgDays + " ngày");
        }
    }

    // Chạy tính toán ban đầu
    recalculateStats();

    // Format tiền VNĐ khi gõ giá đề xuất
    $("#BidAmount").on("input", function () {
        const val = $(this).val();
        if (val && val > 0) {
            $("#bidFormattedText").text("Bằng chữ: " + formatVND(val));
        } else {
            $("#bidFormattedText").text("");
        }
    });

    // Xử lý Upload File Custom
    const dropzone = $("#fileUploadDropzone");
    const fileInput = $("#AttachmentFile");

    dropzone.on("click", function () { fileInput.click(); });

    fileInput.on("change", function () {
        if (this.files && this.files.length > 0) {
            $("#selectedFileNameText").text(this.files[0].name);
            $("#uploadBoxPlaceholder").hide();
            $("#uploadFileSelectedInfo").show();
        }
    });

    $("#btnRemoveSelectedFile").on("click", function (e) {
        e.stopPropagation();
        fileInput.val("");
        $("#uploadFileSelectedInfo").hide();
        $("#uploadBoxPlaceholder").show();
    });

    // Drag & Drop
    dropzone.on("dragover", function (e) { e.preventDefault(); $(this).addClass("dragover"); });
    dropzone.on("dragleave drop", function (e) {
        e.preventDefault();
        $(this).removeClass("dragover");
        if (e.type === "drop" && e.originalEvent.dataTransfer.files.length > 0) {
            fileInput[0].files = e.originalEvent.dataTransfer.files;
            fileInput.trigger("change");
        }
    });

    // XỬ LÝ NỘP FORM CHÀO GIÁ
    $("#bidForm").on("submit", function (e) {
        e.preventDefault();

        const amount = parseFloat($("#BidAmount").val());
        const days = parseInt($("#EstimatedDays").val());
        const phone = $("#PhoneNumber").val();
        const proposal = $("#ProposalText").val();
        const files = fileInput[0].files;
        const fileName = files.length > 0 ? files[0].name : "Không có file";

        const formattedAmount = formatVND(amount);

        // 1. Cập nhật Sidebar Cột Phải
        $("#dispBidAmount").text(formattedAmount);
        $("#dispEstimatedDays").text(days + " ngày");
        $("#dispPhoneNumber").text(phone);
        $("#dispFileName").text(fileName);
        $("#dispProposalText").text(proposal);

        // 2. Ẩn form nhập, hiện thẻ Đã Chào Giá
        $("#bidFormContainer").slideUp(300);
        $("#submittedBidBox").slideDown(300);

        // 3. Tạo Card bài nộp của bạn với data-price và data-days động
        const myBidHtml = `
            <div class="bid-item-card border-success bg-light" id="myBidCard" data-price="${amount}" data-days="${days}">
                <div class="d-flex align-items-start gap-3">
                    <img src="https://ui-avatars.com/api/?name=Nguyen+Quoc+Truong&background=00b14f&color=fff&size=64" class="freelancer-avatar" alt="Avatar" />
                    <div class="flex-grow-1">
                        <div class="d-flex justify-content-between align-items-center mb-1">
                            <h6 class="freelancer-name mb-0">
                                Nguyễn Quốc Trường <span class="badge bg-success text-white ms-2"><i class="bi bi-star-fill"></i> Báo giá của bạn</span>
                            </h6>
                            <span class="bid-price-tag text-success">${formattedAmount}</span>
                        </div>
                        <p class="freelancer-headline text-muted small mb-2">Lập trình viên ASP.NET Core / Digital Marketer</p>
                        <div class="d-flex align-items-center gap-3 small text-muted mb-2">
                            <span><i class="bi bi-telephone-fill text-success"></i> SĐT: <strong>${phone}</strong></span>
                            <span><i class="bi bi-clock me-1"></i> Hoàn thành trong <strong>${days} ngày</strong></span>
                            <span><i class="bi bi-paperclip me-1"></i> File: <strong>${fileName}</strong></span>
                        </div>
                        <div class="bid-proposal-preview text-secondary small">
                            "${proposal}"
                        </div>
                    </div>
                </div>
            </div>
        `;

        if ($("#myBidCard").length > 0) {
            $("#myBidCard").replaceWith(myBidHtml);
        } else {
            $("#bidsList").prepend(myBidHtml);
        }

        // 4. TỰ ĐỘNG TÍNH TOÁN LẠI THANH THỐNG KÊ GIÁ VÀ NGÀY
        recalculateStats();

        // Cuộn xuống bài vừa nộp
        $('html, body').animate({
            scrollTop: $("#myBidCard").offset().top - 80
        }, 500);
    });

    // Nút chỉnh sửa báo giá
    $(document).on("click", ".btn-trigger-edit-bid", function () {
        $("#submittedBidBox").slideUp(300);
        $("#bidFormContainer").slideDown(300);

        $('html, body').animate({
            scrollTop: $("#bidFormBlock").offset().top - 40
        }, 400);
    });
});