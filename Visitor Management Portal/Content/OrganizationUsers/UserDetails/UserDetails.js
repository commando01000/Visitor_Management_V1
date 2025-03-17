document.addEventListener('DOMContentLoaded', () => {
    // Initialize the DataTable
    const table = $('#visitorsHistoryTable').DataTable({
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
    const pageSearchInput = document.querySelector(".visitorsHistory .search-bar input");
    if (pageSearchInput) {
        pageSearchInput.addEventListener("input", (e) => {
            table.search(e.target.value).draw();
        });
    }

    $("#approvalToggle").change(function () {
        var isChecked = $(this).prop("checked");
        var userId = $(this).data("id");

        $.ajax({
            url: "/OrganizationUsers/UpdateVisitApprovalStatus",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                id: userId,
                CreateVisitsWithoutApproval: isChecked
            }),
            dataType: "json",
            success: function (response) {
                if (response.success) {
                    Notify(response.message, "Success"); // Display success message
                } else {
                    Notify(response.message, "Error");
                }
            },
            error: function (xhr, status, error) {
                console.error("❌ Error updating status:", error);
                Notify("An error occurred while updating the approval status.", "Error");
            }
        });
    });
});
