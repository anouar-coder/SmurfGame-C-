# SmurfGame-C-

Jeu **Smurf** en C# — application de bureau **WPF** (.NET 10), architecture en couches
(BL / DAL / UI) suivant le pattern **MVVM**.

## Architecture

| Projet | Rôle |
| --- | --- |
| `SmurfBL` | Logique métier — entités, interfaces, services |
| `SmurfDAL` | Accès aux données — Entity Framework Core, repositories, migrations |
| `SmurfUI` | Interface WPF — Views, ViewModels, Styles, Converters |

Solution : `Smurf.slnx`

## Modèle métier

Entités du jeu (`SmurfBL/Entities`) :

- **Joueur / créatures** — `Smurf`, `Creature`
- **Ennemis** — `Bug`, `Spider`, `BzzFly`
- **Objets** — `Berry`, `Item`, `RedPotion`, `BluePotion`, `Sarsaparilla`
- **Monde** — `Forest`

Le moteur de jeu est centralisé dans `SmurfBL/Services/GameEngine.cs`.

## Prérequis

- Windows 10/11
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Lancer le projet

```bash
dotnet restore
dotnet build
dotnet run --project SmurfUI
```

> Cible : `net10.0-windows` avec `UseWPF` — projet Windows uniquement.

## Auteur

Anwar Ben Brahim — [GitHub](https://github.com/anouar-coder)
