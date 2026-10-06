const url = "https://api.open-meteo.com/v1/forecast?latitude=51.924&longitude=4.478&hourly=temperature_2m&models=knmi_seamless&timezone=auto&forecast_days=1";
//const url = "https://api.open-meteo.com/v1/fout?latitude=51.924&longitude=4.478&hourly=temperature_2m&models=knmi_seamless&timezone=auto&forecast_days=1";


//temperatuur op website gezet
const apiData = document.querySelector("#api-data");


//temperatuur word op pagina getoond
function toonTemperatuur(temperatuur, eenheid) {

    //nieuw p-element maken
    const temperatuurTekst = document.createElement("p");

    //temperatuur en eenheid in p-element zetten
    temperatuurTekst.textContent = temperatuur + " " + eenheid;

    // p-element op de pagina zetten
    apiData.appendChild(temperatuurTekst);
}


//laadmelding laten zien
function toonLaadstatus() {
    apiData.textContent = "Gegevens laden...";
}


//foutmelding laten zien als de API niet werkt
function toonFoutmelding() {
    apiData.textContent = "De gegevens konden niet worden geladen.";
}


//gegevens ophalen van de API
async function haalDataOp() {

    //eerst de laadmelding laten zien
    toonLaadstatus();

    try {

        //gegevens ophalen met fetch
        const response = await fetch(url);


        //controleren of ophalen goed is gegaan
        if (!response.ok) {
            throw new Error("Er ging iets mis bij het ophalen van de gegevens.");
        }


        //opgehaalde gegevens omzetten naar JS
        const data = await response.json();


        //huidige uur ophalen
        const huidigUur = new Date().getHours();

        //temperatuur van huidige uur ophalen
        const temperatuur = data.hourly.temperature_2m[huidigUur];

        //eenheid van temperatuur ophalen
        const eenheid = data.hourly_units.temperature_2m;


        //laadmelding weghalen
        apiData.textContent = "";


        //temperatuur laten zien
        toonTemperatuur(temperatuur, eenheid);

    } catch (error) {

        //foutmelding laten zien als er iets fout gaat
        toonFoutmelding();

        //fout in de console laten zien
        console.log(error);
    }
}


//API uitvoeren
haalDataOp();