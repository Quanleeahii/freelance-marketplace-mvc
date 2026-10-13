function goToStep(step) {
    $(".wizard-content").removeClass("active");
    $("#wizardStep" + step).addClass("active");

    $(".step-item").removeClass("active");
    $("#stepHeader" + step).addClass("active");
}

$(document).ready(function () {
    // 1. Đếm ký tự tiêu đề
    $("#Title").on("input", function () {
        $("#titleCharCount").text($(this).val().length + "/200");
    });

    // 2. Lọc SubCategory theo Category chính
    const $subSelect =$("#SubCategoryId");
    const $allSubOptions =$subSelect.find("option").clone();
    $("#CategoryId").on("change", function () {
        const parentId = $(this).val();$subSelect.empty().append('<option value="">-- Chọn dịch vụ có sẵn --</option>');
        if (parentId) {
            $allSubOptions.each(function () {
                if ($(this).data("parent") == parentId) {
                    $subSelect.append($(this).clone());
                }
            });
        }
    });

    // 3. Xử lý Tag Kỹ năng
    let skillsList = [];
    $("#skillInput").on("keydown", function (e) {
        if (e.key === "Enter" || e.key === ",") {
            e.preventDefault();
            const tagVal = $.trim($(this).val().replace(",", ""));
            if (tagVal && !skillsList.includes(tagVal)) {
                skillsList.push(tagVal);
                renderSkills();
            }
            $(this).val("");
        }
    });

    function renderSkills() {
        const $container =$("#skillsTagContainer");
        $container.empty();
        skillsList.forEach((skill, index) => {
            $container.append(`
                <span class="skill-badge">
                    ${skill}
                    <span class="remove-tag text-danger fw-bold ms-1" style="cursor:pointer;" data-index="${index}">&times;</span>
                </span>
            `);
        });
        $("#Skills").val(skillsList.join(","));
    }

    $(document).on("click", ".remove-tag", function () {
        skillsList.splice($(this).data("index"), 1);
        renderSkills();
    });

    // 4. Format định dạng tiền VNĐ
    function formatVND(amount) {
        if (!amount || amount <= 0) return "";
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
    }
    $("#BudgetMin").on("input", function () { $("#budgetMinFormatted").text(formatVND($(this).val())); });$("#BudgetMax").on("input", function () { $("#budgetMaxFormatted").text(formatVND($(this).val())); });

    // 5. Hiển thị tên file đính kèm
    $("#attachmentFile").on("change", function () {
        if (this.files[0]) {
            $("#uploadText").text("Đã chọn file: " + this.files[0].name);
        }
    });

    // 6. Xử lý submit bằng AJAX qua Restful API
    $("#jobCreateForm").on("submit", function (e) {
        e.preventDefault();

        // Clear thông báo lỗi cũ
        $(".error-message").text("");
        $(".input-vlance, .select-vlance, .textarea-vlance").removeClass("input-error");

        const formData = {
            categoryId: $("#CategoryId").val() ? parseInt($("#CategoryId").val()) : 0,
            subCategoryId: $("#SubCategoryId").val() ? parseInt($("#SubCategoryId").val()) : null,
            title: $("#Title").val(),
            jobCategoryType: $("#JobCategoryType").val(),
            workType: $("#WorkType").val(),
            locationCity: $("#LocationCity").val(),
            description: $("#Description").val(),
            skills: $("#Skills").val(),
            experienceLevel: $("#ExperienceLevel").val(),
            paymentType: $("#PaymentType").val(),
            budgetMin: $("#BudgetMin").val() ? parseFloat($("#BudgetMin").val()) : null,
            budgetMax: $("#BudgetMax").val() ? parseFloat($("#BudgetMax").val()) : null,
            deadline: $("#Deadline").val()
        };

        $.ajax({
            url: '/api/jobs/create',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    window.location.href = '/Home/Index';
                }
            },
            error: function (xhr) {
                if (xhr.responseJSON && xhr.responseJSON.field) {
                    const field = xhr.responseJSON.field;
                    const msg = xhr.responseJSON.message;

                    // Chuyển đúng bước tương ứng với trường bị lỗi
                    if (field === "Title" || field === "CategoryId") {
                        goToStep(1);
                    } else if (field === "Description") {
                        goToStep(2);
                    }

                    // Báo chữ đỏ + highlight viền đỏ
                    $("#err-" + field).text(msg);
                    $("#" + field).addClass("input-error");
                } else {
                    alert("Có lỗi xảy ra từ máy chủ!");
                }
            }
        });
    });
});