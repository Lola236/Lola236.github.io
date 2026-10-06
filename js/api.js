const url = "https://api.open-meteo.com/v1/forecast?latitude=51.924&longitude=4.478&hourly=temperature_2m&models=knmi_seamless&timezone=auto&forecast_days=1";

const apiData = document.querySelector("#api-data");


function toonTemperatuur(temperatuur, eenheid) {
    const temperatuurTekst = document.createElement("p");
    temperatuurTekst.textContent = temperatuur + " " + eenheid;

    apiData.appendChild(temperatuurTekst);
}


function toonLaadstatus() {
    apiData.textContent = "Gegevens laden...";
}


function toonFoutmelding() {
    apiData.textContent = "De gegevens konden niet worden geladen.";
}


async function haalDataOp() {
    toonLaadstatus();

    try {
        const response = await fetch(url);

        if (!response.ok) {
            throw new Error("Er ging iets mis bij het ophalen van de gegevens.");
        }

        const data = await response.json();

        console.log(data);

        const huidigUur = new Date().getHours();
        const temperatuur = data.hourly.temperature_2m[huidigUur];
        const eenheid = data.hourly_units.temperature_2m;

        apiData.textContent = "";

        toonTemperatuur(temperatuur, eenheid);

    } catch (error) {
        toonFoutmelding();
        console.log(error);
    }
}


haalDataOp();