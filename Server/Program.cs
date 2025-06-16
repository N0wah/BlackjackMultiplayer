using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Player
    {
        public TcpClient Client { get; set; }
        public NetworkStream Stream => Client.GetStream();
        public string Name { get; set; }
        public List<string> Hand { get; set; } = new List<string>();
        public bool HasStood { get; set; } = false;
        public bool IsBusted { get; set; } = false;
    }

    class Program
    {
        static List<Player> players = new List<Player>();
        static int currentPlayerIndex = 0;
        static TcpListener listener;
        static List<string> deck = new List<string>();
        static List<string> dealerHand = new List<string>();

        static void Main()
        {
            Console.WriteLine("Starting server...");
            listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();

            Thread acceptThread = new Thread(AcceptClients);
            acceptThread.Start();
        }

        static void AcceptClients()
        {
            while (players.Count < 2)
            {
                TcpClient client = listener.AcceptTcpClient();
                Player player = new Player { Client = client, Name = $"Player{players.Count + 1}" };
                players.Add(player);
                Console.WriteLine($"{player.Name} connecté.");
                SendMessage(player, $"Bienvenue {player.Name}. En attente des autres joueurs...");
            }

            Broadcast("Tous les joueurs sont connectés. Le jeu commence !");
            InitializeDeck();
            DealInitialCards();
            BroadcastGameState();

            while (true)
            {
                Player currentPlayer = players[currentPlayerIndex];
                if (currentPlayer.IsBusted || currentPlayer.HasStood)
                {
                    NextTurn();
                    continue;
                }

                SendMessage(currentPlayer, "Votre tour. Tapez 'hit' pour piocher une carte ou 'stand' pour rester.");
                string action = ReadMessage(currentPlayer);
                if (action == "hit")
                {
                    string card = DrawCard();
                    currentPlayer.Hand.Add(card);
                    if (CalculateHandValue(currentPlayer.Hand) > 21)
                    {
                        currentPlayer.IsBusted = true;
                        SendMessage(currentPlayer, $"Vous avez tiré {card} et busted!");
                    }
                    else
                    {
                        SendMessage(currentPlayer, $"Vous avez tiré {card}.");
                    }
                }
                else if (action == "stand")
                {
                    currentPlayer.HasStood = true;
                    SendMessage(currentPlayer, "Vous avez choisi de rester.");
                }

                BroadcastGameState();

                if (IsGameOver())
                {
                    DealerTurn();
                    BroadcastGameState();
                    EndGame();
                    break;
                }

                NextTurn();
            }
        }

        static void InitializeDeck()
        {
            string[] suits = { "♠", "♥", "♦", "♣" };
            string[] values = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
            foreach (var suit in suits)
                foreach (var value in values)
                    deck.Add($"{value}{suit}");

            var rng = new Random();
            for (int i = 0; i < deck.Count; i++)
            {
                int j = rng.Next(i, deck.Count);
                (deck[i], deck[j]) = (deck[j], deck[i]);
            }
        }

        static void DealInitialCards()
        {
            foreach (var player in players)
            {
                player.Hand.Add(DrawCard());
                player.Hand.Add(DrawCard());
            }

            dealerHand.Add(DrawCard());
            dealerHand.Add(DrawCard());
        }

        static string DrawCard()
        {
            var card = deck[0];
            deck.RemoveAt(0);
            return card;
        }

        static int CalculateHandValue(List<string> hand)
        {
            int value = 0;
            int aceCount = 0;

            foreach (var card in hand)
            {
                string rank = card.Substring(0, card.Length - 1);
                if (int.TryParse(rank, out int num))
                    value += num;
                else if (rank == "A")
                {
                    value += 11;
                    aceCount++;
                }
                else
                    value += 10;
            }

            while (value > 21 && aceCount > 0)
            {
                value -= 10;
                aceCount--;
            }

            return value;
        }

        static void Broadcast(string message)
        {
            foreach (var player in players)
                SendMessage(player, message);
        }

        static void BroadcastGameState()
        {
            foreach (var player in players)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Game State:");
                foreach (var p in players)
                {
                    sb.AppendLine($"{p.Name}: {string.Join(", ", p.Hand)} (Value: {CalculateHandValue(p.Hand)})");
                }
                sb.AppendLine($"Dealer: {string.Join(", ", dealerHand)} (Value: {CalculateHandValue(dealerHand)})");
                SendMessage(player, sb.ToString());
            }
        }

        static void SendMessage(Player player, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message + "\n");
            player.Stream.Write(buffer, 0, buffer.Length);
        }

        static string ReadMessage(Player player)
        {
            byte[] buffer = new byte[1024];
            int byteCount = player.Stream.Read(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, byteCount).Trim().ToLower();
        }

        static void NextTurn()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        }

        static bool IsGameOver()
        {
            return players.TrueForAll(p => p.HasStood || p.IsBusted);
        }

        static void DealerTurn()
        {
            Broadcast("Dealer's turn.");
            while (CalculateHandValue(dealerHand) < 17)
            {
                string card = DrawCard();
                dealerHand.Add(card);
                Broadcast($"Dealer tire {card}.");
                Thread.Sleep(1000);
            }
        }

        static void EndGame()
        {
            int dealerValue = CalculateHandValue(dealerHand);
            bool dealerBust = dealerValue > 21;

            foreach (var p in players)
            {
                int playerValue = CalculateHandValue(p.Hand);
                if (p.IsBusted)
                {
                    SendMessage(p, "Vous avez busté. Vous perdez !");
                }
                else if (dealerBust)
                {
                    SendMessage(p, "Le dealer a busté. Vous gagnez !");
                }
                else if (playerValue > dealerValue)
                {
                    SendMessage(p, $"Vous avez gagné ! Votre {playerValue} bats celui du dealer {dealerValue}.");
                }
                else if (playerValue == dealerValue)
                {
                    SendMessage(p, $"Egalité! Votre {playerValue} est égale à celui du dealer {dealerValue}.");
                }
                else
                {
                    SendMessage(p, $"Vous perdez. Le dealer {dealerValue} bats votre {playerValue}.");
                }
            }

        }
    }
}
