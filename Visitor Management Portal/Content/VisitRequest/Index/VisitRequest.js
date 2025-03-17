document.addEventListener('DOMContentLoaded', () => {
    const visitorDetails = document.querySelectorAll('.visitorDetails');
    const AddNewVisitRequest = document.querySelector('.addNewVisitRequest');
    const showFilteredResults = document.getElementById('showFilteredResultsBtn');
    const searchInput = document.getElementById('requestedBySearch');
    const dropdownOptions = document.getElementById('requestedByOptions');
    const resetFiltersBtn = document.getElementById('resetFiltersBtn');
    const filterVisitRequestsModal = document.getElementById('filterVisitRequestsModal');

    const filterIcon = document.getElementById("filterIcon"); // Select the filter icon

    var allUsers = [];
    let originalTableData = [];

    // Initialize the DataTable
    const table = $('#visitorsTable').DataTable({
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
            { "orderable": false, "targets": [9] },
        ],
        "buttons": ['print'],
        "language": {
            "emptyTable": "No visit requests available." // ✅ Custom empty table message
        },
    });

    // Capture the initial data from the rendered table
    saveOriginalData();

    function saveOriginalData() {
        originalTableData = [];

        // Loop through each row of the DataTable
        table.rows().every(function () {
            let row = $(this.node()); // Get the row as a jQuery object
            let visiteRequestID = row.find("td:eq(1).visitorDetails").data("id"); // Extract VisiteRequestID

            let rowData = [
                row.find("td:eq(0)").text().trim(),  // Visit Serial
                row.find("td:eq(1) .fw-500").text().trim(), // Requested By Name
                row.find("td:eq(1) p:last-child").text().trim(), // Organization
                row.find("td:eq(2)").text().trim(), // Visitors Count
                row.find("td:eq(3)").text().trim(), // Purpose
                row.find("td:eq(4)").text().trim(), // Date
                row.find("td:eq(5)").text().trim(), // Time
                row.find("td:eq(6)").text().trim(), // Duration
                row.find("td:eq(7)").text().trim(), // Location
                row.find("td:eq(8)").text().trim(), // Status
                row.find("td:eq(9)").text().trim(), // Approved By
                visiteRequestID || "-" // Include VisiteRequestID at the end
            ];

            originalTableData.push(rowData);
        });
    }



    const datatableSearchInput = document.querySelector('#visitorsTable_filter input[type="search"]');

    const pageSearchInput = document.querySelector(".VisitRequest_topSection .search-bar input");

    if (datatableSearchInput && pageSearchInput) {
        pageSearchInput.addEventListener("input", (e) => {
            datatableSearchInput.value = e.target.value;

            table.search(e.target.value).draw();
        });
    }


    visitorDetails.forEach((visitorDetail) => {
        visitorDetail.addEventListener('click', () => {
            const Id = visitorDetail.getAttribute('data-id');
            window.location.href = `/VisitRequest/VistRequestDetails?visitRequestId=${Id}`;
        });
    });

    // Close dropdown when clicking anywhere inside the modal (except on the input)
    filterVisitRequestsModal.addEventListener('click', (event) => {
        // Check if the clicked element is NOT inside the dropdown or the input field
        if (event.target !== searchInput && !dropdownOptions.contains(event.target)) {
            dropdownOptions.classList.add('hidden'); // Hide the dropdown
        }
    });

    // Prevent closing when clicking inside the dropdown itself
    dropdownOptions.addEventListener('click', (event) => {
        event.stopPropagation(); // Stop event from reaching the modal click listener
    });

    // Show dropdown when input is clicked
    searchInput.addEventListener('focus', () => {
        dropdownOptions.classList.remove('hidden');
    });

    AddNewVisitRequest.addEventListener('click', () => {
        window.location.href = '/VisitRequest/AddNewVisit';
    });

    const fetchOrganizationUsers = async () => {
        try {
            const response = await fetch('/VisitRequest/GetOrganizationUsers');
            if (!response.ok) throw new Error('Network response was not ok');
            const data = await response.json();
            allUsers = data;
            return data;
        } catch (error) {
            console.error('Error fetching organization users:', error);
            return [];
        }
    };

    // Populate the dropdown with user options
    const populateOptions = (data) => {
        dropdownOptions.innerHTML = '';

        if (data.length === 0) {
            dropdownOptions.innerHTML = '<div class="dropdown-item">No results found</div>';
        } else {
            data.forEach((item) => {
                const option = document.createElement('div');
                option.textContent = item.UserName;
                option.dataset.value = item.UserID;
                option.classList.add('dropdown-item');

                // ✅ On click, set the selected value inside the input box
                option.addEventListener('click', () => {
                    searchInput.value = item.UserName; // Set text in input
                    searchInput.dataset.selectedId = item.UserID; // Store selected ID
                    dropdownOptions.classList.add('hidden'); // Hide dropdown after selection
                });

                dropdownOptions.appendChild(option);
            });
        }
        dropdownOptions.classList.remove('hidden');
    };

    const showLoadingSpinner = () => {
        dropdownOptions.innerHTML = '<div class="dropdown-item loading-spinner">Loading...</div>';
        dropdownOptions.classList.remove('hidden');
    };

    searchInput.addEventListener('focus', async () => {
        if (allUsers.length === 0) {
            showLoadingSpinner();
            const data = await fetchOrganizationUsers();
            allUsers = data;
            populateOptions(data);
        } else {
            populateOptions(allUsers);
        }
    });

    // Handle input typing for real-time filtering
    searchInput.addEventListener('input', () => {
        const searchTerm = searchInput.value.toLowerCase();
        const filteredUsers = allUsers.filter(user => user.UserName.toLowerCase().includes(searchTerm));
        populateOptions(filteredUsers);
    });

    showFilteredResults.addEventListener('click', () => {
        // Capture values from the form
        const visitDate = document.getElementById('date').value || null;
        const visitTime = document.getElementById('time').value || null;
        const requestedByName = document.getElementById('requestedBySearch').value.trim().toLowerCase() || null;
        const visitStatus = document.getElementById('visit-statusn-dropdown').value || null;

        debugger;
        //  Perform Client-Side Filtering
        const filteredData = originalTableData.filter(visitRequest => {
            return (
                (!visitDate || visitRequest[5] === visitDate) && // Date Match
                (!visitTime || visitRequest[6] === visitTime) && // Time Match
                (!requestedByName || visitRequest[1].toLowerCase().includes(requestedByName)) && // Requested By Name Match
                (!visitStatus || visitRequest[9] === visitStatus) // Status Match
            );
        });

        console.log("Filtered Visit Requests:", filteredData);

        // ✅ Check if any filter is applied and update the filter icon
        if (visitDate || visitTime || requestedByName || visitStatus) {
            filterIcon.classList.add("active-filter"); // Highlight icon
        } else {
            filterIcon.classList.remove("active-filter"); // Remove highlight if no filters
        }

        updateDataTable(filteredData); // Update table with filtered results
        $('#filterVisitRequestsModal').modal('hide'); // Close modal after filtering
    });

    // Function to update DataTable with filtered results
    function updateDataTable(data) {
        table.clear();

        if (!Array.isArray(data) || data.length === 0) {
            table.draw();
            return;
        }

        data.forEach(visitRequest => {
            table.row.add([
                visitRequest[0] || "-", // Visit Serial
                `<div class="d-flex align-items-center justify-content-start gap-1 visitorDetails" data-id="${visitRequest[0]}">
                <i class="fa-solid fa-user-gear mx-1" style="color: #A7AEC2; font-size: 15px"></i>
                <div class="text-content-container">
                    <p class="p-0 m-0 fw-500">${visitRequest[1] || "--"}</p>
                    <p class="p-0 m-0">${visitRequest[2] || "--"}</p>
                </div>
            </div>`,
                `<i class="fa-solid fa-user mx-1"></i> ${visitRequest[3] || "--"}`, // Visitors Count
                visitRequest[4] || "--", // Purpose
                visitRequest[5] || "-", // Date
                visitRequest[6] || "-", // Time
                visitRequest[7] || "--", // Duration
                visitRequest[8] || "--", // Location
                visitRequest[9] || "--", // Status
                visitRequest[10] || "--" // Approved By
            ]);
        });

        table.draw();

        rebindTableEvents();
    }

    //  Function to rebind events after updating the table
    function rebindTableEvents() {
        console.log("🔄 Rebinding table events...");

        // Rebind click event for visitor details
        $(".visitorDetails").off("click").on("click", function () {
            const rowIndex = $(this).closest("tr").index(); // Get row index
            const visitRequestId = originalTableData[rowIndex][11]; // ✅ Get VisiteRequestID from stored data (last column)

            console.log("📌 Visit Request Clicked (Correct ID):", visitRequestId);

            if (visitRequestId && visitRequestId !== "-") {
                window.location.href = `/VisitRequest/VistRequestDetails?visitRequestId=${visitRequestId}`;
            } else {
                console.warn("⚠️ No VisitRequestID found for this row.");
            }
        });

        // Rebind events for edit buttons
        $(".editVisitor").off("click").on("click", function () {
            const rowIndex = $(this).closest("tr").index();
            const visitRequestId = originalTableData[rowIndex][11]; // Get correct ID

            console.log("📝 Edit Visitor Clicked:", visitRequestId);

            if (visitRequestId && visitRequestId !== "-") {
                window.location.href = `/VisitorsHub/EditVisitor/${visitRequestId}`;
            } else {
                console.warn("⚠️ No VisitRequestID found for this row.");
            }
        });

        // Rebind delete event
        $(".deleteVisitor").off("click").on("click", function () {
            const rowIndex = $(this).closest("tr").index();
            const visitRequestId = originalTableData[rowIndex][11]; // Get correct ID

            if (!visitRequestId || visitRequestId === "-") {
                console.warn("⚠️ No VisitRequestID found for this row.");
                return;
            }

            if (confirm("Are you sure you want to delete this visitor?")) {
                fetch(`/VisitorsHub/DeleteVisitor?id=${visitRequestId}`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" }
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.message.includes("successfully")) {
                            Notify("Visitor deleted successfully.", "Success");
                            showFilteredResults.click(); // Refresh table after deletion
                        } else {
                            Notify("Error deleting visitor: " + data.message, "Error");
                        }
                    })
                    .catch(error => {
                        console.error("Error:", error);
                    });
            }
        });
    }

    // Reset filters button
    resetFiltersBtn.addEventListener('click', () => {
        document.getElementById('date').value = "";
        document.getElementById('time').value = "";

        const requestedBySearch = document.getElementById('requestedBySearch');
        requestedBySearch.value = "";
        delete requestedBySearch.dataset.selectedId;

        // Reset the dropdown to the default placeholder option
        const visitStatusDropdown = document.getElementById('visit-statusn-dropdown');
        visitStatusDropdown.selectedIndex = 0; // Resets to "Select Status"

        table.search("").columns().search("").draw(); // Reset all filters
        filterIcon.classList.remove("active-filter"); // Remove filter highlight
        updateDataTable(originalTableData);
        $('#filterVisitRequestsModal').modal('hide'); // Close modal after resetting
    });
});
