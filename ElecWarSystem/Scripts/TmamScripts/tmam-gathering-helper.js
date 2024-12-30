function GetTmamDetail(tmamID) {
    window.location.href = `${window.location.origin}/TmamGathering/TmamDetails?id=${tmamID}`
}

function RecieveTmam(id) {
    console.log(id);
    $.ajax({
        url: window.location.origin + "/TmamGathering/MakeTmamRecive",
        type: "POST",
        async: false,
        data: {
            "unitID": id
        }, success: function () {
            window.location.href = `${window.location.origin}/TmamGathering/RecievedTmam`
        }
    })
}

function rebackTmam(id) {
    console.log(id);
    $.ajax({
        url: window.location.origin + "/TmamGathering/MakeTmamReturn",
        type: "POST",
        async: false,
        data: {
            "unitID": id
        }, success: function () {
            window.location.href = `${window.location.origin}/TmamGathering/RecievedTmam`
        }
    })
}

function showPopup(id) {
    var x = document.getElementById(id).offsetTop;
    var y = document.getElementById(id).offsetLeft;
    document.getElementById("contain").style.transform = $`translate(${x},${y})`;
}