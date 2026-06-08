# QuizApp 🎓🤖

QuizApp to prosta, wieloplatformowa aplikacja zbudowana w technologii **.NET MAUI** (działająca natywnie na Windows, a także gotowa na iOS i Androida). Została stworzona w celu ułatwienia i przyspieszenia procesu nauki za pomocą testów oraz quizów generowanych przez sztuczną inteligencję (np. ChatGPT, Gemini, Claude) lub tworzonych na bazie własnych materiałów.

Dzięki zoptymalizowanemu i estetycznemu UI, pozwala w wygodny sposób rozwiązywać i sprawdzać quizy, a także śledzić i archiwizować swoje postępy.

## ✨ Główne Funkcje

- **Pełna Wieloplatformowość** – natywny kod MAUI. Aplikacja z miejsca gotowa do działania na PC z Windowsem oraz na urządzeniach mobilnych.
- **Szybki Import Danych** – zamiast mozolnie ręcznie wpisywać dane, po prostu wygeneruj kod JSON w ulubionym modelu językowym AI i od razu wklej go do edytora lub załaduj z zapisanego pliku.
- **Inteligentne Rozpoznawanie Pytań** – aplikacja sama decyduje (na podstawie JSON-a), czy dane pytanie jest pytaniem jednokrotnego (Radio buttons) czy wielokrotnego (Checkboxes) wyboru.
- **System Zarządzania Quizami** – trwały zapis wgranych quizów z możliwością ich archiwizacji. Aktywne i zarchiwizowane quizy są podzielone na dwie oddzielne listy, z czego lista archiwum jest domyślnie zwinięta dla zachowania czystego widoku.
- **Natychmiastowy Feedback i Wyjaśnienia** – od razu po odpowiedzeniu na dane pytanie, otrzymujesz czytelne podświetlenie graficzne (zieleń/czerwień) oraz, jeśli opcjonalnie przygotowałeś je w prompcie, dedykowane wyjaśnienie ukazywane pod przyciskiem odpowiedzi.

## 📝 Format JSON - Jak prosić AI o testy?

Aplikacja jest przystosowana do wczytywania konkretnej struktury danych w formacie `.json`. Wewnątrz aplikacji (w menu głównym) znajduje się przycisk pozwalający na szybkie skopiowanie gotowego polecenia do schowka:

> Pamiętaj, aby Twoja odpowiedź składała się WYŁĄCZNIE z kodu w pliku w formacie JSON zgodnego z poniższym formatem (bez bloków markdown np. ```json).
> Każde pytanie ma dowolną ilość odpowiedzi, dowolną ilość poprawnych odpowiedzi, oraz może, ale nie musi zawierać wyjaśnienia (explanation).
> Jeśli quiz generowany jest z pytań podanych przez użytkownika z pliku, masz NIE modyfikować pytań ani odpowiedzi, a pytania, które nie są a,b,c,d po prostu pominąć.
> Jeśli wśród pytań są takie, które nie mają zaznaczonej poprawnej odpowiedzi, pomiń je.
> Na koniec wypisz użytkownikowi pytania, które pominąłeś.
> 
> WZÓR:
> {
>   "title": "Nazwa Quizu",
>   "questions": [
>     {
>       "text": "Treść pytania",
>       "options": [
>         { "text": "Opcja 1", "isCorrect": false },
>         { "text": "Opcja 2", "isCorrect": true }
>       ],
>       "explanation": "Krótkie wyjaśnienie poprawnej odpowiedzi."
>     }
>   ]
> }

## 🚀 Jak uruchomić projekt na Windows?
### Wymagania
* Środowisko **.NET 9.0 SDK** z zainstalowanym workloadem `maui` (`dotnet workload install maui`).

### Uruchomienie z terminala
Sklonuj to repozytorium i użyj narzędzia `dotnet build` aby zbudować i otworzyć aplikację.
```bash
git clone https://github.com/TwojProfil/QuizApp.git
cd QuizApp
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

### Uruchomienie z Visual Studio 2022
1. Otwórz plik `QuizApp.csproj`.
2. Z menu wyboru platformy wybierz "Windows Machine".
3. Kliknij w zielony trójkąt, aby uruchomić (lub F5).

## 🛠️ Technologia
Aplikacja została oparta na środowisku **.NET MAUI** używającym języka `C#` oraz `XAML` na warstwę frontendową (UI). Za zapisywanie stanu odpowiada parser `System.Text.Json` i wbudowane mechanizmy przechowywania danych `FileSystem.AppDataDirectory`.
