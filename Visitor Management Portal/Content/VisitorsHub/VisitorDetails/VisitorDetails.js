document.addEventListener('DOMContentLoaded', () => {

    // Initialize the DataTable
    const table = $('#visitorsHistoryTable').DataTable({
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
});


function DeleteVisitorHub(id) {
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

function ConfirmDeleteVisitor(id) {
    $.ajax({
        url: '/VisitorsHub/DeleteVisitorHub',
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
