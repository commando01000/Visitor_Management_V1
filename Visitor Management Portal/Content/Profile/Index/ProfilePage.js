document.addEventListener('DOMContentLoaded', function () {
    const saveChangesBtn = document.querySelector('.save-changes-btn');
    const buildingDropdown = document.getElementById('building');
    const errorMessage = document.getElementById('building-error');
    const zoneDropdown = document.getElementById('zone');
    const zoneErrorMessage = document.getElementById('zone-error');
    const changePasswordBtn = document.querySelector('.Complete-Reset-Password-btn');

    let isDataLoaded = false;
    let isZoneDataLoaded = false;

    saveChangesBtn.addEventListener('click', function () {
        const profileInfoVM = collectFormData();
        updateProfile(profileInfoVM);
    });

    buildingDropdown.addEventListener('click', function () {
        if (!isDataLoaded && buildingDropdown.children.length <= 1) {
            $(document).trigger('ajaxStart');
            errorMessage.style.display = 'none';
            fetchBuildings();
        }
    });

    zoneDropdown.addEventListener('click', function () {
        if (!isZoneDataLoaded && zoneDropdown.children.length <= 1) {
            $(document).trigger('ajaxStart');
            zoneErrorMessage.style.display = 'none';
            fetchZones();
        }
    });

    changePasswordBtn.addEventListener('click', function () {
        const oldPassword = document.getElementById('old-password').value;
        const newPassword = document.getElementById('new-password').value;
        const confirmPassword = document.getElementById('confirm-password').value;

        if (newPassword !== confirmPassword) {
            Notify('New password and confirm password do not match', NotifyStatus.Error);
        } else if (!oldPassword || !newPassword) {
            Notify('Please fill out all fields', NotifyStatus.Error);
        } else {
            changePassword(oldPassword, newPassword);
        }
    });
    function changePassword(oldPassword, newPassword) {
        $(document).trigger('ajaxStart');

        fetch('/Profile/ChangePassword', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                oldPassword: oldPassword,
                newPassword: newPassword
            })
        })
            .then(response => response.json())
            .then(data => {
                $(document).trigger('ajaxStop');

                if (data.success) {
                    $('#changePasswordModal').modal('hide');

                    document.getElementById('old-password').value = '';
                    document.getElementById('new-password').value = '';
                    document.getElementById('confirm-password').value = '';

                    Notify(data.message, NotifyStatus.Success);
                } else {
                    Notify(data.message, NotifyStatus.Error);
                }
            })
            .catch(error => {
                $(document).trigger('ajaxStop');
                Notify('An error occurred while changing the password.', NotifyStatus.Error);
            });
    }

    function collectFormData() {
        const buildingId = document.getElementById('building').value;
        const zoneId = document.getElementById('zone').value;

        return {
            Name: document.getElementById('full-name').value,
            PhoneNumber: document.getElementById('phone-number').value,
            JobTitle: document.getElementById('job-title').value,
            FloorNumber: document.getElementById('floor-number').value,
            Building: buildingId === "0" ? null : {
                Id: buildingId,
                Name: document.getElementById('building').options[document.getElementById('building').selectedIndex].text
            },
            Zone: zoneId === "0" ? null : {
                Id: zoneId,
                Name: document.getElementById('zone').options[document.getElementById('zone').selectedIndex].text
            }
        };
    }

    function updateProfile(profileInfoVM) {
        $(document).trigger('ajaxStart');

        fetch('/Profile/UpdateProfile', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(profileInfoVM)
        })
            .then(response => {
                if (response.ok) return response.json();
                throw new Error('Network response was not ok.');
            })
            .then(data => {
                if (data.success) {
                    Notify(data.message, NotifyStatus.Success);

                    let nameElement = document.querySelector('.name');
                    let titleElement = document.querySelector('.title');

                    let nameElement_profile = document.querySelector('#profile-name');
                    let titleElement_profile = document.querySelector('#profile-jobTitle');
                    
                    nameElement.textContent = profileInfoVM.Name;
                    titleElement.textContent = profileInfoVM.JobTitle;

                    nameElement_profile.textContent = profileInfoVM.Name;
                    titleElement_profile.textContent = profileInfoVM.JobTitle;

                    updateProgressBar(data.completenessPercentage);
                } else {
                    Notify(data.message, NotifyStatus.Error);
                }
            })
            .catch(error => {
                alert('An error occurred while updating the profile.');
                Notify(error.message, NotifyStatus.Error);
            })
            .finally(() => {
                $(document).trigger('ajaxStop');
            });
    }

    function updateProgressBar(completenessPercentage) {
        const progressBar = document.querySelector('.progress');
        const progressText = document.querySelector('.progress-text');

        progressBar.style.width = `${completenessPercentage}%`;

        if (completenessPercentage === 100) {
            progressBar.style.backgroundColor = 'green';
            progressText.textContent = 'You have completed 100%';
            progressText.style.color = 'green';
        } else {
            progressBar.style.backgroundColor = '#1E3A8A';
            progressText.textContent = `You only need ${100 - completenessPercentage}% more`;
            progressText.style.color = '';
        }
    }

    function fetchBuildings() {
        fetch('/Location/GetAllBuildings')
            .then(response => {
                if (response.ok) return response.json();
                throw new Error('Failed to fetch buildings.');
            })
            .then(data => {
                const selectedBuildingId = '@Model.Building?.Id';

                while (buildingDropdown.children.length > 1) {
                    buildingDropdown.removeChild(buildingDropdown.lastChild);
                }

                data.forEach(building => {
                    const option = document.createElement('option');
                    option.value = building.Id;
                    option.textContent = building.Name;
                    buildingDropdown.appendChild(option);
                });

                if (selectedBuildingId) {
                    const selectedOption = buildingDropdown.querySelector(`option[value="${selectedBuildingId}"]`);
                    if (selectedOption) {
                        buildingDropdown.insertBefore(selectedOption, buildingDropdown.children[1]);
                        selectedOption.selected = true;
                        selectedOption.style.backgroundColor = "#d3e0e9";
                    }
                }

                $(document).trigger('ajaxStop');
                isDataLoaded = true;
            })
            .catch(error => {
                $(document).trigger('ajaxStop');
                errorMessage.textContent = 'Failed to load buildings. Please try again.';
                errorMessage.style.display = 'block';
                console.error('Error:', error);
            });
    }

    function fetchZones() {
        fetch('/Location/GetAllZones')
            .then(response => {
                if (response.ok) return response.json();
                throw new Error('Failed to fetch zones.');
            })
            .then(data => {
                const selectedZoneId = '@Model.Zone?.Id';

                while (zoneDropdown.children.length > 1) {
                    zoneDropdown.removeChild(zoneDropdown.lastChild);
                }

                data.forEach(zone => {
                    const option = document.createElement('option');
                    option.value = zone.Id;
                    option.textContent = zone.Name;
                    zoneDropdown.appendChild(option);
                });

                if (selectedZoneId) {
                    const selectedOption = zoneDropdown.querySelector(`option[value="${selectedZoneId}"]`);
                    if (selectedOption) {
                        zoneDropdown.insertBefore(selectedOption, zoneDropdown.children[1]);
                        selectedOption.selected = true;
                        selectedOption.style.backgroundColor = "#d3e0e9";
                    }
                }

                $(document).trigger('ajaxStop');
                isZoneDataLoaded = true;
            })
            .catch(error => {
                $(document).trigger('ajaxStop');
                zoneErrorMessage.textContent = 'Failed to load zones. Please try again.';
                zoneErrorMessage.style.display = 'block';
                console.error('Error:', error);
            });
    }
});