function UpdateUnitComboBox() {
    $.ajax({
        url: window.location.origin + "/Unit/GetUnitsByZone",
        type: "GET",
        async: true,
        data: {
            "zoneID": $("#zone-name").val()
        }, success: function (result) {
            $("#unit-name").empty();
            $("#unit-name").append("<option></option>");
            for (var index in result) {
                var item = `<option value="${result[index]['ID']}">${result[index]['UnitName']}</option>`;
                $("#unit-name").append(item);
            }
        }
    })
}

function SignUp() {
    $.ajax({
        url: window.location.origin + "/User/CreateAccount",
        type: "POST",
        async: false,
        data: {
            "UserName": $("#username").val(),
            "Password": $("#password").val(),
            "confirmPassword": $("#confirmPassword").val(),
            "UnitID": parseInt($("#unit-name").val())
        }
    })
}