# Projet Blackjack Multijoueur en C# avec Sockets

## Description

Ce projet est un jeu de **Blackjack multijoueur** développé en C# en mode client-serveur.  
Il utilise la programmation socket pour permettre à plusieurs joueurs de se connecter simultanément, jouer et interagir en temps réel.

---

## Fonctionnalités

- Serveur centralisé qui gère la logique du jeu et la synchronisation des joueurs
- Plusieurs clients pouvant se connecter et jouer en même temps
- Gestion des connexions/déconnexions des joueurs
- Distribution des cartes, gestion des tours, et calcul des résultats (victoire, défaite, égalité)
- Communication réseau via TCP sockets
- Synchronisation en temps réel des états de jeu entre clients et serveur
- Affichage console simple côté client

---

## Architecture

- **Serveur** :  
  - Ecoute les connexions entrantes  
  - Gère les sessions de jeu et les règles  
  - Envoie les états du jeu à chaque client  
  - Reçoit les actions des joueurs (tirer une carte, rester, etc.)

- **Client** :  
  - Interface console pour interagir avec le joueur  
  - Envoie les actions au serveur  
  - Affiche les cartes, les scores et les résultats reçus du serveur

---

## Technologies utilisées

- Langage : C# (.NET 6+ recommandé)
- Réseau : Sockets TCP (System.Net.Sockets)
- Environnement de développement : Visual Studio ou VS Code
- Plateforme : Windows, Linux, macOS (dotnet core compatible)

---

## Installation & Exécution

### Prérequis

- .NET SDK 6 ou supérieur installé : https://dotnet.microsoft.com/download

### Compilation

- Ouvrir le projet serveur et client dans Visual Studio ou en ligne de commande
- Pour compiler via CLI :

```bash
cd Serveur
dotnet new console
cd ../Client
dotnet new console
```
Oubliez pas de re-modifier le fichier Program.cs qui est généré automatiquement 

- Ensuite vous pouvez lancez un serveur et deux client
```bash
cd Serveur
dotnet run
cd Client
dotnet run
```
