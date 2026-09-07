# BDBlio - Gestionnaire de Bibliothèque de Bandes Dessinées

Application C# moderne pour gérer votre bibliothèque de Bandes Dessinées avec interface 3D, scan de codes-barres, et intégration Amazon France.

## Fonctionnalités

### 🏠 Écran d'Accueil
- Interface avec onglets modernes et effets 3D
- Navigation fluide entre les sections

### 📱 Ajouter une BD
- Scan de code-barres (EAN/ISBN)
- Recherche par titre, auteur ou collection
- Récupération automatique des données depuis Amazon France
- Affichage de la couverture et des détails

### 🔍 Rechercher une BD
- Scan rapide de code-barres
- Recherche textuelle (titre, auteur, collection)
- Vérification dans la base de données locale
- Affichage du statut (présente/non présente)

### 📚 Ma Bibliothèque
- Affichage de toutes les BD
- Tri par :
  - Titre
  - Auteur
  - Collection
- Filtrage avancé

### ⚙️ Paramètres
- **Export** : CSV ou Excel
- **Import** : CSV ou Excel
- **Mappage de colonnes** : Configuration flexible des données à importer
- Aucune colonne n'est obligatoire

## Architecture

```
BDBlio/
├── BDBlio.sln
├── src/
│   ├── BDBlio.Desktop/          # Interface WPF ultramoderne avec effets 3D
│   ├── BDBlio.Core/             # Modèles de données
│   ├── BDBlio.Data/             # Accès aux données (Entity Framework)
│   └── BDBlio.Services/         # Services métier (Amazon, Scan, Import/Export)
└── README.md
```

## Technologies

- **Langage** : C# 12
- **Framework UI** : WPF avec ModernWpf
- **Base de données** : SQLite avec Entity Framework Core
- **APIs externes** : Amazon France
- **Scan** : ZXing.NET
- **Import/Export** : CsvHelper, EPPlus (Excel)
- **MVVM** : Community Toolkit MVVM

## Installation

1. Cloner le repository
2. Ouvrir `BDBlio.sln` dans Visual Studio
3. Restaurer les packages NuGet
4. Compiler la solution
5. Lancer `BDBlio.Desktop`

## Configuration

### Clé API Amazon
Vous devez configurer vos identifiants Amazon dans les paramètres :
- API Key
- API Secret
- Region (France)

## Utilisation

### Premier lancement
1. Configuration dans Paramètres
2. Ajouter des BD via scan ou recherche
3. Consulter votre bibliothèque

### Importer une bibliothèque existante
1. Aller dans Paramètres
2. Cliquer sur "Importer CSV/Excel"
3. Sélectionner le fichier
4. Mapper les colonnes (optionnel)
5. Valider l'importation

## Licence

MIT
