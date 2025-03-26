//#region ajax calls functions
function loadTotalVisitsChart(selectedPeriod, selectedWeek = 0) {
    $.ajax({
        url: '/Dashboard/TotalVisitChartCall',
        type: 'GET',
        data: { Period: selectedPeriod, weekNumber: selectedWeek },
        dataType: 'json',
        success: function (response) {
            console.log("TotalVisits Data:", response);

            //#region Empty State
            if (response.TotalVisitsCharts.length === 0) {
                $("#total-visits-empty").show();
                $("#total-visits-data").hide();
                return;
            }

            $("#total-visits-empty").hide();
            $("#total-visits-data").show();
            //#endregion

            //#region total visits
            var totalVisitsData = response.TotalVisitsCharts.map(item => item.TotalVisits);
            var Names = response.TotalVisitsCharts.map(item => item.Name);
            $("#date-range").html(response.DateFromatedRange);
            $("#total-visits-for-this-period").html(`${response.TotoaVisitsPerPeriod} Visit`);

            chartTotalVisits.destroy();
            chartTotalVisits = new ApexCharts(document.querySelector("#total-visits"), optionsTotalVisits);
            chartTotalVisits.render();

            chartTotalVisits.updateOptions({
                series: [{ name: 'Total Visits', data: totalVisitsData }],
                xaxis: { categories: Names },
                colors: response.TotalVisitsCharts.map(item => item.IsItLargestVisits ? '#12204B' : '#D0D3D9')
            });

            if (response.IsItMonth) {
            // Set click event dynamically after chart is updated
                chartTotalVisits.addEventListener("dataPointSelection", function (event, chartContext, config) {
                    let categoryName = Names[config.dataPointIndex];
                    let weekNumber = parseInt(categoryName.replace("W-", ""), 10);

                    var option = $("#totalVisits-selectInput option[value='-1']");

                    // First, set it as the selected value
                    $("#totalVisits-selectInput").val("-1").change();

                    // Then update its properties
                    option.text("Week " + weekNumber).prop("disabled", true).show();

                    // Call the function with clicked data
                    loadTotalVisitsChart(2, weekNumber);
                });
            }            
            //#endregion
        },
        error: function (xhr, status, error) {
            console.error("Error fetching TotalVisits data:", error);
        }
    });
}

function loadPurposeChart(selectedPeriod) {
    $.ajax({
        url: '/Dashboard/PurposeChartCall',
        type: 'GET',
        data: { Period: selectedPeriod },
        dataType: 'json',
        success: function (response) {
            console.log("PurposeChart Data:", response);

            //#region Empty State
            if (response.length === 0 || response.every(item => item.VisitsNumber === 0)) {
                $("#purpose-visits-empty").show();
                $("#purpose-visits-data").hide();
                return;
            }

            $("#purpose-visits-empty").hide();
            $("#purpose-visits-data").show();
            //#endregion

            //#region Visits By Purpose
            var VisitsPurposePrecentage = response.map(item => item.VisitsPercentage);
            var purposeNames = response.map(item => item.PurposeName);

            chartVisitsByPurpose.updateOptions({
                series: VisitsPurposePrecentage,
                labels: purposeNames
            });

            response.forEach(purpose => {
                $(`#${CSS.escape(purpose.PurposeName)}-value`).html(purpose.VisitsNumber);
            });
            //#endregion
        },
        error: function (xhr, status, error) {
            console.error("Error fetching PurposeChart data:", error);
        }
    });
}

function loadPeakTimeChart(selectedPeriod) {
    $.ajax({
        url: '/Dashboard/PeakTimeChartCall',
        type: 'GET',
        data: { Period: selectedPeriod },
        dataType: 'json',
        success: function (response) {
            console.log("PeakTimeChart Data:", response);
           // debugger;
            //#region Empty State
            if (response.analyses.length === 0) {
                $("#Peak-time-empty").show();
                $("#Peak-time-data").hide();
                return;
            }

            $("#Peak-time-empty").hide();
            $("#Peak-time-data").show();
            //#endregion

            //#region Peak Time Chart
            var VisitorsNumbers = response.analyses.map(item => item.VisitorsCount);
            var HoursLabels = response.analyses.map(item => item.VisitingTime);
            var ColorsColumns = response.analyses.map(item => item.WhichColor);

                // Update chart data
                PeakTime.updateSeries([{ data: VisitorsNumbers }]);

                // Update chart options
                PeakTime.updateOptions({
                    xaxis: { categories: HoursLabels },  // Move categories inside xaxis
                    colors: ColorsColumns
                });

            $("#peak-time-max").html(response.PeakTime);
            $("#max-visitors-number").html(response.MaxVisitors);
            //#endregion
        },
        error: function (xhr, status, error) {
            console.error("Error fetching PeakTimeChart data:", error);
        }
    });
}

function loadVisitsDepartmentChart(selectedPeriod) {
    $.ajax({
        url: '/Dashboard/VisitsByDepartmentChartCall',
        type: 'GET',
        data: { Period: selectedPeriod },
        dataType: 'json',
        success: function (response) {
            console.log("VisitsDepartmentChart Data:", response);

            //#region Empty State
            if (response.length === 0) {
                $("#department-visits-empty").show();
                $("#department-visits-data").hide();
                return;
            }

            $("#department-visits-empty").hide();
            $("#department-visits-data").show();
            //#endregion

            //#region Peak Time Chart
            var VisitNumberByDepartmentNumbers = response.map(item => item.Count);
            var DepartmentNamesLabels = response.map(item => item.DepartmentName);
            var ColorsColumns = response.map(item => item.color);

            // Update the chart series (data values)
            chartVisitsDepartment.updateSeries(VisitNumberByDepartmentNumbers);

            // Update the chart options, ensuring labels are updated instead of xaxis categories
            chartVisitsDepartment.updateOptions({
                labels: DepartmentNamesLabels, // Correct way to set labels for a donut chart
                colors: ColorsColumns
            });

            //#endregion

            //#region peak time lebals 
            var container = $(".labels-department-chart-card");
            container.empty(); // Clear existing content

            var row = $("<div>").addClass("row");

            response.forEach(function (item, index) {
                var col = $("<div>").addClass("col-6 d-flex justify-content-between align-items-center");

                var dot = $("<span>").addClass("dot").css("background-color", item.color);
                var departmentName = $("<span>").text(item.DepartmentName);
                var count = $("<span>").addClass("fw-bold ms-auto").text(item.Count);

                col.append(dot, departmentName, count);
                row.append(col);

                // Create a new row after every 2 items
                if ((index + 1) % 2 === 0 || index === response.length - 1) {
                    container.append(row);
                    row = $("<div>").addClass("row mt-3"); // Reset row
                }
            });
            //#endregion
        },
        error: function (xhr, status, error) {
            console.error("Error fetching VisitsDepartmentChart data:", error);
        }
    });
}

function loadVisitsByLocationChart(selectedPeriod) {
    $.ajax({
        url: '/Dashboard/VisitsByZoneChartCall',
        type: 'GET',
        data: { Period: selectedPeriod },
        dataType: 'json',
        success: function (response) {
            console.log("VisitsByLocationChart Data:", response);
            //#region Empty State
            if (response.visitsByZones.length === 0) {
                $("#location-empty").show();
                $("#location-data").hide();
                return;
            }

            $("#location-empty").hide();
            $("#location-data").show();
            //#endregion

            //#region Peak Time Chart
            var VisitNumberByZoneNumbers = response.visitsByZones.map(item => item.NumberVisitsPerZone);
            var ZoneNamesLabels = response.visitsByZones.map(item => item.Zone_Name);
            var ColorsColumns = response.visitsByZones.map(item => item.ZoneColumnColor);

            // Update the bar chart properly
            chartVisitRequestsByLocation.updateSeries([{
                name: 'visits',
                data: VisitNumberByZoneNumbers // Ensure this is an array of numbers
            }]);

            chartVisitRequestsByLocation.updateOptions({
                xaxis: {
                    categories: ZoneNamesLabels // Correct way to label bars
                },
                colors: ColorsColumns // Set colors for each bar
            });

            //#endregion

            //#region Busiest Areas

            for (i = 1; i <= 3; i++) {
                $(`#Busiest-Area-Card-${i}`).show();
            }
            let BusiestAreasLength = !response.BusiestAreas ? 0 : response.BusiestAreas.length;

            for (i = 3; i > BusiestAreasLength; i--) {
                $(`#Busiest-Area-Card-${i}`).css("display", "none");
            }

            response.BusiestAreas.forEach(function (item, index) {

                var container = $(`#Busiest-Area-Data-${index + 1}`);
                container.empty();

                var row = $("<div>").addClass(`pt-2 mb-0`);

                var contant_1st_line = $("<div>").addClass("d-flex justify-content-between");

                var zone_name = $("<span>")
                    .addClass("text-dark text-truncate")  
                    .attr("data-title", item.Zone_Name)  
                    .text(item.Zone_Name);

                var numberVisitsZone = $("<span>").addClass("visit-count").text(item.NumberVisitsPerZone + " visit");

                contant_1st_line.append(zone_name, numberVisitsZone);

                var contant_2nd_line = $("<div>").addClass("meeting-area").text(item.MeetingAreaName);

                row.append(contant_1st_line, contant_2nd_line);

                container.append(row);              
            });
            //#endregion
        },
        error: function (xhr, status, error) {
            console.error("Error fetching VisitsByLocationChart data:", error);
        }
    });
}

//#endregion
//#region event change selected Period
$("#purpose-selectInput").change(function () {
    var selectedValue = $(this).val();
    loadPurposeChart(selectedValue); 
});

$("#peakTime-selectInput").change(function () {
    var selectedValue = $(this).val();
    loadPeakTimeChart(selectedValue);
});

$("#visitsDepartment-selectInput").change(function () {
    var selectedValue = $(this).val();
    loadVisitsDepartmentChart(selectedValue);
});

$("#visitsByLocation-selectInput").change(function () {
    var selectedValue = $(this).val();
    loadVisitsByLocationChart(selectedValue);
});

$("#totalVisits-selectInput").change(function () {

    var option = $("#totalVisits-selectInput option[value='-1']");

    option.text("No Data").prop("disabled", false).hide();

    var selectedValue = $(this).val();

    if (!(selectedValue === "-1"))
        loadTotalVisitsChart(selectedValue);
});
//#endregion
// #region Visits By Purpose

var optionsVisitsByPurpose = {
    series: [],
    chart: {
        width: '100%',
        height: '350px',
        type: 'polarArea'
    },
    colors: ['#5F7BB5', '#90A7D8', '#C1D1F0', '#3E5A94'],
    labels: [],
    legend: {
        show: false  
    },
    fill: {
        opacity: 1
    },
    stroke: {
        width: 1
    },
    yaxis: {
        show: false
    },
    plotOptions: {
        polarArea: {
            rings: {
                strokeWidth: 0
            },
            spokes: {
                strokeWidth: 0
            }
        }
    }
};

var chartVisitsByPurpose = new ApexCharts(document.getElementById('chart-Visits-by-purpose'), optionsVisitsByPurpose)

chartVisitsByPurpose.render();
// #endregion
// #region Visit requests by location
var optionsVisitRequestsByLocation = {
    annotations: {
        position: 'back'
    },
    dataLabels: {
        enabled: false
    },
    chart: {
        type: 'bar',
        height: 300
    },
    fill: {
        opacity: 1
    },
    plotOptions: {
        bar: {
            distributed: true, // Enables different colors per column
            borderRadius: 5, // Makes bars rounded
            columnWidth: '50%' // Adjusts the width of columns
        }
    },
    series: [{
        name: 'visits',
        data: [0,0] // Explicitly setting color for the series
    }],
    xaxis: {
        categories: ["NA", "NA"],
    },
    yaxis: {
        show: false // Hides Y-axis numbers
    },
    grid: {
        show: false // Removes background grid lines
    },
    colors: ["#000","#000"],
    states: {
        hover: {
            filter: {
                type: 'darken', // Darkens the hovered column
                value: 0.3 // Adjust darkness (0.1 = slight, 0.9 = very dark)
            }
        }
    },
    legend: {
        show: false // Hides the unwanted legend
    }

}

var chartVisitRequestsByLocation = new ApexCharts(document.querySelector("#chart-Visit-requests-by-location"), optionsVisitRequestsByLocation);

chartVisitRequestsByLocation.render();
// #endregion
// #region Peak time

var OptionsPeakTime = {
    series: [
        {
            name: "Visitors",
            data: [10,10],
        }
    ],
    chart: {
        type: "bar",
        height: '260px',
    },
    plotOptions: {
        bar: {
            distributed: true,
            horizontal: false,
            borderRadius: 5,
            columnWidth: "55%",
            endingShape: "rounded",
        },
    },
    dataLabels: {
        enabled: false,
    },
    stroke: {
        show: true,
        width: 2,
        colors: ["transparent"],
    },
    xaxis: {
        categories: ["NA","NA"],
    },
    fill: {
        opacity: 1,
    },
    tooltip: {
        //y: {
        //    formatter: function (val) {
        //        return "$ " + val + " thousands";
        //    },
        //},
    },
    yaxis: {
        show: false // Hides Y-axis numbers
    },
    grid: {
        show: false // Removes background grid lines
    },
    colors: ['#D0D3D9', '#D0D3D9'], // Default colors for each column
    states: {
        hover: {
            filter: {
                type: 'darken', // Darkens the hovered column
                value: 0.3 // Adjust darkness (0.1 = slight, 0.9 = very dark)
            }
        }
    },
    legend: {
        show: false // Hides the unwanted legend
    }

};

var PeakTime = new ApexCharts(document.querySelector("#Peak-time"), OptionsPeakTime);

PeakTime.render();
// #endregion
// #region Total Visits

var optionsTotalVisits = {
    annotations: {
        position: 'back'
    },
    dataLabels: {
        enabled: false
    },
    chart: {
        type: 'bar',
        height: 290,
        padding: {
            bottom: 0, 
        },
    },
    fill: {
        opacity: 1
    },
    plotOptions: {
        bar: {
            distributed: true, // Enables different colors per column
            borderRadius: 5, // Makes bars rounded
            columnWidth: '50%' // Adjusts the width of columns
        }
    },
    series: [{
        name: 'Total visits',
        data: [0,0]
    }],
    xaxis: {
        categories: ["NA","NA"]
    },
    yaxis: {
        show: false // Hides Y-axis numbers
    },
    grid: {
        show: false, // Removes background grid lines
        padding: {
            left: 0,
            right: 0,
            bottom: 0
        }
    },
    colors: ['#D0D3D9', '#D0D3D9'], // Default colors for each column
    states: {
        hover: {
            filter: {
                type: 'darken', // Darkens the hovered column
                value: 0.3 // Adjust darkness (0.1 = slight, 0.9 = very dark)
            }
        }
    },
    legend: {
        show: false // Hides the unwanted legend
    }

};

var chartTotalVisits = new ApexCharts(document.querySelector("#total-visits"), optionsTotalVisits);

chartTotalVisits.render();

// #endregion
// #region Visits by department
var optionsVisitsDepartment = {
    series: [],
    labels: [],
    chart: {
        type: 'donut',
    },
    plotOptions: {
        pie: {
            donut: {
                size: '75%',
                background: 'transparent',
                labels: {
                    show: true,
                    name: {
                        show: true,
                        offsetY: -5
                    },
                    value: {
                        show: true,
                        fontSize: '22px',
                        fontWeight: 600,
                        color: '#946B2D', // Goldish color for number
                        offsetY: 10
                    },
                    total: {
                        show: true,
                        label: 'Total',
                        fontSize: '14px',
                        color: '#666'
                    }
                }
            }
        }
    },
    stroke: {
        width: 10, // Adds space between segments
        colors: ['#fff'], // Creates the gap effect
        lineCap: 'round' // Makes the ends rounded
    },
    colors: [],
    dataLabels: {
        enabled: false
    },
    legend: {
        show: false // Hides legend
    },
    responsive: [{
        breakpoint: 480,
        options: {
            chart: {
                width: 200
            }
        }
    }]
};

var chartVisitsDepartment = new ApexCharts(document.querySelector("#chart-Visits-by-department"), optionsVisitsDepartment);
chartVisitsDepartment.render();
//#endregion
// #region intial charts data
$(document).ready(function () {
    setTimeout(function () {
        loadTotalVisitsChart("1");
        loadPurposeChart("0");
        loadPeakTimeChart("0");
        loadVisitsDepartmentChart("0");
        loadVisitsByLocationChart("0");
    }, 100);
});
// #endregion