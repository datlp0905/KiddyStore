// JavaScript Document
function validatePassword() {
	var oldPassword = document.getElementById("old_password").value;
	var newPassword = document.getElementById("new_password").value;
	var confirmPassword = document.getElementById("confirm_password").value;
	
	var errOldPassword = document.getElementById("error-old-password");
	var errNewPassword = document.getElementById("error-new-password");
	var errConfirmPassword = document.getElementById("error-confirm-password");
	
	var check = true;
	if(oldPassword.length === 0) {
		check = false;
		errOldPassword.innerHTML = "Please enter your old password!";
	} else {
		errOldPassword.innerHTML = "";
	}
	
	if(newPassword.length === 0) {
		check = false;
		errNewPassword.innerHTML = "New password is required!";
	} else if(!newPassword.match("[A-Z][A-Za-z0-9_]+")) {
		check = false;
		errNewPassword.innerHTML = "Password must have this format A[A-z0-9]";
	} else {
		errNewPassword.innerHTML = "";
	}
	
	if(confirmPassword !== newPassword || confirmPassword.length === 0) {
		check = false;
		errConfirmPassword.innerHTML = "Confirm password is not correct!";
	}
	return check;
}