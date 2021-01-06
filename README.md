# Napredni Iks-Oks

## dresscode.ing & VegaIT Sourcing Hackaton

Aplikacija izradjena u svrhe učestvovanja na Hackaton-u organizovanom od strane instagram stranice [dresscode.ing](https://www.instagram.com/dresscode.ing/) i kompanije [VegaIT Sourcing](https://www.instagram.com/vegaitsourcing/).

## Aplikacija

Aplikacija predstavlja napredniju verziju igre Iks-Oks. Šta tačno predstavlja naprednije?

- Veličina nije 3x3 već 4x4
- Moguće je pobediti spajanjem 4 znaka X ili O
    - u jednom redu
    - u jednoj koloni
    - u jednoj dijagonali
    - **tako da čine jedan kvadrat (prikazano na slici)**

![Primer](https://user-images.githubusercontent.com/39384168/103683284-495f5e00-4f8a-11eb-8dff-8c4819d7f565.png)


## Pokretanje
Aplikacija je izradjena u programskom jeziku C# (Windows Forms .NET Framework) u okruzenju Visual Studio 2019.

Ako posedujete Visual Studio možete pokrenuti aplikaciju jednostavnim učitavanjem koda i klikom na Start.

U slučaju da ne posedujete Visual Studio kreiran je [**.msi**](https://github.com/tepke22/advanced-tic-tac-toe/blob/master/AdvancedTicTacToeSetup.msi) fajl za jednostavnu instalaciju aplikacije na vaš računar. Nažalost Windows vam neće baš olako dozvoliti da instalirate ovu aplikaciju pa je nakon pokretanja **.msi** fajla potrebno kliknuti na **"More info"** i zatim **"Run anyway"**.

## Igra
Klikom na dugme **"Start"** nasumično se odredjuje ko prvi igra, ako funkcija Random vrati **0** prvi igrač je **"Računar"** a ako izbaci **1** prvi igrač je **"Čovek"**.

Znak se postavlja klikom na odredjeno polje, Računar igra 1 sekundu kasnije, nakon vašeg poteza. Ako Računar igra prvi potrebno je sačekati da on odigra prvi potez.

Ako dodje do nečije pobede ili je nerešeno, iskače prozor sa objavom pobednika tj. ispisom "Nerešeno" i pitanjem da li želite da restartujete igru.

## Način razmišljanja Računara

- Ako računar uvidi priliku da pobedi, iskoristiće je
- Ukoliko nema priliku da pobedi proveriće imate li vi (Čovek) priliku da pobedite, ako imate, sabotiraće vas
- Ukoliko se ne desi ništa od gore navedenih slučajeva upisuje odgovarajući znak na Random odabrano mesto