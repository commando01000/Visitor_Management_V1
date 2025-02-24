document.addEventListener('DOMContentLoaded', () => {

    const backBtn = document.querySelector(".cancle");


    backBtn.addEventListener('click', () => {
        window.history.back();
    });
})