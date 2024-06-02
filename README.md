# Aplikacja Bankowa

## Opis Projektu

Celem tego projektu jest stworzenie prostej aplikacji bankowej, która pozwala użytkownikom na:
- Tworzenie kont bankowych
- Logowanie się do swoich kont
- Sprawdzanie salda
- Wykonywanie podstawowych operacji finansowych, takich jak wpłaty, wypłaty, przelewy
- Przeglądanie historii transakcji

Aplikacja została stworzona przez: 
- Rafał Suchorski
- Mikołaj Szymanowicz
- Oliwia Szaforz

## Funkcjonalności

- **Tworzenie konta:** Użytkownicy mogą stworzyć nowe konto bankowe podając swoje imię, nazwisko, datę urodzenia oraz hasło. Po stworzeniu konta, użytkownik otrzymuje unikalny identyfikator użytkownika (UID).
- **Logowanie:** Użytkownicy mogą zalogować się do swojego konta używając UID i hasła.
- **Wpłaty i wypłaty:** Po zalogowaniu, użytkownicy mogą wpłacać i wypłacać pieniądze ze swojego konta.
- **Przelewy:** Użytkownicy mogą przelewać pieniądze na inne konta, podając UID odbiorcy.
- **Historia transakcji:** Użytkownicy mogą przeglądać historię swoich transakcji.
- **Wylogowanie:** Użytkownicy mogą wylogować się ze swojego konta.

## Instrukcja Użytkowania

1. **Uruchomienie aplikacji:** Uruchom aplikację za pomocą odpowiedniego środowiska programistycznego lub terminala.
2. **Menu główne:** 
    - **Logowanie:** Wybierz opcję "Log in to account", aby zalogować się do swojego konta.
    - **Tworzenie konta:** Wybierz opcję "Create new account", aby stworzyć nowe konto.
    - **Informacje o projekcie:** Wybierz opcję "Information about project", aby wyświetlić informacje o projekcie.
    - **Wyjście:** Wybierz opcję "Exit", aby zamknąć aplikację.
3. **Logowanie:** Podaj swój UID oraz hasło, aby zalogować się do swojego konta.
4. **Menu po zalogowaniu:**
    - **Wpłata:** Wybierz opcję "Deposit money", aby wpłacić pieniądze.
    - **Wypłata:** Wybierz opcję "Withdraw money", aby wypłacić pieniądze.
    - **Przelew:** Wybierz opcję "Transfer money", aby przelać pieniądze na inne konto.
    - **Historia transakcji:** Wybierz opcję "Transaction history", aby przeglądać historię swoich transakcji.
    - **Wylogowanie:** Wybierz opcję "Log out", aby wylogować się ze swojego konta.

## Wymagania

- .NET 6 lub nowszy
- System operacyjny Windows, macOS lub Linux

## Instalacja

1. **Klonowanie repozytorium:**
    ```sh
    git clone https://github.com/alosuri/ATM.git
    cd twoje_repozytorium
    ```

2. **Budowanie projektu:**
    ```sh
    dotnet build
    ```

3. **Uruchomienie aplikacji:**
    ```sh
    dotnet run
    ```

## Struktura Projektu

- `Program.cs`: Główna klasa aplikacji zawierająca logikę biznesową i interakcje z użytkownikiem.
- `users.json`: Plik przechowujący dane użytkowników i transakcji.

## Twórcy

- **Rafał Suchorski**
- **Mikołaj Szymanowicz**
- **Oliwia Szaforz**


