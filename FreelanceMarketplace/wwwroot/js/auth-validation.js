// Ẩn/Hiện mật khẩu
function togglePassword(inputId, btn) {
    const input = document.getElementById(inputId);
    if (!input) return;
    
    if (input.type === 'password') {
        input.type = 'text';
        btn.textContent = 'Ẩn';
    } else {
        input.type = 'password';
        btn.textContent = 'Hiện';
    }
}

// Xử lý Validate Form Client-side
document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');

    function showError(elId, msg) {
        const errEl = document.getElementById(elId);
        if (errEl) errEl.textContent = msg;
    }

    function clearErrors() {
        document.querySelectorAll('.field-error-msg').forEach(el => el.textContent = '');
    }

    // Form Đăng nhập
    if (loginForm) {
        loginForm.addEventListener('submit', (e) => {
            clearErrors();
            let isValid = true;

            const email = document.getElementById('email').value.trim();
            const password = document.getElementById('password').value;

            if (!email) {
                showError('emailErr', 'Vui lòng nhập Email.');
                isValid = false;
            }

            if (!password) {
                showError('passwordErr', 'Vui lòng nhập mật khẩu.');
                isValid = false;
            }

            if (!isValid) {
                e.preventDefault();
            }
        });
    }

    // Form Đăng ký
    if (registerForm) {
        registerForm.addEventListener('submit', (e) => {
            clearErrors();
            let isValid = true;

            const fullName = document.getElementById('fullName').value.trim();
            const email = document.getElementById('regEmail').value.trim();
            const password = document.getElementById('regPassword').value;
            const confirmPassword = document.getElementById('confirmPassword').value;
            const terms = document.getElementById('terms').checked;

            if (!fullName) {
                showError('fullNameErr', 'Vui lòng nhập họ và tên.');
                isValid = false;
            }

            if (!email) {
                showError('regEmailErr', 'Vui lòng nhập Email.');
                isValid = false;
            }

            if (!password || password.length < 6) {
                showError('regPasswordErr', 'Mật khẩu phải từ 6 ký tự trở lên.');
                isValid = false;
            }

            if (password !== confirmPassword) {
                showError('confirmPasswordErr', 'Mật khẩu xác nhận không khớp.');
                isValid = false;
            }

            if (!terms) {
                showError('termsErr', 'Bạn phải chấp nhận điều khoản dịch vụ.');
                isValid = false;
            }

            if (!isValid) {
                e.preventDefault();
            }
        });
    }
});