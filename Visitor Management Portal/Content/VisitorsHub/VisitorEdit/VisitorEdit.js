const list_Item = document.querySelector(".sidebar-menu .VisitorsHub")
list_Item.classList.add('active-sidebar-menu');

const backBtn = document.querySelector(".cancle");


backBtn.addEventListener('click', () => {
    window.history.back();
});
const submitBtn = document.querySelector(".custome-btn-action");
submitBtn.addEventListener('click', function (event) {
    event.preventDefault(); // Prevent default form submission

    var visitorData = {
        Id: document.getElementById("visitor-id").value, // Ensure ID is included
        FirstName: document.getElementById("first-name").value,
        MiddleName: document.getElementById("middle-name").value,
        LastName: document.getElementById("last-name").value,
        IdNumber: document.getElementById("id-number").value,
        JobTitle: document.getElementById("job-title").value,
        OrganizationName: document.getElementById("OrganizationName").value,
        EmailAddress: document.getElementById("email-address").value,
        PhoneNumber: document.getElementById("phone-number").value
    };


    $.ajax({
        url: '/VisitorsHub/EditVisitor',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(visitorData),
        dataType: 'json',
        success: function (data) {
            if (data.Status) {
                debugger;
                Notify(data.Message, NotifyStatus.Success, 3000);
            } else {
                Notify(data.Message, NotifyStatus.Error);
            }
        },
        error: function (xhr, status, error) {
            Notify("An error occurred: " + error, NotifyStatus.Error);
        }
    });


});