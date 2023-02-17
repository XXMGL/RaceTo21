

namespace RaceTo21
{
    public class Card
    {
        public string id;//Shot name of the card
        public string displayName;//full name of the card
        public Card(string shortCardName,string longCardName)
        {
            id = shortCardName;
            displayName = longCardName;
        }
    }
}
