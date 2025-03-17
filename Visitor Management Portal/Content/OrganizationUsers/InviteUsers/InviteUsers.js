document.addEventListener('DOMContentLoaded', async () => {

    const backBtn = document.querySelector(".cancle");
    const searchInput = document.getElementById('ReportingToSearch');
    const dropdownOptions = document.getElementById('ReportingToOptions');
    const continueBtn = document.querySelector(".submit-btn");
    const roleDropdown = document.getElementById('roleDropdown');

    const buildingDropdown = document.getElementById('building');
    const zoneDropdown = document.getElementById('zone');
    const floorNumber = document.getElementById('floor-number');

    const emailAddress = document.getElementById('email-address');
    const password = document.getElementById('password');
    const phoneNumber = document.getElementById('phone-number');
    const fullName = document.getElementById('full-name');
    const jobTitle = document.getElementById('job-title');


    roleDropdown.addEventListener('focus', handleRoleDropdownFocus);
    searchInput.addEventListener('focus', handleSearchInputFocus);
    searchInput.addEventListener('input', handleSearchInputInput);

    searchInput.addEventListener('input', checkMandatoryFields);

    let allUsers = [];
    var selectedUserId;
    let buildings = [];
    zoneDropdown.disabled = true; // should select a building first

    backBtn.addEventListener('click', () => {
        window.history.back();
    });

    continueBtn.addEventListener("click", function () {

        if (!checkMandatoryFields()) {
            return;
        }

        saveOrganizationUser();
    });

    buildingDropdown.addEventListener('change', async (event) => {
        const buildingId = event.target.value;
        // Reset dependent dropdowns
        zoneDropdown.innerHTML = `<option value="">Select zone</option>`;
        zoneDropdown.disabled = true;

        if (buildingId) {
            showLoadingText(zoneDropdown, 'Select zone');
            zones = await fetchZones(buildingId);
            populateDropdown(zoneDropdown, zones, 'Select zone');
            zoneDropdown.disabled = false; // Enable zone dropdown after fetching
        }
        checkMandatoryFields();
    });


    checkMandatoryFields();

    function saveOrganizationUser() {

        const building = buildingDropdown.value;
        const zone = zoneDropdown.value;
        const reqBy = selectedUserId;
        const floor = floorNumber.value;
        const role = roleDropdown.value;

        // Constructing the request payload
        const requestData = {
            Name: fullName.value,
            BuildingId: building,
            ZoneId: zone,
            RoleId: role,
            Floor: floor,
            JobTitle: jobTitle.value,
            Email: emailAddress.value,
            Password: password.value,
            Phone: phoneNumber.value,
            ReportingtoId: reqBy,
        };
        //debugger;
        // AJAX request to `VisitorRequest/AddVisitRequest`
        $.ajax({
            url: '/OrganizationUsers/CreateUser',
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
                        window.location.href = "/OrganizationUsers/Index";
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

    function handleSearchInputInput(event) {
        console.log('ReportingTo search input changed');
        const query = event.target.value.trim();
        const filteredUsers = query.length > 0 ? filterUsers(query) : allUsers;
        populateOptions(filteredUsers, "@Model.Reportingto");
    }

    async function handleSearchInputFocus() {
        console.log('ReportingTo search input focused');
        if (allUsers.length === 0) {
            showLoadingSpinner();
            allUsers = await fetchOrganizationUsers();
        }
        populateOptions(allUsers);
    }

    async function fetchOrganizationUsers() {
        console.log('Fetching organization users...');
        try {
            const response = await fetch('/VisitRequest/GetOrganizationUsers');
            if (!response.ok) throw new Error('Network response was not ok');
            const data = await response.json();
            console.log('Organization users fetched:', data);
            return data;
        } catch (error) {
            console.error('Error fetching organization users:', error);
            return [];
        }
    };

    const showLoadingSpinner = () => {
        console.log('Showing loading spinner for ReportingTo');
        dropdownOptions.innerHTML = '<div class="dropdown-item loading-spinner">Loading...</div>';
        dropdownOptions.classList.remove('hidden');
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

    async function handleRoleDropdownFocus(event) {
        console.log('Role dropdown focused');
        const selectedRoleValue = event.target.value;
        // Extract the roles dynamically from the HTML
        const roles = Array.from(roleDropdown.options).map(option => ({
            Id: option.value,
            Name: option.textContent
        })).filter(role => role.Id !== ""); // Exclude the default "Choose role" option

        // Find the currently selected role
        const selectedRole = roles.find(role => role.Id === selectedRoleValue);

        // Repopulate the dropdown
        populateDropdown(roleDropdown, roles, 'Select role', selectedRole);
    }

    const populateDropdown = (element, data, placeholder, selectedValue = null) => {
        console.log(`Populating dropdown for ${element.id} with ${data.length} items`);
        element.innerHTML = `<option value="">${placeholder}</option>`;
        if (selectedValue) {
            const selectedOption = document.createElement('option');
            selectedOption.value = selectedValue.Id;
            selectedOption.textContent = selectedValue.Name;
            selectedOption.selected = true;
            selectedOption.style.backgroundColor = "#d3e0e9";
            element.appendChild(selectedOption);
        }
        data.forEach((item) => {
            if (!selectedValue || item.Id !== selectedValue.Id) {
                const option = document.createElement('option');
                option.value = item.Id;
                option.textContent = item.Name;
                element.appendChild(option);
            }
        });
        element.disabled = data.length === 0; // Disable if no data available
    };

    const showLoadingText = (element, placeholderText) => {
        element.innerHTML = `<option value="">${placeholderText} (Loading...)</option>`;
        element.disabled = true;
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

    if (buildings.length === 0) {
        showLoadingText(buildingDropdown, 'Select building');
        buildings = await fetchBuildings();
        populateDropdown(buildingDropdown, buildings, 'Select building');
    }


    function checkMandatoryFields() {

        let allFilled = true;

        if (!searchInput.value.trim()) {
            allFilled = false;
        }

        // Validate that the requestedBySearch input must match a dropdown option
        if (!selectedUserId) {
            allFilled = false;
        }

        continueBtn.disabled = !allFilled;
        if (allFilled) {
            continueBtn.classList.remove('custom-class');
        } else {
            continueBtn.classList.add('custom-class');
        }
        return allFilled;
    }

    const selectOption = (item) => {
        searchInput.value = item.UserName;
        // store the id of the selected user
        selectedUserId = item.UserID;
        dropdownOptions.classList.add('hidden');
        checkMandatoryFields();
    };
})