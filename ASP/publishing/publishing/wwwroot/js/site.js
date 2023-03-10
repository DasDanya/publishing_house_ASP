
function ConfirmAction(message) {
    if (!confirm(message)) {
        event.preventDefault();
    }
}

function Confirm(element) {
    let table = document.querySelector('#tabl');
    var status = table.rows[element.closest('tr').rowIndex].cells[2].innerHTML;
    ;
    if (status.trim() == "Выполняется") {
        if (!confirm("Вы подтверждаете выполнение заказа?")) {
            event.preventDefault();
        }
    }
}

function CheckBoxes() {
    var boxes = document.getElementsByClassName('check_box');
    var checked = false;
    for (i = 0; i < boxes.length; i++) {
        if (boxes[i].checked) {
            checked = true;
            break;
        }
    }
    if (!checked) {
        alert("Пожалуйста, выберите сотрудника(-ов) для регистрации заказа");
        event.preventDefault();
    }
}

function SetDate() {

    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1;
    var min_yyyy = today.getFullYear();
    var max_yyyy = today.getFullYear() + 10;
    if (dd < 10) {
        dd = '0' + dd;
    }

    if (mm < 10) {
        mm = '0' + mm;
    }

    today = min_yyyy + '-' + mm + '-' + dd;
    max_date = max_yyyy + '-' + mm + '-' + dd;
    document.getElementById("datefield").setAttribute("min", today);
    document.getElementById("datefield").setAttribute("max", max_date);
}


