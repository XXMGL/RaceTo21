using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class CardTable
    {
        public CardTable()
        {
            Console.WriteLine("Setting Up Table...");
        }

        /* Shows the name of each player and introduces them by table position.
         * Is called by Game object.
         * Game object provides list of players.
         * Calls Introduce method on each player object.
         */
        public void ShowPlayers(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Introduce(i+1); // List is 0-indexed but palyer number start from 1
                players[i].ShowPoint();// Show players points
            }
        }

        /* Gets the user input for number of players.
         * Is called by Game object.
         * Returns number of players to Game object.
         */
        public int GetNumberOfPlayers()
        {
            Console.Write("How many players? ");
            string response = Console.ReadLine();//ask for input
            int numberOfPlayers;
            while (int.TryParse(response, out numberOfPlayers) == false
                || numberOfPlayers < 2 || numberOfPlayers > 8)//keep looping if the users input is not a number or is not in the range of 3 to 7
            {
                Console.WriteLine("Invalid number of players.");
                Console.Write("How many players?");
                response = Console.ReadLine();// input again
            }
            return numberOfPlayers;
        }

        /* Gets the name of a player
         * Is called by Game object
         * Game object provides player number
         * Returns name of a player to Game object
         */
        public string GetPlayerName(int playerNum)
        {
            Console.Write("What is the name of player# " + playerNum + "? ");
            string response = Console.ReadLine();//ask for input
            while (response.Length < 1)//can ont input nothing
            {
                Console.WriteLine("Invalid name.");
                Console.Write("What is the name of player# " + playerNum + "? ");
                response = Console.ReadLine();//input again
            }
            return response;
        }

        /* Ask player whether they want to draw cards. If yes, ask them how many cards they want to draw. Otherwise, the player will become stay
        * Is called by Game object.
        * Game object provides list of players.
        * Calls name in each player
        */
        public bool OfferACard(Player player)
        {
            while (true)//keep looping until return 
            {
                Console.Write(player.name + ", do you want a card? (Y/N)");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    return true;// return true when player input string start with y
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    return false;//return false when player input string start with n
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");// do not return if player input a invalid string
                }
            }
        }

        /* ask player how many cards they want
        * Is called by Game object.
        * Game object provides list of players.
        * Calls name in each player
        */
        public int OfferNumber(Player player)
        {
            while (true)//keep looping until gain a valid number
            {
                Console.Write(player.name + ", how many cards do you want? (1 to 3)");
                string Num = Console.ReadLine();
                switch (Num)//return different integer in different conditons
                {
                    case "1":
                        return 1;
                    case "2":
                        return 2;
                    case "3":
                        return 3;
                    default:
                        Console.Write("Please input a valid number");//ask again if player input a invalid number
                        break;
                }
            }
        }
        /* Shows the cards the player have.
        * Is called by Game object and cardTable object.
        * Game object provides list of players. Card object provides list of cards
        * Calls cards in each player
        */
        public void ShowHand(Player player)
        {
            if (player.cards.Count > 0)//only run this method if the player have cards
            {
                Console.Write(player.name + " has: ");
                foreach (Card card in player.cards)// print all cards in player's hand
                {
                    Console.Write(card.displayName + "  ");
                }
                Console.Write("Score: " + player.score + "/21 ");//print player's score
                if (player.status != PlayerStatus.active)
                {
                    Console.Write("(" + player.status.ToString().ToUpper() + ")");//print out the player status if the player are not in active status
                }
                Console.WriteLine();//go to next line
            }
        }

        /* Shows the cards in player's hands
        * Is called by Game object.
        * Game object provides list of players.
        */
        public void ShowHands(List<Player> players)
        {
            foreach (Player player in players)///show all payer hands
            {
                ShowHand(player);
            }
        }

        /* Announce the winner of this round
        * Is called by Game object.
        * Game object provides list of players.
        * Calls name, score, point in each player
        */
        public void AnnounceWinner(Player player)
        {
          
            if (player != null&& player.score != 0)//print the winner and add scores to his points
            {
                Console.WriteLine(player.name + " wins!");
                player.point += player.score;
            }
            else if(player.score == 0)//if the winner's score is 0, means everyone stay at the first round. No one win in that condition
            {
                Console.WriteLine("No one win");
            }
            else//if the input is null, means every one is busted
            {
                Console.WriteLine("Everyone busted!");
            }
            Console.Write("Press <Enter> to start next round ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }

        /* Announce the final winner of this game(the highest points after 3 rounds)
         * Is called by Game object.
         * Game object provides list of players.
         * Calls name in each player.
         */
        public void AnnounceFinalWinner(Player player)
        {
            Console.WriteLine(player.name+" win this game!");
        }


        /* return true when player want to keep playing
        * Is called by Game object.
        * Game object provides list of players.
        * Calls name in each player.
        */
        public bool CountinuePlay(Player player)
        {
            while (true)//keep looping until return
            {
                Console.Write(player.name + ", do you want to continue? (Y/N)");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    return true;
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    return false;
                }
                else//require for an appropriate input
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }

        }
       
    }
}