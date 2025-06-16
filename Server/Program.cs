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
            BroadcastGameState(hideDealerSecondCard: true);

            // Chaque joueur joue entièrement son tour à tour
            foreach (var player in players)
            {
                SendMessage(player, "C'est votre tour.");
                while (!player.IsBusted && !player.HasStood)
                {
                    BroadcastGameState(hideDealerSecondCard: true);

                    int handValue = CalculateHandValue(player.Hand);
                    if (handValue == 21)
                    {
                        SendMessage(player, "Blackjack ! Vous avez atteint 21, votre tour est terminé.");
                        player.HasStood = true;
                        break; // Fin du tour pour ce joueur
                    }

                    SendMessage(player, "Tapez 'hit' pour piocher une carte ou 'stand' pour rester.");
                    string action = ReadMessage(player);

                    if (action == "hit")
                    {
                        string card = DrawCard();
                        player.Hand.Add(card);
                        handValue = CalculateHandValue(player.Hand);
                        if (handValue > 21)
                        {
                            player.IsBusted = true;
                            SendMessage(player, $"Vous avez tiré {card} et busté !");
                        }
                        else if (handValue == 21)
                        {
                            SendMessage(player, $"Vous avez tiré {card} et atteint 21 ! Votre tour est terminé.");
                            player.HasStood = true;
                        }
                        else
                        {
                            SendMessage(player, $"Vous avez tiré {card}.");
                        }
                    }
                    else if (action == "stand")
                    {
                        player.HasStood = true;
                        SendMessage(player, "Vous avez choisi de rester.");
                    }
                }
            }

            // Tous les joueurs ont fini => tour du dealer
            Broadcast("Tous les joueurs ont fini leur tour. Tour du dealer !");
            BroadcastGameState(hideDealerSecondCard: false);
            DealerTurn();
            BroadcastGameState(hideDealerSecondCard: false);
            EndGame();
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

        static void BroadcastGameState(bool hideDealerSecondCard)
        {
            foreach (var player in players)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Game State:");

                foreach (var p in players)
                {
                    sb.AppendLine($"{p.Name}: {string.Join(", ", p.Hand)} (Value: {CalculateHandValue(p.Hand)})");
                }

                if (hideDealerSecondCard && dealerHand.Count >= 2)
                {
                    sb.AppendLine($"Dealer: {dealerHand[0]}, XX");
                }
                else
                {
                    sb.AppendLine($"Dealer: {string.Join(", ", dealerHand)} (Value: {CalculateHandValue(dealerHand)})");
                }

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

        static void DealerTurn()
        {
            Broadcast("Dealer's turn.\n");
            while (CalculateHandValue(dealerHand) < 17)
            {
                string card = DrawCard();
                dealerHand.Add(card);
                Broadcast($"Dealer tire {card}.\n");
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
                    SendMessage(p, $"Vous avez gagné ! Votre {playerValue} bat celui du dealer {dealerValue}.");
                }
                else if (playerValue == dealerValue)
                {
                    SendMessage(p, $"Égalité ! Votre {playerValue} est égale à celui du dealer {dealerValue}.");
                }
                else
                {
                    SendMessage(p, $"Vous perdez. Le dealer {dealerValue} bat votre {playerValue}.");
                }
            }
        }
    }
}
