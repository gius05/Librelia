document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('resetPasswordForm');
    const passwordInput = document.getElementById('Password');
    const confirmPasswordInput = document.getElementById('ConfirmPassword');
    const togglePasswordBtn = document.getElementById('togglePassword');

    const passwordStrengthBar = document.querySelector('#passwordStrength .progress-bar');
    const lengthCheck = document.getElementById('lengthCheck');
    const uppercaseCheck = document.getElementById('uppercaseCheck');
    const numberCheck = document.getElementById('numberCheck');
    const specialCheck = document.getElementById('specialCheck');

    // Toggle password visibility
    togglePasswordBtn.addEventListener('click', () => {
        const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordInput.setAttribute('type', type);

        const eyeIcon = togglePasswordBtn.querySelector('i');
        if (eyeIcon) {
            eyeIcon.classList.toggle('bi-eye');
            eyeIcon.classList.toggle('bi-eye-slash');
        }
    });

    // Password strength checker
    passwordInput.addEventListener('input', () => {
        const password = passwordInput.value;
        let strength = 0;

        const hasLength = password.length >= 8;
        const hasUpperCase = /[A-Z]/.test(password);
        const hasNumber = /[0-9]/.test(password);
        const hasSpecial = /[!@#$%^&*(),.?":{}|<>]/.test(password);

        // Update checklist UI
        lengthCheck.className = hasLength ? 'text-success' : 'text-muted';
        uppercaseCheck.className = hasUpperCase ? 'text-success' : 'text-muted';
        numberCheck.className = hasNumber ? 'text-success' : 'text-muted';
        specialCheck.className = hasSpecial ? 'text-success' : 'text-muted';

        // Update strength bar
        if (hasLength) strength += 25;
        if (hasUpperCase) strength += 25;
        if (hasNumber) strength += 25;
        if (hasSpecial) strength += 25;

        passwordStrengthBar.style.width = `${strength}%`;

        if (strength <= 25) {
            passwordStrengthBar.className = 'progress-bar bg-danger';
        } else if (strength <= 50) {
            passwordStrengthBar.className = 'progress-bar bg-warning';
        } else if (strength <= 75) {
            passwordStrengthBar.className = 'progress-bar bg-info';
        } else {
            passwordStrengthBar.className = 'progress-bar bg-success';
        }
    });

    // Confirm password check
    confirmPasswordInput.addEventListener('input', () => {
        if (passwordInput.value !== confirmPasswordInput.value) {
            confirmPasswordInput.setCustomValidity('Le password non corrispondono');
        } else {
            confirmPasswordInput.setCustomValidity('');
        }
    });

    // Form validation on submit
    form.addEventListener('submit', (event) => {
        const password = passwordInput.value;

        const hasLength = password.length >= 8;
        const hasUpperCase = /[A-Z]/.test(password);
        const hasNumber = /[0-9]/.test(password);
        const hasSpecial = /[!@#$%^&*(),.?":{}|<>]/.test(password);

        if (!hasLength || !hasUpperCase || !hasNumber || !hasSpecial) {
            passwordInput.setCustomValidity('La password non soddisfa i requisiti');
            form.classList.add('was-validated');
            event.preventDefault(); // Blocca il submit
        } else {
            passwordInput.setCustomValidity('');
        }

        if (password !== confirmPasswordInput.value) {
            confirmPasswordInput.setCustomValidity('Le password non corrispondono');
            form.classList.add('was-validated');
            event.preventDefault(); // Blocca il submit
        } else {
            confirmPasswordInput.setCustomValidity('');
        }
    });
});
