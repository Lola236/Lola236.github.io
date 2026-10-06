//alle projecten
const projecten = [
    {
        titel: "SmartKas",
        studiejaar: 1,
        beschrijving: [
            "Tijdens dit project heb ik samen met mijn groepje een SmartKas gemaakt. Met behulp van de microbit en sensoren konden we onder andere de temperatuur en de vochtigheid van de grond meten.",
            "Hierdoor kon worden bepaald wanneer een plant water nodig had. Daarnaast hebben we een website gemaakt waarop gebruikers konden inloggen en informatie over verschillende planten konden bekijken."
        ],
        technieken: "HTML - CSS - JavaScript - microbit"
    },

    {
        titel: "Scrum Escape Game",
        studiejaar: 1,
        beschrijving: [
            "In het tweede semester heb ik samen met mijn groepje een leerzame escape game gemaakt.",
            "Het doel van het spel was om gebruikers op een leuke en interactieve manier kennis te laten maken met Scrum. Het spel bestond uit verschillende kamers die onderdelen van Scrum vertegenwoordigden, zoals Sprint Planning, Daily Scrum, Sprint Review en Retrospective.",
            "De speler moest vragen en opdrachten goed beantwoorden om naar de volgende kamer te komen. Bij een fout antwoord kreeg de speler een obstakel dat eerst opgelost moest worden."
        ],
        technieken: "Java - Scrum - Game development"
    },

    {
        titel: "Teamflow",
        studiejaar: 1,
        beschrijving: [
            "Tijdens dit project heb ik samen met mijn groepje gewerkt aan Teamflow.",
            "Teamflow is een chatapplicatie gericht op teams die werken volgens Scrum. Het doel was om communicatie en projectmanagement beter met elkaar te verbinden.",
            "Teamleden konden met elkaar communiceren en berichten koppelen aan Scrum-elementen zoals epics, user stories en taken."
        ],
        technieken: "Scrum - Softwareontwikkeling - Chatapplicaties"
    },

    {
        titel: "FloraBid",
        studiejaar: 2,
        beschrijving: [
            "In het tweede jaar heb ik samen met mijn groepje gewerkt aan FloraBid, een veilingwebsite voor bloemen en planten.",
            "Gebruikers konden via de applicatie deelnemen aan veilingen en biedingen plaatsen. Binnen de applicatie waren verschillende rollen, waaronder klanten, veilingmeesters en aanvoerders.",
            "Iedere rol had een eigen account en eigen functionaliteiten. Voor dit project hebben we een full-stack webapplicatie ontwikkeld met een frontend, backend en database.",
            "De frontend is ontwikkeld met JavaScript en React. De backend is ontwikkeld in C# met ASP.NET Core."
        ],
        technieken: "C# - JavaScript - React - ASP.NET Core - Entity Framework Core - LINQ - MS SQL - Scrum"
    },

    {
        titel: "AI Boodschappenplanner",
        studiejaar: 2,
        beschrijving: [
            "Voor dit project heb ik samen met mijn projectgroep een AI-agent ontwikkeld die gebruikers helpt met het plannen van hun boodschappen.",
            "De gebruiker kon verschillende gegevens invoeren, zoals allergieën, wat de gebruiker wilde eten, het aantal personen en het beschikbare budget.",
            "Op basis van deze gegevens maakte de AI-agent een passende boodschappenlijst. Hierbij hield de agent rekening met de wensen en beperkingen van de gebruiker."
        ],
        technieken: "AI-agents - LLM's - Workflows - Scrum"
    }
];


//plekken uit html ophalen waar de projecten komen te staan
const jaar1Container = document.querySelector("#projecten-jaar1");
const jaar2Container = document.querySelector("#projecten-jaar2");


//3 filterknoppen uit de html ophalen
//deze knoppen kan gebruiker kiezen welke projecten worden getoond
const filterAlles = document.querySelector("#filter-alles");
const filterJaar1 = document.querySelector("#filter-jaar1");
const filterJaar2 = document.querySelector("#filter-jaar2");
const filterKnoppen = document.querySelectorAll(".project-filter button");


//2sections voor jaar 1 en jaar 2 uit de html ophalen
const jaar1Section = document.querySelector("#jaar1");
const jaar2Section = document.querySelector("#jaar2");


//door alle projecten uit de array heen gaan
//ieder project  nieuwe html element gemaakt
projecten.forEach((project) => {

    //html elementen voor project maken
    const article = document.createElement("article");
    const projectKnop = document.createElement("button");
    const titel = document.createElement("h3");
    const projectContent = document.createElement("div");


    //classestoevoegen zodatelementen juiste css krijgt
    projectKnop.classList.add("project-toggle");
    projectContent.classList.add("project-content");
    projectContent.classList.add("verborgen");


    //titel van project in h3 zetten.
    titel.textContent = project.titel;


    //door alle stukken van de beschrijving heen gaan
    //ieder stuk tekst wordt nieuwe paragraaf gemaakt
    project.beschrijving.forEach((tekst) => {

        const paragraaf = document.createElement("p");

        //beschrijving in paragraaf zetten
        paragraaf.textContent = tekst;

        //paragraaf toevoegen aan inhoud van project
        projectContent.appendChild(paragraaf);
    });


    //paragraaf maken waarin de gebruikte technieken komen
    const technieken = document.createElement("p");
    technieken.classList.add("techniques");

    // "technieken:" dikgedrukt
    const techniekenTitel = document.createElement("strong");
    techniekenTitel.textContent = "Technieken:";

    //titel nieuwe regel en technieken toevoegen
    technieken.appendChild(techniekenTitel);
    technieken.appendChild(document.createElement("br"));
    technieken.appendChild(document.createTextNode(project.technieken));

    //technieken toevoegen aan inhoud van project
    projectContent.appendChild(technieken);


    //alle gemaakte onderdelen samenvoegen tot 1 project
    projectKnop.appendChild(titel);

    article.appendChild(projectKnop);
    article.appendChild(projectContent);


    //als gebruiker op project klikt wordt de inhoud geopend of dicht
    projectKnop.addEventListener("click", () => {

        //class verborgen toevoegen of weghalen
        projectContent.classList.toggle("verborgen");

        //class open toevoegen of weghalen zodat pijltje draait
        projectKnop.classList.toggle("open");
    });


    //controleren bij welk studiejaar project hoort
    //daarna wordt project bij jaar 1 of 2 op pagina gezet
    if (project.studiejaar === 1) {

        jaar1Container.appendChild(article);

    } else {

        jaar2Container.appendChild(article);
    }
});


//als gebruiker op 1e jaar klikt wordt alleen de section van jaar 1 getoond
filterJaar1.addEventListener("click", () => {

    jaar1Section.style.display = "block";
    jaar2Section.style.display = "none";
});


//als gebruiker op 2e jaar klikt, wordt alleen de section van jaar 2 getoond
filterJaar2.addEventListener("click", () => {

    jaar1Section.style.display = "none";
    jaar2Section.style.display = "block";
});


//als gebruiker op Alles klikt, word allebei getoond
filterAlles.addEventListener("click", () => {

    jaar1Section.style.display = "block";
    jaar2Section.style.display = "block";
});


//bij alle filterknoppen controleren wanneer erop wordt geklikt
filterKnoppen.forEach((knop) => {

    knop.addEventListener("click", () => {

        //eerst de actieve class bij alle knoppen weghalen
        filterKnoppen.forEach((andereKnop) => {
            andereKnop.classList.remove("active-filter");
        });

        //daarna de actieve class toevoegen aan knop waarop is geklikt
        knop.classList.add("active-filter");
    });
});