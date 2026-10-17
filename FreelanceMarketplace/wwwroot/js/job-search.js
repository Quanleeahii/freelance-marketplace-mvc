document.addEventListener("DOMContentLoaded", function () {
    const urlParams = new URLSearchParams(window.location.search);

    const removeTones = str => str ? str.normalize('NFD').replace(/[\u0300-\u036f]/g, '').replace(/đ/g, 'd').replace(/Đ/g, 'd').toLowerCase() : '';

    const cityInput = document.getElementById("cityInput");
    const cityHidden = document.getElementById("cityHidden");
    const cityMenu = document.getElementById("cityDropdownMenu");

    const state = {
        keyword: urlParams.get("Keyword") || document.getElementById("keywordInput")?.value || "",
        locationCity: urlParams.get("LocationCity") || cityHidden?.value || "",
        categoryId: urlParams.get("CategoryId") || null,
        subCategoryId: urlParams.get("SubCategoryId") || null,
        workType: urlParams.get("WorkType") || null,
        workMode: urlParams.get("WorkMode") || null,
        paymentType: urlParams.get("PaymentType") || null,
        skillIds: urlParams.getAll("SkillIds").map(id => parseInt(id)).filter(id => !isNaN(id)),
        pageNumber: parseInt(urlParams.get("PageNumber")) || 1,
        pageSize: 10
    };

    if (cityInput && cityMenu) {
        function filterCities(forceAll = false) {
            const raw = cityInput.value.trim();
            const q = forceAll ? '' : removeTones(raw);
            const items = cityMenu.querySelectorAll('.dropdown-item-vlance');
            let hasMatch = false;

            items.forEach(item => {
                const text = item.textContent.trim();
                const match = !q || removeTones(text).includes(q) || text.toLowerCase().includes(raw.toLowerCase());
                item.style.display = match ? 'block' : 'none';
                if (match) hasMatch = true;
            });
            cityMenu.style.display = hasMatch ? 'block' : 'none';
        }

        cityInput.addEventListener('focus', () => {
            cityInput.select();
            filterCities(true);
        });

        cityInput.addEventListener('input', () => filterCities(false));

        cityMenu.addEventListener('mousedown', (e) => {
            const item = e.target.closest('.dropdown-item-vlance');
            if (!item) return;

            const val = item.getAttribute('data-id');
            cityInput.value = item.textContent.trim();
            cityHidden.value = val;
            state.locationCity = val;
            cityMenu.style.display = 'none';
            state.pageNumber = 1;
            fetchJobs();
        });

        cityInput.addEventListener('blur', () => {
            setTimeout(() => {
                cityMenu.style.display = 'none';
                if (!cityInput.value.trim() || cityInput.value.trim().toLowerCase() === 'toan quoc') {
                    cityInput.value = 'Toàn quốc';
                    cityHidden.value = '';
                    state.locationCity = '';
                } else {
                    state.locationCity = cityInput.value.trim();
                }
            }, 250);
        });
    }

    async function fetchJobs() {
        const container = document.getElementById("jobListContainer");
        const pagination = document.getElementById("paginationContainer");

        container.style.opacity = "0.35";
        container.style.pointerEvents = "none";

        const params = new URLSearchParams();
        if (state.keyword) params.append("Keyword", state.keyword);
        if (state.locationCity && state.locationCity !== "Toàn quốc") {
            params.append("LocationCity", state.locationCity);
        }
        if (state.categoryId) params.append("CategoryId", state.categoryId);
        if (state.subCategoryId) params.append("SubCategoryId", state.subCategoryId);
        if (state.workType) params.append("WorkType", state.workType);
        if (state.workMode) params.append("WorkMode", state.workMode);
        if (state.paymentType) params.append("PaymentType", state.paymentType);
        
        if (state.skillIds && state.skillIds.length > 0) {
            state.skillIds.forEach(id => params.append("SkillIds", id));
        }

        params.append("PageNumber", state.pageNumber);
        params.append("PageSize", state.pageSize);

        const newUrl = `/jobs?${params.toString()}`;
        window.history.pushState({ path: newUrl }, "", newUrl);

        try {
            const res = await fetch(`/api/jobs?${params.toString()}`);
            const result = await res.json();

            container.style.opacity = "1";
            container.style.pointerEvents = "auto";

            if (!result.success || !result.data || result.data.length === 0) {
                container.innerHTML = `
                    <div class="text-center py-5 bg-white rounded border">
                        <i class="bi bi-search fs-1 text-muted"></i>
                        <p class="mt-3 text-muted">Không tìm thấy công việc phù hợp với tiêu chí lọc.</p>
                    </div>`;
                pagination.innerHTML = "";
                return;
            }

            renderJobList(result.data);
            renderPagination(result.meta);
        } catch (err) {
            container.style.opacity = "1";
            container.style.pointerEvents = "auto";
            container.innerHTML = `<div class="alert alert-danger">Lỗi kết nối máy chủ khi lấy dữ liệu việc làm.</div>`;
        }
    }

    function renderJobList(jobs) {
        const container = document.getElementById("jobListContainer");
        const template = document.getElementById("jobCardTemplate");
        container.innerHTML = "";

        jobs.forEach(job => {
            const card = template.content.cloneNode(true);

            const titleLink = card.querySelector(".job-title");
            titleLink.textContent = job.title;
            titleLink.href = `/jobs/${job.id}`;

            const workTypeEl = card.querySelector(".badge-work-type");
            if (workTypeEl) {
                const isFullTime = job.workType === 2 || job.workType === '2' || job.workType === 'FullTime';
                workTypeEl.textContent = isFullTime ? 'Toàn thời gian' : 'Bán thời gian';
            }

            const clientName = job.clientProfile?.displayName 
                || job.clientProfile?.user?.fullName 
                || "Khách hàng";
card.querySelector(".client-name").textContent = clientName;
            card.querySelector(".job-id").textContent = job.id;
            card.querySelector(".job-city").textContent = job.locationCity || "Toàn quốc";
            card.querySelector(".job-category").textContent = job.category?.name || "Chung";

            const minBudget = job.budgetMin ? Number(job.budgetMin).toLocaleString('vi-VN') + ' đ' : '0 đ';
            const maxBudget = job.budgetMax ? Number(job.budgetMax).toLocaleString('vi-VN') + ' đ' : null;
            card.querySelector(".job-budget").textContent = maxBudget ? `${minBudget} - ${maxBudget}` : minBudget;

            card.querySelector(".job-deadline").textContent = job.deadline ? new Date(job.deadline).toLocaleDateString('vi-VN') : 'Đang mở';

            const desc = job.description || "";
            const MAX_LEN = 130;
            const descEl = card.querySelector(".job-desc");
            const linkEl = card.querySelector(".job-link");

            if (desc.length > MAX_LEN) {
                descEl.textContent = desc.substring(0, MAX_LEN).trim() + "...";
                linkEl.href = `/jobs/${job.id}`;
                linkEl.classList.remove("d-none");
            } else {
                descEl.textContent = desc;
                linkEl.classList.add("d-none");
            }

            const skillsContainer = card.querySelector(".skills-list");
            skillsContainer.innerHTML = "";
            (job.skills || []).forEach(s => {
                const badge = document.createElement("span");
                badge.className = "badge bg-light text-secondary border fw-normal";
                badge.textContent = s.name;
                skillsContainer.appendChild(badge);
            });

            card.querySelector(".proposals-count").textContent = `${(job.proposals || []).length} chào giá`;

            container.appendChild(card);
        });
    }

    function renderPagination(meta) {
        const container = document.getElementById("paginationContainer");
        if (!meta || meta.totalPages <= 1) {
            container.innerHTML = "";
            return;
        }

        let html = '<ul class="pagination pagination-sm gap-1">';
        for (let i = 1; i <= meta.totalPages; i++) {
            html += `
                <li class="page-item ${i === meta.currentPage ? 'active' : ''}">
                    <button class="page-link rounded-circle btn-page" data-page="${i}">${i}</button>
                </li>`;
        }
        html += '</ul>';
        container.innerHTML = html;

        container.querySelectorAll(".btn-page").forEach(btn => {
            btn.addEventListener("click", function () {
                state.pageNumber = parseInt(this.dataset.page);
                fetchJobs();
                window.scrollTo({ top: 0, behavior: "smooth" });
            });
        });
    }

    document.getElementById("btnSearch")?.addEventListener("click", () => {
        state.keyword = document.getElementById("keywordInput").value.trim();
        state.pageNumber = 1;
        fetchJobs();
    });

    document.getElementById("keywordInput")?.addEventListener("keypress", (e) => {
        if (e.key === "Enter") {
            document.getElementById("btnSearch").click();
        }
    });

    const skillToggleBtn = document.getElementById("skillAllToggle");
    if (skillToggleBtn) {
        skillToggleBtn.addEventListener("click", function () {
            const dropdown = document.getElementById("skillsDropdownList");
            const arrow = this.querySelector(".skill-arrow-toggle");
            
            const isHidden = dropdown.classList.contains("d-none");
            if (isHidden) {
                dropdown.classList.remove("d-none");
                arrow.classList.add("rotated");
            } else {
                dropdown.classList.add("d-none");
                arrow.classList.remove("rotated");
            }
        });
    }

    document.querySelectorAll(".filter-skill-item").forEach(cb => {
        cb.addEventListener("change", function () {
            const checkedBoxes = document.querySelectorAll(".filter-skill-item:checked");
            state.skillIds = Array.from(checkedBoxes).map(box => parseInt(box.value));
            
            const skillAllItem = document.getElementById("skillAllToggle");
            if (state.skillIds.length > 0) {
                skillAllItem?.classList.remove("active");
            } else {
                skillAllItem?.classList.add("active");
            }

            state.pageNumber = 1;
            fetchJobs();
        });
    });

    document.getElementById("btnClearSkills")?.addEventListener("click", function (e) {
        e.stopPropagation();
        document.querySelectorAll(".filter-skill-item").forEach(cb => cb.checked = false);
        
        const skillAllItem = document.getElementById("skillAllToggle");
        skillAllItem?.classList.add("active");
        
        state.skillIds = [];
        state.pageNumber = 1;
        fetchJobs();
    });

    document.querySelectorAll(".arrow-toggle:not(.skill-arrow-toggle)").forEach(arrow => {
        arrow.addEventListener("click", function (e) {
            e.stopPropagation();
            const targetSelector = this.dataset.target;
            const target = document.querySelector(targetSelector);
            const isCurrentlyOpen = !target.classList.contains("d-none");

            document.querySelectorAll(".sub-category-list:not(#skillsDropdownList)").forEach(list => list.classList.add("d-none"));
            document.querySelectorAll(".arrow-toggle:not(.skill-arrow-toggle)").forEach(a => a.classList.remove("rotated"));

            if (!isCurrentlyOpen) {
                target.classList.remove("d-none");
                this.classList.add("rotated");
            }
        });
    });

    document.querySelectorAll(".parent-name").forEach(el => {
        el.addEventListener("click", function () {
            document.querySelectorAll(".category-parent-item, .sub-category-item").forEach(i => i.classList.remove("active"));
            this.closest(".category-parent-item").classList.add("active");
            state.categoryId = this.dataset.parentId;
            state.subCategoryId = null;
            state.pageNumber = 1;
            fetchJobs();
        });
    });

    document.querySelector(".select-all-cat")?.addEventListener("click", function () {
        document.querySelectorAll(".category-parent-item, .sub-category-item").forEach(i => i.classList.remove("active"));
        document.getElementById("catAllItem")?.classList.add("active");
        document.querySelectorAll(".sub-category-list:not(#skillsDropdownList)").forEach(list => list.classList.add("d-none"));
        document.querySelectorAll(".arrow-toggle:not(.skill-arrow-toggle)").forEach(a => a.classList.remove("rotated"));

        state.categoryId = null;
        state.subCategoryId = null;
        state.pageNumber = 1;
        fetchJobs();
    });

    document.querySelectorAll(".sub-category-item").forEach(el => {
        el.addEventListener("click", function () {
            document.querySelectorAll(".category-parent-item, .sub-category-item").forEach(i => i.classList.remove("active"));
            this.classList.add("active");
            state.categoryId = this.dataset.parentId;
            state.subCategoryId = this.dataset.subId;
            state.pageNumber = 1;
            fetchJobs();
        });
    });

    document.querySelectorAll("#workTypeTabs .nav-link").forEach(tab => {
        tab.addEventListener("click", function () {
            document.querySelectorAll("#workTypeTabs .nav-link").forEach(t => t.classList.remove("active"));
            this.classList.add("active");
            state.workType = this.dataset.workType || null;
            state.pageNumber = 1;
            fetchJobs();
        });
    });

    document.querySelectorAll(".filter-work-mode").forEach(r => {
        r.addEventListener("change", function () {
            state.workMode = this.value || null;
            state.pageNumber = 1;
            fetchJobs();
        });
    });

    document.querySelectorAll(".filter-payment-type").forEach(r => {
        r.addEventListener("change", function () {
            state.paymentType = this.value || null;
            state.pageNumber = 1;
            fetchJobs();
        });
    });

    fetchJobs();
});