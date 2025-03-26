
var NotifyStatus = {
    Info: "Info",
    Success: "Success",
    Error: "Error"
}

// Helper function to format time consistently with leading zeros
function formatTime(time) {
    const date = new Date('1970-01-01T' + time); // Assuming time is in "HH:mm" format
    const hours = date.getHours();
    const minutes = date.getMinutes();
    const period = hours >= 12 ? 'PM' : 'AM';
    const formattedHours = hours % 12 === 0 ? 12 : hours % 12; // Convert to 12-hour format
    const formattedMinutes = minutes < 10 ? '0' + minutes : minutes; // Ensure two-digit minutes
    const formattedHour = formattedHours < 10 ? '0' + formattedHours : formattedHours; // Ensure two-digit hours
    return formattedHour + ':' + formattedMinutes + ' ' + period;
}

function Notify(message, notifyStatus, time = 3000, redirectURL = null) {
    let iconPath;
    let className = notifyStatus + 'Class'
    switch (notifyStatus) {
        case "Success":
            iconPath = "success.svg";
            break;
        case "Error":
            iconPath = "error.svg";
            break;
        case "Info":
            iconPath = "info.svg";
            break;
        default:
            iconPath = "info.svg";
    }
    var snackbarResult = `
                  <div class="align-items-center snackbarSelector" id="snackbar">
                      <div style="padding:1rem;">
                          <img src="/Content/Images/NotifyIcons/${iconPath}" alt="msg-cion" class="massage-icon"> <span class="message">${message}</span>
                      </div>
                      <div class="notify-progress ${className}"></div>
                  </div>`;

    $('body').append(snackbarResult);

    var snackbar = $(".snackbarSelector");
    snackbar.addClass("show");

    setTimeout(function () {
        snackbar.removeClass("show");
        setTimeout(function () {
            snackbar.remove();
            if (redirectURL) {
                window.location.href = redirectURL;
            }
        }, 300);
    }, time);
}


function HighlightInput(inputElement, time = 2700) {

    inputElement.addClass('highlight');

    setTimeout(function () {
        inputElement.removeClass('highlight');
    }, time);
}

function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

// u can use vanilla js here
function HighlightField(inputElement, time = 2700) {
    inputElement.classList.add('highlight');

    setTimeout(function () {
        inputElement.classList.remove('highlight');
    }, time);
}
