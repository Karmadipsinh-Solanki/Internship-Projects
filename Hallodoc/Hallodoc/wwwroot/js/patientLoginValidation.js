document.getElementById('patientLogin').addEventListener('submit', function (event) {
    event.preventDefault();

    clearErrorMessages();

    const email = document.getElementById('email');
    const password = document.getElementById('password');

    if (!validateEmail(email.value)) {
        displayErrorMessage(email, 'Email is invalid');
    }

    if (password.value.length < 4) {
        displayErrorMessage(password, 'Password must be at least 4 characters long');
    }

    if (!email.value || !password.value) {
        if (!email.value) {
            displayErrorMessage(email, 'Email is required');
        }

        if (!password.value) {
            displayErrorMessage(password, 'Password is required');
        }
    } else {
        // Form is valid, submit the form or perform other actions here
        console.log('Form is valid, submitting...');
        this.submit();
    }
});

function validateEmail(email) {
    const re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

function displayErrorMessage(input, message) {
    input.setCustomValidity(message);
    input.reportValidity();
}

function clearErrorMessages() {
    const emailError = document.getElementById('emailError');
    const passwordError = document.getElementById('passwordError');

    emailError.innerText = '';
    passwordError.innerText = '';
}