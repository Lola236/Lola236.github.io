//contactformulier en invoervelden uit html ophalen
const contactForm = document.querySelector("#contact-form");

const naam = document.querySelector("#naam");
const naamFout = document.querySelector("#naam-fout");

const email = document.querySelector("#email");
const emailFout = document.querySelector("#email-fout");

const bericht = document.querySelector("#bericht");
const berichtFout = document.querySelector("#bericht-fout");

//succesmelding
const succesmelding = document.querySelector("#succesmelding");


//uitgevoerd als  gebruiker op versturen klikt
contactForm.addEventListener("submit", (event) => {

    //voorkomt dat formulier meteen wordt verstuurd eerst controleren of alles goed is ingevuld
    event.preventDefault();


    //naam controleren
    //als veld leeg is, wordt er foutmelding getoond
    if (naam.value.trim() === "") {
        naamFout.textContent = "Vul je naam in.";
        naam.setAttribute("aria-invalid", "true");
    } else {

        //als naam goed is ingevuld, wordt foutmelding weggehaald
        naamFout.textContent = "";
        naam.setAttribute("aria-invalid", "false");
    }


    //email controleren
    //eerst controleren of veld leeg is
    if (email.value.trim() === "") {
        emailFout.textContent = "Vul je e-mailadres in.";
        email.setAttribute("aria-invalid", "true");

    //controleren of ingevulde email geldig is
    } else if (!email.validity.valid) {
        emailFout.textContent = "Vul een geldig e-mailadres in.";
        email.setAttribute("aria-invalid", "true");

    } else {

        //als email goed is, wordt foutmelding weggehaald
        emailFout.textContent = "";
        email.setAttribute("aria-invalid", "false");
    }


    //bericht controleren
    //eerst controleren of veld leeg is
    if (bericht.value.trim() === "") {
        berichtFout.textContent = "Vul een bericht in.";
        bericht.setAttribute("aria-invalid", "true");

    //controleren of bericht minimaal 10 tekens heeft
    } else if (bericht.value.trim().length < 10) {
        berichtFout.textContent = "Je bericht moet minimaal 10 tekens bevatten.";
        bericht.setAttribute("aria-invalid", "true");

    } else {

        //als bericht goed is, wordt foutmelding weggehaald
        berichtFout.textContent = "";
        bericht.setAttribute("aria-invalid", "false");
    }


    //controleren of alle drie de velden goed zijn ingevuld
    if (
        naam.value.trim() !== "" &&
        email.value.trim() !== "" &&
        email.validity.valid &&
        bericht.value.trim().length >= 10
    ) {

        //als alles goed is, wordt succesmelding getoond
        succesmelding.textContent =
            "Bedankt voor je bericht! Het formulier is succesvol ingevuld.";

    } else {

        //als er nog een fout is, wordt er geen succesmelding getoond
        succesmelding.textContent = "";
    }
});