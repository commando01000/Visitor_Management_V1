
var NotifyStatus = {
    Info: "Info",
    Success: "Success",
    Error: "Error"
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
