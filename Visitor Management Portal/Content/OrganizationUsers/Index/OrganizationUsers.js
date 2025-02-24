document.addEventListener('DOMContentLoaded', () => {
    // Initialize DataTable
    const table = $('#organizationUsersTable').DataTable({
        "pageLength": 5,
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
            { "orderable": false, "targets": [3] },
        ],
        "buttons": ['print']
    });

    // Search functionality
    const datatableSearchInput = document.querySelector('#organizationUsersTable_filter input[type="search"]');
    const pageSearchInput = document.querySelector(".organization-users-search-bar input");

    if (datatableSearchInput && pageSearchInput) {
        pageSearchInput.addEventListener("input", (e) => {
            datatableSearchInput.value = e.target.value;
            table.search(e.target.value).draw();
        });
    }

    // Add User Button
    const organization_users_add_btn = document.querySelector('.organization-users-add-btn');
    organization_users_add_btn.addEventListener('click', () => {
        window.location.href = "/OrganizationUsers/InviteUsers";
    });

    // User Details Click
    document.querySelectorAll('.userDetails').forEach((user) => {
        user.addEventListener('click', () => {
            const userId = user.getAttribute('data-userId');
            window.location.href = `/OrganizationUsers/UserDetails?userId=${userId}`;
        });
    });

    // Edit User Click
    document.querySelectorAll('.editUser').forEach((user) => {
        user.addEventListener('click', () => {
            const userId = user.getAttribute('data-userId');
            window.location.href = `/OrganizationUsers/UserDetails?userId=${userId}`;
        });
    });

    document.querySelectorAll('.deleteUser').forEach((user) => {
        user.addEventListener('click', () => {
            const userId = user.getAttribute('data-userId');

            // jQuery Confirm Dialog
            $.confirm({
                title: 'Delete User',
                content: 'Are you sure you want to delete this user?',
                type: 'red',
                buttons: {
                    confirm: {
                        text: 'Delete',
                        btnClass: 'btn-red',
                        action: function () {
                            deleteUser(userId, user);
                        }
                    },
                    cancel: {
                        text: 'Cancel',
                        action: function () {
                          
                        }
                    }
                }
            });
        });
    });

    function deleteUser(userId, userElement) {
        fetch(`/OrganizationUsers/DeleteUser?userId=${userId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    const row = userElement.closest('tr');
                    table.row(row).remove().draw();
                    Notify("User deleted successfully", NotifyStatus.Success);

                } else {
                    Notify(data.message, NotifyStatus.Error);

                }
            })
            .catch(error => {
                console.error('Error:', error);
  Notify("An error occurred while deleting the user", NotifyStatus.Error);
            });
    }
});