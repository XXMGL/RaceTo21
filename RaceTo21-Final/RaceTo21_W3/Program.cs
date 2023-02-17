using System;

namespace RaceTo21
{
    class Program
    {
        static void Main(string[] args)
        {
            CardTable cardTable = new CardTable();
            Game game = new Game(cardTable);
            while (game.nextTask != Task.GameOver)//run the game.DoNextTask function until the task go to GameOver
            {
                game.DoNextTask();
            }
        }
    }
}

