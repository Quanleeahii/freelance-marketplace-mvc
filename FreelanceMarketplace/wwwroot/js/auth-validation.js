function togglePassword(inputId, btn) {
    const input = document.getElementById(inputId);
    if (!input) return;

    const isPassword = input.type === 'password';
    input.type = isPassword ? 'text' : 'password';
    btn.textContent = isPassword ? 'Ẩn' : 'Hiện';
}

document.addEventListener('DOMContentLoaded', () => {
    function showError(inputEl, errId, msg) {
        const errEl = document.getElementById(errId);
        if (errEl) errEl.textContent = msg || '';
        if (inputEl) inputEl.classList.add('is-invalid');
    }

    function clearError(inputEl, errId) {
        const errEl = document.getElementById(errId);
        if (errEl) errEl.textContent = '';
        if (inputEl) inputEl.classList.remove('is-invalid');
    }

    // ==========================================
    // 1. XỬ LÝ FORM ĐĂNG KÝ (Nếu đang ở trang Register)
    // ==========================================
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        const fullName = document.getElementById('fullName');
        const email = document.getElementById('email');
        const password = document.getElementById('password');
        const confirmPassword = document.getElementById('confirmPassword');
        const terms = document.getElementById('terms');

        function validateFullName() {
            const val = fullName.value.trim();
            if (!val) {
                showError(fullName, 'fullNameErr', fullName.getAttribute('data-val-required'));
                return false;
            }
            if (val.length > 100) {
                showError(fullName, 'fullNameErr', fullName.getAttribute('data-val-length'));
                return false;
            }
            clearError(fullName, 'fullNameErr');
            return true;
        }

        function validateEmail() {
            const val = email.value.trim();
            if (!val) {
                showError(email, 'emailErr', email.getAttribute('data-val-required'));
                return false;
            }
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(val)) {
                showError(email, 'emailErr', email.getAttribute('data-val-email'));
                return false;
            }
            clearError(email, 'emailErr');
            return true;
        }

        function validatePassword() {
            const val = password.value;
            if (!val) {
                showError(password, 'passwordErr', password.getAttribute('data-val-required'));
                return false;
            }
            if (val.length < 6) {
                showError(password, 'passwordErr', password.getAttribute('data-val-length'));
                return false;
            }
            clearError(password, 'passwordErr');
            return true;
        }

        function validateConfirmPassword() {
            const val = confirmPassword.value;
            if (!val) {
                showError(confirmPassword, 'confirmPasswordErr', confirmPassword.getAttribute('data-val-required'));
                return false;
            }
            if (val !== password.value) {
                showError(confirmPassword, 'confirmPasswordErr', confirmPassword.getAttribute('data-val-equalto'));
                return false;
            }
            clearError(confirmPassword, 'confirmPasswordErr');
            return true;
        }

        function validateTerms() {
            if (!terms.checked) {
                showError(terms, 'termsErr', terms.getAttribute('data-val-required') || 'Vui lòng đồng ý điều khoản.');
                return false;
            }
            clearError(terms, 'termsErr');
            return true;
        }

        fullName.addEventListener('blur', validateFullName);
        fullName.addEventListener('input', () => { if (fullName.value.trim()) clearError(fullName, 'fullNameErr'); });

        email.addEventListener('blur', validateEmail);
        email.addEventListener('input', () => { if (email.value.trim()) clearError(email, 'emailErr'); });

        password.addEventListener('blur', validatePassword);
        password.addEventListener('input', () => {
            if (password.value) clearError(password, 'passwordErr');
            if (confirmPassword.value) validateConfirmPassword();
        });

        confirmPassword.addEventListener('blur', validateConfirmPassword);
        confirmPassword.addEventListener('input', () => {
            if (confirmPassword.value === password.value) clearError(confirmPassword, 'confirmPasswordErr');
        });

        terms.addEventListener('change', validateTerms);

        registerForm.addEventListener('submit', (e) => {
            const isNameOk = validateFullName();
            const isEmailOk = validateEmail();
            const isPassOk = validatePassword();
            const isConfirmOk = validateConfirmPassword();
            const isTermsOk = validateTerms();

            if (!isNameOk || !isEmailOk || !isPassOk || !isConfirmOk || !isTermsOk) {
                e.preventDefault();
            }
        });
    }

    // ==========================================
    // 2. XỬ LÝ FORM ĐĂNG NHẬP (Nếu đang ở trang Login)
    // ==========================================
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        const loginEmail = document.getElementById('loginEmail');
        const loginPassword = document.getElementById('loginPassword');

        function validateLoginEmail() {
            const val = loginEmail.value.trim();
            if (!val) {
                showError(loginEmail, 'loginEmailErr', loginEmail.getAttribute('data-val-required'));
                return false;
            }
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(val)) {
                showError(loginEmail, 'loginEmailErr', loginEmail.getAttribute('data-val-email'));
                return false;
            }
            clearError(loginEmail, 'loginEmailErr');
            return true;
        }

        function validateLoginPassword() {
            const val = loginPassword.value;
            if (!val) {
                showError(loginPassword, 'loginPasswordErr', loginPassword.getAttribute('data-val-required'));
                return false;
            }
            clearError(loginPassword, 'loginPasswordErr');
            return true;
        }

        loginEmail.addEventListener('blur', validateLoginEmail);
        loginEmail.addEventListener('input', () => { if (loginEmail.value.trim()) clearError(loginEmail, 'loginEmailErr'); });

        loginPassword.addEventListener('blur', validateLoginPassword);
        loginPassword.addEventListener('input', () => { if (loginPassword.value) clearError(loginPassword, 'loginPasswordErr'); });

        loginForm.addEventListener('submit', (e) => {
            const isEmailOk = validateLoginEmail();
            const isPassOk = validateLoginPassword();

            if (!isEmailOk || !isPassOk) {
                e.preventDefault();
            }
        });
    }
});