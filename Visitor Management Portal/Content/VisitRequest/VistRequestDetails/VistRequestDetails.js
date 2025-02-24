document.addEventListener('DOMContentLoaded', () => {

    const searchInput = $('#requestedBySearch');
    const dropdownOptions = $('#requestedByOptions');
    const visitLocationDropdown = document.getElementById('visit-location-dropdown');
    const officeLocation = document.querySelector('.office-lcation');
    const meetingAreaLocation = document.querySelector('.meetingArea-lcation');
    const continueBtn = document.getElementById('saveMeetingAreaBtn');

    const meetingAreaSearch = document.getElementById('meetingAreaSearch');
    const meetingAreaOptions = document.getElementById('meetingAreaOptions');
    const buildingDropdown = document.getElementById('building');
    const zoneDropdown = document.getElementById('zone');

    visitLocationDropdown.value = 0; // Default value
    var meetingAreaVal;
    let buildings = [];
    let allUsers = []; // Store users globally

    getOrganizationUsers();

    function getOrganizationUsers() {

        // Fetch organization users when the page loads
        $.ajax({
            url: '/VisitRequest/GetOrganizationUsers',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                allUsers = response; // Store users globally
                console.log("Organization Users: ", allUsers);
            },
            error: function (xhr, status, error) {
                console.error("Error fetching organization users:", error);
            }
        });
    }


    $("#saveMeetingAreaBtn").on("click", function () {
        const visitRequestData = {
            VisiteRequestID: $("#editVisitRequestModel").data("visit-id"), // Assuming the visit request ID is stored in modal data
            Subject: $("#subject").val().trim(),
            Purpose: $("#status").val(),
            VisitTime: $("#date").val() + " " + $("#time").val(),
            RequestedBy: $("#requestedBySearch").attr("data-user-id"), // Get selected user ID
            StatusReason: $("#status").val(),
            Location: visitLocationDropdown.value,
            MeetingArea: meetingAreaVal,
        };
        debugger;
        // Ensure validation before proceeding
        if (!validateVisitRequestData(visitRequestData)) {
            return;
        }

        $.ajax({
            url: "/VisitRequest/UpdateVisitRequest",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(visitRequestData),
            dataType: "json",
            success: function (response) {
                if (response.Status) {
                    Notify(response.Message, "Success");
                    setTimeout(() => {
                        $("#editVisitRequestModel").modal("hide"); // Close modal after success
                        window.location.reload(); // Refresh page
                    }, 2000);
                } else {
                    Notify(response.Message, "Error");
                }
            },
            error: function (xhr, status, error) {
                console.error("Error updating visit request:", error);
                console.error("Response:", xhr.responseText);
                Notify("An error occurred while updating the visit request.", "Error");
            }
        });
    });

    // Function to validate the form inputs before submitting
    function validateVisitRequestData(data) {
        // Convert object values to an array and check if any field is filled
        const hasAtLeastOneFieldFilled = Object.values(data).some(value => value && value.toString().trim() !== "");

        if (!hasAtLeastOneFieldFilled) {
            alert("Please fill in at least one field to update.");
            return false;
        }
        return true;
    }


    // Initialize the DataTable
    const table = $('#VisitorsDetailsTable').DataTable({
        "pageLength": 7,
        "lengthChange": false,
        destroy: true,
        responsive: false,
        scrollX: true,
        scrollCollapse: true,
        paging: true,
        searching: true,
        autoWidth: false,
        ordering: true,
        info: true,
        "columnDefs": [
            { "orderable": false, "targets": [2] },
        ],
        "buttons": ['print']
    });

    // Custom search implementation
    const pageSearchInput = document.querySelector(".Visitors-details .search-bar input");
    if (pageSearchInput) {
        pageSearchInput.addEventListener("input", (e) => {
            table.search(e.target.value).draw();
        });
    }


    // Show all users when focusing on searchInput
    searchInput.on('focus', function () {
        populateOptions(allUsers); // Show all users on focus
    });

    // Filter users dynamically as user types
    searchInput.on('input', function () {
        const query = searchInput.val().trim().toLowerCase();

        if (query.length < 2) {
            populateOptions(allUsers); // Show all users if less than 2 chars
            return;
        }

        const filteredUsers = allUsers.filter(user =>
            user.UserName.toLowerCase().includes(query)
        );

        populateOptions(filteredUsers);
    });

    // Populate dropdown with filtered users
    const populateOptions = (users) => {
        dropdownOptions.empty();

        if (users.length === 0) {
            dropdownOptions.html('<div class="dropdown-item">No results found</div>');
        } else {
            users.forEach(user => {
                const option = $(`<div class="dropdown-item">${user.UserName}</div>`);
                option.attr('data-value', user.UserID);
                option.on('click', function () {
                    selectOption(user);
                });
                dropdownOptions.append(option);
            });
        }
        dropdownOptions.removeClass('hidden');
    };

    // Handle user selection
    const selectOption = (user) => {
        searchInput.val(user.UserName);
        searchInput.attr('data-user-id', user.UserID);
        dropdownOptions.addClass('hidden');
    };

    if (visitLocationDropdown) {
        // Event listener for when the user selects a visit location
        visitLocationDropdown.addEventListener("change", async () => {
            const selectedValue = visitLocationDropdown.value; // Get selected value (0 or 1)

            if (selectedValue === "0") {
                officeLocation.classList.remove("d-none");
                meetingAreaLocation.classList.add("d-none");
            } else {
                officeLocation.classList.add("d-none");
                meetingAreaLocation.classList.remove("d-none");

                // Load buildings dynamically if not already loaded
                if (buildings.length === 0) {
                    showLoadingText(buildingDropdown, "Loading buildings...");
                    buildings = await fetchBuildings();
                    populateDropdown(buildingDropdown, buildings, "Select building");
                }
            }
            checkMandatoryFields(); // Ensure validation
        });
    }

    /**
 * When a Building is Selected, Enable & Load Zones
 */
    buildingDropdown.addEventListener('change', async (event) => {
        const buildingId = event.target.value;

        // Reset dependent dropdowns
        zoneDropdown.innerHTML = `<option value="">Select zone</option>`;
        meetingAreaSearch.value = "";
        meetingAreaOptions.innerHTML = "";
        zoneDropdown.disabled = true;
        meetingAreaSearch.disabled = true;
        meetingAreaSearch.placeholder = "Select zone first";

        if (buildingId) {
            showLoadingText(zoneDropdown, 'Select zone');
            zones = await fetchZones(buildingId);
            populateDropdown(zoneDropdown, zones, 'Select zone');
            zoneDropdown.disabled = false; // Enable zone dropdown after fetching
        }

        checkMandatoryFields();
    });

    /**
 * When a Zone is Selected, Enable & Load Meeting Areas
 */
    zoneDropdown.addEventListener('change', async (event) => {
        const zoneId = event.target.value;

        // Reset meeting area
        meetingAreaSearch.value = "";
        meetingAreaOptions.innerHTML = "";
        meetingAreaSearch.disabled = true;

        if (zoneId) {
            meetingAreas = await fetchMeetingAreas(zoneId);
            populateMeetingAreaOptions(meetingAreas);
            meetingAreaSearch.disabled = meetingAreas.length === 0;
            meetingAreaSearch.placeholder = "Search and select a meeting area";
        }

        checkMandatoryFields();
    });

    /**
     * Filter Meeting Areas Based on Search Input
     */
    meetingAreaSearch.addEventListener('input', (event) => {
        const query = event.target.value.trim();
        if (query.length > 0) {
            const filteredMeetingAreas = meetingAreas.filter(area =>
                area.Name.toLowerCase().includes(query.toLowerCase())
            );
            populateMeetingAreaOptions(filteredMeetingAreas);
        } else {
            populateMeetingAreaOptions(meetingAreas);
        }
    });

    // Hide meeting area dropdown when clicking outside
    document.addEventListener('click', function (event) {
        if (!meetingAreaSearch.contains(event.target) && !meetingAreaOptions.contains(event.target)) {
            meetingAreaOptions.classList.add('hidden');
        }
    });

    function checkMandatoryFields() {
        let allFilled = true;

        if (visitLocationDropdown.value === "1") {
            if (!buildingDropdown.value) {
                allFilled = false;
            }

            if (!zoneDropdown.value) {
                allFilled = false;
            }

            if (!meetingAreaSearch.value.trim()) {
                allFilled = false;
            }
        }

        continueBtn.disabled = !allFilled;
        if (allFilled) {
            continueBtn.classList.remove('custom-class');
        } else {
            continueBtn.classList.add('custom-class');
        }
        return allFilled;
    }

    const showLoadingText = (element, placeholderText) => {
        element.innerHTML = `<option value="">${placeholderText} (Loading...)</option>`;
        element.disabled = true;
    };

    const populateDropdown = (element, data, defaultText) => {
        element.innerHTML = `<option value="">${defaultText}</option>`;
        data.forEach(item => {
            const option = document.createElement('option');
            option.value = item.Id;
            option.textContent = item.Name;
            element.appendChild(option);
        });
        element.disabled = data.length === 0; // Disable if no data available
    };

    const fetchBuildings = async () => {
        try {
            const response = await fetch('/Location/GetAllBuildings');
            if (!response.ok) throw new Error('Network response was not ok');
            const data = await response.json();
            return data;
        } catch (error) {
            console.error('Error fetching buildings:', error);
            return [];
        }
    };

    const fetchZones = async (buildingId) => {
        try {
            const response = await fetch(`/Location/GetZonesByBuildingId?buildingId=${buildingId}`);
            if (!response.ok) throw new Error('Network response was not ok');
            const data = await response.json();
            return data;
        } catch (error) {
            console.error('Error fetching zones:', error);
            return [];
        }
    };

    const fetchMeetingAreas = async (zoneId) => {
        try {
            const response = await fetch(`/Location/GetMeetingAreasByZoneId?zoneId=${zoneId}`);
            if (!response.ok) throw new Error('Network response was not ok');
            const data = await response.json();
            return data;
        } catch (error) {
            console.error('Error fetching meeting areas:', error);
            return [];
        }
    };



    const populateMeetingAreaOptions = (data) => {
        meetingAreaOptions.innerHTML = '';
        if (data.length === 0) {
            meetingAreaOptions.innerHTML = '<div class="dropdown-item">No results found</div>';
        } else {
            data.forEach((item) => {
                const option = document.createElement('div');
                option.textContent = item.Name;
                option.dataset.value = item.Id;
                option.classList.add('dropdown-item');
                option.addEventListener('click', () => selectMeetingArea(item));
                meetingAreaOptions.appendChild(option);
            });
        }
        meetingAreaOptions.classList.remove('hidden');
    };

    /**
     * Select a Meeting Area
     */
    const selectMeetingArea = (item) => {
        console.log("GGGGGG: ", item);
        meetingAreaSearch.value = item.Name;
        meetingAreaVal = item.Id;
        meetingAreaOptions.classList.add('hidden');
        checkMandatoryFields();
    };
});
