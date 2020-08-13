const radioSelection = document.querySelector(".radio-form");
const radioOptions = document.querySelectorAll('input[name="OneSeason"]');
const seasonTextBox = document.querySelector('.specified-season');

function reveal() {
    for (var i = 0; i < radioOptions.length; i++) {
        if (radioOptions[i].checked) {
            radioOptions[i].value == "false" ? seasonTextBox.classList.add('form-hide') : seasonTextBox.classList.remove('form-hide');
        }
    }

}

radioSelection.addEventListener('change', reveal, false);
