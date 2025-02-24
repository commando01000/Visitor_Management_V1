document.addEventListener('DOMContentLoaded', () => {
    const visitorDetails = document.querySelectorAll('.visitorDetails');
    const AddNewVisitRequest = document.querySelector('.addNewVisitRequest');
    // Initialize the DataTable
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
        "columnDefs": [
            { "orderable": false, "targets": [9] },
        ],
        "buttons": ['print']
    });
    const datatableSearchInput = document.querySelector('#visitorsTable_filter input[type="search"]');

    const pageSearchInput = document.querySelector(".VisitRequest_topSection .search-bar input");

    if (datatableSearchInput && pageSearchInput) {
        pageSearchInput.addEventListener("input", (e) => {
            datatableSearchInput.value = e.target.value;

            table.search(e.target.value).draw();
        });
    }

    visitorDetails.forEach((visitorDetail) => {
        visitorDetail.addEventListener('click', () => {
            window.location.href = '/VisitRequest/VistRequestDetails';
        });
    });

    AddNewVisitRequest.addEventListener('click', () => {
        window.location.href = '/VisitRequest/AddNewVisit';
    });
});
