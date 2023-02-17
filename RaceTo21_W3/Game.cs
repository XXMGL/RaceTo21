using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class Game
    {
        private int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        private int currentPlayer = 0; // current player on list
        public Task nextTask; // keeps track of game state
        private bool cheating = false; // lets you cheat for testing purposes if true
        private int highScore;//The highest score in this round
        private int round;//The number of round players attend.

        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();//Shuffle the deck at the beginning of each game
            nextTask = Task.GetNumberOfPlayers;
        }

        /* Adds a player to the current game
         * Called by Game object
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
            if (nextTask == Task.GetNumberOfPlayers)//task 1: get the number of players
            {
                numberOfPlayers = cardTable.GetNumberOfPlayers();
                nextTask = Task.GetNames;
            }
            else if (nextTask == Task.GetNames)//task 2: get the name of each player
            {
                for (var count = 1; count <= numberOfPlayers; count++)//keep looping until add all player to list
                {
                    var name = cardTable.GetPlayerName(count);
                    AddPlayer(name); //add player to the list with the name
                }
                foreach(var player in players)//give each player 100 point
                {
                    player.point = 100;
                }
                nextTask = Task.IntroducePlayers;
            }
            else if (nextTask == Task.IntroducePlayers)//task 3: introduce the name of each player
            {
                round = 0;//initialize the round
                cardTable.ShowPlayers(players);
                nextTask = Task.PlayerTurn;
            }
            else if (nextTask == Task.PlayerTurn)//the play turn of each player, including whether drawing crads, how many cards are willing to draw
            {
                //cardTable.ShowHands(players);
                Player player = players[currentPlayer];//set the player as current player
                if (player.status == PlayerStatus.active)//only run following lines if playerstatus is active
                {
                    Console.WriteLine("===========Player turn=============");
                    if (cardTable.OfferACard(player))//if current player require to draw cards
                    {
                        int Num = cardTable.OfferNumber(player);//the number of cards the player willing to draw
                        for(int i=0; i<Num; i++)
                        {
                            Card card = deck.DealTopCard();//get the last card in the dec
                            player.cards.Add(card);//add that card to player's hand
                        }
                        player.score = ScoreHand(player);//get the player's score
                        if (player.score > 21)//compare the score and decide the status of current player
                        {
                            player.status = PlayerStatus.bust;
                        }
                        else if (player.score == 21)
                        {
                            player.status = PlayerStatus.win;
                        }
                    }
                    else//if current player do not want to draw cards
                    {
                        player.status = PlayerStatus.stay;
                    }
                }
                cardTable.ShowHand(player);//show the player's hand
                nextTask = Task.CheckForEnd;
            }
            else if (nextTask == Task.CheckForEnd)//task4: check the score and status of each player to decide whether their is a winner or someone is busted
            {
                if (!CheckActivePlayers())//check whether there is any player do not draw cards but in active status
                {
                    Console.WriteLine("=============Result============");
                    round++;//round +1 and enter the next round
                    Player winner = DoFinalScoring();//calculate the scores inn different condition
                    cardTable.AnnounceWinner(winner);//Announce the winner of this turn
                    nextTask = Task.NextRound;
                }
                else
                {
                    currentPlayer++;//Go to the next player
                    if (currentPlayer > players.Count - 1)
                    {
                        currentPlayer = 0; // back to the first player
                    }
                    nextTask = Task.PlayerTurn;//Go back to play turn
                }
            }
            else if(nextTask == Task.NextRound)//task 5: go to the next round and do some intialize works like shuffling players or showing player' points
            {
                clearScore();//Set all players' score to 0
                foreach(var player in players)//remove all players' cards
                {
                    int Num = player.cards.Count;
                    for (int i = 0;i< Num; i++)
                        {
                            player.cards.RemoveAt(0);
                        }
                }
                if (!CheckWinner())//Check whther there is someone meet the condition to win this game.
                {
                    AskPlayer();//Ask for whether keep playing
                    if (players.Count > 1)
                    {
                        ShufflePlayer();//shuffle the sequence of players
                        deck.Shuffle();//shuffle the cards deck
                        nextTask = Task.PlayerTurn;//go to the play turn
                    }
                    else if (players.Count == 1)//End game if only one player left
                    {
                        cardTable.AnnounceWinner(null);
                        nextTask = Task.GameOver;
                    }
                    else//All players are leave, end this game
                    {
                        Console.WriteLine("No one left");
                        nextTask = Task.GameOver;
                    }
                }
                else
                {
                    Player Winer = FindWinner();//Find the player with the highest score
                    cardTable.AnnounceFinalWinner(Winer);
                    nextTask = Task.GameOver;
                }
                
            }
            else // we shouldn't get here
            {
                Console.WriteLine("I'm sorry, I don't know what to do now!");
                nextTask = Task.GameOver;
            }
        }

        /* calculate the scores of the player.
         * Is called by DoNextTask()
         * player object provide list of players
         * Calls name, status in each player
         * return an integer represent the score of the player
         */
        public int ScoreHand(Player player)//return the score each player have
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active)//for cheating mode
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
                foreach (Card card in player.cards)//convert face card into number and add the face value to the score
                {
                    string cd = card.id;
                    string faceValue = cd.Remove(cd.Length - 2);//
                    switch (faceValue)
                    {
                        case "K":
                        case "Q":
                        case "J":
                            score = score + 10;//J, Q, K all worth 10
                            break;
                        case "A":
                            score = score + 1;
                            break;
                        default:
                            score = score + int.Parse(faceValue);//convert string to integer
                            break;
                    }
                }
            }
            return score;
        }

        /*check whether there is any player do not draw cards but in active status
       * Is called by DoNextTask()
       * player object provide list of players
       * Calls name, status in each player
       * return an booliean to decide whether there is active player
       */
        public bool CheckActivePlayers()
        {
            int remainningPlayer = players.Count;//use this variable to detect how many players are not busted
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
            if (remainningPlayer == 1)//Only one player is not busted, end this turn and annouce winner
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

        /*Do the scoring of this round and find the winner of this round
      * Is called by DoNextTask()
      * player object provide list of players
      * Calls name, status in each player
      * return the player who hits 21 or has the highest score in this round
      */
        public Player DoFinalScoring()
        {
            highScore = 0;//initialize the high score
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
                        player.point -= 21;//minus 21 points to who is busted
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

        /*Set all pllayers scores to 0
     * Is called by DoNextTask()
     * player object provide list of players
     * Calls name in each player
     */
        public void clearScore()
        {
            foreach(var player in players)
            {
                player.score = 0;
            }
        }

        /*Ask the player whether keep playing the game
     * Is called by DoNextTask()
     * player object provide list of players
     * Calls name, status, point in each player
     * return an booliean to decide whether there is active player
     */
        public void AskPlayer()//ask player for whether keep playing
        {
            deck = new Deck();//Create a new deck for a new round
            foreach (var player in players)
            {
                
                player.score = 0;//initialize players score
                if (player.status != PlayerStatus.quit)//only ask the player if the player is not quit
                {
                    player.ShowPoint();//Show the point this player have
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
                    if (!cardTable.CountinuePlay(player))// true when the player is quit
                    {
                        player.status = PlayerStatus.quit;//set the player status to quit
                        //players.Remove(player);
                    }
                }
                
            }
        }

        /*shuffle the players positions
     * Is called by DoNextTask()
     * player object provide list of players
     * Calls count in each player
     */
        public void ShufflePlayer()
        {
            Console.WriteLine("Shuffling Players Positions...");

            Random ran = new Random();//Create a random number
            for (int i = 0; i < players.Count; i++)
            {
                //Switch the positons of two player
                Player p1 = players[i];
                int swapindex = ran.Next(players.Count);
                players[i] = players[swapindex];
                players[swapindex] = p1;
            }
        }

        /*check whether the game has played 3 round or someone already earn enough points
     * Is called by DoNextTask()
     * player object provide list of players
     * Calls point in each player
     * return an booliean to decide whether the game should end
     */
        public bool CheckWinner()
        {
            if (round >= 3)
            {
                return true;
            }
            foreach (var player in players)
            {
                if (player.point >= 135)
                {
                    return true;
                }
            }
            return false;
        }

        /*Find the player with the highest score
     * Is called by DoNextTask()
     * player object provide list of players
     * Calls point in each player
     * return the player who has the highest score
     */
        public Player FindWinner()
        {
            int highpoint = 0;
            foreach (var player in players)//Make comparations between players and find out the player with highest score
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
