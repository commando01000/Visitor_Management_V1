function loadPendingApprovalVisitRequests() {
    $.ajax({
        url: '/Dashboard/PendingApprovalVisitRequestsListCall',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            console.log("pending-approval-visits-requests Data:", response);

            //#region Empty State
            if (Object.keys(response).length === 0) {
                $("#pending-approval-empty").show();
                $("#pending-approval-visits-requests-contanier").hide();
                return;
            }

            $("#pending-approval-empty").hide();
            $("#pending-approval-visits-requests-contanier").show();
            //#endregion

            //#region pending-approval-visits-requests
            var container = $("#pending-approval-visits-requests-contanier");
            container.empty(); 

            $.each(response, function (key, value) {

                var totalItems = Object.keys(response).length; // Get total count
                var currentIndex = Object.keys(response).indexOf(key); // Get current index

                // Create the <li> element
                var listItem = $('<li>').addClass('d-flex justify-content-between align-items-center py-2');

                // Add 'mb-4' only if it's NOT the last item
                if (currentIndex < totalItems - 1) {
                    listItem.addClass('mb-4');
                }
                // Create the <div> with image and text
                var contentDiv = $('<div>');
                var image = $('<img>').attr({
                    src: '/Content/Images/examp.svg', // Adjust the path if needed
                    width: 20,
                    height: 20,
                    alt: 'File Icon'
                });
                var text = " " + value; // Using the JSON value

                // Append image and text to div
                contentDiv.append(image).append(text);

                // Create the <a> element with FontAwesome icon
                var link = $('<a>').attr('href', `${key}`).addClass('fa fa-chevron-right arrow-icon-dashboard-pending');

                // Append elements to the <li>
                listItem.append(contentDiv).append(link);

                // Append <li> to the <ul>
                container.append(listItem);
            });
            //#endregion
        },
        error: function (xhr, status, error) {
            console.error("Error fetching pending-approval-visits-requests data:", error);
        }
    });
}

function loadTopVisitors(selectedPeriod) {
    $.ajax({
        url: '/Dashboard/TopVisitorsListCall',
        type: 'GET',
        data: { Period: selectedPeriod },
        dataType: 'json',
        success: function (response) {
            console.log("Top Visitors Data:", response);

            //#region Empty State
            if (response.length === 0) {
                $("#top-visitors-empty").show();
                $("#top-visitor-data").hide();
                return;
            }

            $("#top-visitors-empty").hide();
            $("#top-visitor-data").show();
            //#endregion

            var container = $(".visitor-list");
            container.empty();

            response.forEach(function (visitor, index) {

                //var isLast = index === response.length - 1;
                //var marginClass = isLast ? "" : "mb-4";

                var visitorCard = $(`
            <div class="d-flex justify-content-between align-items-center mb-4">
                <div class="d-flex align-items-center">
                    <i class="fas fa-user-circle visitor-icon"></i>
                    <div class="ms-2">
                        <p class="text-dark m-0">${visitor.Visitor_Name}</p>
                        <p class="text-muted m-0">${visitor.Visitor_Comp}</p>
                    </div>
                </div>
                <span class="visit-count ms-auto">${visitor.NumberVisits}</span>
            </div>
        `);

                container.append(visitorCard);
            });
        },
        error: function (xhr, status, error) {
            console.error("Error fetching Top Visitors data:", error);
        }
    });
}

//function TotalVisitsNumbers(selectedPeriod) {
//    $.ajax({
//        url: '/Dashboard/TotalVisitsNumberAnalysis',
//        type: 'GET',
//        data: { Period: selectedPeriod },
//        dataType: 'json',
//        success: function (response) {
//            console.log("TotalVisitsNumbers Data:", response);

//            //#region Total Visits Numbers
//            $("#total-visits-number").html(response.Totalvisits);
//            $("#active-visits-number").html(response.ActiveVisits);
//            //#endregion
//        },
//        error: function (xhr, status, error) {
//            console.error("Error fetching TotalVisitsNumbers data:", error);
//        }
//    });
//}

$(document).ready(function () {
    loadPendingApprovalVisitRequests();
    loadTopVisitors(0);
});

$("#topVistors-selectInput").change(function () {
    var selectedValue = $(this).val();
    loadTopVisitors(selectedValue);
});