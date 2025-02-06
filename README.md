# Inštalácia CodeHub Projektu

## Požiadavky
- .NET 7.0 (alebo vyššia)
- SQL Server (lokálne)
- Visual Studio alebo iný IDE podporujúci .NET (napr. VSCode)

## Inštalácia

### 1. Naklonujte repozitár

Naklonujte tento repozitár do svojho lokálneho prostredia:

```bash
git clone https://github.com/vas-uzivatel/codehub.git
cd codehub
```

### 2. Nastavte SQL Server databázu

Vytvorte databázu:

Spustite tento SQL skript na vytvorenie databázy:

```sql
CREATE DATABASE CodeHubDB;
```

Nastavte pripojenie v `appsettings.json`:

Skontrolujte, či máte správny pripojovací reťazec v súbore `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.\\sqlexpress;Initial Catalog=CodeHubDB;Integrated Security=True;Trust Server Certificate=True"
  }
}
```

### 3. Vytvorenie databázovej schémy

Spustite migrácie na vytvorenie tabuliek:

```bash
dotnet ef database update
```

### 4. Spustenie aplikácie

Pre spustenie aplikácie použite príkaz:

```bash
dotnet run
```

Aplikácia bude dostupná na: [https://localhost:5001](https://localhost:5001).

