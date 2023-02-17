using System;
using System.Collections.Generic;
using System.Linq; // for using alternate shuffle method

namespace RaceTo21
{
    public class Deck
    {
        List<Card> cards = new List<Card>();
        /*Building a deck
        * Is called by Game object.
        */
        public Deck()
        {
            Console.WriteLine("*********** Building deck...");
            string[] suits = { "Spades", "Hearts", "Clubs", "Diamonds" };//create an array that contain the name of each face card

            for (int cardVal = 1; cardVal <= 13; cardVal++)//keep looping until all card value has been included
            {
                foreach (string cardSuit in suits)//keep looping until all card value including J,Q,K,A are included
                {
                    string cardName;
                    string cardLongName;
                    switch (cardVal)//change the number into the card's name
                    {
                        case 1:
                            cardName = "A";
                            cardLongName = "Ace";
                            break;
                        case 11:
                            cardName = "J";
                            cardLongName = "Jack";
                            break;
                        case 12:
                            cardName = "Q";
                            cardLongName = "Queen";
                            break;
                        case 13:
                            cardName = "K";
                            cardLongName = "King";
                            break;
                        default:
                            cardName = cardVal.ToString();//convert the integer to string
                            cardLongName = cardName;
                            break;
                    }
                    cards.Add(new Card(cardName +" "+cardSuit.First<char>(), cardLongName + " of "+cardSuit));//Add each card to the list
                }
            }
        }

        /* Shuhffling the deck
       * Is called by Game object.
       */
        public void Shuffle()
        {
            Console.WriteLine("Shuffling Cards...");

            Random rng = new Random();//creating a random number
            for (int i=0; i<cards.Count; i++)
            {
                //change the position of randomly two cards
                Card tmp = cards[i];
                int swapindex = rng.Next(cards.Count);
                cards[i] = cards[swapindex];
                cards[swapindex] = tmp;
            }
        }

        /* get the last card in the deck and remove it from the deck
       * Is called by Game object.
       * card object provides list of players.
       */
        public Card DealTopCard()
        {
            Card card = cards[cards.Count - 1];//get the last card in the dec
            cards.RemoveAt(cards.Count - 1);//remove the card
            return card;
        }
    }
}

