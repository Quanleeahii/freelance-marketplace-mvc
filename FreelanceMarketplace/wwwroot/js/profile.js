document.addEventListener("DOMContentLoaded", function () {
    const getEl = id => document.getElementById(id);
    const removeTones = str => str ? str.normalize('NFD').replace(/[\u0300-\u036f]/g, '').replace(/đ/g, 'd').replace(/Đ/g, 'd').toLowerCase() : '';

    const profileForm = getEl("form-user-profile");
    const avatarInput = getEl("avatar-file-input");
    const avatarPreview = getEl("avatar-preview");
    const sidebarAvatar = getEl("sidebar-avatar-img");
    const sidebarUserName = getEl("sidebar-user-name");
    const hiddenAvatarUrl = getEl("AvatarUrl");
    const submitBtn = getEl("btn-submit-profile");
    const btnSpinner = getEl("btn-spinner");
    const avatarSpinner = getEl("avatar-spinner");
    const saveSuccessMsg = getEl("save-success-msg");

    const cityInput = getEl("locationCityInput");
    const cityMenu = getEl("locationCityMenu");
    const hiddenCity = getEl("City");

    const freelancerForm = getEl("form-freelancer-profile");
    const submitFreelancerBtn = getEl("btn-submit-freelancer");
    const freelancerSpinner = getEl("btn-freelancer-spinner");
    const freelancerSuccessMsg = getEl("freelancer-success-msg");

    const clientForm = getEl("form-client-profile");
    const submitClientBtn = getEl("btn-submit-client");
    const clientSpinner = getEl("btn-client-spinner");
    const clientSuccessMsg = getEl("client-success-msg");

    function clearValidationErrors() {
        document.querySelectorAll("[data-valmsg-for]").forEach(el => el.textContent = "");
        if (saveSuccessMsg) saveSuccessMsg.classList.add("d-none");
        if (freelancerSuccessMsg) freelancerSuccessMsg.classList.add("d-none");
        if (clientSuccessMsg) clientSuccessMsg.classList.add("d-none");
    }

    function displayValidationErrors(errors) {
        if (!errors) return;
        for (const [key, messages] of Object.entries(errors)) {
            const errorSpan = document.querySelector(`[data-valmsg-for="${key}"]`);
            if (errorSpan && messages && messages.length > 0) {
                errorSpan.textContent = messages[0];
            }
        }
    }

    function getRequestHeaders(formEl) {
        const headers = {
            "Content-Type": "application/json"
        };
        const token = formEl?.querySelector('input[name="__RequestVerificationToken"]')?.value;
        if (token) {
            headers["RequestVerificationToken"] = token;
        }
        return headers;
    }

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

        cityInput.addEventListener('input', () => {
            filterCities(false);
            if (!cityInput.value.trim() && hiddenCity) {
                hiddenCity.value = '';
            }
        });

        cityMenu.addEventListener('mousedown', (e) => {
            const item = e.target.closest('.dropdown-item-vlance');
            if (!item) return;

            const selectedCity = item.textContent.trim();
            cityInput.value = selectedCity;
            if (hiddenCity) hiddenCity.value = selectedCity;
            cityMenu.style.display = 'none';

            const cityErr = document.querySelector('[data-valmsg-for="City"]');
            if (cityErr) cityErr.textContent = '';
        });

        cityInput.addEventListener('blur', () => {
            setTimeout(() => {
                cityMenu.style.display = 'none';
            }, 250);
        });
    }

    if (avatarInput) {
        avatarInput.addEventListener("change", async function () {
            const file = this.files[0];
            if (!file) return;

            clearValidationErrors();
            if (avatarSpinner) avatarSpinner.classList.remove("d-none");

            const formData = new FormData();
            formData.append("file", file);

            try {
                const response = await fetch("/api/user-profile/avatar", {
                    method: "PUT",
                    body: formData
                });

                if (response.ok) {
                    const data = await response.json();
                    if (avatarPreview) avatarPreview.src = data.avatarUrl;
                    if (sidebarAvatar) sidebarAvatar.src = data.avatarUrl;
                    if (hiddenAvatarUrl) hiddenAvatarUrl.value = data.avatarUrl;

                    const navAvatar = document.getElementById("navbarUserAvatar");
                    if (navAvatar) navAvatar.src = data.avatarUrl;
                } else {
                    const error = await response.json().catch(() => null);
                    if (error?.message) {
                        const avatarErrorSpan = document.querySelector('[data-valmsg-for="AvatarUrl"]');
                        if (avatarErrorSpan) avatarErrorSpan.textContent = error.message;
                    }
                }
            } finally {
                if (avatarSpinner) avatarSpinner.classList.add("d-none");
                avatarInput.value = "";
            }
        });
    }

    if (profileForm) {
        profileForm.addEventListener("submit", async function (e) {
            e.preventDefault();
            clearValidationErrors();

            const fullNameVal = getEl("FullName")?.value.trim() || "";
            const payload = {
                fullName: fullNameVal,
                phone: getEl("Phone")?.value.trim() || null,
                city: hiddenCity?.value.trim() || null,
                address: getEl("Address")?.value.trim() || null,
                avatarUrl: hiddenAvatarUrl?.value.trim() || null
            };

            if (submitBtn) submitBtn.disabled = true;
            if (btnSpinner) btnSpinner.classList.remove("d-none");

            try {
                const response = await fetch("/api/user-profile/basic", {
                    method: "PUT",
                    headers: getRequestHeaders(profileForm),
                    body: JSON.stringify(payload)
                });
                if (response.status === 401) {
    window.location.href = "/login?returnUrl=/profile";
    return;
}
                if (response.status === 204 || response.ok) {
                    if (saveSuccessMsg) saveSuccessMsg.classList.remove("d-none");
                    if (sidebarUserName) sidebarUserName.textContent = fullNameVal;

                    const navUserName = document.getElementById("navbarUserName");
                    if (navUserName) navUserName.textContent = fullNameVal;

                    setTimeout(() => {
                        if (saveSuccessMsg) saveSuccessMsg.classList.add("d-none");
                    }, 3500);
                } else if (response.status === 400) {
                    const data = await response.json().catch(() => null);
                    if (data?.errors) {
                        displayValidationErrors(data.errors);
                    }
                }
            } finally {
                if (submitBtn) submitBtn.disabled = false;
                if (btnSpinner) btnSpinner.classList.add("d-none");
            }
        });
    }

    if (freelancerForm) {
        freelancerForm.addEventListener("submit", async function (e) {
            e.preventDefault();
            clearValidationErrors();

            const isAvailableEl = getEl("IsAvailable");
            const expLevelEl = getEl("ExperienceLevel");
            const expLevelVal = expLevelEl?.value;
            const workTypeEl = getEl("WorkType");
            const workTypeVal = workTypeEl?.value;

            const payload = {
                categoryId: null,
                bio: getEl("Bio")?.value.trim() || null,
                personalWebsite: getEl("PersonalWebsite")?.value.trim() || null,
                experienceLevel: expLevelVal ? parseInt(expLevelVal, 10) : null,
                workType: workTypeVal ? parseInt(workTypeVal, 10) : null,
                isAvailable: isAvailableEl ? isAvailableEl.checked : false
            };

            if (submitFreelancerBtn) submitFreelancerBtn.disabled = true;
            if (freelancerSpinner) freelancerSpinner.classList.remove("d-none");

            try {
                const response = await fetch("/api/user-profile/freelancer", {
                    method: "PUT",
                    headers: getRequestHeaders(freelancerForm),
                    body: JSON.stringify(payload)
                });
                if (response.status === 401) {
    window.location.href = "/login?returnUrl=/profile";
    return;
}
                if (response.status === 204 || response.ok) {
                    if (freelancerSuccessMsg) freelancerSuccessMsg.classList.remove("d-none");
                    setTimeout(() => {
                        if (freelancerSuccessMsg) freelancerSuccessMsg.classList.add("d-none");
                    }, 3500);
                } else if (response.status === 400) {
                    const data = await response.json().catch(() => null);
                    if (data?.errors) {
                        displayValidationErrors(data.errors);
                    }
                }
            } finally {
                if (submitFreelancerBtn) submitFreelancerBtn.disabled = false;
                if (freelancerSpinner) freelancerSpinner.classList.add("d-none");
            }
        });
    }

    if (clientForm) {
        clientForm.addEventListener("submit", async function (e) {
            e.preventDefault();
            clearValidationErrors();

            const payload = {
                displayName: getEl("ClientDisplayName")?.value.trim() || null,
                website: getEl("ClientWebsite")?.value.trim() || null,
                description: getEl("ClientDescription")?.value.trim() || null
            };

            if (submitClientBtn) submitClientBtn.disabled = true;
            if (clientSpinner) clientSpinner.classList.remove("d-none");

            try {
                const response = await fetch("/api/user-profile/client", {
                    method: "PUT",
                    headers: getRequestHeaders(clientForm),
                    body: JSON.stringify(payload)
                });
                if (response.status === 401) {
    window.location.href = "/login?returnUrl=/profile";
    return;
}
                if (response.status === 204 || response.ok) {
                    if (clientSuccessMsg) clientSuccessMsg.classList.remove("d-none");
                    setTimeout(() => {
                        if (clientSuccessMsg) clientSuccessMsg.classList.add("d-none");
                    }, 3500);
                } else if (response.status === 400) {
                    const data = await response.json().catch(() => null);
                    if (data?.errors) {
                        displayValidationErrors(data.errors);
                    }
                }
            } finally {
                if (submitClientBtn) submitClientBtn.disabled = false;
                if (clientSpinner) clientSpinner.classList.add("d-none");
            }
        });
    }
});