using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class Game
    {
        int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        public string nextTask; // keeps track of game state
        private bool cheating = false; // lets you cheat for testing purposes if true
        public int highScore;//The highest score in this round
        public int round;//The number of round players attend.

        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();
            deck.ShowAllCards();
            nextTask = "GetNumberOfPlayers";
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        public void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

        /* Figures out what task to do next in game
         * as represented by field nextTask
         * Calls methods required to complete task
         * then sets nextTask.
         */
        public void DoNextTask()
        {
            Console.WriteLine("================================"); // this line should be elsewhere right?
            if (nextTask == "GetNumberOfPlayers")
            {
                numberOfPlayers = cardTable.GetNumberOfPlayers();
                nextTask = "GetNames";
            }
            else if (nextTask == "GetNames")
            {
                for (var count = 1; count <= numberOfPlayers; count++)
                {
                    var name = cardTable.GetPlayerName(count);
                    AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
                }
                foreach(var player in players)
                {
                    player.point = 100;
                }
                nextTask = "IntroducePlayers";
            }
            else if (nextTask == "IntroducePlayers")
            {
                round = 0;
                cardTable.ShowPlayers(players);
                nextTask = "PlayerTurn";
            }
            else if (nextTask == "PlayerTurn")
            {
                cardTable.ShowHands(players);
                Player player = players[currentPlayer];
                if (player.status == PlayerStatus.active)
                {
                    if (cardTable.OfferACard(player))
                    {
                        int Num = cardTable.OfferNumber(player);
                        for(int i=0; i<Num; i++)
                        {
                            Card card = deck.DealTopCard();
                            player.cards.Add(card);
                        }
                        player.score = ScoreHand(player);
                        if (player.score > 21)
                        {
                            player.status = PlayerStatus.bust;
                        }
                        else if (player.score == 21)
                        {
                            player.status = PlayerStatus.win;
                        }
                    }
                    else
                    {
                        player.status = PlayerStatus.stay;
                    }
                }
                cardTable.ShowHand(player);
                nextTask = "CheckForEnd";
            }
            else if (nextTask == "CheckForEnd")
            {
                if (!CheckActivePlayers())
                {
                    round++;//round +1 and enter the next round
                    Player winner = DoFinalScoring();//calculate the scores inn different condition
                    cardTable.AnnounceWinner(winner);
                    nextTask = "NextRound";
                }
                else
                {
                    currentPlayer++;//Go to the next player
                    if (currentPlayer > players.Count - 1)
                    {
                        currentPlayer = 0; // back to the first player...
                    }
                    nextTask = "PlayerTurn";//Go back to play turn
                }
            }
            else if(nextTask == "NextRound")
            {
                clearScore();
                foreach(var player in players)
                {
                    int Num = player.cards.Count;
                    for (int i = 0;i< Num; i++)
                        {
                            player.cards.RemoveAt(0);
                        }
                }
                if (!CheckWinner())
                {
                    AskPlayer();//Ask for whether keep playing
                    if (players.Count > 1)
                    {
                        ShufflePlayer();//shuffle the sequence of players
                        deck.Shuffle();//shuffle the cards deck
                        nextTask = "PlayerTurn";//go to the play turn
                    }
                    else if (players.Count == 1)//End game if only one player left
                    {
                        cardTable.AnnounceWinner(null);
                        nextTask = "GameOver";
                    }
                    else//All players are leave, end this game
                    {
                        Console.WriteLine("No one left");
                        nextTask = "GameOver";
                    }
                }
                else
                {
                    Player Winer = FindWinner();//Find the player with the highest score
                    cardTable.AnnounceFinalWinner(Winer);
                    nextTask = "GameOver";
                }
                
            }
            else // we shouldn't get here...
            {
                Console.WriteLine("I'm sorry, I don't know what to do now!");
                nextTask = "GameOver";
            }
        }

        public int ScoreHand(Player player)//return the score each player have
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active)
            {
                string response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.name + "'s score be?");
                    response = Console.ReadLine();
                }
                return score;
            }
            else
            {
                foreach (Card card in player.cards)
                {
                    string cd = card.id;
                    string faceValue = cd.Remove(cd.Length - 1);
                    switch (faceValue)
                    {
                        case "K":
                        case "Q":
                        case "J":
                            score = score + 10;
                            break;
                        case "A":
                            score = score + 1;
                            break;
                        default:
                            score = score + int.Parse(faceValue);
                            break;
                    }
                }
            }
            return score;
        }

        public bool CheckActivePlayers()
        {
            int remainningPlayer = players.Count;
            foreach (var player in players)
            {
                switch (player.status)
                {
                    case PlayerStatus.win:
                        return false;

                    case PlayerStatus.bust:
                        remainningPlayer--;
                        break;
                }
            }
            if (remainningPlayer == 1)
            {
                return false;
            }
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.active)
                {
                    return true; // at least one player is still going!
                }            
            }
            return false; // everyone has stayed or busted, or someone won!
        }

        public Player DoFinalScoring()
        {
            highScore = 0;
            foreach (var player in players)
            {
                cardTable.ShowHand(player);
                switch (player.status)
                {
                    case PlayerStatus.win:
                        break;
                    case PlayerStatus.stay:
                        break;
                    case PlayerStatus.bust:
                        player.point -= 21;
                        break;
                }
                if (player.status == PlayerStatus.win) // someone hit 21
                {
                    return player;
                }
                if (player.status == PlayerStatus.stay || player.status == PlayerStatus.active) // still could win...
                {
                    if (player.score > highScore)
                    {
                        highScore = player.score;
                    }
                }            
                // if busted don't bother checking!
            }
            if (highScore >= 0) // someone scored, anyway!
            {
                // find the FIRST player in list who meets win condition
                return players.Find(player => player.score == highScore);
            }
           
            return null; // everyone must have busted because nobody won!
        }

        public void clearScore()
        {
            foreach(var player in players)
            {
                player.score = 0;
            }
        }
     

        public void AskPlayer()//ask player for whether keep playing
        {
            deck = new Deck();//Create a new deck for a new round
            foreach (var player in players)
            {
                player.ShowPoint();//Show the point this player have
                player.score = 0;//initialize players score
                player.status = PlayerStatus.active;
                if (player.point <= 0)
                {
                    player.status = PlayerStatus.quit;//Set the status of this player to quit
                    Console.WriteLine(player.name + "out");
                }
                else if (player.point >= 200)
                {
                    player.status = PlayerStatus.end;//Set the status of this player to be the winner
                    Console.WriteLine(player.name + "win this game!");
                }
                if (!cardTable.CountinuePlay(player))
                {
                    player.status = PlayerStatus.quit;
                    //players.Remove(player);
                }
              
                
            }
        }
        public void ShufflePlayer()
        {
            Console.WriteLine("Shuffling Players...");

            Random ran = new Random();

            // one-line method that uses Linq:
            // cards = cards.OrderBy(a => rng.Next()).ToList();

            // multi-line method that uses Array notation on a list!
            // (this should be easier to understand)
            for (int i = 0; i < players.Count; i++)
            {
                Player p1 = players[i];
                int swapindex = ran.Next(players.Count);
                players[i] = players[swapindex];
                players[swapindex] = p1;
            }
        }
        public bool CheckWinner()
        {
            if (round >= 3)
            {
                return true;
            }
            foreach (var player in players)
            {
                if (player.point >= 200)
                {
                    return true;
                }
            }
            return false;
        }
        public Player FindWinner()
        {
            int highpoint = 0;
            foreach (var player in players)
            {
                if (player.point > highpoint)
                {
                    highpoint = player.point;
                }
            }
            return players.Find(player => player.point == highpoint);
        }

    }
}
