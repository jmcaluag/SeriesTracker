var radioSelection = document.querySelector(".radio-form");
var radioOptions = document.querySelectorAll('input[name="radio-season"]');
var seasonTextBox = document.querySelector('.specified-season');

function reveal() {
    for (var i = 0; i < radioOptions.length; i++) {
        if (radioOptions[i].checked) {
            radioOptions[i].value == "true" ? seasonTextBox.classList.add('form-hide') : seasonTextBox.classList.remove('form-hide');
        }
    }

}

radioSelection.addEventListener('change', reveal, false);
