window.onload = function checkingTheme() {
    try {
        let themeCheck = localStorage.getItem('theme');
        if (themeCheck == null || themeCheck == 'light') {
            document.getElementById('patientdetail').style.backgroundColor = "#f5f5f5";
            document.getElementById('main-sub-div').style.backgroundColor = "#f5f5f5";
            document.getElementById('body').style.backgroundColor = "#f5f5f5";
            document.getElementsByClassName('change-theme')[1].classList.add('hide-icon');
            document.getElementsByClassName('change-theme')[0].classList.remove('hide-icon');
        } else {
            document.getElementById('main-div').style.backgroundColor = "black";
            document.getElementById('main-sub-div').style.backgroundColor = "black";
            document.getElementById('body').style.backgroundColor = "black";
            document.getElementsByClassName('change-theme')[0].classList.add('hide-icon');
            document.getElementsByClassName('change-theme')[1].classList.remove('hide-icon');
        }
    } catch (error) {
        console.log('no themes decided yet');
    }
}
function changeTheme() {
    let theme = localStorage.getItem('theme');
    if (theme == null || theme == 'light') {
        localStorage.setItem('theme', 'dark');
        document.getElementById('main-div').style.backgroundColor = "black";
        document.getElementById('main-sub-div').style.backgroundColor = "black";
        document.getElementById('body').style.backgroundColor = "black";
        document.getElementsByClassName('change-theme')[0].classList.add('hide-icon');
        document.getElementsByClassName('change-theme')[1].classList.remove('hide-icon');
    } else if (theme == 'dark') {
        localStorage.setItem('theme', 'light');
        document.getElementById('main-div').style.backgroundColor = "#f5f5f5";
        document.getElementById('main-sub-div').style.backgroundColor = "#f5f5f5";
        document.getElementById('body').style.backgroundColor = "#f5f5f5";
        document.getElementsByClassName('change-theme')[1].classList.add('hide-icon');
        document.getElementsByClassName('change-theme')[0].classList.remove('hide-icon');
    }
}