function myValidate() {
    var check = true;
    var firstName = document.getElementById("first_name").value;
    var lastName = document.getElementById("last_name").value;
    var password = document.getElementById("password").value;
    var confirm = document.getElementById("confirm").value;
    var email = document.getElementById("email").value;
    if (firstName == "") {
        document.getElementById("fNameValidate").innerHTML = "First name cannot be blank!";
        check = false;

    } else {
        document.getElementById("fNameValidate").innerHTML = "";
    }

    if (lastName == "") {
        document.getElementById("lastNameValidate").innerHTML = "Last name cannot be blank!";
        check = false;
    } else {
        document.getElementById("lastNameValidate").innerHTML = "";
    }

    if (password == "") {
        document.getElementById("passwordValidate").innerHTML = "Password must have more than 8 characters!";
        check = false;
    } else {
        document.getElementById("passwordValidate").innerHTML = "";
    }

    if (password.match("[A-Z][A-Za-z0-9_]+") == null) {
        document.getElementById("passwordValidate").innerHTML = "Password must have this format A[A-z0-9]";
        check = false;
    } else {
        document.getElementById("passwordValidate").innerHTML = "";
    }

    if (confirm != password || confirm == "") {
        document.getElementById("confirmValidate").innerHTML = "Confirm password is not correct!";
        check = false;
    } else {
        document.getElementById("confirmValidate").innerHTML = "";
    }

    if (email.trim().match(/^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{1,5}|[0-9]{1,3})(\]?)$/) == null) {
        document.getElementById("emailValidate").innerHTML = "Email is not correct!";
        check = false;
    } else {
        document.getElementById("emailValidate").innerHTML = "";
    }

    return (check);
}