document.addEventListener('DOMContentLoaded', async () => {

    const searchInput = $('#requestedBySearch');
    const dropdownOptions = $('#requestedByOptions');
    const visitLocationDropdown = document.getElementById('visit-location-dropdown');
    const officeLocation = document.querySelector('.office-lcation');
    const meetingAreaLocation = document.querySelector('.meetingArea-lcation');
    const continueBtn = document.getElementById('saveMeetingAreaBtn');
    const EditVisitRequest = document.querySelector('.editVisitRequest');


    const meetingAreaSearch = document.getElementById('meetingAreaSearch');
    const meetingAreaOptions = document.getElementById('meetingAreaOptions');
    const buildingDropdown = document.getElementById('building');
    const zoneDropdown = document.getElementById('zone');

    const showFilteredResults = document.getElementById('showFilteredResultsBtn');
    const filterVisitorHubModal = document.getElementById('filterVisitorHubModal');
    const filterVisitorsIcon = document.getElementById("filterVisitorsIcon");
    const resetVisitorsFiltersBtn = document.getElementById('resetFiltersBtn');
    const searchInputFilter = document.querySelector('#filterVisitorHubModal #requestedBySearch');
    const dropdownOptionsFilter = document.querySelector('#filterVisitorHubModal #requestedByOptions');
    const editPersonalDataModal = document.getElementById('editPersonalDataModal');

    visitLocationDropdown.value = 0; // Default value
    var meetingAreaVal;
    let buildings = [];
    let allUsers = []; // Store users globally
    let allVisitors = []; // Store visitors globally
    let originalVisitorsTableData = [];
    let currentVisitRequestId = {};

    // Initialize the DataTable
    const table = $('#VisitorsDetailsTable').DataTable({
        "pageLength": 7,
        "lengthChange": false,
        destroy: true,
        responsive: true,
        scrollX: true,
        scrollCollapse: true,
        paging: true,
        searching: true,
        autoWidth: true,
        ordering: true,
        info: true,
        "columnDefs": [
            { "orderable": false, "targets": [4] }, // Disable sorting on the Action column
        ],
        "buttons": ['print']
    });

    getOrganizationUsers();
    getCurrentUsersIds();

    // Capture the initial data from the rendered table
    saveVisitorsOriginalData();
    function saveVisitorsOriginalData() {
        originalVisitorsTableData = [];

        // Loop through each row of the DataTable
        table.rows().every(function () {
            let row = $(this.node()); // Get the row as a jQuery object
            let visitorID = row.data("id") || "-"; // Extract Visitor ID from data-id attribute, default to "-"

            // Extract data from each column
            let rowData = [
                row.find("td:eq(0)").html().trim(), // Full HTML of the first column (including icon)
                row.find("td:eq(0) h6").text().trim(), // Visitor Name (for filtering)
                row.find("td:eq(0) p").text().trim(), // Organization (for filtering)
                row.find("td:eq(1)").text().trim(), // Email
                row.find("td:eq(2)").text().trim(), // ID Number
                row.find("td:eq(3)").text().trim(), // Status
                row.find("td:eq(4)").html().trim(), // Action (HTML for edit/delete buttons)
                visitorID // Visitor ID
            ];

            originalVisitorsTableData.push(rowData);
        });

        console.log("Original Visitors Table Data:", originalVisitorsTableData);
    }

    // get currentVisitRequestId from (data-visit-id) attribute in edit-data class
    const record = document.getElementsByClassName("edit-data");
    currentVisitRequestId = record[0].getAttribute("data-visit-id");

    // get cuurent users ids from VisitorsDetailsTable (data-id) on every row
    function getCurrentUsersIds() {
        const rows = document.querySelectorAll('tr[data-id]');
        return Array.from(rows).map(row => row.getAttribute('data-id'));
    }

    // Capture the initial data from the rendered table
    function getOrganizationUsers() {

        // Fetch organization users when the page loads
        $.ajax({
            url: '/VisitRequest/GetOrganizationUsers',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                allUsers = response; // Store users globally
                //console.log("Organization Users: ", allUsers);
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

    // Custom search implementation
    const pageSearchInput = document.querySelector(".Visitors-details .search-bar input");
    if (pageSearchInput) {
        pageSearchInput.addEventListener("input", (e) => {
            table.search(e.target.value).draw();
        });
    }

    filterVisitorHubModal.addEventListener('click', (event) => {
        // Check if the clicked element is NOT inside the dropdown or the input field
        if (event.target !== searchInputFilter && !dropdownOptionsFilter.contains(event.target)) {
            dropdownOptionsFilter.classList.add('hidden'); // Hide the dropdown
        }
    });

    // Bind click event for the edit button using event delegation
    $('#VisitorsDetailsTable tbody').on('click', 'i.fa-pencil', function () {
        // Check if the button is disabled
        if ($(this).hasClass('disabled')) {
            return; // Do nothing if the button is disabled
        }

        // Get the row and visitor ID
        const row = $(this).closest('tr');
        const visitorId = row.data('id'); // Get the visitor ID from the data-id attribute

        //window.location.href = `/VisitorsHub/EditVisitor/${visitorId}`;
        // open the editPersonalDataModal
        openEditVisitorModal(visitorId);
    });

    function openEditVisitorModal(visitorId) {
        // Find the table row with the matching visitorId
        const row = $(`#VisitorsDetailsTable tbody tr[data-id="${visitorId}"]`);

        if (row.length === 0) {
            console.error(`Visitor with ID ${visitorId} not found in the table.`);
            return;
        }

        // Extract data from the table row
        const visitorName = row.find('td:eq(0) .text-content-container p.fw-500').text().trim();
        const organization = row.find('td:eq(0) .text-content-container p:eq(1)').text().trim();
        const email = row.find('td:eq(1)').text().trim();
        const idNumber = row.find('td:eq(2)').text().trim();
        const status = row.find('td:eq(3)').text().trim();

        // Extract the hidden phone number and job title from data attributes
        const phoneNumber = row.data('phone-number') || ''; // Fallback to empty string if not present
        const jobTitle = row.data('job-title') || ''; // Fallback to empty string if not present

        // Split the visitorName into first, middle, and last names (if possible)
        const nameParts = visitorName.split(' ');
        const firstName = nameParts[0] || '';
        const middleName = nameParts.length > 2 ? nameParts[1] : ''; // Middle name if exists
        const lastName = nameParts.length > 1 ? nameParts[nameParts.length - 1] : '';

        // Populate the modal fields with the extracted data
        $('#firstName').val(firstName);
        $('#middleName').val(middleName);
        $('#lastName').val(lastName);
        $('#idNumberr').val(idNumber);
        $('#jobTitle').val(jobTitle); // Now populated from the hidden data attribute
        $('#organizationName').val(organization);
        $('#emailAddress').val(email);
        $('#phoneNumber').val(phoneNumber); // Now populated from the hidden data attribute

        // Store the visitorId in the modal for later use (e.g., when saving)
        $('#editPersonalDataModal').data('visitorId', visitorId);

        // Open the modal using Bootstrap's modal method
        $('#editPersonalDataModal').modal('show');
    }

    $('#savePersonalDataBtn').on('click', function () {
        // Get the visitorId stored in the modal
        const visitorId = $('#editPersonalDataModal').data('visitorId');

        if (!visitorId) {
            console.error('Visitor ID not found in modal data.');
            alert('Error: Visitor ID not found. Please try again.');
            return;
        }

        // Collect the updated data from the modal fields
        const updatedData = {
            visitorId: visitorId,
            FirstName: $('#firstName').val().trim(),
            MiddleName: $('#middleName').val().trim(),
            LastName: $('#lastName').val().trim(),
            IdNumber: $('#idNumberr').val().trim(),
            JobTitle: $('#jobTitle').val().trim(),
            OrganizationName: $('#organizationName').val().trim(),
            EmailAddress: $('#emailAddress').val().trim(),
            PhoneNumber: $('#phoneNumber').val().trim()
        };
        debugger;
        // Basic client-side validation (Optional)
        if (!updatedData.FirstName || !updatedData.LastName) {
            Notify('First Name and Last Name are required.', 'Error');
            return;
        }
        if (!updatedData.EmailAddress || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(updatedData.EmailAddress)) {
            Notify('Please enter a valid email address.', 'Error');
            return;
        }

        // Send the updated data to the backend via AJAX
        $.ajax({
            url: `/VisitorsHub/EditVisitor/${visitorId}`, // Adjust the URL to match your backend endpoint
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(updatedData),
            success: function (response) {
                // On success, update the table row with the new data
                const row = $(`#VisitorsDetailsTable tbody tr[data-id="${visitorId}"]`);

                if (row.length === 0) {
                    console.error(`Row with visitor ID ${visitorId} not found in the table.`);
                    return;
                }

                // Update the table row with the new data
                const fullName = `${updatedData.FirstName} ${updatedData.MiddleName ? updatedData.MiddleName + ' ' : ''}${updatedData.LastName}`;
                row.find('td:eq(0) .text-content-container p.fw-500').text(fullName); // Update Visitor Name
                row.find('td:eq(0) .text-content-container p:eq(1)').text(updatedData.OrganizationName); // Update Organization
                row.find('td:eq(1)').text(updatedData.EmailAddress); // Update Email
                row.find('td:eq(2)').text(updatedData.IdNumber); // Update Id Number
                // Status (td:eq(3)) is not updated since it's not part of the modal

                // Update the hidden data attributes for phone number and job title
                row.data('phone-number', updatedData.PhoneNumber);
                row.data('job-title', updatedData.JobTitle);
                // Close the modal
                $('#editPersonalDataModal').modal('hide');
                // Provide feedback to the user
                Notify('Visitor data updated successfully.', 'Success');
                saveVisitorsOriginalData();
            },
            error: function (err) {
                console.error('Error updating visitor:', err);
                Notify('Error updating visitor. Please try again.', 'Error');
            }
        });
    });

    // Bind click event for the delete button using event delegation
    $('#VisitorsDetailsTable tbody').on('click', 'i.fa-trash-can', function () {
        // Check if the button is disabled
        if ($(this).hasClass('disabled')) {
            return; // Do nothing if the button is disabled
        }

        // Get the row and visitor ID
        const row = $(this).closest('tr');
        const visitorId = row.data('id'); // Get the visitor ID from the data-id attribute

        // Store the row in a variable to use it later for removal
        window.currentRow = row; // Store the row globally to access it in the success callback

        // Call the DeleteVisitor function
        DeleteVisitor(visitorId);
    });


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

    allVisitors = await fetchVisitors();

    searchInputFilter.addEventListener('focus', async () => {
        if (allVisitors.length === 0) {
            showLoadingSpinner();
            const data = await fetchVisitors();
            allVisitors = data;
            populateOptionsVisitors(data);
        } else {
            populateOptionsVisitors(allVisitors);
        }
    });

    // Handle input typing for real-time filtering
    searchInputFilter.addEventListener('input', () => {
        const searchTerm = searchInputFilter.value.toLowerCase();
        const filteredUsers = allVisitors.filter(user => user.Name.toLowerCase().includes(searchTerm));
        populateOptionsVisitors(filteredUsers);
    });

    showFilteredResults.addEventListener('click', () => {
        const visitorName = document.querySelector('#filterVisitorHubModal #requestedBySearch').value.trim().toLowerCase() || null;
        const visitorIDNumber = document.getElementById("idNumber").value || null;
        const visitStatus = document.getElementById('visit-statusn-dropdown').value || null;

        const filteredData = originalVisitorsTableData.filter(visitor => {
            const nameMatch = !visitorName ||
                visitor[1].toLowerCase().includes(visitorName) || // Match Visitor Name
                visitor[2].toLowerCase().includes(visitorName);  // Match Organization
            return (
                nameMatch &&
                (!visitStatus || visitor[5] === visitStatus) && // Match Status
                (!visitorIDNumber || visitor[4].toLowerCase().includes(visitorIDNumber)) // Match ID Number
            );
        });

        resetVisitorsFiltersBtn.addEventListener('click', () => {

            // Reset filter inputs
            document.querySelector('#filterVisitorHubModal #requestedBySearch').value = '';
            document.getElementById("idNumber").value = '';
            document.getElementById('visit-statusn-dropdown').value = '';

            // Clear the DataTable and repopulate with original data
            table.clear();
            originalVisitorsTableData.forEach(visitor => {
                table.row.add([
                    visitor[0], // Full HTML of the first column (including icon)
                    visitor[3], // Email
                    visitor[4], // ID Number
                    visitor[5], // Status
                    visitor[6]  // Action (HTML for edit/delete buttons)
                ]);
            });
            table.draw();

            // Remove active filter highlight
            filterVisitorsIcon.classList.remove("active-filter");

            // Close the modal
            $('#filterVisitorHubModal').modal('hide');
        });

        console.log("Filtered Visitors:", filteredData);

        // Clear the existing DataTable rows
        table.clear();

        // Add the filtered data to the DataTable
        filteredData.forEach(visitor => {
            table.row.add([
                visitor[0], // Full HTML of the first column (including icon)
                visitor[3], // Email
                visitor[4], // ID Number
                visitor[5], // Status
                visitor[6]  // Action (HTML for edit/delete buttons)
            ]);
        });

        // Redraw the DataTable
        table.draw();

        // Highlight filter icon if filters are applied
        if (visitorName || visitStatus || visitorIDNumber) {
            filterVisitorsIcon.classList.add("active-filter");
        } else {
            filterVisitorsIcon.classList.remove("active-filter");
        }

        // Close the modal
        $('#filterVisitorHubModal').modal('hide');
    });

    EditVisitRequest.addEventListener('click', () => {
        const visitorIds = getCurrentUsersIds().join(','); // Get all visitor IDs and join them as a comma-separated string
        const editVisitRequestUrl = `/VisitRequest/EditVisitRequest?visitRequestId=${currentVisitRequestId}&visitorIds=${visitorIds}`;
        window.location.href = editVisitRequestUrl;  // Redirect to the action with parameters
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

    // Populate the dropdown with user options
    const populateOptionsVisitors = (data) => {
        dropdownOptionsFilter.innerHTML = '';

        if (data.length === 0) {
            dropdownOptionsFilter.innerHTML = '<div class="dropdown-item">No results found</div>';
        } else {
            data.forEach((item) => {
                const option = document.createElement('div');
                option.textContent = item.Name;
                option.dataset.value = item.Id;
                option.classList.add('dropdown-item');

                // ✅ On click, set the selected value inside the input box
                option.addEventListener('click', () => {
                    searchInputFilter.value = item.Name; // Set text in input
                    searchInputFilter.dataset.selectedId = item.Id; // Store selected ID
                    dropdownOptionsFilter.classList.add('hidden'); // Hide dropdown after selection
                });

                dropdownOptionsFilter.appendChild(option);
            });
        }
        dropdownOptionsFilter.classList.remove('hidden');
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

    const showLoadingSpinner = () => {
        dropdownOptionsFilter.innerHTML = '<div class="dropdown-item loading-spinner">Loading...</div>';
        dropdownOptionsFilter.classList.remove('hidden');
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
        meetingAreaSearch.value = item.Name;
        meetingAreaVal = item.Id;
        meetingAreaOptions.classList.add('hidden');
        checkMandatoryFields();
    };
});

function ConfirmDeleteVisitRequest(id) {
    $.ajax({
        url: '/VisitRequest/DeleteVisitRequest',
        type: 'POST',
        data: JSON.stringify({ id: id }),  // Send ID as JSON
        contentType: 'application/json; charset=utf-8', // Ensure correct format
        dataType: 'json',
        success: function (response) {
            console.log("Visit request deleted successfully:", response);
            Notify(response.Message, "Success");

            if (response.RedirectUrl) {
                setTimeout(() => {
                    window.location.href = response.RedirectUrl;
                }, 3000);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error deleting visit request:", error);

            let errorMessage = "An error occurred while deleting the visit request.";
            if (xhr.responseText) {
                try {
                    let errorResponse = JSON.parse(xhr.responseText);
                    if (errorResponse.Message) {
                        errorMessage = errorResponse.Message;
                    }
                } catch (e) {
                    console.error("Error parsing error response:", e);
                }
            }

            Notify(errorMessage, "Error");
        }
    });
}

function DeleteVisitRequest(id) {
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
            ConfirmDeleteVisitRequest(id);
        }
    });
}

const fetchVisitors = async () => {
    try {
        const response = await fetch('/VisitorsHub/GetVisitors');
        if (!response.ok) throw new Error('Network response was not ok');
        const data = await response.json();
        allUsers = data;
        return data;
    } catch (error) {
        console.error('Error fetching organization users:', error);
        return [];
    }
};
function ConfirmDeleteVisitor(id) {
    $.ajax({
        url: '/VisitRequest/RemoveVisitRequestVisitors',
        type: 'POST',
        data: JSON.stringify({ VisitorId: id, VisitRequestId: $("#editVisitRequestModel").data("visit-id") }),  // Send ID as JSON
        contentType: 'application/json; charset=utf-8', // Ensure correct format
        dataType: 'json',
        success: function (response) {
            if (response.Status) {
                console.log("Visitor deleted successfully:", response);
                // Remove the row from the DataTable
                const table = $('#VisitorsDetailsTable').DataTable();
                table.row(window.currentRow).remove().draw(); // Remove the row and redraw the table
                Notify(response.Message, "Success");
            }
            else {
                Notify(response.Message, "Error");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error deleting visit request:", error);

            let errorMessage = "An error occurred while deleting the visitors.";
            if (xhr.responseText) {
                try {
                    let errorResponse = JSON.parse(xhr.responseText);
                    if (errorResponse.Message) {
                        errorMessage = errorResponse.Message;
                    }
                } catch (e) {
                    console.error("Error parsing error response:", e);
                }
            }

            Notify(errorMessage, "Error");
        }
    });
}

function DeleteVisitor(id) {
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
            ConfirmDeleteVisitor(id);
        }
    });
}