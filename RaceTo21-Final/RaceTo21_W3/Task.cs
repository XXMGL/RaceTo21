

namespace RaceTo21
{
    /* The stages dueing the gameplay
    * Is called by Game object.
    */
    public enum Task
    {
        GetNumberOfPlayers,//the stage ask for the number of the players
        GetNames,//ask for name of each player
        IntroducePlayers,//introduce the name of each player
        PlayerTurn,//the play turn of each player, including whether drawing crads, how many cards are willing to draw
        CheckForEnd,//check the score and status of each player to decide whether their is a winner or someone is busted
        NextRound,//go to the next round and do some intialization works like shuffling players or showing player' points
        GameOver//End
    }
}
