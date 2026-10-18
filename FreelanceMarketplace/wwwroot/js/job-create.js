function goToStep(step) {
    document.querySelectorAll('.wizard-content').forEach(el => {
        el.classList.remove('active');
        el.style.display = 'none';
    });
    const cur = document.getElementById('wizardStep' + step);
    if (cur) {
        cur.classList.add('active');
        cur.style.display = 'block';
    }

    document.querySelectorAll('.step-item').forEach(el => el.classList.remove('active'));
    const header = document.getElementById('stepHeader' + step);
    if (header) header.classList.add('active');
}

document.addEventListener('DOMContentLoaded', () => {
    const getEl = id => document.getElementById(id);
    const removeTones = str => str ? str.normalize('NFD').replace(/[\u0300-\u036f]/g, '').replace(/đ/g, 'd').replace(/Đ/g, 'd').toLowerCase() : '';

    function toggleError(inputEl, errId, isValid, msg = '') {
        const errEl = getEl(errId);
        if (errEl) errEl.textContent = isValid ? '' : msg;
        if (inputEl) inputEl.classList.toggle('is-invalid', !isValid);
        return isValid;
    }

    const jobForm = getEl('jobCreateForm');
    if (!jobForm) return;

    const pCatInput = getEl('parentCategoryInput'), pCatMenu = getEl('parentCategoryMenu'), pCatHidden = getEl('ParentCategoryId');
    const subCatInput = getEl('subCategoryInput'), subCatMenu = getEl('subCategoryMenu'), catIdHidden = getEl('CategoryId');
    const title = getEl('Title'), desc = getEl('Description');
    const budgetMin = getEl('BudgetMin'), budgetMax = getEl('BudgetMax');
    const workType = getEl('WorkType'), workMode = getEl('WorkMode'), paymentType = getEl('PaymentType');
    const cityInput = getEl('locationCityInput'), cityMenu = getEl('locationCityMenu'), cityHidden = getEl('LocationCity');
    const deadline = getEl('Deadline'), titleCount = getEl('titleCharCount');
    const attachInput = getEl('attachmentInput'), btnTriggerFile = getEl('btnTriggerFile'), fileInfo = getEl('fileSelectedInfo');
    const skillsAnchor = getEl('skillsValidationAnchor');

    if (deadline) {
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        const yyyy = tomorrow.getFullYear();
        const mm = String(tomorrow.getMonth() + 1).padStart(2, '0');
        const dd = String(tomorrow.getDate()).padStart(2, '0');
        deadline.min = `${yyyy}-${mm}-${dd}`;
    }

    function setupDropdown(input, menu, onSelect) {
        if (!input || !menu) return;

        function filter(forceAll = false) {
            const raw = input.value.trim();
            const q = forceAll ? '' : removeTones(raw);
            const items = menu.querySelectorAll('.dropdown-item-vlance');
            let hasMatch = false;

            items.forEach(item => {
                const text = item.textContent.trim();
                const match = !q || removeTones(text).includes(q) || text.toLowerCase().includes(raw.toLowerCase());
                item.style.display = match ? 'block' : 'none';
                if (match) hasMatch = true;
            });
            menu.style.display = hasMatch ? 'block' : 'none';
        }

        input.addEventListener('focus', () => {
            if (!input.disabled) {
                input.select();
                filter(true);
            }
        });

        input.addEventListener('input', () => {
            filter(false);
            if (!input.value.trim()) {
                if (input === pCatInput) pCatHidden.value = '';
                if (input === subCatInput) catIdHidden.value = '';
            }
        });

        menu.addEventListener('mousedown', (e) => {
            const item = e.target.closest('.dropdown-item-vlance');
            if (!item) return;
            input.value = item.textContent.trim();
            menu.style.display = 'none';
            onSelect(item.getAttribute('data-id'), item.textContent.trim());
        });

        input.addEventListener('blur', () => {
            setTimeout(() => {
                menu.style.display = 'none';
            }, 250);
        });
    }

    setupDropdown(pCatInput, pCatMenu, async (id) => {
        pCatHidden.value = id;
        toggleError(pCatInput, 'ParentCategoryIdErr', true);
        subCatInput.value = '';
        subCatInput.disabled = true;
        subCatMenu.innerHTML = '';
        catIdHidden.value = '';

        try {
            const res = await fetch(`/api/job-posting/subcategories/${id}`);
            if (res.ok) {
                const data = await res.json();
                if (data.length > 0) {
                    subCatMenu.innerHTML = data.map(i => `<div class="dropdown-item-vlance" data-id="${i.id}">${i.name}</div>`).join('');
                    subCatInput.disabled = false;
                }
            }
        } catch (err) {
            console.error(err);
        }
    });

    setupDropdown(subCatInput, subCatMenu, id => {
        catIdHidden.value = id;
        toggleError(subCatInput, 'CategoryIdErr', true);
    });

    setupDropdown(cityInput, cityMenu, id => {
        cityHidden.value = id;
    });

    cityInput.addEventListener('blur', () => {
        if (!cityInput.value.trim()) {
            cityInput.value = 'Toàn quốc';
            cityHidden.value = 'Toàn quốc';
        }
    });

    const skillInput = getEl('skillSearchInput'), skillMenu = getEl('skillDropdownMenu');
    const skillContainer = getEl('selectedSkillsContainer');
    const selectedSkills = new Set();

    function addSkill(id, name) {
        if (!id || selectedSkills.has(id)) return;
        selectedSkills.add(id);
        toggleError(skillInput, 'SkillIdsErr', true);

        const badge = document.createElement('span');
        badge.className = 'badge text-white p-2 d-inline-flex align-items-center gap-2 me-2 mb-2';
        badge.style.backgroundColor = '#0f172a';
        badge.style.border = '1px solid #0f172a';
        badge.style.borderRadius = '6px';
        badge.style.fontSize = '13px';
        badge.style.fontWeight = '500';
        badge.innerHTML = `<span>${name}</span> <span role="button" class="fw-bold" style="cursor:pointer; font-size:14px; color:#94a3b8;">&times;</span>`;

        const removeBtn = badge.querySelector('span[role="button"]');
        removeBtn.onmouseenter = () => removeBtn.style.color = '#ffffff';
        removeBtn.onmouseleave = () => removeBtn.style.color = '#94a3b8';
        removeBtn.onclick = () => {
            selectedSkills.delete(id);
            badge.remove();
        };

        if (skillContainer) {
            skillContainer.appendChild(badge);
        }
    }

    if (skillInput && skillMenu) {
        function filterSkills(forceAll = false) {
            const raw = skillInput.value.trim();
            const q = forceAll ? '' : removeTones(raw);
            const items = skillMenu.querySelectorAll('.dropdown-item-vlance');
            let hasMatch = false;

            items.forEach(item => {
                const text = item.textContent.trim();
                const match = !q || removeTones(text).includes(q) || text.toLowerCase().includes(raw.toLowerCase());
                item.style.display = match ? 'block' : 'none';
                if (match) hasMatch = true;
            });
            skillMenu.style.display = hasMatch ? 'block' : 'none';
        }

        skillInput.addEventListener('focus', () => filterSkills(true));
        skillInput.addEventListener('input', () => filterSkills(false));

        skillMenu.addEventListener('mousedown', (e) => {
            const item = e.target.closest('.dropdown-item-vlance');
            if (!item) return;

            const id = item.getAttribute('data-id');
            const name = item.textContent.trim();

            addSkill(id, name);

            skillInput.value = '';
            skillMenu.style.display = 'none';
        });

        skillInput.addEventListener('blur', () => {
            setTimeout(() => {
                skillMenu.style.display = 'none';
                skillInput.value = '';
            }, 250);
        });
    }

    if (btnTriggerFile && attachInput) {
        btnTriggerFile.onclick = () => attachInput.click();
        attachInput.onchange = () => {
            if (attachInput.files.length) {
                getEl('fileName').textContent = attachInput.files[0].name;
                fileInfo.style.display = 'block';
                btnTriggerFile.style.display = 'none';
            }
        };
        getEl('btnRemoveFile').onclick = () => {
            attachInput.value = '';
            fileInfo.style.display = 'none';
            btnTriggerFile.style.display = 'inline';
        };
    }

    const validate = {
        pCat: () => {
            const hasVal = !!pCatHidden.value;
            return toggleError(pCatInput, 'ParentCategoryIdErr', hasVal, pCatHidden.getAttribute('data-val-required'));
        },
        subCat: () => {
            const hasVal = !!catIdHidden.value;
            return toggleError(subCatInput, 'CategoryIdErr', hasVal, catIdHidden.getAttribute('data-val-required'));
        },
        title: () => {
            const val = title.value.trim();
            const min = parseInt(title.getAttribute('data-val-minlength-min')) || 10;
            if (!val) return toggleError(title, 'TitleErr', false, title.getAttribute('data-val-required'));
            return toggleError(title, 'TitleErr', val.length >= min, title.getAttribute('data-val-minlength'));
        },
        desc: () => {
            const val = desc.value.trim();
            const min = parseInt(desc.getAttribute('data-val-minlength-min')) || 20;
            if (!val) return toggleError(desc, 'DescriptionErr', false, desc.getAttribute('data-val-required'));
            return toggleError(desc, 'DescriptionErr', val.length >= min, desc.getAttribute('data-val-minlength'));
        },
        budget: () => {
            const min = budgetMin.value ? parseFloat(budgetMin.value) : null;
            const max = budgetMax.value ? parseFloat(budgetMax.value) : null;
            let valid = true;

            if (min !== null && min < 200000) {
                valid = toggleError(budgetMin, 'BudgetMinErr', false, budgetMin.getAttribute('data-val-range'));
            } else {
                toggleError(budgetMin, 'BudgetMinErr', true);
            }

            if (max !== null) {
                if (max < 200000) {
                    valid = toggleError(budgetMax, 'BudgetMaxErr', false, budgetMax.getAttribute('data-val-range'));
                } else if (min !== null && max <= min) {
                    valid = toggleError(budgetMax, 'BudgetMaxErr', false, '');
                } else {
                    toggleError(budgetMax, 'BudgetMaxErr', true);
                }
            } else {
                toggleError(budgetMax, 'BudgetMaxErr', true);
            }

            return valid;
        },
        skills: () => {
            const errorMsg = skillsAnchor ? (skillsAnchor.getAttribute('data-val-minlength') || skillsAnchor.getAttribute('data-val-required')) : '';
            return toggleError(skillInput, 'SkillIdsErr', selectedSkills.size > 0, errorMsg);
        },
        deadline: () => {
            if (!deadline.value) {
                return toggleError(deadline, 'DeadlineErr', false, deadline.getAttribute('data-val-required'));
            }

            const selectedDate = new Date(deadline.value);
            selectedDate.setHours(0, 0, 0, 0);

            const today = new Date();
            today.setHours(0, 0, 0, 0);

            return toggleError(deadline, 'DeadlineErr', selectedDate > today, '');
        }
    };

    title.addEventListener('input', () => {
        if (titleCount) titleCount.textContent = `${title.value.length}/200`;
        if (title.value.trim().length >= 10) toggleError(title, 'TitleErr', true);
    });
    desc.addEventListener('input', () => {
        if (desc.value.trim().length >= 20) toggleError(desc, 'DescriptionErr', true);
    });

    pCatInput.addEventListener('blur', () => {
        setTimeout(() => {
            if (!pCatHidden.value && !pCatInput.value.trim()) {
                validate.pCat();
            }
        }, 250);
    });

    subCatInput.addEventListener('blur', () => {
        setTimeout(() => {
            if (!catIdHidden.value && !subCatInput.value.trim()) {
                validate.subCat();
            }
        }, 250);
    });

    const preventNegative = (e) => {
        if (['-', '+', 'e', 'E'].includes(e.key)) {
            e.preventDefault();
        }
    };
    budgetMin.addEventListener('keydown', preventNegative);
    budgetMax.addEventListener('keydown', preventNegative);

    title.onblur = validate.title;
    desc.onblur = validate.desc;
    budgetMin.onblur = budgetMax.onblur = validate.budget;
    budgetMin.oninput = budgetMax.oninput = validate.budget;
    deadline.onchange = validate.deadline;

    window.handleStepChange = function (target) {
        if (target === 2 && (!validate.pCat() || !validate.subCat() || !validate.title())) return;
        if (target === 3 && (!validate.desc() || !validate.skills())) return;
        goToStep(target);
    };

    async function submitJob() {
        if (!validate.pCat() || !validate.subCat() || !validate.title()) {
            goToStep(1);
            return;
        }
        if (!validate.desc() || !validate.skills()) {
            goToStep(2);
            return;
        }

        const formData = new FormData();
        formData.append("ParentCategoryId", parseInt(pCatHidden.value) || 0);
        formData.append("CategoryId", parseInt(catIdHidden.value) || 0);
        formData.append("Title", title.value.trim());
        formData.append("WorkType", parseInt(workType.value) || 0);
        formData.append("WorkMode", parseInt(workMode.value) || 0);
        formData.append("PaymentType", parseInt(paymentType.value) || 0);
        formData.append("LocationCity", cityHidden.value || "Toàn quốc");
        formData.append("Description", desc.value.trim());

        if (budgetMin.value) formData.append("BudgetMin", parseFloat(budgetMin.value));
        if (budgetMax.value) formData.append("BudgetMax", parseFloat(budgetMax.value));
        if (deadline.value) formData.append("Deadline", deadline.value);

        selectedSkills.forEach(id => formData.append("SkillIds", id));
        if (attachInput && attachInput.files.length > 0) {
            formData.append("AttachmentFile", attachInput.files[0]);
        }

        try {
            const token = jobForm.querySelector('input[name="__RequestVerificationToken"]')?.value;
            const headers = {};
            if (token) headers['RequestVerificationToken'] = token;

            const res = await fetch('/api/job-posting', {
                method: 'POST',
                headers: headers,
                body: formData
            });

            if (res.status === 401) {
                window.location.href = '/login?returnUrl=/jobs/create';
                return;
            }

            const data = await res.json();

            if (res.ok) {
                window.location.href = '/jobs';
            } else {
                if (data.errors) {
                    for (const [field, messages] of Object.entries(data.errors)) {
                        const errSpan = getEl(field + 'Err');
                        const inputEl = getEl(field);
                        if (errSpan) errSpan.textContent = messages[0];
                        if (inputEl) inputEl.classList.add('is-invalid');
                    }
                }
            }
        } catch (err) {
            console.error(err);
        }
    }

    const btnSubmit = getEl('btnSubmitJob');
    if (btnSubmit) btnSubmit.onclick = e => { e.preventDefault(); submitJob(); };
});