
# BlackjackMultiplayer

Un jeu de Blackjack multijoueur en console C# avec communication TCP client-serveur.

## Fonctionnalités

- Serveur gère plusieurs joueurs (2 joueurs max pour l’instant).
- Les joueurs jouent **chacun leur tour**, jusqu’à ce qu’ils "stand" ou "bust".
- Le dealer a une carte visible et une carte cachée, révélée après les tours des joueurs.
- Si un joueur atteint 21, le tour passe automatiquement au joueur suivant.
- Communication réseau entre client et serveur via TCP.
- Interface console simple côté client.
- Gestion des mains, calcul de la valeur des cartes avec prise en compte des As.
- Messages informatifs envoyés aux joueurs pour guider le jeu.

## Architecture

- **Server** : Console app C# avec gestion du jeu, du deck, des joueurs, des tours et du dealer.
- **Client** : Console app C# qui se connecte au serveur, reçoit les messages et envoie les commandes (hit/stand).

## Build

1. Build le serveur :  
```bash
dotnet build Server/Server.csproj
```

2. Build le client (dans un autre terminal) :  
```bash
dotnet build Client/Client.csproj
```
## Lancement

1. Démarrer le serveur :  
```bash
dotnet run --project Server
```

2. Démarrer chaque client (dans un autre terminal) :  
```bash
dotnet run --project Client
```

## Logique de jeu

- Le serveur attend 2 joueurs connectés.
- Distribue 2 cartes à chaque joueur et 2 cartes au dealer (une visible, une cachée).
- Chaque joueur joue à son tour :  
  - Tape "hit" pour piocher une carte.  
  - Tape "stand" pour rester.  
- Si le joueur dépasse 21, il "bust" et son tour se termine.  
- Si le joueur atteint exactement 21, un message est affiché et le tour se termine.  
- Après tous les joueurs, le dealer révèle sa carte cachée et tire jusqu’à 17+.  
- Le serveur affiche les résultats finaux.

## Améliorations possibles

- Support plus de 2 joueurs.
- Interface graphique client.
- Gestion plus avancée des erreurs réseau.
- Ajout d’un lobby ou matchmaking.
- Implémentation de la mise d’argent et d’un système de points.

