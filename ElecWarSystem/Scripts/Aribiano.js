/* Author  : Sayed
* Purpose  : change english numbers to arabic numbers
*
* */

window.onbeforeunload = function () {
    if (window.event.clientX < 0 && window.event.clientY < 0) {
        if (confirm("Are you sure?")) {
            window.close();
        }
    }
    
    
}
window.onload = function () {
    numbersE2A();
}
function dateE2A(englishNumbers) {
    englishNumbers = englishNumbers.toString();
    var arabicNumbers = ["٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩"];
    var arabicNumber = "";
    for (var index = 0; index < englishNumbers.length; index++) {
        if (englishNumbers[index] == '-')
            // if you tried to add - it will be reversed
            arabicNumber += '/';
        else
            arabicNumber += arabicNumbers[englishNumbers[index]];
    }
    // NOTE : try to do this
    // arabicNumber.split("-").reverse().join("-")
    // there is no reverse function on a string in js just in an array so we do this
    // 02-09-2021 -> ['02', '09', '2021'] -> ['2021','09','02'] -> 2021-09-02
    return arabicNumber;
}
/*
* Author : Sayed
* convert english numbers in a text to arabic numbers
* param {string} englishText content of a tag
*/

function numbersE2A() {
    var arabicNumbers = ["٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩"];
    var englishNumbers = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];

    var tags = $('.aribiano');
    for (var tagIndex = 0; tagIndex < tags.length; tagIndex++) {
        var arabicNumber = "";
        var englishText = tags[tagIndex].innerHTML;
        if (tags[tagIndex].tagName == 'INPUT' || tags[tagIndex].tagName == 'TEXTAREA')
            englishText = tags[tagIndex].value;

        for (var letterIndex = 0 ; letterIndex < englishText.length ; letterIndex++) {
            if (englishNumbers.includes(englishText[letterIndex]))
                arabicNumber += arabicNumbers[englishText[letterIndex]];
            else
                arabicNumber += englishText[letterIndex];
        }

        if (tags[tagIndex].tagName == 'INPUT' || tags[tagIndex].tagName == 'TEXTAREA') {
            tags[tagIndex].value = arabicNumber;
        }
        else
            tags[tagIndex].innerHTML = arabicNumber;
    }
}


function numbersEn2Ar(englishWord) {
    var arabicNumbers = ["٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩"];
    var englishNumbers = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];
    var arabicNumber = "";
    for (var letterIndex = 0; letterIndex < englishWord.length; letterIndex++) {
        if (englishNumbers.includes(englishWord[letterIndex])) {
            arabicNumber += arabicNumbers[englishNumbers.indexOf(englishWord[letterIndex])];
            
        }
        else
            arabicNumber += englishWord[letterIndex];
    }
    return arabicNumber;
}


