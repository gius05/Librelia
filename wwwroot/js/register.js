

document.addEventListener('DOMContentLoaded', () => {

    const registrationForm = document.getElementById('registrationForm');
    const firstNameInput = document.getElementById('Name');
    const lastNameInput = document.getElementById('Surname');
    const emailInput = document.getElementById('Email');
    const roleInput = document.getElementById('Role');
    const isExternalInput = document.getElementById('IsExternal');
    const passwordInput = document.getElementById('Password');
    const confirmPasswordInput = document.getElementById('ConfirmPassword');
    const togglePasswordBtn = document.getElementById('togglePassword');
    const termsCheck = document.getElementById('termsCheck');
    const registrationSuccess = document.getElementById('registrationSuccess');
    const registrationError = document.getElementById('registrationError');
    const registrationErrorMessage = document.getElementById('registrationErrorMessage');


    // Password strength elements
    const passwordStrengthBar = document.querySelector('#passwordStrength .progress-bar');
    const lengthCheck = document.getElementById('lengthCheck');
    const uppercaseCheck = document.getElementById('uppercaseCheck');
    const numberCheck = document.getElementById('numberCheck');
    const specialCheck = document.getElementById('specialCheck');

    // Toggle password visibility
    togglePasswordBtn.addEventListener('click', () => {
        const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordInput.setAttribute('type', type);

        // Toggle eye icon
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

        // Check password criteria
        const hasLength = password.length >= 8;
        const hasUpperCase = /[A-Z]/.test(password);
        const hasNumber = /[0-9]/.test(password);
        const hasSpecial = /[!@#$%^&*(),.?":{}|<>]/.test(password);

        // Update checklist
        lengthCheck.className = hasLength ? 'text-success' : 'text-muted';
        uppercaseCheck.className = hasUpperCase ? 'text-success' : 'text-muted';
        numberCheck.className = hasNumber ? 'text-success' : 'text-muted';
        specialCheck.className = hasSpecial ? 'text-success' : 'text-muted';

        // Calculate strength
        if (hasLength) strength += 25;
        if (hasUpperCase) strength += 25;
        if (hasNumber) strength += 25;
        if (hasSpecial) strength += 25;

        // Update strength bar
        passwordStrengthBar.style.width = `${strength}%`;

        // Update bar color based on strength
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

    // Check password match
    confirmPasswordInput.addEventListener('input', () => {
        if (passwordInput.value !== confirmPasswordInput.value) {
            confirmPasswordInput.setCustomValidity('Le password non corrispondono');
        } else {
            confirmPasswordInput.setCustomValidity('');
        }
    });

    // Form validation
    registrationForm.addEventListener('submit', async (event) => {
        event.preventDefault();

        // Reset previous validation
        registrationForm.classList.remove('was-validated');
        registrationSuccess.classList.add('d-none');
        registrationError.classList.add('d-none');

        // Custom validation
        let isValid = true;

        // Validate first name
        if (!firstNameInput.value.trim()) {
            firstNameInput.setCustomValidity('Inserisci il tuo nome');
            isValid = false;
        } else {
            firstNameInput.setCustomValidity('');
        }

        // Validate last name
        if (!lastNameInput.value.trim()) {
            lastNameInput.setCustomValidity('Inserisci il tuo cognome');
            isValid = false;
        } else {
            lastNameInput.setCustomValidity('');
        }
        // Validate role
        if (!roleInput.value.trim()) {
            roleInput.setCustomValidity("Devi scegliere un ruolo");
            isValid = false;
        } else {
            roleInput.setCustomValidity('');
        }

        // Validate isExternal
        if (!isExternalInput.value.trim()) {
            isExternalInput.setCustomValidity("Devi la tua posizione");
            isValid = false;
        } else {
            isExternalInput.setCustomValidity('');
        }

        // Validate email
        if (!emailInput.value.trim() || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(emailInput.value)) {
            emailInput.setCustomValidity('Inserisci un indirizzo email valido');
            isValid = false;
        } else {
            emailInput.setCustomValidity('');
        }



        // Validate password
        const password = passwordInput.value;
        const hasLength = password.length >= 8;
        const hasUpperCase = /[A-Z]/.test(password);
        const hasNumber = /[0-9]/.test(password);
        const hasSpecial = /[!@#$%^&*(),.?":{}|<>]/.test(password);

        if (!hasLength || !hasUpperCase || !hasNumber || !hasSpecial) {
            passwordInput.setCustomValidity('La password non soddisfa i requisiti');
            isValid = false;
        } else {
            passwordInput.setCustomValidity('');
        }

        // Validate password confirmation
        if (passwordInput.value !== confirmPasswordInput.value) {
            confirmPasswordInput.setCustomValidity('Le password non corrispondono');
            isValid = false;
        } else {
            confirmPasswordInput.setCustomValidity('');
        }

        // Validate terms acceptance
        if (!termsCheck.checked) {
            termsCheck.setCustomValidity('Devi accettare i termini e condizioni');
            isValid = false;
        } else {
            termsCheck.setCustomValidity('');
        }

        // Show validation
        registrationForm.classList.add('was-validated');

        if (isValid) {
            registrationForm.submit();  // 🔥 INVIA IL FORM AL SERVER
        }
      
    });

    // Clear custom validation on input
    const inputs = [firstNameInput, lastNameInput, emailInput, roleInput, isExternalInput, passwordInput, confirmPasswordInput];
    inputs.forEach(input => {
        input.addEventListener('input', () => {
            input.setCustomValidity('');
            registrationError.classList.add('d-none');
        });
    });

    termsCheck.addEventListener('change', () => {
        termsCheck.setCustomValidity('');
    });
});