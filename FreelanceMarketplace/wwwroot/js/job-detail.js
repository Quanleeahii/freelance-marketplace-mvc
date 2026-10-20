$(document).ready(function () {
    const numberFormatter = new Intl.NumberFormat('vi-VN');

    function formatVND(amount) {
        return numberFormatter.format(amount) + " VNĐ";
    }

    function recalculateStats() {
        let prices = [];
        let daysList = [];

        $(".bid-item-card:not(.d-none)").each(function () {
            const price = parseFloat($(this).attr("data-price"));
            const days = parseFloat($(this).attr("data-days"));

            if (!isNaN(price) && price > 0) prices.push(price);
            if (!isNaN(days) && days > 0) daysList.push(days);
        });

        const totalBids = prices.length;
        $("#bidCount").text(totalBids);

        if (totalBids > 0) {
            const minPrice = Math.min(...prices);
            const maxPrice = Math.max(...prices);
            const sumPrice = prices.reduce((a, b) => a + b, 0);
            const avgPrice = Math.round(sumPrice / totalBids);

            const sumDays = daysList.reduce((a, b) => a + b, 0);
            const avgDays = Math.round((sumDays / totalBids) * 10) / 10;

            $("#statMinPrice").text(formatVND(minPrice));
            $("#statMaxPrice").text(formatVND(maxPrice));
            $("#statAvgPrice").text(formatVND(avgPrice));
            $("#statAvgDays").text(avgDays + " ngày");
        } else {
            $("#statMinPrice").text("0 VNĐ");
            $("#statMaxPrice").text("0 VNĐ");
            $("#statAvgPrice").text("0 VNĐ");
            $("#statAvgDays").text("0 ngày");
        }
    }

    $("#BidAmount").on("input", function () {
        const val = parseFloat($(this).val());
        if (!isNaN(val) && val > 0) {
            $("#bidFormattedText").text("Bằng chữ: " + formatVND(val));
        } else {
            $("#bidFormattedText").text("");
        }
    });

    const fileInput = $("#AttachmentFile");

    $("#btnTriggerFile").on("click", function () {
        fileInput.click();
    });

    fileInput.on("change", function () {
        if (this.files && this.files.length > 0) {
            $("#selectedFileNameText").text(this.files[0].name);
            $("#fileSelectedInfo").removeClass("d-none").addClass("d-flex");
        }
    });

    $("#btnRemoveSelectedFile").on("click", function (e) {
        e.preventDefault();
        fileInput.val("");
        $("#fileSelectedInfo").removeClass("d-flex").addClass("d-none");
    });

    $("#bidForm").on("submit", function (e) {
        e.preventDefault();

        const alertBox = $("#proposalAlertMessage");
        alertBox.addClass("d-none").text("");

        const formData = new FormData(this);
        const proposalId = parseInt($("#ProposalId").val()) || 0;
        const isUpdate = proposalId > 0;

        const endpoint = isUpdate ? `/api/proposals/${proposalId}` : `/api/proposals`;
        const methodType = isUpdate ? "PUT" : "POST";

        const amount = parseFloat($("#BidAmount").val());
        const days = parseInt($("#DeliveryDays").val());
        const phone = $("#PhoneNumber").val();
        const coverLetter = $("#CoverLetter").val() || "";
        const files = fileInput[0].files;
        const fileName = files.length > 0 ? files[0].name : ($("#selectedFileNameText").text().trim() || "Không có");

        const submitBtn = $("#btnSubmitProposal");
        submitBtn.prop("disabled", true);

        $.ajax({
            url: endpoint,
            type: methodType,
            data: formData,
            contentType: false,
            processData: false,
            success: function (res) {
                submitBtn.prop("disabled", false);

                $("#dispBidAmount").text(formatVND(amount));
                $("#dispEstimatedDays").text(days + " ngày");
                $("#dispPhoneNumber").text(phone);
                $("#dispFileName").text(fileName);
                $("#dispProposalText").text(coverLetter.length > 0 ? coverLetter : "Không có");

                $("#bidFormContainer").addClass("d-none");
                $("#submittedBidBox").removeClass("d-none");

                const myBidCard = $("#myBidCard");
                myBidCard.attr("data-price", amount);
                myBidCard.attr("data-days", days);
                $("#myBidPriceTag").text(formatVND(amount));
                $("#myBidPhone").text(phone);
                $("#myBidDays").text(days + " ngày");
                $("#myBidFileName").text(fileName);

                if (coverLetter.length > 0) {
                    $("#myBidCoverLetterText").text(coverLetter);
                    $("#myBidCoverLetterBox").removeClass("d-none");
                } else {
                    $("#myBidCoverLetterBox").addClass("d-none");
                }

                myBidCard.removeClass("d-none");
                recalculateStats();
            },
            error: function (err) {
                submitBtn.prop("disabled", false);
                if (err.responseJSON && err.responseJSON.message) {
                    alertBox.text(err.responseJSON.message).removeClass("d-none");
                } else {
                    alertBox.text("Có lỗi xảy ra trong quá trình xử lý.").removeClass("d-none");
                }
            }
        });
    });

    $(document).on("click", ".btn-trigger-edit-bid", function () {
        $("#submittedBidBox").addClass("d-none");
        $("#bidFormContainer").removeClass("d-none");
        $("#submitBtnText").text("Cập nhật báo giá");

        $('html, body').animate({
            scrollTop: $("#bidFormBlock").offset().top - 30
        }, 200);
    });
});