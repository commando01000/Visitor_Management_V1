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
            window.location.href = `/OrganizationUsers/EditUser?userId=${userId}`;
        });
    });

    document.querySelectorAll('.deleteUser').forEach((user) => {
        user.addEventListener('click', () => {
            const userId = user.getAttribute('data-userId');

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
                    deleteUser(userId, user);
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