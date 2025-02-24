document.addEventListener("DOMContentLoaded", function () {
    // Get DOM elements
    const steps = document.querySelectorAll('.step');
    const stepBars = document.querySelectorAll('.step-bar');
    const backBtn = document.querySelector('.back');
    const cancelBtn = document.querySelector('.cancel');
    const continueBtn = document.querySelector('.continue');
    let currentStep = 1;

    function updateSteps() {
        steps.forEach((step, index) => {
            if (index + 1 === currentStep) {
                step.classList.remove('d-none');
            } else {
                step.classList.add('d-none');
            }
        });

        stepBars.forEach((bar, index) => {
            if (index + 1 === currentStep) {
                bar.classList.add('step-bar-active');
            } else {
                bar.classList.remove('step-bar-active');
            }
        });

        backBtn.classList.toggle('d-none', currentStep === 1);

        continueBtn.textContent = currentStep === steps.length ? 'Finish' : 'Continue';
    }

    function validateRequiredFields(containerId) {
        let isValid = true;
        let container = document.getElementById(containerId);

        container.querySelectorAll('.required-input').forEach(input => {
            if (input.value.trim() === '') {
                isValid = false;
                HighlightInput($(input), 1000);
            }
        });

        return isValid;
    }

    continueBtn.addEventListener('click', () => {
        if (!validateRequiredFields('buildingForm')) {
            return;
        }
        if (currentStep < steps.length) {
            currentStep++;
            updateSteps();
        } else {
            collectAndSendBuildingWithZonesAndMeetingAreas();
        }
    });

    backBtn.addEventListener('click', () => {
        if (currentStep > 1) {
            currentStep--;
            updateSteps();
        }
    });

    cancelBtn.addEventListener('click', () => {
        window.location.href = '/OrganizationSetup/Index';
    });

    updateSteps();
});

//================================================================

let zones = [];
let meetingAreas = [];

function toggleEmptyMessage() {
    let rowCount = $("#newZonesTable tbody tr").length;
    if (rowCount === 0) {
        $("#newZonesTable tbody").html(`<tr class="empty-row"><td colspan="4" class="text-center text-muted">No zones added yet.</td></tr>`);
    } else {
        $(".empty-row").remove();
    }
}

//add Zone
$("#saveZoneBtn").click(function () {
    let zoneName = $("#model-zone-name").val().trim();
    let zoneCode = $("#model-zone-code").val().trim();
    let isExcluded = $("#exclude-from-office-selection").prop("checked") ? "Yes" : "No";

    if (zoneName === "") {
        HighlightInput($("#model-zone-name"), 1000);
        return;
    }

    zones.push({ name: zoneName, code: zoneCode, isExcluded: isExcluded });

    $('#addZoneModel').modal('hide');

    $("#newZonesTable tbody .empty-row").remove();

    let newRow = `
        <tr>
            <td>${zoneName}</td>
            <td>${zoneCode || "--"}</td>
            <td>
                <label class="switch">
                    <input type="checkbox" ${isExcluded === "Yes" ? "checked" : ""}>
                    <span class="slider"></span>
                </label>
            </td>
            <td><a class="fs-5 link-dark delete-zone"><i class="fa-regular fa-trash-can"></i></a></td>
        </tr>`;

    $("#newZonesTable tbody").append(newRow);

    populateZoneSelect();
    toggleEmptyMessage();
});

//clear Zone Modal
$('#addZoneModel').on('hidden.bs.modal', function () {
    $("#model-zone-name").val("");
    $("#model-zone-code").val("");
    $("#exclude-from-office-selection").prop("checked", false);
});

//remove Zone
$(document).on("click", ".delete-zone", function () {
    //remove the zone from the zones array
    let zoneName = $(this).closest("tr").find("td:first").text();
    zones = zones.filter(zone => zone.name !== zoneName);

    //remove related meeting areas
    meetingAreas = meetingAreas.filter(area => area.relatedZone !== zoneName);
    $(`#newMeetingsAreasTable tbody tr`).each(function () {
        if ($(this).find("td:eq(2)").text().trim() === zoneName) {
            console.log(zoneName)
            $(this).remove();
        }
    });

    $(this).closest("tr").remove();

    populateZoneSelect();
    toggleEmptyMessage();
    toggleEmptyMessageForMeetingArea();
});

//populate Zone Select Dropdown
function populateZoneSelect() {
    $('#model-related-zone').empty();
    $('#model-related-zone').append('<option value="">Select Zone</option>');
    zones.forEach(zone => {
        $('#model-related-zone').append(`<option value="${zone.name}">${zone.name}</option>`);
    });
}

//for Meeting Areas
function toggleEmptyMessageForMeetingArea() {
    let rowCount = $("#newMeetingsAreasTable tbody tr").length;
    if (rowCount === 0) {
        $("#newMeetingsAreasTable tbody").html(`<tr class="empty-meeting-row"><td colspan="4" class="text-center text-muted">No Meeting Areas added yet.</td></tr>`);
    } else {
        $(".empty-meeting-row").remove();
    }
}

//add Meeting Area
$("#saveMeetingAreaBtn").click(function () {
    let isValidArea = true;
    let meetingAreaName = $("#model-meeting-area-name").val().trim();
    let relatedZone = $("#model-related-zone").val();
    let meetingAreaCode = $("#model-meeting-area-code").val().trim();
    let isAvailable = $("#available-for-visit-requests").prop("checked") ? "Yes" : "No";

    if (meetingAreaName === "") {
        HighlightInput($("#model-meeting-area-name"), 1000);
        isValidArea = false;
    }

    if (relatedZone === "") {
        HighlightInput($("#model-related-zone"), 1000);
        isValidArea = false;
    }

    if (!isValidArea) {
        return;
    }

    //add meeting area to the array
    meetingAreas.push({ name: meetingAreaName, code: meetingAreaCode, relatedZone: relatedZone, isAvailable: isAvailable });

    $('#addMeetingAreaModal').modal('hide');

    $("#newMeetingsAreasTable tbody .empty-meeting-row").remove();

    let newRow = `
        <tr>
            <td>${meetingAreaName}</td>
            <td>${meetingAreaCode || "--"}</td>
            <td>${relatedZone}</td>
            <td>
                <label class="switch">
                    <input type="checkbox" ${isAvailable === "Yes" ? "checked" : ""}>
                    <span class="slider"></span>
                </label>
            </td>
            <td><a class="fs-5 link-dark delete-area"><i class="fa-regular fa-trash-can"></i></a></td>
        </tr>`;

    $("#newMeetingsAreasTable tbody").append(newRow);

    toggleEmptyMessageForMeetingArea();
});

//clear Meeting Area Modal
$('#addMeetingAreaModal').on('hidden.bs.modal', function () {
    $("#model-meeting-area-name").val("");
    $("#model-meeting-area-code").val("");
    $("#model-related-zone").val("");
    $("#available-for-visit-requests").prop("checked", false);
});

//remove Meeting Area
$(document).on("click", ".delete-area", function () {
    //remove the meeting area from the array
    let meetingAreaName = $(this).closest("tr").find("td:first").text();
    meetingAreas = meetingAreas.filter(area => area.name !== meetingAreaName);

    $(this).closest("tr").remove();

    toggleEmptyMessageForMeetingArea();
});

function collectZoneFormData() {
    let formData = new FormData();

    $("#newZonesTable tbody tr").each(function (index, row) {
        if ($(row).hasClass("empty-row")) return;

        let zoneName = $(row).find("td:eq(0)").text().trim();
        let zoneCode = $(row).find("td:eq(1)").text().trim();
        let isExcluded = $(row).find("input[type='checkbox']").is(":checked");

        formData.append(`Zones[${index}].Name`, zoneName);
        formData.append(`Zones[${index}].Code`, zoneCode);
        formData.append(`Zones[${index}].IsExcludeFromOfficeSelection`, isExcluded);
    });

    return formData;
}


function collectAreaFormData() {
    let formData = new FormData();

    $("#newMeetingsAreasTable tbody tr").each(function (index, row) {
        if ($(row).hasClass("empty-meeting-row")) return; // Skip empty row

        let meetingAreaName = $(row).find("td:eq(0)").text().trim();
        let meetingAreaCode = $(row).find("td:eq(1)").text().trim();
        let relatedZone = $(row).find("td:eq(2)").text().trim();
        let isAvailable = $(row).find("input[type='checkbox']").is(":checked");

        formData.append(`MeetingAreas[${index}].Name`, meetingAreaName);
        formData.append(`MeetingAreas[${index}].Code`, meetingAreaCode);
        formData.append(`MeetingAreas[${index}].IsAvailableForVisitRequests`, isAvailable);
        formData.append(`MeetingAreas[${index}].ZoneName`, relatedZone);
    });

    return formData;
}

function collectAndSendBuildingWithZonesAndMeetingAreas() {
    let formData = new FormData();

    let buildingName = $("#building-name").val().trim();
    let buildingCode = $("#building-code").val().trim();
    let buildingAddress = $("#building-address").val().trim();
    let buildingLink = $("#location-link").val().trim();
    let contactPerson = $("#contact-person").val();
    let isExcludeBuilding = $("#exclude-from-office").is(":checked");

    let zoneData = collectZoneFormData();
    let areaData = collectAreaFormData();

    formData.append("Code", buildingCode);
    formData.append("Name", buildingName);
    formData.append("Address", buildingAddress);
    formData.append("LocationLink", buildingLink);
    formData.append("ContactPersonId", contactPerson);
    formData.append("IsExcludeFromOfficeSelection", isExcludeBuilding);

    if (zoneData) {
        for (let pair of zoneData.entries()) {
            formData.append(pair[0], pair[1]);
        }
    }

    if (areaData) {
        for (let pair of areaData.entries()) {
            formData.append(pair[0], pair[1]);
        }
    }


    $.ajax({
        url: "/OrganizationSetup/AddBuildingWithZonesAndMeetingAreas",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.Status) {

                Notify(response.Message, NotifyStatus.Success);
                setTimeout(function () {
                    window.location.href = '/OrganizationSetup/Index';
                }, 1500);

            } else {
                Notify(response.Message, NotifyStatus.Error);
            }
        },
        error: function (error) {
            console.error("Error:", error);
            Notify("An error occurred while creating request, Please try again", NotifyStatus.Error);
        }
    });
}