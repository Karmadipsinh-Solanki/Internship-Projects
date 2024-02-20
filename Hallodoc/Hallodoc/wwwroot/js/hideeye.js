 let eye = false;
 function onpress() {
     if (!eye) {
         document.getElementsByClassName('person-icon')[1].classList.add('hide-eye');
         document.getElementsByClassName('person-icon')[0].classList.remove('hide-eye');
         document.getElementById('password').type = "text";
         eye = true;
     } else {
         document.getElementsByClassName('person-icon')[1].classList.remove('hide-eye');
         document.getElementsByClassName('person-icon')[0].classList.add('hide-eye');
         document.getElementById('password').type = "password";
         eye = false;
     }
 }
 function onpresss() {
     if (!eye) {
         document.getElementsByClassName('person-icon')[3].classList.add('hide-eye');
         document.getElementsByClassName('person-icon')[2].classList.remove('hide-eye');
         document.getElementById('retype_password').type = "text";
         eye = true;
     } else {
         document.getElementsByClassName('person-icon')[3].classList.remove('hide-eye');
         document.getElementsByClassName('person-icon')[2].classList.add('hide-eye');
         document.getElementById('retype_password').type = "password";
         eye = false;
     }
 }