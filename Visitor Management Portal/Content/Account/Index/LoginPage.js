document.addEventListener('DOMContentLoaded', function () {

    CheckForErrorMessages();

    const email = document.querySelector('#email');
    const password = document.querySelector('#password');
    // Wrapper Elements
    const signUpWrapper = document.querySelector('.form-wrapper.singUp-form-wrapper');
    const loginWrapper = document.querySelector('.login-form-wrapper');
    const signUpTabsWrapper = document.querySelector('.singUp-NavTabs');
    const forgotPasswordWrapper = document.querySelector('.resetPassword-NavTabs');

    // Navigation Buttons
    const goToSignUpBtn = document.querySelector('.go-to-singup-page');
    const goToLoginBtn = document.querySelector('.go-to-login-page');
    const startSignUpBtn = document.querySelector('.singup-btn');
    const backBtn = document.querySelector('.back-btn');
    const forgetPasswordBtn = document.querySelector('.forget-password-btn');
    const loginBtn = document.querySelector('.login-btn');

    // Step Navigation Buttons
    const continueBtn = document.querySelector('.Continue-btn');
    const completeRegisterBtn = document.querySelector('.Complete-SignUp-btn');

    // Step Wrappers
    const firstStepWrapper = document.querySelector('.first-singup-step');
    const lastStepWrapper = document.querySelector('.last-singup-step');

    // Progress Bar Elements
    const progressBarStep1 = document.querySelector('.bar-1');
    const progressBarStep2 = document.querySelector('.bar-2');


    // Reset Password Variables
    const resetPasswordNavTabs = document.querySelector('.resetPassword-NavTabs');
    const resetPasswordStepOne = document.querySelector('.first-resetpassword-step');
    const resetPasswordStepTwo = document.querySelector('.second-resetpassword-step');
    const resetPasswordStepThree = document.querySelector('.last-resetpassword-step');

    // Inputs and Buttons
    const resetPasswordEmailInput = document.querySelector('#reset-email');
    const resetPasswordActionBtn = document.querySelector('.Reset-Password-btn-first-step');
    const resetPasswordBackToLoginBtns = document.querySelectorAll('.Back-to-login-btn');
    const resetPasswordContinueBtn = document.querySelector('.Continue-btn-resetPassword');
    const resetPasswordCodeInputs = document.querySelectorAll('.code-input');
    const resetPasswordResendBtn = document.querySelector('.resend-email-btn');
    const completeResetPasswordBtn = document.querySelector('.Complete-Reset-Password-btn');
    const resetPasswordNewPasswordInput = document.getElementById('new-password');
    const resetPasswordConfirmPasswordInput = document.getElementById('confirm-password');


    /** ========================
     * Navigation Logic
     * ======================== */
    goToSignUpBtn.addEventListener('click', () => {
        loginWrapper.classList.add('d-none');
        signUpWrapper.classList.remove('d-none');
        document.title = "Sign Up";
    });

    goToLoginBtn.addEventListener('click', () => {
        loginWrapper.classList.remove('d-none');
        signUpWrapper.classList.add('d-none');
        document.title = "Sign Up";
    });

    startSignUpBtn.addEventListener('click', () => {
        backBtn.classList.remove('d-none');
        signUpWrapper.classList.add('d-none');
        signUpTabsWrapper.classList.remove('d-none');
    });

    backBtn.addEventListener('click', () => {
        if (!firstStepWrapper.classList.contains('d-none')) {
            // Back to sign-up start
            backBtn.classList.add('d-none');
            signUpTabsWrapper.classList.add('d-none');
            signUpWrapper.classList.remove('d-none');
        } else {
            // Back to first step from the second step
            firstStepWrapper.classList.remove('d-none');
            lastStepWrapper.classList.add('d-none');
            updateProgressBar(false);
        }
    });

    forgetPasswordBtn.addEventListener('click', () => {
        loginWrapper.classList.add('d-none');
        forgotPasswordWrapper.classList.remove('d-none');
    })


    loginBtn.addEventListener('click', (event) => {
        event.preventDefault();

        const email = document.querySelector('#Login-email').value;
        const password = document.querySelector('#password').value;

        $(document).trigger('ajaxStart');

        fetch('/Account/Login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                email: email,
                password: password
            })
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    Notify(data.message, NotifyStatus.Success);
                    window.location.href = '/Dashboard/Dashboard';
                } else {
                    Notify(data.message, NotifyStatus.Error);
                }
            })
            .catch(error => {
                console.error('Error:', error);
                Notify('An unexpected error occurred.', NotifyStatus.Error);
            })
            .finally(() => {
                $(document).trigger('ajaxStop');
            });
    });


    completeRegisterBtn.addEventListener('click', (event) => {
        event.preventDefault();

        const fullName = document.querySelector('#sign-up-fullName').value;
        const email = document.querySelector('#sign-up-email').value;
        const password = document.querySelector('#sign-up-password').value;
        const organizationName = document.querySelector('#sign-up-OrganizationName').value;
        const organizationDomain = document.querySelector('#sign-up-DomainName').value;

        $(document).trigger('ajaxStart');

        fetch('/Account/Register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                fullName: fullName,
                email: email,
                password: password,
                organizationName: organizationName,
                organizationDomain: organizationDomain
            })
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    Notify(data.message, NotifyStatus.Success);
                    window.location.href = '/Dashboard/Index';
                } else {
                    Notify(data.message, NotifyStatus.Error);
                }
            })
            .catch(error => {
                console.error('Error:', error);
                Notify('An unexpected error occurred.', NotifyStatus.Error);
            })
            .finally(() => {
                $(document).trigger('ajaxStop');
            });

    });

    /** ========================
     * Validation Logic
     * ======================== */
    const validateStep = (inputs, button) => {
        const allFilled = Array.from(inputs).every(input => input.value.trim() !== '');
        if (allFilled) {
            button.classList.remove('btn-non-active');
            button.disabled = false;
        } else {
            button.classList.add('btn-non-active');
            button.disabled = true;
        }
    };

    // First Step Validation
    const firstStepInputs = document.querySelectorAll('.first-singup-step input');
    firstStepInputs.forEach(input => {
        input.addEventListener('input', () => validateStep(firstStepInputs, continueBtn));
    });

    // Last Step Validation
    const lastStepInputs = document.querySelectorAll('.last-singup-step input');
    lastStepInputs.forEach(input => {
        input.addEventListener('input', () => validateStep(lastStepInputs, completeRegisterBtn));
    });

    /** ========================
     * Step Transition Logic
     * ======================== */
    continueBtn.addEventListener('click', () => {
        if (!continueBtn.classList.contains('btn-non-active')) {
            firstStepWrapper.classList.add('d-none');
            lastStepWrapper.classList.remove('d-none');
            updateProgressBar(true);
        }
    });

    /** ========================
     * Helper Functions
     * ======================== */
    const updateProgressBar = (toSecondStep) => {
        if (toSecondStep) {
            progressBarStep1.classList.remove('active-bar');
            progressBarStep1.classList.add('inactive-bar');
            progressBarStep2.classList.remove('inactive-bar');
            progressBarStep2.classList.add('active-bar');
        } else {
            progressBarStep1.classList.remove('inactive-bar');
            progressBarStep1.classList.add('active-bar');
            progressBarStep2.classList.remove('active-bar');
            progressBarStep2.classList.add('inactive-bar');
        }
    };

    /** ========================
   * Reset Password
   * ======================== */

    // Enable/Disable Reset Password Button based on email input
    resetPasswordEmailInput.addEventListener('input', () => {
        if (resetPasswordEmailInput.value.trim() !== '') {
            resetPasswordActionBtn.classList.remove('btn-non-active');
            resetPasswordActionBtn.disabled = false;
        } else {
            resetPasswordActionBtn.classList.add('btn-non-active');
            resetPasswordActionBtn.disabled = true;
        }
    });

    // Code Input Auto-Focus and Validation
    resetPasswordCodeInputs.forEach((input, index) => {
        input.addEventListener('input', () => {
            const value = input.value.trim();
            if (/^\d$/.test(value)) {
                input.style.borderColor = '#1E3A8A';
                if (index < resetPasswordCodeInputs.length - 1) {
                    resetPasswordCodeInputs[index + 1].focus();
                }
            } else {
                input.style.borderColor = '#ccc';
                input.value = '';
            }

            const allFilled = Array.from(resetPasswordCodeInputs).every(input => /^\d$/.test(input.value.trim()));
            if (allFilled) {

                console.log('All inputs filled');
                console.log(allFilled);
                resetPasswordContinueBtn.classList.remove('btn-non-active');
                resetPasswordContinueBtn.disabled = false;
            } else {
                resetPasswordContinueBtn.classList.add('btn-non-active');
                resetPasswordContinueBtn.disabled = true;
            }
        });

        input.addEventListener('keydown', (e) => {
            if (e.key === 'Backspace' && input.value === '' && index > 0) {
                resetPasswordCodeInputs[index - 1].focus();
            }
        });

        input.addEventListener('paste', (e) => {
            e.preventDefault();
        });
    });
    // Step Transitions
    // Helper Function to Set Active Bar
    function setActiveBars(activeBarIndices) {
        document.querySelectorAll('.Register-Step-bar .step-bar').forEach((bar, index) => {
            if (activeBarIndices.includes(index + 1)) {
                bar.classList.add('step-bar-active');
            } else {
                bar.classList.remove('step-bar-active');
            }
        });
    }

    // Step Transitions
    resetPasswordActionBtn.addEventListener('click', () => {

        // Send OTP Code to the email provided using AJAX

        SendOTPCodeToEmail(resetPasswordEmailInput.value);
    });

    function SendOTPCodeToEmail(email) {
        $.ajax({
            url: '/Account/ForgetPasswordOTP', // Replace with your actual controller name
            type: 'POST',
            data: { Email: email },
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    Notify(response.message, "Success"); // Display success message

                    resetPasswordStepOne.classList.add('d-none');
                    resetPasswordStepTwo.classList.remove('d-none');

                    setActiveBars([2]);
                } else {
                    Notify(response.message, "Error"); // Display error message

                    resetPasswordStepOne.classList.remove('d-none');
                    resetPasswordStepTwo.classList.add('d-none');

                    setActiveBars([1]);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error:", error);
                resetPasswordStepOne.classList.remove('d-none');
                resetPasswordStepTwo.classList.add('d-none');

                setActiveBars([1]);
                Notify("An error occurred while sending OTP. Please try again.", "Error");
            }
        });
    }


    resetPasswordContinueBtn.addEventListener('click', () => {

        ValidateOTP(resetPasswordCodeInputs);

    });

    function ValidateOTP(codeInputs) {
        // Extract OTP code from input fields
        let otpCode = Array.from(codeInputs).map(input => input.value).join('');

        // Get the email from the input field (you may need to adjust this selector)
        let email = resetPasswordEmailInput.value;

        // Ensure OTP and email are provided
        if (!email || otpCode.length !== codeInputs.length) {
            Notify("Please enter a valid OTP.", "Error");
            return;
        }

        // AJAX request to validate OTP
        $.ajax({
            url: '/Account/ValidateOTP', // Replace with actual controller name
            type: 'GET',
            data: { Email: email, OTP: otpCode },
            dataType: 'json',
            success: function (response) {
                if (response.Status) {
                    console.log(response);
                    //Notify(response.message, response.success); // Show success message
                    resetPasswordStepTwo.classList.add('d-none');
                    resetPasswordStepThree.classList.remove('d-none');
                    setActiveBars([3]); // Proceed to next step
                    resetPasswordStepTwo.classList.add('d-none');
                    resetPasswordStepThree.classList.remove('d-none');
                } else {
                    setActiveBars([2]);
                    resetPasswordStepTwo.classList.remove('d-none');
                    resetPasswordStepThree.classList.add('d-none');
                    Notify(response.Message, "Error"); // Show error message
                }
            },
            error: function (xhr, status, error) {
                console.error("Error:", error);
                setActiveBars([2])
                resetPasswordStepTwo.classList.remove('d-none');
                resetPasswordStepThree.classList.add('d-none');;
                Notify("An error occurred while validating OTP. Please try again.", "Error");
            }
        });
    }



    resetPasswordResendBtn.addEventListener('click', () => {
        //alert('Verification code resent to your email!');
    });

    resetPasswordBackToLoginBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            resetPasswordNavTabs.classList.add('d-none');
            loginWrapper.classList.remove('d-none');
        });
    });
    completeResetPasswordBtn.addEventListener('click', () => {

        resetPassword(resetPasswordNewPasswordInput.value, resetPasswordConfirmPasswordInput.value);
    });

    function resetPassword(newPassword, confirmPassword) {
        // Validate if passwords match
        if (newPassword !== confirmPassword) {
            Notify("Passwords do not match. Please try again.", "Error");
            return;
        }

        // Validate password strength (optional)
        if (newPassword.length < 3) {
            Notify("Password must be at least 3 characters long.", "Error");
            return;
        }

        // AJAX request to reset password
        $.ajax({
            url: '/Account/ResetPassword',
            type: 'POST',
            data: { Email: resetPasswordEmailInput.value, password: newPassword },
            dataType: 'json',
            success: function (response) {
                if (response.Status) {
                    Notify("Password reset successfully. You can now log in.", "Success");
                    resetPasswordNavTabs.classList.add('d-none');
                    loginWrapper.classList.remove('d-none');
                } else {
                    Notify(response.Message, "Error");
                    resetPasswordNavTabs.classList.remove('d-none');
                    loginWrapper.classList.add('d-none');
                }
            },
            error: function (xhr, status, error) {
                console.error("Error:", error);
                Notify("An error occurred while resetting the password.", "Error");
                resetPasswordNavTabs.classList.remove('d-none');
                loginWrapper.classList.add('d-none');
            }
        });
    }

});



function CheckForErrorMessages() {

    var message = errorMessage;
    var status = errorStatus;

    if (message) {
        Notify(message, status);
    }
}
