
namespace RaceTo21
{
	public enum PlayerStatus
	{
		active,//Player are available to draw cards or stay
		stay,//the player cannot do anything untill the next round
		bust,//the player cannot do anything untill the next round
		win,//the player win this round
		quit,//the player quit from this game
		end//the player is the final winner of this game
	}
}

