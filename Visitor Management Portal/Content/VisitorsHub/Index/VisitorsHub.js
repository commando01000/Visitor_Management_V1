document.addEventListener('DOMContentLoaded', () => {

    const addNewVistor = document.querySelector('.addNewVistor');
    const editVisitor = document.querySelectorAll('.editVisitor');
    const visitorDetails = document.querySelectorAll('.visitorDetails');
    const deleteButtons = document.querySelectorAll(".deleteVisitor");

    const showFilteredResults = document.getElementById('showFilteredResultsBtn');
    const filterVisitorHubModal = document.getElementById('filterVisitorHubModal');
    const filterVisitorsIcon = document.getElementById("filterVisitorsIcon");
    const resetVisitorsFiltersBtn = document.getElementById('resetFiltersBtn');
    const searchInput = document.getElementById('requestedBySearch');
    const dropdownOptions = document.getElementById('requestedByOptions');

    var allUsers = [];
    let originalVisitorsTableData = [];

    const visitorsTable = $('#visitorsTable').DataTable({
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
        columnDefs: [
            { orderable: false, targets: [1] }
        ],
        buttons: ['print'],
        "language": {
            "emptyTable": "No visitors available." // ✅ Custom empty table message
        }
    });

    // Custom search functionality
    const datatableSearchInput = document.querySelector('#visitorsTable_filter input[type="search"]');
    const pageSearchInput = document.querySelector(".VisitorsHub_topSection .search-bar input");

    if (datatableSearchInput && pageSearchInput) {
        pageSearchInput.addEventListener("input", (e) => {
            datatableSearchInput.value = e.target.value;
            visitorsTable.search(e.target.value).draw();
        });
    }

    // Close dropdown when clicking anywhere inside the modal (except on the input)
    filterVisitorHubModal.addEventListener('click', (event) => {
        // Check if the clicked element is NOT inside the dropdown or the input field
        if (event.target !== searchInput && !dropdownOptions.contains(event.target)) {
            dropdownOptions.classList.add('hidden'); // Hide the dropdown
        }
    });

    // Prevent closing when clicking inside the dropdown itself
    dropdownOptions.addEventListener('click', (event) => {
        event.stopPropagation(); // Stop event from reaching the modal click listener
    });

    // Capture the initial data from the rendered table
    saveVisitorsOriginalData();

    function saveVisitorsOriginalData() {
        originalVisitorsTableData = [];

        // Loop through each row of the DataTable
        visitorsTable.rows().every(function () {
            let row = $(this.node()); // Get the row as a jQuery object
            let visitorID = row.data("id"); // Extract Visitor ID

            let rowData = [
                row.find("td:eq(0) h6").text().trim(), // ✅ Corrected: Visitor Name inside <h6>
                row.find("td:eq(0) p").text().trim(), // ✅ Organization Name inside <p>
                row.find("td:eq(1)").text().trim(), // Email
                row.find("td:eq(2)").text().trim(), // ID Number
                row.find("td:eq(3)").text().trim(), // Status
                visitorID || "-" // Include VisitorID at the end
            ];

            originalVisitorsTableData.push(rowData);
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

    // Populate the dropdown with user options
    const populateOptions = (data) => {
        dropdownOptions.innerHTML = '';

        if (data.length === 0) {
            dropdownOptions.innerHTML = '<div class="dropdown-item">No results found</div>';
        } else {
            data.forEach((item) => {
                const option = document.createElement('div');
                option.textContent = item.Name;
                option.dataset.value = item.Id;
                option.classList.add('dropdown-item');

                // ✅ On click, set the selected value inside the input box
                option.addEventListener('click', () => {
                    searchInput.value = item.Name; // Set text in input
                    searchInput.dataset.selectedId = item.Id; // Store selected ID
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
            const data = await fetchVisitors();
            allUsers = data;
            populateOptions(data);
        } else {
            populateOptions(allUsers);
        }
    });

    // Handle input typing for real-time filtering
    searchInput.addEventListener('input', () => {
        const searchTerm = searchInput.value.toLowerCase();
        const filteredUsers = allUsers.filter(user => user.Name.toLowerCase().includes(searchTerm));
        populateOptions(filteredUsers);
    });

    showFilteredResults.addEventListener('click', () => {
        const visitorName = document.getElementById('requestedBySearch').value.trim().toLowerCase() || null;
        const visitorIDNumber = document.getElementById("idNumber").value || null;
        const visitStatus = document.getElementById('visit-statusn-dropdown').value || null;

        const filteredData = originalVisitorsTableData.filter(visitor => {
            return (
                (!visitorName || visitor[0].toLowerCase().includes(visitorName)) && // Match Visitor Name
                (!visitStatus || visitor[4] === visitStatus) && // Match Status
                (!visitorIDNumber || visitor[3].toLowerCase().includes(visitorIDNumber)) // Match ID Number
            );
        });

        console.log("Filtered Visitors:", filteredData);

        //debugger;

        // Highlight filter icon if filters are applied
        if (visitorName || visitStatus || visitorIDNumber) {
            filterVisitorsIcon.classList.add("active-filter");
        } else {
            filterVisitorsIcon.classList.remove("active-filter");
        }

        updateVisitorsTable(filteredData);
        $('#filterVisitorHubModal').modal('hide'); // Close modal after resetting
    });

    // Function to update DataTable with filtered results
    function updateVisitorsTable(data) {
        visitorsTable.clear();

        if (!Array.isArray(data) || data.length === 0) {
            visitorsTable.draw();
            return;
        }

        data.forEach(visitor => {
            visitorsTable.row.add([
                `<div class="d-flex align-items-center visitorDetails" data-id="${visitor[4]}">
                    <i class="fa-solid fa-user-gear mx-1" style="color: #A7AEC2; font-size: 20px"></i>
                    <div class="text-content-container">
                        <h6>${visitor[0] || "--"}</h6>
                        <p class="p-0 m-0">${visitor[1] || "--"}</p>
                    </div>
                </div>`,
                visitor[2] || "--", // Email
                visitor[3] || "--", // ID Number
                visitor[4] || "--", // Status
                `<div class="btns d-flex align-items-center gap-3">
                    <i class="fa-solid fa-pencil fs-5 editVisitor" style="color: black; cursor:pointer;" data-id="${visitor[5]}"></i>
                    <i class="fa-regular fa-trash-can fs-5 deleteVisitor" style="color: black; cursor:pointer;" data-id="${visitor[5]}"></i>
                </div>`
            ]);
        });

        visitorsTable.draw();
        rebindVisitorsTableEvents();
    }

    // Function to rebind events after filtering
    function rebindVisitorsTableEvents() {
        console.log("🔄 Rebinding Visitors Table events...");

        $(".visitorDetails").off("click").on("click", function () {
            const rowIndex = $(this).closest("tr").index();
            const visitorId = originalVisitorsTableData[rowIndex][4]; // Get Visitor ID

            console.log("📌 Visitor Clicked (Correct ID):", visitorId);

            if (visitorId && visitorId !== "-") {
                window.location.href = `/VisitorsHub/VisitorDetails/${visitorId}`;
            } else {
                console.warn("⚠️ No VisitorID found for this row.");
            }
        });

        $(".editVisitor").off("click").on("click", function () {
            const visitorId = $(this).data("id");
            window.location.href = `/VisitorsHub/EditVisitor/?id=${visitorId}`;
        });

        $(".deleteVisitor").off("click").on("click", function () {
            const visitorId = $(this).data("id");

            if (confirm("Are you sure you want to delete this visitor?")) {
                fetch(`/VisitorsHub/DeleteVisitorHub?id=${visitorId}`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" }
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.Message.includes("successfully")) {
                            Notify("Visitor deleted successfully.", "Success");
                            location.reload();
                        } else {
                            Notify("Error deleting visitor: " + data.Message, "Error");
                        }
                    })
                    .catch(error => {
                        console.error("Error:", error);
                    });
            }
        });
    }

    // ✅ Reset Filters Button
    resetVisitorsFiltersBtn.addEventListener('click', () => {

        document.getElementById("idNumber").value = "";
        document.getElementById("requestedBySearch").value = "";
        delete document.getElementById("requestedBySearch").dataset.selectedId;

        const visitStatusDropdown = document.getElementById('visit-statusn-dropdown');
        visitStatusDropdown.selectedIndex = 0; // Resets to "Select Status"
        searchInput.value = "";
        visitorsTable.search("").columns().search("").draw();
        filterVisitorsIcon.classList.remove("active-filter");
        updateVisitorsTable(originalVisitorsTableData);
        $('#filterVisitorHubModal').modal('hide'); // Close modal after resetting
    });


    addNewVistor.addEventListener('click', () => {
        window.location.href = "/VisitorsHub/AddVisitor";
    })

    editVisitor.forEach(visitor => {
        visitor.addEventListener('click', () => {
            window.location.href = `/VisitorsHub/EditVisitor/?id=${visitorId}`;
        })
    })

    visitorDetails.forEach((visitor) => {
        visitor.addEventListener('click', (event) => {
            event.stopPropagation();

            const row = visitor.closest('tr');
            const visitorId = row.getAttribute('data-id');

            window.location.href = `/VisitorsHub/VisitorDetails/${visitorId}`;
        });
    });

    deleteButtons.forEach(button => {
        button.addEventListener("click", function () {
            const visitorId = this.getAttribute("data-id");

            if (confirm("Are you sure you want to delete this visitor?")) {
                fetch(`/VisitorsHub/DeleteVisitorHub?id=${visitorId}`, {
                    method: "POST", // Use POST instead of DELETE to avoid CORS issues
                    headers: { "Content-Type": "application/json" }
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.message.includes("successfully")) {
                            Notify("Visitor deleted successfully.", NotifyStatus.Success);
                            location.reload();
                        } else {
                            Notify("Error deleting visitor: " + data.message, NotifyStatus.Error);
                        }
                    })
                    .catch(error => {
                        console.error("Error:", error);
                    });
            }
        });
    });
});

const list_Item = document.querySelector(".sidebar-menu .VisitorsHub")
list_Item.classList.add('active-sidebar-menu');


