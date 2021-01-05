# Napredni Iks-Oks

## dresscode.ing & VegaIT Sourcing Hackaton

Aplikacija izradjena u svrhe učestvovanja u Hackaton-u organizovanom od strane instagram stranice [dresscode.ing](https://www.instagram.com/dresscode.ing/) i kompanije [VegaIT Sourcing](https://www.instagram.com/vegaitsourcing/).

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

U slučaju da ne posedujete Visual Studio kreiran je [**.msi**](https://github.com/tepke22/advanced-tic-tac-toe/blob/master/AdvancedTicTacToeSetup.msi) fajl za jednostavnu instalaciju aplikacije na vaš računar.

## Igra
Klikom na dugme **"Start"** nasumično se odredjuje ko prvi igra, ako funkcija Random vrati **0** prvi igrač je **"Računar"** a ako izbaci **1** prvi igrač je **"Čovek"**.

Ako dodje do nečije pobede ili je nerešeno, iskače prozor sa objavom pobednika tj. ispisom "Nerešeno" i pitanjem da li restartovati igru.