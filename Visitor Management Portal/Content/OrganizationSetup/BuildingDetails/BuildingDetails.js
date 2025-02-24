document.addEventListener("DOMContentLoaded", () => {

    const tabs = document.querySelectorAll('.nav-tabs li a');
    const contents = document.querySelectorAll('div[id^="tab"]');

    function activateTab(tab) {
        tabs.forEach(t => t.parentElement.classList.remove('active'));
        contents.forEach(content => content.style.display = 'none');
        tab.parentElement.classList.add('active');
        const target = document.querySelector(tab.dataset.target);
        const url = tab.dataset.url;
        if (url.includes('ZonesDetails')) {
            ZonesTabInit();
        }
        // Initialize specific tab functionality if needed
        //if (url.includes('MeetingAreasDetails')) {
        //    MeetingAreasTabInit();
        //}


        target.style.display = 'block';
    }

    // Add event listeners to tabs
    tabs.forEach(tab => {
        tab.addEventListener('click', function (event) {
            event.preventDefault();
            activateTab(this);
        });
    });

    const defaultTab = document.querySelector('.nav-tabs li a.active') || tabs[0];
    if (defaultTab) {
        activateTab(defaultTab);
    }

    // Zones Tab Initialization Function
    function ZonesTabInit() {
        const table = $('#zonesTable').DataTable({
            pageLength: 5,
            lengthChange: false,
            destroy: true,
            responsive: true,
            autoWidth: false,
            ordering: true,
            info: true,
            paging: true,
            searching: true,
            columnDefs: [
                { orderable: false, targets: [1] },

            ],
            buttons: ['print'],
            columns: [
                { width: '100px' },
                { width: '140px' },
                { width: '220px' },
                { width: '150px' },

            ]
        });


        const datatableSearchInput = document.querySelector('#zonesTable_filter input[type="search"]');
        const pageSearchInput = document.querySelector(".zones_topSection .search-bar input");

        // For search bar
        if (datatableSearchInput && pageSearchInput) {
            pageSearchInput.addEventListener("input", (e) => {
                datatableSearchInput.value = e.target.value;
                table.search(e.target.value).draw();
            });
        }
    }





    document.querySelector('.switch input').addEventListener('change', function () {
        const statusText = document.querySelector('.building-status .status-text');

        if (this.checked) {
            statusText.classList.remove('inactive');
            statusText.classList.add('active');
        } else {
            statusText.classList.remove('active');
            statusText.classList.add('inactive');
        }
    });

});

function UpdateBuildingExcludeFromOfficeStatus(elem) {
    var $checkbox = $(elem);
    var buildingId = $checkbox.data("id");
    var previousState = $checkbox.prop("checked");
    var newState = !previousState;

    console.log("Building ID:", buildingId);
    console.log("New Checked Status:", newState);

    $.ajax({
        url: "/OrganizationSetup/UpdateBuildingExcludeFromOfficeStatus",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ id: buildingId, isExcludeFromOfficeSelection: newState }),
        success: function (response) {
            if (response.Status) {
                Notify(response.Message, NotifyStatus.Success);
            } else {
                Notify(response.Message, NotifyStatus.Error);
                $checkbox.prop("checked", previousState);
            }
        },
        error: function () {
            Notify("An error occurred while updating status.", NotifyStatus.Error);
            $checkbox.prop("checked", previousState);
        }
    });
}

//update building data
$(".edit-data").on("click", function () {
    let $this = $(this);

    $("#model-building-code").val($this.attr("data-code"));
    $("#model-building-name").val($this.attr("data-name"));
    $("#model-building-address").val($this.attr("data-address"));
    $("#model-building-Location").val($this.attr("data-location"));

    let contactPersonId = $this.attr("data-contactpersone");
    let $contactPersonDropdown = $("#contact-person");

    if ($contactPersonDropdown.find("option[value='" + contactPersonId + "']").length > 0) {
        $contactPersonDropdown.val(contactPersonId);
    } else {
        $contactPersonDropdown.val("");
    }
    $("#building-id").val($this.attr("data-id"));

});

$("#editBuildingBtn").on("click", function () {

    let id = $("#building-id").val();

    let code = $("#model-building-code").val();
    let name = $("#model-building-name").val();
    let address = $("#model-building-address").val();
    let location = $("#model-building-Location").val();
    let contactPersonId = $("#contact-person").val();
    var contactPersonName = $('#contact-person option:selected').data('name');
    console.log(contactPersonName);
    if (name === "") {
        HighlightInput($("#model-building-name"), 1000);
        return;
    }

    if (location) {
        try {

            new URL(location);
        } catch (e) {
            HighlightInput($("#model-building-Location"), 1000);
            return;
        }
    }


    let formData = new FormData();
    formData.append("Id", id);
    formData.append("Code", code);
    formData.append("Name", name);
    formData.append("Address", address);
    formData.append("LocationLink", location);
    formData.append("ContactPersonId", contactPersonId || null);
    $("#editBuildingModel").modal("hide");

    $.ajax({
        url: "/OrganizationSetup/UpdateBuildingDetails",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.Status) {

                Notify(response.Message, NotifyStatus.Success);

                //update data-*
                let editLink = $(".edit-data[data-id='" + id + "']");

                editLink.attr("data-code", code);
                editLink.attr("data-name", name);
                editLink.attr("data-address", address);
                editLink.attr("data-location", location);
                editLink.attr("data-contactpersone", contactPersonId);
                // Update text in HTML
                $('#code_text').text(code || '--').attr('title', code || '--');
                $('#name_text').text(name || '--').attr('title', name || '--');
                $('#address_text').text(address || '--').attr('title', address || '--');

                if (location) {
                    $('#location_text')
                        .text(location)
                        .attr('title', location)
                        .attr('href', location)
                        .removeClass('no-link');

                } else {
                    $('#location_text')
                        .text('--')
                        .attr('title', '--')
                        .removeAttr('href')
                        .addClass('no-link');
                }

                if (contactPersonId && contactPersonId.trim() !== "") {

                    $('#contact_persone_text')
                        .text(contactPersonName || '--')
                        .attr('title', contactPersonName || '--');
                } else {

                    $('#contact_persone_text')
                        .text('--')
                        .attr('title', '');
                }


            } else {
                Notify(response.Message, NotifyStatus.Error);
            }
        },
        error: function (error) {
            console.error("Error:", error);
            Notify("An error occurred while creating request, Please try again", NotifyStatus.Error);
        }
    });
});

//==============add new zone====================
//function toggleEmptyMessage() {
//    let rowCount = $("#zonesTable tbody tr").length;
//    if (rowCount === 0) {
//        $("#zonesTable tbody").html(`<tr class="empty-row"><td colspan="4" class="text-center text-muted">No zones added yet.</td></tr>`);
//    } else {
//        $(".empty-row").remove();
//    }
//}


$('#addZoneModel').on('hidden.bs.modal', function () {
    $("#model-zone-name").val("");
    $("#model-zone-code").val("");
    $("#exclude-from-office-selection").prop("checked", false);
});


function addNewZone() {
    let zoneName = $("#model-zone-name").val().trim();
    let zoneCode = $("#model-zone-code").val().trim();
    let isExcluded = $("#exclude-from-office-selection").prop("checked");
    let buildingId = $("#zone-building-id").val().trim();

    if (zoneName === "") {
        HighlightInput($("#model-zone-name"), 1000);
        return;
    }

    $('#addZoneModel').modal('hide');
    $("#newZonesTable tbody .empty-row").remove();

    let formData = new FormData();

    formData.append("Name", zoneName);
    formData.append("Code", zoneCode);
    formData.append("ExcludeFromOfficeToggle", isExcluded);
    formData.append("BuildingId", buildingId);

    $.ajax({
        url: "/OrganizationSetup/AddZone",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.Status) {

                Notify(response.Message, NotifyStatus.Success);
                //append tr in table and draw table 
                let data = {
                    Id: response.Id,
                    Name: zoneName,
                    Code: zoneCode,
                    IsExcludeFromOfficeSelection: isExcluded,
                };
                drawZoneTableRow(data);
                callMeetingPartialAgain();

            } else {
                Notify(response.Message, NotifyStatus.Error);
            }
        },
        error: function (error) {
            console.error("Error:", error);
            Notify("An error occurred while creating zone", NotifyStatus.Error);
        }
    });

    // toggleEmptyMessage();
}

function drawZoneTableRow(data) {
    let table = $("#zonesTable").DataTable();

    let rowIndex = table.row.add([
        data.Code,
        data.Name,
        `<label class="switch not-allowed-content">
                <input type="checkbox" ${data.IsExcludeFromOfficeSelection ? "checked" : ""} disabled>
                <span class="slider"></span>
            </label>`,
        `<span class="fs-5 link-dark editZone" style="margin-right:1.5rem;"
                  onclick="editZone(this)"
                  data-id="${data.Id}" 
                  data-code="${data.Code}" 
                  data-name="${data.Name}" 
                  data-isexclude="${data.IsExcludeFromOfficeSelection}">
                <i class="fa-solid fa-pencil fs-5"></i>
            </span>
            <span class="fs-5 link-dark deleteZone" onclick="deleteZone('${data.Id}')">
                <i class="fa-regular fa-trash-can"></i>
            </span>`
    ]).draw(false).index();

    let rowNode = table.row(rowIndex).node();
    $(rowNode).attr("data-id", data.Id);
    $(rowNode).attr("data-name", data.Name);
}

//============add new zone==================

//===========delete zone====================
function deleteZone(zoneId) {
    $("#request_id").val(zoneId);
    $("#deleteModal").modal("show");
}

$("#confirmDelete").on("click", function () {
    let zoneId = $("#request_id").val();
    let table = $("#zonesTable").DataTable();

    $("#deleteModal").modal("hide");

    $.ajax({
        url: "/OrganizationSetup/DeleteZone",
        type: "POST",
        data: { zoneId: zoneId },
        success: function (response) {
            if (response.Status) {
                Notify(response.Message, NotifyStatus.Success);
                let row = $("#zonesTable").find(`tr[data-id='${zoneId}']`);
                table.row(row).remove().draw(false);
                callMeetingPartialAgain();
            } else {
                Notify(response.Message, NotifyStatus.Error);
            }
        },
        error: function (error) {
            console.error("Error:", error);
            Notify("An error occurred while deleting the zone", NotifyStatus.Error);
        }
    });

    $("#request_id").val('');
    // toggleEmptyMessage();
});


//===========delete zone====================



function updateZone(updatedData) {
    let table = $("#zonesTable").DataTable();

    // Find the row using the 'data-id' attribute
    let row = table.row($(`#zonesTable tbody tr[data-id="${updatedData.Id}"]`));

    // If the row exists, update its data
    if (row.any()) {
        row.data([
            updatedData.Code || "--",
            updatedData.Name,
            `<label class="switch not-allowed-content">
                <input type="checkbox" ${updatedData.IsExcludeFromOfficeSelection ? "checked" : ""} disabled>
                <span class="slider"></span>
            </label>`,
            `<span class="fs-5 link-dark editZone" style="margin-right:1.5rem;"
                  onclick="editZone(this)"
                  data-id="${updatedData.Id}" 
                  data-code="${updatedData.Code}" 
                  data-name="${updatedData.Name}" 
                  data-isexclude="${updatedData.IsExcludeFromOfficeSelection}">
                <i class="fa-solid fa-pencil fs-5"></i>
            </span>
            <span class="fs-5 link-dark deleteZone" onclick="deleteZone('${updatedData.Id}')">
                <i class="fa-regular fa-trash-can"></i>
            </span>`
        ]).draw(false);
    }
}

function editZone(element) {

    $("#zone-id-up").val('');
    $("#model-zone-name-up").val("");
    $("#model-zone-code-up").val("");
    $("#exclude-from-office-selection-up").prop("checked", false);

    let zoneId = $(element).attr("data-id");
    let zoneCode = $(element).attr("data-code");
    let zoneName = $(element).attr("data-name");
    let isExclude = $(element).attr("data-isexclude");
    console.log(isExclude);
    if (isExclude === 'True' || isExclude === 'true') {
        isExclude = true;
    } else {
        isExclude = false;
    }

    $("#zone-id-up").val(zoneId);
    $("#model-zone-name-up").val(zoneName);
    $("#model-zone-code-up").val(zoneCode);
    $("#exclude-from-office-selection-up").prop("checked", isExclude);

    $("#editZoneModel").modal("show");
}


$("#editZoneBtn").on("click", function () {
    let buildingId = $("#zone-building-id-up").val();
    let zoneId = $("#zone-id-up").val();
    let updatedName = $("#model-zone-name-up").val();
    let updatedCode = $("#model-zone-code-up").val();
    let isExclude = $("#exclude-from-office-selection-up").is(":checked");

    if (updatedName === "") {
        HighlightInput($("#model-zone-name-up"), 1000);
        return;
    }


    let formData = new FormData();

    formData.append("Id", zoneId);
    formData.append("BuildingId", buildingId);
    formData.append("Name", updatedName);
    formData.append("Code", updatedCode);
    formData.append("ExcludeFromOfficeToggle", isExclude);

    $("#editZoneModel").modal("hide");

    $.ajax({
        url: "/OrganizationSetup/EditZone",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.Status) {
                Notify(response.Message, NotifyStatus.Success);
                updateZoneRow(zoneId, updatedName, updatedCode, isExclude);
                callMeetingPartialAgain();
            } else {
                Notify(response.Message, NotifyStatus.Error);
            }
        },
        error: function (error) {
            console.error("Error:", error);
            Notify("An error occurred while updating the zone", NotifyStatus.Error);
        }
    });

});

function updateZoneRow(zoneId, updatedName, updatedCode, isExclude) {
    let table = $("#zonesTable").DataTable();

    let row = $("#zonesTable").find(`tr[data-id='${zoneId}']`);
    let rowIndex = table.row(row).index();

    table.row(rowIndex).data([
        updatedCode,
        updatedName,
        `<label class="switch not-allowed-content">
                <input type="checkbox" ${isExclude ? "checked" : ""} disabled>
                <span class="slider"></span>
            </label>`,
        `<span class="fs-5 link-dark editZone" style="margin-right:1.5rem;"
                  onclick="editZone(this)"
                  data-id="${zoneId}" 
                  data-code="${updatedCode}" 
                  data-name="${updatedName}" 
                  data-isexclude="${isExclude}">
                <i class="fa-solid fa-pencil fs-5"></i>
            </span>
            <span class="fs-5 link-dark deleteZone" onclick="deleteZone('${zoneId}')">
                <i class="fa-regular fa-trash-can"></i>
            </span>`
    ]).draw(false);
}



//============================meeting areas handling================================
$("#navigate_to_areas").on("click", function (e) {
    e.preventDefault();

    let buildingId = $(this).attr("data-buildingid");

    if (!$('#tab3').hasClass("loaded")) {

        loadMeetingAreaPartial(buildingId);
    }

});

// MeetingAreas Tab Initialization Function
function MeetingAreasTabInit() {
    const table = $('#meetingAreasTable').DataTable({
        pageLength: 5,
        lengthChange: false,
        destroy: true,
        responsive: true,
        autoWidth: false,
        ordering: true,
        info: true,
        paging: true,
        searching: true,
        columnDefs: [
            { orderable: false, targets: [1] },

        ],
        buttons: ['print'],
        columns: [
            { width: '150px' },
            { width: '150px' },
            { width: '150px' },
            { width: '250px' },
            { width: '100px' },

        ],
        drawCallback: function (settings) {
            $('#meetingAreasTable tbody tr').each(function (index) {
                if (index % 2 === 0) {
                    this.style.backgroundColor = 'red';
                    console.log(this.style.backgroundColor);
                    $(this).css('background-color', 'green');
                } else {
                    $(this).css('background-color', 'red');
                }
            });
        }
    });


    const datatableSearchInput = document.querySelector('#meetingAreasTable_filter input[type="search"]');
    const pageSearchInput = document.querySelector(".meetingAreas_topSection  .search-bar input");


    if (datatableSearchInput && pageSearchInput) {
        pageSearchInput.addEventListener("input", (e) => {
            datatableSearchInput.value = e.target.value;
            table.search(e.target.value).draw();
        });
    }
}

function callMeetingPartialAgain() {
    $('#tab3').removeClass("loaded");

}

function loadMeetingAreaPartial(buildingId, showMsg = true) {
    if (showMsg) {
        $('#tab3').html('<p class="text-center mt-4">Loading data...</p>');
    }
    console.log('call meetings')
    $.ajax({
        url: "/OrganizationSetup/MeetingAreasByBuilding",
        type: "GET",
        global: false,
        data: { buildingId: buildingId },
        success: function (data) {
            $('#tab3').html(data).addClass("loaded");
            MeetingAreasTabInit();
        },
        error: function (xhr, status, error) {
            console.error("Error loading tab content:", error);
            if (showMsg) {
                $("#tab3").html('<p class="text-center text-danger">Failed to load data. Please try again.</p>');
            }
        }
    });
}
function saveMeetingArea() {
    let name = $("#model-meeting-area-name").val().trim();
    let code = $("#model-meeting-area-code").val().trim();
    let appearInVisitRequests = $("#available-for-visit-requests").is(":checked");
    let buildingId = $("#meeting-area-building-id").val();
    let zoneId = $("#model-related-zone").val();
    let isValidArea = true;

    if (name === "") {
        HighlightInput($("#model-meeting-area-name"), 1000);
        isValidArea = false;
    }

    if (zoneId === "") {
        HighlightInput($("#model-related-zone"), 1000);
        isValidArea = false;
    }

    if (!isValidArea) {
        return;
    }

    let formData = new FormData();

    formData.append("Name", name);
    formData.append("Code", code);
    formData.append("AppearInVisitRequests", appearInVisitRequests);
    formData.append("BuildingId", buildingId);
    formData.append("ZoneId", zoneId);

    $("#addMeetingAreaModal").modal("hide");

    $.ajax({
        url: "/OrganizationSetup/AddMeetingArea",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.Status) {

                Notify(response.Message, NotifyStatus.Success);
                //call partial
                loadMeetingAreaPartial(buildingId, false);

            } else {
                Notify(response.Message, NotifyStatus.Error);
            }
        },
        error: function (error) {
            console.error("Error:", error);
            Notify("An error occurred while creating meeting area", NotifyStatus.Error);
        }
    });

}

$(document).on('hidden.bs.modal', '#addMeetingAreaModal', function () {
    $("#model-meeting-area-name").val('');
    $("#model-meeting-area-code").val('');
    $("#available-for-visit-requests").prop("checked", false);
    $("#model-related-zone").val('');
});



function deleteArea(areaId) {
    $("#request_area_id").val(areaId);
    $("#deleteAreaModal").modal("show");
}

function confirmDeleteArea() {
    let areaId = $("#request_area_id").val();

    $("#deleteAreaModal").modal("hide");

    $.ajax({
        url: "/OrganizationSetup/DeleteMeetingArea",
        type: "POST",
        data: { areaId: areaId },
        success: function (response) {
            if (response.Status) {
                Notify(response.Message, NotifyStatus.Success);
                let buildingId = $('#meeting-area-building-id').val();
                loadMeetingAreaPartial(buildingId, false);

            } else {
                Notify(response.Message, NotifyStatus.Error);
            }
        },
        error: function (error) {
            console.error("Error:", error);
            Notify("An error occurred while deleting the area", NotifyStatus.Error);
        }
    });

    $("#request_area_id").val('');
}

function updateArea(element) {
    let meetingAreaId = $(element).data("id");
    let name = $(element).data("name");
    let code = $(element).data("code");
    let zoneId = $(element).data("zone");
    let available = $(element).data("availabilty");

    // Populate modal fields
    $("#editMeetingAreaModal").data("meeting-area-id", meetingAreaId);
    $("#model-meeting-area-name-up").val(name);
    $("#model-meeting-area-code-up").val(code);
    $("#available-for-visit-requests-up").prop("checked", available === 'True' ? true : false);
    if ($("#model-related-zone-up option[value='" + zoneId + "']").length > 0) {
        $("#model-related-zone-up").val(zoneId);
    } else {
        $("#model-related-zone-up").val('');
    }
}

function updateMeetingArea() {
    let meetingAreaId = $("#editMeetingAreaModal").data("meeting-area-id");
    let name = $("#model-meeting-area-name-up").val().trim();
    let code = $("#model-meeting-area-code-up").val().trim();
    let appearInVisitRequests = $("#available-for-visit-requests-up").is(":checked");
    let buildingId = $("#meeting-area-building-id-up").val();
    let zoneId = $("#model-related-zone-up").val();
    let isValidArea = true;

    if (name === "") {
        HighlightInput($("#model-meeting-area-name-up"), 1000);
        isValidArea = false;
    }

    if (zoneId === "") {
        HighlightInput($("#model-related-zone-up"), 1000);
        isValidArea = false;
    }

    if (!isValidArea) {
        return;
    }

    let formData = new FormData();
    formData.append("Id", meetingAreaId);
    formData.append("Name", name);
    formData.append("Code", code);
    formData.append("AppearInVisitRequests", appearInVisitRequests);
    formData.append("BuildingId", buildingId);
    formData.append("ZoneId", zoneId);

    $("#editMeetingAreaModal").modal("hide");

    $.ajax({
        url: "/OrganizationSetup/EditMeetingArea",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.Status) {
                Notify(response.Message, NotifyStatus.Success);
                // Reload the list
                loadMeetingAreaPartial(buildingId, false);
            } else {
                Notify(response.Message, NotifyStatus.Error);
            }
        },
        error: function (error) {
            console.error("Error:", error);
            Notify("An error occurred while updating the meeting area", NotifyStatus.Error);
        }
    });
}

$(document).on('hidden.bs.modal', '#editMeetingAreaModal', function () {
    $("#model-meeting-area-name-up").val('');
    $("#model-meeting-area-code-up").val('');
    $("#available-for-visit-requests-up").prop("checked", false);
    $("#model-related-zone-up").val('');
});

//============================meeting areas handling================================
