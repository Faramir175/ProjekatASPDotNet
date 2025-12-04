# MojAtar - Sistem za upravljanje poljoprivrednim gazdinstvom

**MojAtar** je veb aplikacija razvijena kao deo završnog rada na Fakultetu organizacionih nauka. Sistem omogućava poljoprivrednicima kompletno digitalno vođenje gazdinstva, od unosa mehanizacije i parcela, preko praćenja agrotehničkih mera, do automatskog proračuna troškova, prinosa i zarade.

## 🛠 Korišćene tehnologije

Projekat je razvijen prateći principe **Clean Architecture**.

* **Platforma:** .NET 8 (ASP.NET Core MVC)
* **Baza podataka:** Microsoft SQL Server (Entity Framework Core)
* **Frontend:** Razor Views, Bootstrap 5, JavaScript
* **Generisanje PDF-a:** Rotativa.AspNetCore

## 📋 Detaljne funkcionalnosti

Sistem je dizajniran tako da se svi podaci vezuju striktno za ulogovanog korisnika (vlasnika gazdinstva).

### 1. Upravljanje matičnim podacima (Resursi gazdinstva)
Korisnik samostalno definiše i menja resurse koje koristi u radu:
* **Parcele:** Evidencija zemljišnih poseda.
* **Mehanizacija:** Unos **radnih mašina** (traktori, kombajni) i **priključnih mašina** (plugovi, sejalice).
* **Resursi:** Evidencija materijala (gorivo, seme, hemija).
* **Kulture:** Definisanje poljoprivrednih kultura koje se uzgajaju.

### 2. Agrotehničke mere (Radnje)
Centralni deo sistema je evidentiranje radova na parcelama. Sistem podržava:
* **Obične radnje:** (npr. oranje, đubrenje) gde se beleže utrošeni resursi i korišćene mašine.
* **Specijalne radnje - SETVA:** Povezuje kulturu sa parcelom i beleži površinu koja je posejana.
* **Specijalne radnje - ŽETVA:** Unos ostvarenog prinosa za određenu kulturu.
    * *Automatizacija:* Nakon uspešne žetve, ostvareni prinos se automatski dodaje na **raspoloživu količinu** te kulture u sistemu.

### 3. Proračuni i Logika
* **Obračun troškova:** Za svaku radnju sistem automatski računa ukupne troškove na osnovu cene utrošenih resursa (gorivo, seme...).
* **Upravljanje zalihama:** Stanje kultura se automatski ažurira nakon žetve (povećanje) ili prodaje (smanjenje).

### 4. Prodaja i Finansije
* **Prodaja proizvoda:** Mogućnost prodaje kultura samo ukoliko postoji raspoloživa količina na stanju (nakon žetve).
* **Istorija cena:** Praćenje kretanja cena kultura kroz vreme.
* **Izveštavanje:** Generisanje detaljnih **PDF izveštaja** o prihodima (od prodaje) i rashodima (troškovi radnji) za odabrani vremenski period.

## 🚀 Uputstvo za pokretanje

### Preduslovi
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* SQL Server

### Instalacija

1.  **Klonirajte repozitorijum:**
    ```bash
    git clone [https://github.com/Faramir175/ProjekatASPDotNet.git](https://github.com/Faramir175/ProjekatASPDotNet.git)
    ```

2.  **Podešavanje baze:**
    Proverite `ConnectionStrings` u `appsettings.json` (Web projekat).

3.  **Kreiranje baze (Migracije):**
    Pozicionirajte se u folder gde je DbContext i pokrenite:
    ```bash
    dotnet ef database update
    ```

4.  **Pokretanje:**
    ```bash
    dotnet run --project MojAtar.Web
    ```

---
*Autor: Milan Fara*