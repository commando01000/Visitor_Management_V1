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
});
