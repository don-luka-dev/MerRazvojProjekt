Customer Management Demo

Ovo je mali proof-of-concept projekt za upravljanje većom bazom kupaca. Ideja je bila simulirati sustav koji radi s velikim brojem zapisa i pokazati kako se mogu efikasno raditi filtriranje, paginacija i bulk operacije.

Backend je napravljen u .NET Web API-ju, a frontend u najnovijem Angularu koristeći standalone komponente i signal-based API. Za UI koristim PrimeNG komponente, dok je layout napravljen s Tailwindom.

Baza se automatski seed-a sa 100 000 zapisa kako bi se moglo testirati ponašanje aplikacije na većoj količini podataka.

How to run

Prvo klonirati repozitorij.

git clone https://github.com/don-luka-dev/MerRazvojProjekt.git

Backend

U backend folderu pokrenuti aplikaciju.

dotnet run

Prilikom pokretanja će se automatski napraviti baza, pokrenuti migracije i generirati 100 000 kupaca.

Frontend

U frontend folderu instalirati pakete.

npm install

Zatim pokrenuti Angular aplikaciju.

ng serve

Aplikacija će biti dostupna na http://localhost:4200

Design decisions

Pokušao sam držati controllere što jednostavnijima, pa je sva poslovna logika smještena u service layer. Controlleri uglavnom samo primaju request i vraćaju odgovarajuće HTTP statuse.

Lista kupaca koristi server-side filtriranje, sortiranje i paginaciju kako frontend ne bi morao učitavati svih 100 000 zapisa odjednom.

Za mapiranje između entiteta i DTO objekata koristim Mapster, a FluentValidation koristim za validaciju ulaznih podataka prije nego što dođu do poslovne logike.

Bulk deactivate endpoint prima listu ID-eva i deaktivira ih u jednom database round-tripu koristeći ExecuteUpdateAsync, bez učitavanja svih entiteta u memoriju.

Umjesto pravog brisanja koristi se soft delete, odnosno kupac se samo označi kao neaktivan.

Dodao sam i dva middlewarea: jedan za globalno hvatanje iznimki i jedan za logiranje API poziva zajedno s vremenom odgovora.

Seed generira 100 000 kupaca i sprema ih u batchovima od 5000 zapisa kako bi se ubrzao unos u bazu.

What I would improve with more time

Dodao bih indekse u bazi za kolone koje se često filtriraju, poput imena, grada i države. Također bih dodao testove za servisni sloj i možda caching za stats endpoint.

Time spent

Backend implementacija oko 5 sati.
Frontend implementacija oko 4 sata.
Testiranje i sitna poboljšanja oko 1 sat.

Ukupno otprilike 10 sati.
