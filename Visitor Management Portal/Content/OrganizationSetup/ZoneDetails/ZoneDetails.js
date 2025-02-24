document.addEventListener('DOMContentLoaded', function () {

    debugger;

    const table = $('#meetingAreasTable').DataTable({
        pageLength: 6,
        lengthChange: false,
        destroy: true,
        responsive: true,
        autoWidth: false,
        ordering: true,
        info: true,
        paging: true,
        searching: true,
        columnDefs: [
            { orderable: false, targets: [1] }
        ],
        buttons: ['print'],
        columns: [
            { width: '150px' },
            { width: '150px' },
            { width: '150px' },
            { width: '150px' },
            { width: '150px' },
            { width: '150px' },
        ]
    });

    // Custom search functionality
    const datatableSearchInput = document.querySelector('#meetingAreasTable_filter input[type="search"]');
    const pageSearchInput = document.querySelector(".meetingAreas_topSection .search-bar input");

    // For search bar
    if (datatableSearchInput && pageSearchInput) {
        pageSearchInput.addEventListener("input", (e) => {
            datatableSearchInput.value = e.target.value;
            table.search(e.target.value).draw();
        });
    }

    handleToggleZoneExcludeFromOfficeLogic();

    handleToggleAreaAvailabilityLogic();

    handleEditZone();

    handleEditZoneSubmit();

    handleDeleteAreaBtns();

    handleEditArea();

    handleEditAreaSubmit();

    handleAddAreaSubmit();

    handleSelectBuildingLogic();    

});

function handleToggleZoneExcludeFromOfficeLogic() {

    let toggle = document.querySelector("#zoneToggle");

    console.log("toggle", toggle);

    toggle.addEventListener("change", function () {

        debugger;

        //loader
        document.getElementById("Pageloading").style.visibility = "visible";

        let toggleValue = document.querySelector("#zoneToggle").getAttribute("data-ExcludeFromOfficeSelection") === "True";
        document.querySelector("#zoneToggle").getAttribute("data-ExcludeFromOfficeSelection") === toggleValue;

        console.log("toggleValue", toggleValue);

        let zoneId = toggle.getAttribute("data-id");

        const statusText = document.querySelector('.building-status .status-text');

        if (toggle.checked) {
            statusText.classList.remove('inactive');
            statusText.classList.add('active');
        } else {
            statusText.classList.remove('active');
            statusText.classList.add('inactive');
        }

        updateZoneExcludeStatus(zoneId);
    });
}

function updateZoneExcludeStatus(zoneId) {

    console.log("zoneId :", zoneId);

    fetch(`/OrganizationSetup/ToggleZoneExcludeFromOffice`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ zoneId: zoneId })
    })
        .then(response => response.json())
        .then(data => {
            console.log("data", data);
            if (data.Status) {
                console.log("Zone exclude status updated successfully");
                window.location.reload(); // TODO Can be refactored to be done manually ?
            } else {
                console.error("Error updating Zone exclude status:", data.Error);
            }
        })
        .catch(error => console.error("Error updating zone exclude status:", error));
}

function handleEditZone() {

    debugger;

    const editButton = document.querySelector("#editZoneBtn");

    editButton.addEventListener("click", function () {

        debugger;

        const id = editButton.getAttribute("data-id");
        const name = editButton.getAttribute("data-name");
        const code = editButton.getAttribute("data-code");
        const building = editButton.getAttribute("data-building");
        const excludeFromOffice = document.getElementById("zoneToggle").getAttribute("data-ExcludeFromOfficeSelection") === "True";

        console.log("Edit button clicked for zone with ID:", id);
        console.log("Edit button clicked for zone with Name:", name);
        console.log("Edit button clicked for zone with code:", code);
        console.log("Edit button clicked for zone with Building:", building);
        console.log("Edit button clicked for zone with excludeFromOffice:", excludeFromOffice);

        document.getElementById("selectedZoneId").setAttribute("value", id);
        document.getElementById("editModel-zone-name").value = name;
        document.getElementById("editModel-zone-code").value = code;
        document.getElementById("excludeFromOffice-toggle").checked = excludeFromOffice ? true : false;

        const buildingDropdown = document.getElementById("editZone-related-building");
        if (buildingDropdown) {

            let found = false;

            for (let option of buildingDropdown.options) {
                if (option.value === building) {
                    option.selected = true;
                    found = true;
                    break;
                }
            }

            if (!found) {
                buildingDropdown.selectedIndex = 0;
            }
        }
    });
}

function handleEditZoneSubmit() {
    debugger;

    document.getElementById("editZoneSubmitBtn").addEventListener("click", function () {
        debugger;

        let isValid = true;

        const id = document.getElementById("selectedZoneId");
        const name = document.getElementById("editModel-zone-name");
        const code = document.getElementById("editModel-zone-code");
        const excludeFromOfficeToggle = document.getElementById("excludeFromOffice-toggle").checked;
        const building = document.getElementById("editZone-related-building");

        console.log("id", id.value);
        console.log("name", name.value);
        console.log("code", code.value);
        console.log("ecludeFromOfficeToggle", excludeFromOfficeToggle);
        console.log("building", building.value);

        if (!id.value) {
            isValid = false;
            Notify("Please select an area to edit.", NotifyStatus.Error);
            return;
        }

        if (!building.value) {
            isValid = false;
            HighlightField(building);
        }

        if (!name.value) {
            isValid = false;
            HighlightField(name);
        }

        if (!isValid) {
            Notify("Please fill in all required fields.", NotifyStatus.Error);
            return;
        }

        console.log("Form is valid. Submitting data...");

        const editZoneVM = {
            id: id.value,
            buildingId: building.value,
            Name: name.value,
            code: code.value,
            excludeFromOfficeToggle: excludeFromOfficeToggle
        };

        console.log("Form is valid. Submitting data...", editZoneVM);

        //loader
        document.getElementById("Pageloading").style.visibility = "visible";

        fetch("/OrganizationSetup/EditZone", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(editZoneVM)
        })
            .then(response => {
                return response.json();
            })
            .then(data => {

                if (data.Status) {

                    console.log("Success:", data);

                    let modal = bootstrap.Modal.getInstance(document.getElementById("editZoneModal"));
                    if (modal) {
                        modal.hide();
                    }

                    Notify(data.Message, NotifyStatus.Success);

                    setTimeout(() => {
                        // TODO Make a better logic to append it directly or when reload make the selcted tab is Meeting
                        window.location.reload();

                    }, 3000);

                } else {
                    console.error("Error:", data);
                    Notify(data.Message, NotifyStatus.Error);
                }
            })
            .catch(error => {
                console.error("Error:", error);
                Notify("Failed to add Meeting Area.", NotifyStatus.Error);
            })
            .finally(() => {
                document.getElementById("Pageloading").style.visibility = "hidden";
                //resetAndHideNewZoneModel();
            });
    });
}

function handleToggleAreaAvailabilityLogic() {
    let toggles = document.querySelectorAll("#meetingAreasTable .area-toggle");

    if (toggles.length === 0) {
        console.log("No toggle switches found");
        return;
    }

    toggles.forEach(toggle => {
        toggle.addEventListener("change", function () {
            let modelId = this.getAttribute("data-model-id");
            updateModelStatus(modelId);
        });
    });
}

function updateModelStatus(meetingAreaId) {
    console.log("Meeting Area ID:", meetingAreaId);

    fetch(`/OrganizationSetup/ToggleMeetingAvailability`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ meetingAreaId: meetingAreaId })
    })
        .then(response => response.json())
        .then(data => {
            console.log("data", data);
            if (data.Status) {
                console.log("Meeting status updated successfully");
            } else {
                console.error("Error updating meeting status:", data.Error);
            }
        })
        .catch(error => console.error("Error updating status:", error));
}

function handleDeleteAreaBtns() {

    // ---------------------------- Sweet Alert -----------------------
    const deleteButtons = document.querySelectorAll(".delete-btn");

    deleteButtons.forEach(button => {
        button.addEventListener("click", function () {

            const id = this.getAttribute("data-id");

            console.log("Delete button clicked for user with ID:", id);

            Swal.fire({
                title: "Are you sure?",
                text: "This action cannot be undone!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#d33",
                cancelButtonColor: "#1E3A8A",
                confirmButtonText: "Yes, delete it!"
            }).then((result) => {
                if (result.isConfirmed) {
                    deleteArea(id);
                }
            });
        });
    });
}

function deleteArea(areaId) {
    document.getElementById("Pageloading").style.visibility = "visible";

    fetch(`/OrganizationSetup/DeleteMeetingArea?areaId=${areaId}`, {
        method: "GET",
        headers: { "Content-Type": "application/json" }
    })
        .then(response => response.json())
        .then(data => {
            if (data.Status) {
                Swal.fire("Deleted!", "The meeting area has been deleted.", "success")
                    .then(() => window.location.reload());
            } else {
                Swal.fire("Error!", "Failed to delete the meeting area.", "error");
            }
        })
        .catch(error => {
            console.error("Error:", error);
            Swal.fire("Error!", "An error occurred while deleting.", "error");
        })
        .finally(() => {
            document.getElementById("Pageloading").style.visibility = "hidden";
        });
}

function handleEditArea() {

    const editButtons = document.querySelectorAll(".editArea");

    editButtons.forEach(button => {

        button.addEventListener("click", function () {
            debugger;

            const id = this.getAttribute("data-id");
            const name = this.getAttribute("data-name");
            const code = this.getAttribute("data-code");
            const building = this.getAttribute("data-building");
            const zone = this.getAttribute("data-zone");
            const availability = this.getAttribute("data-Availabilty") === "True" ? true : false;

            console.log("Edit button clicked for user with ID:", id);
            console.log("Edit button clicked for user with Name:", name);
            console.log("Edit button clicked for user with code:", code);
            console.log("Edit button clicked for user with Building:", building);
            console.log("Edit button clicked for user with Zone:", zone);
            console.log("Edit button clicked for user with Status:", availability);

            document.getElementById("selectedAreaId").setAttribute("value", id);
            document.getElementById("editModel-meeting-area-name").value = name;
            document.getElementById("editModel-meeting-area-code").value = code;
            document.getElementById("editVisit-request-toggle").checked = availability;

            const buildingDropdown = document.getElementById("editModel-related-building");

            if (buildingDropdown) {
                let found = false;

                for (let option of buildingDropdown.options) {
                    if (option.value === building) {
                        option.selected = true;
                        found = true;
                        break;
                    }
                }

                if (!found) {
                    buildingDropdown.selectedIndex = 0;
                }
            }


            debugger;

            getRelatedZonesByBuildingId(building, false);

            const zonesDropdown = document.getElementById("editModel-related-zone");

            setTimeout(() => {
                if (zonesDropdown) {
                    for (let option of zonesDropdown.options) {
                        if (option.value === zone) {
                            option.selected = true;
                            break;
                        }
                    }
                }

            }, 200); // Delay to allow zones to populate


            buildingDropdown.addEventListener("change", function () {
                const selectedId = this.value;
                console.log("Selected Building ID:", selectedId);

                getRelatedZonesByBuildingId(selectedId, false); // is new = false (Edit)
            });
        });
    });
}

function handleEditAreaSubmit() {
    debugger;

    document.getElementById("editMeetingAreaBtn").addEventListener("click", function () {
        debugger;

        let isValid = true;

        const id = document.getElementById("selectedAreaId");
        const buildingId = document.getElementById("editModel-related-building");
        const zoneId = document.getElementById("editModel-related-zone");
        const areaName = document.getElementById("editModel-meeting-area-name");
        const code = document.getElementById("editModel-meeting-area-code");
        const visitRequestToggle = document.getElementById("editVisit-request-toggle").checked;

        console.log("buildingId", buildingId.value);
        console.log("zoneId", zoneId.value);
        console.log("areaName", areaName.value);
        console.log("code", code.value);
        console.log("Available for Visit Request:", visitRequestToggle);

        if (!id.value) {
            isValid = false;
            Notify("Please select an area to edit.", NotifyStatus.Error);
            return;
        }

        if (!buildingId.value) {
            isValid = false;
            HighlightField(buildingId);
        }

        if (!zoneId.value) {
            isValid = false;
            HighlightField(zoneId);
        }

        if (!areaName.value) {
            isValid = false;
            HighlightField(areaName);
        }

        if (!isValid) {
            Notify("Please fill in all required fields.", NotifyStatus.Error);
            return;
        }

        console.log("Form is valid. Submitting data...");

        const editMeetingAreaVM = {
            id: id.value,
            buildingId: buildingId.value,
            zoneId: zoneId.value,
            Name: areaName.value,
            code: code.value,
            AppearInVisitRequests: visitRequestToggle
        };

        console.log("Form is valid. Submitting data...", editMeetingAreaVM);

        //loader
        document.getElementById("Pageloading").style.visibility = "visible";

        fetch("/OrganizationSetup/EditMeetingArea", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(editMeetingAreaVM)
        })
            .then(response => {
                return response.json();
            })
            .then(data => {

                if (data.Status) {

                    console.log("Success:", data);

                    Notify("Meeting Area Edited successfully!", NotifyStatus.Success);

                    resetAndHideEditAreaModel();

                    //TODO Make a better logic to append it directly or when reload make the selcted tab is Meeting
                    setTimeout(() => {
                        window.location.reload();
                    }, 3000);

                } else {
                    console.error("Error:", data);
                    Notify("Failed to add Meeting Area.", NotifyStatus.Error);
                }
            })
            .catch(error => {
                console.error("Error:", error);
                Notify("Failed to add Meeting Area.", NotifyStatus.Error);
            })
            .finally(() => {
                document.getElementById("Pageloading").style.visibility = "hidden";
            });
    });
}

function handleSelectBuildingLogic() {

    const selectNewElement = document.getElementById("model-related-building");

    selectNewElement.addEventListener("change", function () {
        debugger;
        const selectedId = this.value;
        console.log("Selected Building ID:", selectedId);

        getRelatedZonesByBuildingId(selectedId, true); // is new = true (Add)
    });
}

function getRelatedZonesByBuildingId(buildingId, isNewArea = true) {
    console.log("Fetching zones for building ID:", buildingId);

    //loader
    document.getElementById("Pageloading").style.visibility = "visible";

    fetch(`/OrganizationSetup/GetZonesByBuildingId?buildingId=${buildingId}`, {
        method: "GET",
        headers: { "Content-Type": "application/json" }
    })
        .then(response => response.json())
        .then(data => {
            console.log("Received data:", data);

            let selectElement = isNewArea
                ? document.getElementById("model-related-zone")
                : document.getElementById("editModel-related-zone");

            selectElement.disabled = false;

            selectElement.innerHTML = "";

            let defaultOption = document.createElement("option");
            defaultOption.value = "";
            defaultOption.textContent = data.length > 0 ? "Select a Zone" : "No zones available";
            defaultOption.disabled = true;
            defaultOption.selected = true;
            selectElement.appendChild(defaultOption);

            data.forEach(zone => {
                let option = document.createElement("option");
                option.value = zone.Id;
                option.textContent = zone.Name;
                selectElement.appendChild(option);
            });

            //return selectElement; 

        })
        .catch(error => console.error("Error fetching zones:", error))
        .finally(() => {
            // Hide loader AFTER request completes (success or failure)
            document.getElementById("Pageloading").style.visibility = "hidden";
        });;
}

function resetAndHideEditAreaModel() {

    document.getElementById('editMeetingAreaModal').classList.remove('show');
    document.body.classList.remove('modal-open');
    document.querySelector('.modal-backdrop').remove();

    // Reset form fields
    document.getElementById("editModel-meeting-area-name").value = "";
    document.getElementById("editModel-meeting-area-code").value = "";
    document.getElementById("editVisit-request-toggle").checked = true;
    document.getElementById("editModel-related-building").selectedIndex = 0;
    document.getElementById("editModel-related-zone").innerHTML = '<option value="" selected disabled>Select Building First</option>';
}

function handleAddAreaSubmit() {

    document.getElementById("saveMeetingAreaBtn").addEventListener("click", function () {
        let isValid = true;
        debugger;
        const buildingId = document.getElementById("model-related-building");
        const zoneId = document.getElementById("model-related-zone");
        const areaName = document.getElementById("model-meeting-area-name");
        const code = document.getElementById("model-meeting-area-code");
        const visitRequestToggle = document.getElementById("visit-request-toggle").checked;

        console.log("buildingId", buildingId.value);
        console.log("zoneId", zoneId.value);
        console.log("areaName", areaName.value);
        console.log("code", code.value);
        console.log("Available for Visit Request:", visitRequestToggle);


        if (!buildingId.value) {
            isValid = false;
            HighlightField(buildingId);
        }

        if (!zoneId.value) {
            isValid = false;
            HighlightField(zoneId);
        }

        if (!areaName.value) {
            isValid = false;
            HighlightField(areaName);
        }

        if (!isValid) {
            Notify("Please fill in all required fields.", NotifyStatus.Error);
            return;
        }

        console.log("Form is valid. Submitting data...");

        const addMeetingAreaVM = {
            buildingId: buildingId.value,
            zoneId: zoneId.value,
            Name: areaName.value,
            code: code.value,
            AppearInVisitRequests: visitRequestToggle
        };

        console.log("Form is valid. Submitting data...", addMeetingAreaVM);

        //loader
        document.getElementById("Pageloading").style.visibility = "visible";

        fetch("/OrganizationSetup/AddMeetingArea", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(addMeetingAreaVM)
        })
            .then(response => {
                return response.json();
            })
            .then(data => {

                if (data.Status) {

                    console.log("Success:", data);
                    Notify("Meeting Area added successfully!", NotifyStatus.Success);
                    resetAndHideNewAreaModel();

                    setTimeout(() => {
                        // TODO Make a better logic to append it directly or when reload make the selcted tab is Meeting
                        window.location.reload();
                    }, 3000);

                } else {
                    console.error("Error:", data);
                    Notify("Failed to add Meeting Area.", NotifyStatus.Error);
                }
            })
            .catch(error => {
                console.error("Error:", error);
                Notify("Failed to add Meeting Area.", NotifyStatus.Error);
            })
            .finally(() => {
                document.getElementById("Pageloading").style.visibility = "hidden";
            });
    });
}

function resetAndHideNewAreaModel() {

    document.getElementById('addMeetingAreaModal').classList.remove('show');
    document.body.classList.remove('modal-open');
    document.querySelector('.modal-backdrop').remove();

    document.getElementById("model-meeting-area-name").value = "";
    document.getElementById("model-meeting-area-code").value = "";
    document.getElementById("visit-request-toggle").checked = true;
}