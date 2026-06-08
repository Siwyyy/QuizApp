# AI Quiz Loader 🎓🤖

AI Quiz Loader to prosta, wieloplatformowa aplikacja zbudowana w technologii **.NET MAUI** (działająca natywnie na Windows, a także gotowa na iOS i Androida). Została stworzona w celu ułatwienia i przyspieszenia procesu nauki za pomocą testów oraz quizów generowanych przez sztuczną inteligencję (np. ChatGPT, Gemini, Claude).

Dzięki zoptymalizowanemu i estetycznemu UI, pozwala w wygodny sposób rozwiązywać i sprawdzać quizy, a także śledzić i archiwizować swoje postępy.

## ✨ Główne Funkcje

- **Pełna Wieloplatformowość** – natywny kod MAUI. Aplikacja z miejsca gotowa do działania na PC z Windowsem oraz na urządzeniach mobilnych.
- **Szybki Import Danych** – zamiast mozolnie ręcznie wpisywać dane, po prostu wygeneruj kod JSON w ulubionym modelu językowym AI i od razu wklej go do edytora lub załaduj z zapisanego pliku.
- **Inteligentne Rozpoznawanie Pytań** – aplikacja sama decyduje (na podstawie JSON-a), czy dane pytanie jest pytaniem jednokrotnego (Radio buttons) czy wielokrotnego (Checkboxes) wyboru.
- **System Zarządzania Quizami** – trwały zapis wgranych quizów z możliwością ich archiwizacji lub całkowitego usunięcia z pamięci lokalnej dysku.
- **Natychmiastowy Feedback i Wyjaśnienia** – od razu po odpowiedzeniu na dane pytanie, otrzymujesz czytelne podświetlenie graficzne (zieleń/czerwień) oraz, jeśli przygotowałeś je w prompcie, dedykowane wyjaśnienie logiki AI pod odpowiedzią.

## 📝 Format JSON - Jak prosić AI o testy?

Aplikacja jest przystosowana do wczytywania konkretnej struktury danych w formacie `.json`. Aby poprawnie przygotować quiz w ChatGPT lub innym modelu, wklej poniższy _Prompt_:

> Wygeneruj dla mnie quiz na temat **[TUTAJ WPISZ TEMAT]**. Będzie on używany w zewnętrznej aplikacji, więc Twoja odpowiedź MUSI składać się WYŁĄCZNIE z pliku JSON zgodnego z poniższym formatem, bez żadnego formatowania markdown (bez ```json). 
> Zadbaj o to, by niektóre pytania miały jedną poprawną odpowiedź, a inne - kilka. Używaj pola `explanation` do krótkiego uzasadnienia poprawnej odpowiedzi.
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
>       "explanation": "To jedyna dobra odpowiedź, ponieważ..."
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
