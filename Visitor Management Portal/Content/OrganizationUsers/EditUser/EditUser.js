document.addEventListener('DOMContentLoaded', () => {
    console.log(document.getElementById('current-userID').value);
   
    // DOM Elements
    const backBtn = document.querySelector(".cancle");
    const buildingDropdown = document.getElementById('building');
    const zoneDropdown = document.getElementById('zone');
    const roleDropdown = document.getElementById('role');
    const searchInput = document.getElementById('ReportingToSearch');
    const dropdownOptions = document.getElementById('ReportingToOptions');
    const saveChangesBtn = document.querySelector('.save-changes');

    let allUsers = []; // Cache for organization users

    // Event Listeners
    backBtn.addEventListener('click', () => {
        console.log('Back button clicked');
        window.history.back();
    });

    buildingDropdown.addEventListener('focus', handleBuildingDropdownFocus);
    buildingDropdown.addEventListener('change', handleBuildingDropdownChange);

    roleDropdown.addEventListener('focus', handleRoleDropdownFocus);

    searchInput.addEventListener('focus', handleSearchInputFocus);
    searchInput.addEventListener('input', handleSearchInputInput);

    document.addEventListener('click', handleDocumentClick);

    saveChangesBtn.addEventListener('click', handleSaveChanges);

    // Helper Functions
    const showLoadingSpinnerForLocation = (element) => {
        console.log(`Showing loading spinner for ${element.id}`);
        element.innerHTML = '<option value="" disabled selected>Loading...</option>';
        element.classList.remove('hidden');
    };

    const populateDropdown = (element, data, placeholder, selectedValue = null) => {
        console.log(`Populating dropdown for ${element.id} with ${data.length} items`);
        element.innerHTML = `<option value="" disabled>${placeholder}</option>`;
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
    };

    const populateOptions = (users, selectedUserName = null) => {
        console.log(`Populating options for ReportingTo with ${users.length} users`);
        dropdownOptions.innerHTML = '';

        if (selectedUserName) {
            const selectedUser = users.find(user => user.UserName === selectedUserName);
            if (selectedUser) {
                const selectedOption = document.createElement('div');
                selectedOption.className = 'dropdown-item selected';
                selectedOption.textContent = selectedUser.UserName;
                selectedOption.style.backgroundColor = "#d3e0e9";
                selectedOption.onclick = () => selectOption(selectedUser);
                dropdownOptions.appendChild(selectedOption);
            }
        }

        users.forEach(user => {
            if (!selectedUserName || user.UserName !== selectedUserName) {
                const option = document.createElement('div');
                option.className = 'dropdown-item';
                option.textContent = user.UserName;
                option.setAttribute('data-reportingToId', user.UserID);
                option.onclick = () => selectOption(user);
                dropdownOptions.appendChild(option);
            }
        });

        dropdownOptions.classList.remove('hidden');
    };

    const selectOption = (user) => {
        console.log(`Selected user: ${user.UserName}`);
        console.log(`Selected user ID : ${user.UserID}`);
        searchInput.value = user.UserName;
        searchInput.setAttribute('data-reportingToId', user.UserID);
        dropdownOptions.classList.add('hidden');
    };

    const filterUsers = (query) => {
        console.log(`Filtering users with query: ${query}`);
        return allUsers.filter(user => user.UserName.toLowerCase().includes(query.toLowerCase()));
    };

    const showLoadingSpinner = () => {
        console.log('Showing loading spinner for ReportingTo');
        dropdownOptions.innerHTML = '<div class="dropdown-item loading-spinner">Loading...</div>';
        dropdownOptions.classList.remove('hidden');
    };

    // Async Functions
    const fetchBuildings = async () => {

        debugger;
        console.log('Fetching buildings...');
        try {
            const response = await fetch('/Location/GetAllBuildings');
            if (!response.ok) throw new Error('Network response was not ok');
            const data = await response.json();
            console.log('Buildings fetched:', data);
            return data;
        } catch (error) {
            console.error('Error fetching buildings:', error);
            return [];
        }
    };

    const fetchZones = async (buildingId) => {
        console.log(`Fetching zones for building ID: ${buildingId}`);
        try {
            const response = await fetch(`/Location/GetZonesByBuildingId?buildingId=${buildingId}`);
            if (!response.ok) throw new Error('Network response was not ok');
            const data = await response.json();
            console.log('Zones fetched:', data);
            return data;
        } catch (error) {
            console.error('Error fetching zones:', error);
            return [];
        }
    };

    const fetchOrganizationUsers = async () => {
        console.log('Fetching organization users...');
        debugger;
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

    const updateOrganizationUserInfo = async (profileInfoVM) => {
        console.log('Updating organization user info:', profileInfoVM);
        $(document).trigger('ajaxStart');
        try {
            const response = await fetch('/OrganizationUsers/UpdateUserInfo', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(profileInfoVM)
            });
            if (!response.ok) throw new Error('Network response was not ok.');
            const data = await response.json();
            console.log('Update response:', data);
            if (data.success) {
                Notify(data.message, NotifyStatus.Success);
            } else {
                Notify(data.message, NotifyStatus.Error);
            }
        } catch (error) {
            console.error('Error updating user info:', error);
            alert('An error occurred while updating the profile.');
            Notify(error.message, NotifyStatus.Error);
        } finally {
            $(document).trigger('ajaxStop');
        }
    };

    // Event Handlers
    async function handleBuildingDropdownFocus(event) {
        console.log('Building dropdown focused');
        const selectedValue = event.target.value;
        showLoadingSpinnerForLocation(buildingDropdown);
        const buildings = await fetchBuildings();
        const selectedBuilding = buildings.find(b => b.Id === selectedValue);
        populateDropdown(buildingDropdown, buildings, 'Select building', selectedBuilding);
    }

    async function handleBuildingDropdownChange(event) {
        console.log('Building dropdown changed');
        const buildingId = event.target.value;
        if (buildingId) {
            const selectedZoneValue = zoneDropdown.value;
            showLoadingSpinnerForLocation(zoneDropdown);
            const zones = await fetchZones(buildingId);
            const selectedZone = zones.find(z => z.Id === selectedZoneValue);
            populateDropdown(zoneDropdown, zones, 'Select zone', selectedZone);
        } else {
            zoneDropdown.innerHTML = '<option value="">Select zone</option>';
        }
    }

    async function handleRoleDropdownFocus(event) {
        console.log('Role dropdown focused');
        const selectedRoleValue = event.target.value;
        showLoadingSpinnerForLocation(roleDropdown);
        const roles = [
            { Id: 0, Name: 'Administrator' },
            { Id: 1, Name: 'Manager' },
            { Id: 2, Name: 'Employee' },
            { Id: 3, Name: 'Receptionist' }
        ];
        const selectedRole = roles.find(role => role.Id === parseInt(selectedRoleValue));
        populateDropdown(roleDropdown, roles, 'Select role', selectedRole);
    }

    async function handleSearchInputFocus() {
        console.log('ReportingTo search input focused');
        if (allUsers.length === 0) {
            showLoadingSpinner();
            allUsers = await fetchOrganizationUsers();
        }
        populateOptions(allUsers, "@Model.Reportingto");
    }

    function handleSearchInputInput(event) {
        console.log('ReportingTo search input changed');
        const query = event.target.value.trim();
        const filteredUsers = query.length > 0 ? filterUsers(query) : allUsers;
        populateOptions(filteredUsers, "@Model.Reportingto");
    }

    function handleDocumentClick(event) {
        if (!event.target.closest('#Reporting-To')) {
            console.log('Clicked outside ReportingTo dropdown');
            dropdownOptions.classList.add('hidden');
        }
    }

    function handleSaveChanges() {
        console.log('Save changes button clicked');
        const profileInfoVM = {
            CreateVisitsWithoutApproval: document.querySelector('.switch input[type="checkbox"]').checked,
            id: document.getElementById('current-userID').value,
            Name: document.getElementById('full-name').value,
            JobTitle: document.getElementById('job-title').value,
            Email: document.getElementById('email-address').value,
            Password: document.getElementById('password').value,
            Phone: document.getElementById('phone-number').value,
            Role: roleDropdown.options[roleDropdown.selectedIndex].text,
            RoleId: roleDropdown.value,
            Reportingto: searchInput.value,
            ReportingtoId: searchInput.dataset.reportingtoid || '',
            Building: buildingDropdown.options[buildingDropdown.selectedIndex].text,
            BuildingId: buildingDropdown.value,
            Zone: zoneDropdown.options[zoneDropdown.selectedIndex].text,
            ZoneId: zoneDropdown.value,
            Floor: document.getElementById('floor-number').value,
            visitReques: []
        };
        updateOrganizationUserInfo(profileInfoVM);
    }
});