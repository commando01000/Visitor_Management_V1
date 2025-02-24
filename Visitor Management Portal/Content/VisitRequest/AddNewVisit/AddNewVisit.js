const continueBtn = document.querySelector(".continue");
const backBtn = document.querySelector(".back");
const cancelBtn = document.querySelector(".cancel");
const stepTabs = document.querySelectorAll(".step-tab .nav-link");
const tabContents = document.querySelectorAll(".tab-content");
const visitPurpose = document.querySelectorAll(".visit-option");
const visitLocationOptions = document.querySelectorAll(".visit-location-option");
const searchInput = document.getElementById('requestedBySearch');
const dropdownOptions = document.getElementById('requestedByOptions');
const buildingDropdown = document.getElementById('building');
const zoneDropdown = document.getElementById('zone');
const meetingAreaSearch = document.getElementById('meetingAreaSearch');
const meetingAreaOptions = document.getElementById('meetingAreaOptions');
const visitTimeInput = document.getElementById('date');
const visitUntilInput = document.getElementById('time');
const statusDropdown = document.getElementById('status');
const visitPurposeDropdown = document.getElementById('visitPurpose');
const subjectText = document.getElementById('subject');
visitLocationOptions.value = 0;
let currentStep = 1;
let allUsers = [];
let buildings = [];
let zones = [];
let meetingAreas = [];
var selectedUserId;
var meetingAreaVal;
var allVisitors = [];
var selectedVisitors = [];

// Initially disable Zone & Meeting Area
zoneDropdown.disabled = true;
meetingAreaSearch.disabled = true;

// Set default disabled text
zoneDropdown.innerHTML = `<option value="">Select building first</option>`;
meetingAreaSearch.placeholder = "Select zone first";

/**
 * Show "Loading..." Text Inside a Dropdown
 */
const showLoadingText = (element, placeholderText) => {
    element.innerHTML = `<option value="">${placeholderText} (Loading...)</option>`;
    element.disabled = true;
};

continueBtn.disabled = true;
continueBtn.classList.add('custom-class');

function saveVisitRequest() {
    const visitPurpose = visitPurposeDropdown.value;
    const visitLocation = visitLocationOptions.value;
    const visitTime = visitTimeInput.value;
    const visitUntil = visitUntilInput.value;
    const status = statusDropdown.value;
    const building = buildingDropdown.value;
    const zone = zoneDropdown.value;
    const meetingArea = meetingAreaVal;
    const selectedIds = selectedVisitors; // Array of selected visitor IDs
    const subject = subjectText.value;
    const reqBy = selectedUserId;

    // Constructing the request payload
    const requestData = {
        Subject: subject, // Change if dynamic input is needed
        Purpose: parseInt(visitPurpose),
        VisitTime: visitTime,
        VisitUntil: visitUntil,
        Location: visitLocation,
        Building: building,
        Zone: zone,
        MeetingArea: meetingArea,
        StatusReason: parseInt(status),
        RequestedBy: reqBy,
        VisitorsIds: selectedIds
    };
    debugger;
    // AJAX request to `VisitorRequest/AddVisitRequest`
    $.ajax({
        url: '/VisitRequest/AddVisitRequest',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(requestData),
        dataType: 'json',
        success: function (response) {
            console.log("Response:", response);
            if (response.Status) { // Match with C# `Status` property
                Notify(response.Message, "Success");

                // Wait for 2 seconds before redirecting
                setTimeout(() => {
                    window.location.href = "/VisitRequest/Index";
                }, 2000);
            } else {
                alert("Failed to add visit request: " + response.Message);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error adding visit request:", error);
            alert("An error occurred while processing the visit request.");
        }
    });

}


function checkMandatoryFields() {
    let allFilled = true;

    if (!statusDropdown.value) {
        allFilled = false;
    }

    if (!visitTimeInput.value) {
        allFilled = false;
    }

    if (!visitUntilInput.value) {
        allFilled = false;
    }


    const visitTime = new Date(visitTimeInput.value);
    const visitUntil = new Date(visitUntilInput.value);


    const visitTimeTimestamp = visitTime.getTime();
    const visitUntilTimestamp = visitUntil.getTime();


    if (visitUntilTimestamp <= visitTimeTimestamp) {

        allFilled = false;
        Notify("Visit until time should be greater than visit time", NotifyStatus.Error);
        visitUntilInput.value = "";
    }

    if (!visitPurposeDropdown.value) {
        allFilled = false;
    }

    if (!searchInput.value.trim()) {
        allFilled = false;
    }

    // Validate that the requestedBySearch input must match a dropdown option
    if (!selectedUserId) {
        allFilled = false;
    }

    if (visitLocationOptions.value === "1") {
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

// Add event listeners to fields
statusDropdown.addEventListener('change', checkMandatoryFields);
visitTimeInput.addEventListener('change', checkMandatoryFields);
visitUntilInput.addEventListener('change', checkMandatoryFields);
visitPurposeDropdown.addEventListener('change', checkMandatoryFields);
searchInput.addEventListener('input', checkMandatoryFields);

function updateStep(step) {
    stepTabs.forEach(tab => tab.classList.remove("active"));
    tabContents.forEach(content => {
        content.classList.remove("active");
        content.classList.add("d-none");
    });

    document.querySelector(`.nav-link[data-step="${step}"]`).classList.add("active");
    const currentContent = document.querySelector(`#tab${step}`);
    if (currentContent) {
        currentContent.classList.add("active");
        currentContent.classList.remove("d-none");
    }

    backBtn.classList.toggle("d-none", step === 1);
    if (step === 2) {
        getVisitorsFromMyOrg();
    }
}

function getVisitorsFromMyOrg() {
    $.ajax({
        url: '/VisitRequest/GetVisitors', // Adjust the URL if necessary
        type: 'GET',
        dataType: 'json',
        success: function (visitors) {
            console.log("Visitors fetched:", visitors);
            allVisitors = visitors;
        },
        error: function (xhr, status, error) {
            console.error("Error fetching visitors:", error);
        }
    });
}

// Function to add visitor to the list
function addVisitorToList(visitor) {
    var visitorsContainer = $(".visitors-list-contianer");

    var visitorElement = $("<div>")
        .addClass("added-visitor")
        .attr("data-id", visitor.Id); // Add data-id attribute

    var visitorDetails = `
        <div class="d-flex align-items-center gap-3 flex-grow-1">
            <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 32 32">
                <g transform="translate(-34 -303)">
                    <rect width="32" height="32" rx="6" transform="translate(34 303)" fill="#fff" />
                    <path d="M27.035,29.417a5.709,5.709,0,1,0-5.693-5.724A5.694,5.694,0,0,0,27.035,29.417Zm0-9.916a4.192,4.192,0,1,1-4.192,4.223A4.192,4.192,0,0,1,27.035,19.5ZM17.151,38.332H36.983a.761.761,0,0,0,.751-.751,7.182,7.182,0,0,0-7.163-7.163H23.563A7.182,7.182,0,0,0,16.4,37.582.761.761,0,0,0,17.151,38.332Zm6.413-6.413H30.57a5.641,5.641,0,0,1,5.6,4.911H17.964A5.641,5.641,0,0,1,23.563,31.92Z" transform="translate(22.933 290.333)" fill="#2f2f2f" />
                </g>
            </svg>
            <div class="added-visitor-details d-flex flex-column gap-2">
                <div class="name">${visitor.Name}</div>
                <div class="email">${visitor.Email}</div>
            </div>
        </div>
        <div class="remove-container" onclick="removeVisitor('${visitor.Id}')">
            <svg class="remove-visitor" xmlns="http://www.w3.org/2000/svg" width="18.385" height="18.385" viewBox="0 0 18.385 18.385">
                <g transform="translate(9.192 -7.778) rotate(45)">
                    <rect width="2" height="24" rx="1" transform="translate(11)" fill="#2f2f2f" />
                    <rect width="24" height="2" rx="1" transform="translate(0 11)" fill="#2f2f2f" />
                </g>
            </svg>
        </div>
    `;

    visitorElement.html(visitorDetails);
    visitorsContainer.append(visitorElement);
}


// Function to remove visitor from UI and re-add to the selection list
function removeVisitor(visitorId) {
    // Remove visitor from selectedVisitors array
    selectedVisitors = selectedVisitors.filter(id => id !== visitorId);

    // Find the removed visitor object
    var removedVisitor = allVisitors.find(visitor => visitor.Id === visitorId);

    // Ensure the visitor is not already in `allVisitors` before adding back
    if (removedVisitor && !allVisitors.some(v => v.Id === visitorId)) {
        allVisitors.push(removedVisitor);
    }

    console.log("selectedVisitors " + selectedVisitors);
    // Remove from DOM
    $(".added-visitor[data-id='" + visitorId + "']").remove();
}




$(document).ready(function () {
    var searchInput = $(".search-bar input");
    var autocompleteContainer = $("<div>").addClass("autocomplete-results").css({
        position: "absolute",
        top: "100%",
        background: "#fff",
        opacity: 0.8,
        width: "80%",
        border: "1px solid #ddd",
        "z-index": 1000
    });

    $(".search-seciton").append(autocompleteContainer);

    searchInput.on("input", function () {
        var query = $(this).val().trim().toLowerCase();

        if (query.length < 3) {
            autocompleteContainer.empty().hide();
            return;
        }
        console.log(allVisitors);
        // Filter from stored visitors and exclude already selected ones
        var filteredVisitors = allVisitors.filter((visitor, index, self) => {
            console.log("Visitor: " + visitor);
            return !selectedVisitors.includes(visitor.Id) &&
                (visitor.Name.toLowerCase().includes(query) || visitor.Email.toLowerCase().includes(query)) &&
                self.findIndex(v => v.Id === visitor.Id) === index; // Ensure uniqueness
        });

        autocompleteContainer.empty();

        if (filteredVisitors.length === 0) {
            autocompleteContainer.append("<p class='p-2 text-muted'>No results found</p>");
        } else {
            $.each(filteredVisitors, function (index, visitor) {
                var visitorItem = $("<div>")
                    .addClass("autocomplete-item p-2")
                    .css({ "cursor": "pointer", "border-bottom": "1px solid #eee" })
                    .text(visitor.Name + " (" + visitor.Email + ")")
                    .data("visitor", visitor)
                    .on("click", function () {
                        addVisitorToList($(this).data("visitor"));
                        selectedVisitors.push(visitor.Id); // Mark as selected
                        console.log("Selected Visitors: ", selectedVisitors);
                        autocompleteContainer.empty().hide();
                        searchInput.val(""); // Clear search input
                    });

                autocompleteContainer.append(visitorItem);
            });
        }
        autocompleteContainer.show();
    });

    // Hide autocomplete when clicking outside
    $(document).on("click", function (event) {
        if (!$(event.target).closest(".search-seciton").length) {
            autocompleteContainer.hide();
        }
    });
});


function showNextStep() {
    if (currentStep < stepTabs.length) {
        currentStep++;
        updateStep(currentStep);
    }
    else {
        // Save the visit request using the selectedVisitors array and check that the selectedVisitors array is not empty
        selectedVisitors.length > 0 ? saveVisitRequest() : alert("Please select at least one visitor.");
    }
}

function showPreviousStep() {
    if (currentStep > 1) {
        currentStep--;
        updateStep(currentStep);
    }
}

cancelBtn.addEventListener("click", function () {
    window.location.href = "/VisitRequest";
});

continueBtn.addEventListener("click", function () {

    if (!checkMandatoryFields()) {
        return;
    }
    showNextStep();
});

backBtn.addEventListener("click", showPreviousStep);

visitPurpose.forEach(option => {
    option.addEventListener('click', () => {
        visitPurpose.forEach(opt => opt.classList.remove('selected'));
        option.classList.add('selected');
        checkMandatoryFields();
    });
});

visitLocationOptions.forEach(option => {
    const officeLocation = document.querySelector('.office-lcation');
    const meetingAreaLocation = document.querySelector('.meetingArea-lcation');

    option.addEventListener('click', async () => {
        visitLocationOptions.forEach(opt => opt.classList.remove('selected'));
        visitLocationOptions.value = option.dataset.value;
        if (option.dataset.value === "0") {
            officeLocation.classList.remove('d-none');
            meetingAreaLocation.classList.add('d-none');
        } else {
            officeLocation.classList.add('d-none');
            meetingAreaLocation.classList.remove('d-none');

            if (buildings.length === 0) {
                showLoadingText(buildingDropdown, 'Select building');
                buildings = await fetchBuildings();
                populateDropdown(buildingDropdown, buildings, 'Select building');
            }
        }
        checkMandatoryFields();
    });
});

const showLoadingSpinner = () => {
    dropdownOptions.innerHTML = '<div class="dropdown-item loading-spinner">Loading...</div>';
    dropdownOptions.classList.remove('hidden');
};

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

const filterUsers = (query) => {
    return allUsers.filter(user => user.UserName.toLowerCase().includes(query.toLowerCase()));
};

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
            option.addEventListener('click', () => selectOption(item));
            dropdownOptions.appendChild(option);
        });
    }
    dropdownOptions.classList.remove('hidden');
};

const selectOption = (item) => {
    searchInput.value = item.UserName;
    // store the id of the selected user
    selectedUserId = item.UserID;
    dropdownOptions.classList.add('hidden');
    checkMandatoryFields();
};

searchInput.addEventListener('focus', async () => {
    if (allUsers.length === 0) {
        showLoadingSpinner();
        const data = await fetchOrganizationUsers();
        populateOptions(data);
    } else {
        populateOptions(allUsers);
    }
});

searchInput.addEventListener('input', (event) => {
    const query = event.target.value.trim();
    if (query.length > 0) {
        const filteredUsers = filterUsers(query);
        populateOptions(filteredUsers);
    } else {
        populateOptions(allUsers);
    }

    validateRequestedBySearch(); // Revalidate input after typing
});

// Function to validate `requestedBySearch` input
function validateRequestedBySearch() {
    const typedInput = searchInput.value.trim().toLowerCase();
    let matchFound = false;

    document.querySelectorAll('.dropdown-item').forEach(option => {
        if (option.textContent.toLowerCase() === typedInput) {
            selectedUserId = option.dataset.value; // Set valid selected user ID
            matchFound = true;
        }
    });

    if (!matchFound) {
        selectedUserId = null;
    }

    checkMandatoryFields(); // Revalidate form fields
}

document.addEventListener('click', (event) => {
    if (!event.target.closest('#requestedByDropdown')) {
        dropdownOptions.classList.add('hidden');
    }
});

const showLoadingSpinnerForLocation = (element) => {
    element.innerHTML = '<div class="dropdown-item loading-spinner">Loading...</div>';
    element.classList.remove('hidden');
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

/**
 * Populate Meeting Area Dropdown
 */
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

updateStep(currentStep);

