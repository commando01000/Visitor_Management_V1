document.addEventListener('DOMContentLoaded', () => {
    const tabs = document.querySelectorAll('.nav-tabs li a');
    const contents = document.querySelectorAll('div[id^="tab"]');

    function activateTab(tab) {

        document.getElementById("Pageloading").style.visibility = "visible";


        tabs.forEach(t => t.parentElement.classList.remove('active'));
        contents.forEach(content => content.style.display = 'none');
        tab.parentElement.classList.add('active');
        const target = document.querySelector(tab.dataset.target);
        const url = tab.dataset.url;
        if (url) {
            fetch(url)
                .then(response => {
                    if (!response.ok) {
                        throw new Error("Failed to load content");
                    }
                    return response.text();
                })
                .then(html => {
                    target.innerHTML = html;
                    target.style.display = 'block';

                    // Initialize specific tab functionality if needed
                    if (url.includes('Building')) {
                        BuildingTabInit();
                    }
                    // Initialize specific tab functionality if needed
                    if (url.includes('ZonesDetails')) {
                        ZonesTabInit();
                    }
                    // Initialize specific tab functionality if needed
                    if (url.includes('MeetingAreasDetails')) {
                        MeetingAreasTabInit();
                    }
                })
                .catch(error => {
                    console.error("Error loading content:", error);
                });
        } else {
            target.style.display = 'block';
            document.getElementById("Pageloading").style.visibility = "hidden";
        }
    }

    // Add event listeners to tabs
    tabs.forEach(tab => {
        tab.addEventListener('click', function (event) {
            event.preventDefault();
            activateTab(this);
        });
    });

    // Initialize the first tab (Building) on page load
    const defaultTab = document.querySelector('.nav-tabs li a.active') || tabs[0];
    if (defaultTab) {
        activateTab(defaultTab);
    }

    // Building Tab Initialization Function
    function BuildingTabInit() {

        const buildingItem = document.querySelectorAll('.buildingDetails');
        const table = $('#buildingTable').DataTable({
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
                { width: '80px' },
                { width: '120px' },
                { width: '150px' },
                { width: '100px' },
                { width: '120px' },
                { width: '230px' },
                { width: '170px' },
                { width: '140px' }
            ]
        });

        // Custom search functionality
        const datatableSearchInput = document.querySelector('#buildingTable_filter input[type="search"]');
        const pageSearchInput = document.querySelector(".building_topSection .search-bar input");

        // For search bar
        if (datatableSearchInput && pageSearchInput) {
            pageSearchInput.addEventListener("input", (e) => {
                datatableSearchInput.value = e.target.value;
                table.search(e.target.value).draw();
            });
        }

        const add_new_building_btn = document.querySelector(".addBuilding")
        console.log(add_new_building_btn)

        add_new_building_btn.addEventListener('click', () => {
            window.location.href = "/OrganizationSetup/AddNewBuilding";

        })

        // Building Item Click
        buildingItem.forEach((item) => {
            item.addEventListener('click', function () {
                const buildingId = this.getAttribute('data-id');
                if (buildingId) {
                    window.location.href = `/OrganizationSetup/BuildingDetails/${buildingId}`;
                }
            });
        });

        document.getElementById("Pageloading").style.visibility = "hidden";

    }

    // Zones Tab Initialization Function
    function ZonesTabInit() {
        const table = $('#zonesTable').DataTable({
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
        const datatableSearchInput = document.querySelector('#zonesTable_filter input[type="search"]');
        const pageSearchInput = document.querySelector(".zones_topSection .search-bar input");

        // For search bar
        if (datatableSearchInput && pageSearchInput) {
            pageSearchInput.addEventListener("input", (e) => {
                datatableSearchInput.value = e.target.value;
                table.search(e.target.value).draw();
            });
        }

        //-----------------------

        handleDeleteZoneBtns();

        handleToggleZoneExcludeFromOfficeLogic();

        handleAddZoneSubmit();

        handleEditZone();

        handleEditZoneSubmit();

        //-----------------------

        document.getElementById("Pageloading").style.visibility = "hidden";

    }

    // MeetingAreas Tab Initialization Function
    function MeetingAreasTabInit() {
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

        //-----------------------

        handleDeleteAreaBtns();

        handleToggleAreaAvailabilityLogic();

        handleSelectBuildingLogic();

        handleAddAreaSubmit();

        // TODO there is a bug here : at first open doesn't show the selected zone
        handleEditArea();

        handleEditAreaSubmit();

        //-----------------------

        document.getElementById("Pageloading").style.visibility = "hidden";
    }

    function handleEditZoneSubmit() {
        debugger;

        document.getElementById("editZoneBtn").addEventListener("click", function () {
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
                        Notify(data.Message, NotifyStatus.Success);
                        resetAndHideEditZoneModel();

                        setTimeout(() => {
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
                    resetAndHideEditZoneModel();
                });
        });
    }

    function handleAddZoneSubmit() {

        document.getElementById("saveZoneBtn").addEventListener("click", function () {

            let isValid = true;

            var name = document.getElementById("model-zone-name");
            const code = document.getElementById("model-zone-code");
            const excludeFromOfficeToggle = document.getElementById("excludeFromOfficeSelection-toggle").checked;
            const building = document.getElementById("addZone-related-building");

            console.log("name", name.value);
            console.log("code", code.value);
            console.log("buildingId", building.value);
            console.log("excludeFromOfficeToggle:", excludeFromOfficeToggle);

            if (!name.value) {
                isValid = false;
                HighlightField(name);
            }

            if (!building.value) {
                isValid = false;
                HighlightField(building);
            }

            if (!isValid) {
                Notify("Please fill in all required fields.", NotifyStatus.Error);
                return;
            }

            console.log("Form is valid. Submitting data...");

            const addZoneVM = {
                Name: name.value,
                Code: code.value ?? null,
                BuildingId: building.value,
                ExcludeFromOfficeToggle: excludeFromOfficeToggle
            };

            console.log("Form is valid. Submitting data...", addZoneVM);

            //loader
            document.getElementById("Pageloading").style.visibility = "visible";

            fetch("/OrganizationSetup/AddZone", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(addZoneVM)
            })
                .then(response => {
                    return response.json();
                })
                .then(data => {

                    if (data.Status) {

                        console.log("Success:", data);
                        Notify(data.Message, NotifyStatus.Success);

                        resetAndHideNewZoneModel();

                        setTimeout(() => {
                            window.location.reload();
                        }, 3000);

                    } else {
                        console.error("Error:", data);
                        Notify("Failed to add Zone Area.", NotifyStatus.Error);
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

    function handleToggleZoneExcludeFromOfficeLogic() {

        let toggles = document.querySelectorAll("#zonesTable .zone-toggle");

        if (toggles.length === 0) {
            console.log("No toggle switches found");
            return;
        }

        toggles.forEach(toggle => {
            toggle.addEventListener("change", function () {
                let modelId = this.getAttribute("data-model-id");
                updateZoneExcludeStatus(modelId);
            });
        });
    }

    function handleEditZone() {

        const editButtons = document.querySelectorAll(".editZone");

        editButtons.forEach(button => {

            button.addEventListener("click", function () {
                debugger;
                const id = this.getAttribute("data-id");
                const name = this.getAttribute("data-name");
                const code = this.getAttribute("data-code");
                const building = this.getAttribute("data-building");
                const excludeFromOffice = this.getAttribute("data-ExcludeFromOfficeSelection") === "True";

                console.log("Edit button clicked for zone with ID:", id);
                console.log("Edit button clicked for zone with Name:", name);
                console.log("Edit button clicked for zone with code:", code);
                console.log("Edit button clicked for zone with Building:", building);
                console.log("Edit button clicked for zone with excludeFromOffice:", excludeFromOffice);

                document.getElementById("selectedZoneId").setAttribute("value", id);
                document.getElementById("editModel-zone-name").value = name;
                document.getElementById("editModel-zone-code").value = code;
                document.getElementById("excludeFromOffice-toggle").checked = excludeFromOffice;

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
                } else {
                    console.error("Error updating Zone exclude status:", data.Error);
                }
            })
            .catch(error => console.error("Error updating zone exclude status:", error));
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

        // ---------------------------- Hameed Custom Modal -----------------------

        //const deleteButtons = document.querySelectorAll(".delete-btn");
        //const deleteModal = new bootstrap.Modal(document.getElementById("deleteModal"));
        //const confirmDeleteBtn = document.getElementById("confirmDelete");

        //deleteButtons.forEach(button => {
        //    button.addEventListener("click", function () {
        //        selectedAreaId = this.getAttribute("data-id");
        //        console.log("Delete button clicked for user with ID:", selectedAreaId);

        //        deleteModal.show();
        //    });
        //});

        //confirmDeleteBtn.addEventListener("click", function () {
        //    if (selectedAreaId) {
        //        deleteArea(selectedAreaId);
        //        selectedAreaId = null;
        //        deleteModal.hide();
        //    }
        //});
    }

    function handleDeleteZoneBtns() {

        //---------------------------- Sweet Alert -----------------------

        const deleteButtons = document.querySelectorAll(".deleteZone-btn");

        deleteButtons.forEach(button => {
            button.addEventListener("click", function () {

                const id = this.getAttribute("data-id");

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
                        deleteZone(id);
                    }
                });
            });
        });
    }

    function deleteZone(zoneId) {
        document.getElementById("Pageloading").style.visibility = "visible";

        fetch(`/OrganizationSetup/DeleteZone?zoneId=${zoneId}`, {
            method: "GET",
            headers: { "Content-Type": "application/json" }
        })
            .then(response => response.json())
            .then(data => {
                if (data.Status) {
                    Swal.fire("Deleted!", "The Zone has been deleted.", "success")
                        .then(() => window.location.reload());
                } else {
                    Swal.fire("Error!", "Failed to delete the Zone.", "error");
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

                        // TODO Make a better logic to append it directly or when reload make the selcted tab is Meeting
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

    function resetAndHideNewZoneModel() {

        document.getElementById('addZoneModel').classList.remove('show');
        document.body.classList.remove('modal-open');
        document.querySelector('.modal-backdrop').remove();

        document.getElementById("model-zone-name").value = "";
        document.getElementById("model-zone-code").value = "";
        document.getElementById("excludeFromOfficeSelection-toggle").checked = true;
    }

    function resetAndHideEditZoneModel() {

        document.getElementById('editZoneModal').classList.remove('show');
        document.body.classList.remove('modal-open');

        let modalBackdrop = document.querySelector('.modal-backdrop');
        if (modalBackdrop) {
            modalBackdrop.remove();
        }

        document.getElementById("editModel-zone-name").value = "";
        document.getElementById("editModel-zone-code").value = "";
        document.getElementById("excludeFromOffice-toggle").checked = true;
    }

    function resetAndHideNewAreaModel() {

        document.getElementById('addMeetingAreaModal').classList.remove('show');
        document.body.classList.remove('modal-open');
        document.querySelector('.modal-backdrop').remove();

        document.getElementById("model-meeting-area-name").value = "";
        document.getElementById("model-meeting-area-code").value = "";
        document.getElementById("visit-request-toggle").checked = true;
    }

    function resetAndHideEditAreaModel() {

        document.getElementById('editMeetingAreaModal').classList.remove('show');
        document.body.classList.remove('modal-open');

        let modalBackdrop = document.querySelector('.modal-backdrop');
        if (modalBackdrop) {
            modalBackdrop.remove();
        }

        document.getElementById("editModel-meeting-area-name").value = "";
        document.getElementById("editModel-meeting-area-code").value = "";
        document.getElementById("editVisit-request-toggle").checked = true;
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

    function handleToggleAreaAvailabilityLogic() {
        let toggles = document.querySelectorAll("#meetingAreasTable .area-toggle");

        if (toggles.length === 0) {
            console.log("No toggle switches found");
            return;
        }

        toggles.forEach(toggle => {
            toggle.addEventListener("change", function () {
                let modelId = this.getAttribute("data-model-id");
                let isChecked = this.checked;

                updateModelStatus(modelId, isChecked);
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


