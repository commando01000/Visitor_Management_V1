document.addEventListener("DOMContentLoaded", function () {
    let currentStep = 1;
    const steps = document.querySelectorAll(".container > div[class^='step']");
    const progressBars = document.querySelectorAll(".progress_bar .progress");
    const rejectModal = document.getElementById("rejectModal");
    const rejectBtn = document.querySelector(".reject-btn");
    const noBtn = document.getElementById("no-btn");
    const qrCodeContainer = document.querySelector(".QrCodeImage");
    const confirmBtn = document.getElementById("btn-confirm-visit");

    function showStep(stepNumber) {
        steps.forEach((step, index) => {
            step.classList.toggle("hide", index + 1 !== stepNumber);
        });

        progressBars.forEach((progress, index) => {
            progress.classList.toggle("active", index + 1 === stepNumber);
        });

        currentStep = stepNumber;
    }

    document.querySelector(".continue-btn")?.addEventListener("click", function () {
        if (currentStep < steps.length) showStep(currentStep + 1);
        console.log(currentStep);
    });
    document.querySelector(".btn-cancel")?.addEventListener("click", function () {
        if (currentStep > 1) showStep(currentStep - 1);
        console.log(currentStep);
    });

    rejectBtn.addEventListener("click", function () {
        rejectModal.style.display = "flex";
    });

    // reject invitation
    const yesBtn = document.getElementById("yes-btn");
    yesBtn.addEventListener("click", function () {
        $.ajax({
            type: "POST",
            url: "/Visitor/RejectInvitation",
            data: { visitingMemberId: visitingMemberId },
            success: function (response) {
                if (response.Status) {
                    Notify(response.Message, NotifyStatus.Success, 3000);
                } else {
                    Notify(response.Message, NotifyStatus.Error);
                }
                rejectModal.style.display = "none";
            },
            error: function (xhr, status, error) {
                Notify("An error occurred: " + error, NotifyStatus.Error);
                rejectModal.style.display = "none";
            }
        });
    });

    function getVisitorData() {
        return {
            Id: visitorId,
            FullName: document.getElementById("visitor-FullName")?.value || "",
            IdNumber: document.getElementById("visitor-IdNumber")?.value || "",
            JobTitle: document.getElementById("visitor-JobTitle")?.value || "",
            EmailAddress: document.getElementById("visitor-EmailAddress")?.value || "",
            OrganizationName: document.getElementById("visitor-OrganizationName")?.value || "",
            PhoneNumber: document.getElementById("visitor-PhoneNumber")?.value || ""
        };
    }




    function sendVisitorData(visitorData, visitingMemberId) {
        return $.ajax({
            url: "/Visitor/AcceptInvitation",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                model: visitorData,
                visitingMemberId: visitingMemberId
            }),
            dataType: "json"
        }).done(function (data) {
            GenerateQrCode(shortCode, visitingMemberId); 
            Notify(data.Message, data.Status ? NotifyStatus.Success : NotifyStatus.Error, 3000);
        }).fail(function (xhr, status, error) {
            Notify("An error occurred: " + error, NotifyStatus.Error);
        });
    }

    function GenerateQrCode(shortCode, visitingMemberId) {
        return $.ajax({
            url: "/Visitor/GenerateQRCode", 
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                shortCode: shortCode,
                visitingMemberId: visitingMemberId
            }),
            dataType: "json"
        }).done(function (data) {
            qrCodeContainer.src = data.QrCode;
            if (currentStep < steps.length) showStep(currentStep + 1);
        }).fail(function (xhr, status, error) {
            Notify("An error occurred: " + error, NotifyStatus.Error);
        });
    }

    confirmBtn?.addEventListener("click", function (event) {
        event.preventDefault(); 

        const visitorData = getVisitorData();
        let isValid = true;
        $(".form-group input:not([readonly])").each(function () {
            if (!$(this).val().trim()) {
                $(this).addClass("input-error"); 
                isValid = false;
            } else {
                $(this).removeClass("input-error");
            }
        });

        if (!isValid) {
            Notify("Please complete all required fields before proceeding");
            return;
        }

        try {
            sendVisitorData(visitorData, visitingMemberId);
        } catch (error) {
            Notify("Something went wrong. Please try again.", NotifyStatus.Error);
        }
    });


    noBtn.addEventListener("click", function () {
        rejectModal.style.display = "none";
    });

    window.addEventListener("click", function (event) {
        if (event.target === rejectModal) rejectModal.style.display = "none";
    });

    showStep(1);
});


