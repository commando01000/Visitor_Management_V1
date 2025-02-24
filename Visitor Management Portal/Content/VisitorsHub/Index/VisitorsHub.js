document.addEventListener('DOMContentLoaded', () => {

    const addNewVistor = document.querySelector('.addNewVistor');
    const editVisitor = document.querySelectorAll('.editVisitor');
    const visitorDetails = document.querySelectorAll('.visitorDetails');
    const deleteButtons = document.querySelectorAll(".deleteVisitor");

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
        columnDefs: [
            { orderable: false, targets: [1] }
        ],
        buttons: ['print']
    });

    // Custom search functionality
    const datatableSearchInput = document.querySelector('#visitorsTable_filter input[type="search"]');
    const pageSearchInput = document.querySelector(".VisitorsHub_topSection .search-bar input");

    if (datatableSearchInput && pageSearchInput) {
        pageSearchInput.addEventListener("input", (e) => {
            datatableSearchInput.value = e.target.value;
            table.search(e.target.value).draw();
        });
    }


    addNewVistor.addEventListener('click', () => {
        window.location.href = "/VisitorsHub/AddVisitor";
    })
    editVisitor.forEach(visitor => {
        visitor.addEventListener('click', () => {
            window.location.href = "/VisitorsHub/EditVisitor";
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
                fetch(`/VisitorsHub/DeleteVisitor/${visitorId}`, {
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
