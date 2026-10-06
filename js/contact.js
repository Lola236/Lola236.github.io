const contactForm = document.querySelector("#contact-form");

const naam = document.querySelector("#naam");
const naamFout = document.querySelector("#naam-fout");

const email = document.querySelector("#email");
const emailFout = document.querySelector("#email-fout");

const bericht = document.querySelector("#bericht");
const berichtFout = document.querySelector("#bericht-fout");

const succesmelding = document.querySelector("#succesmelding");


contactForm.addEventListener("submit", (event) => {
    event.preventDefault();


    //naam controleren
    if (naam.value.trim() === "") {
        naamFout.textContent = "Vul je naam in.";
        naam.setAttribute("aria-invalid", "true");
    } else {
        naamFout.textContent = "";
        naam.setAttribute("aria-invalid", "false");
    }


    //email controleren
    if (email.value.trim() === "") {
        emailFout.textContent = "Vul je e-mailadres in.";
        email.setAttribute("aria-invalid", "true");
    } else if (!email.validity.valid) {
        emailFout.textContent = "Vul een geldig e-mailadres in.";
        email.setAttribute("aria-invalid", "true");
    } else {
        emailFout.textContent = "";
        email.setAttribute("aria-invalid", "false");
    }


    //bericht controleren
    if (bericht.value.trim() === "") {
        berichtFout.textContent = "Vul een bericht in.";
        bericht.setAttribute("aria-invalid", "true");
    } else if (bericht.value.trim().length < 10) {
        berichtFout.textContent = "Je bericht moet minimaal 10 tekens bevatten.";
        bericht.setAttribute("aria-invalid", "true");
    } else {
        berichtFout.textContent = "";
        bericht.setAttribute("aria-invalid", "false");
    }


    //controleren of alles geldig is
    if (
        naam.value.trim() !== "" &&
        email.value.trim() !== "" &&
        email.validity.valid &&
        bericht.value.trim().length >= 10
    ) {
        succesmelding.textContent =
            "Bedankt voor je bericht! Het formulier is succesvol ingevuld.";
    } else {
        succesmelding.textContent = "";
    }
});