function AlertSuccess(text) {
    Swal.fire(
        'Success!',
        text,
        'success'
    ).then(function () {
    });
}

function AlertError(text) {
    Swal.fire(
        'Error!',
        text,
        'error'
    ).then(function () {
    });
}

function AlertWarning(text, condition) {
    Swal.fire(
        'Warning!',
        text,
        'info'
    ).then(function () {
        if (condition == 'back') {
            window.history.back();
        } else if (condition == 'login') {
            window.location = 'Login.aspx';
        }
    });
}
// ลิงค์นี้ช่วยชีวิต
// https://stackoverflow.com/questions/44729434/using-sweetalert2-to-replace-return-confirm-on-an-asp-net-button
function AlertConfirm(btnDelete) {
    if (btnDelete.dataset.confirmed) {
        // The action was already confirmed by the user, proceed with server event
        btnDelete.dataset.confirmed = false;
        return true;
    } else {
        event.preventDefault();
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then(function (result) {
            if (result.isConfirmed) {
                btnDelete.dataset.confirmed = true;
                btnDelete.click();
            }
        })
    }
}

function AlertTopEndSuccess(text) {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    })

    Toast.fire({
        icon: 'success',
        title: text
    })
}

function AlertTopEndError(text) {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    })

    Toast.fire({
        icon: 'error',
        title: text
    })
}

function AlertTopEndWarning(text) {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    })

    Toast.fire({
        icon: 'info',
        title: text
    })
}