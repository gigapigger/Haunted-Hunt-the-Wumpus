using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus
{
	class Player
	{
		// Ints for storing gold, ammo, turns
		private int gold, ammo, turns;
		private string playerName;
        public Player(string playerName)
        {
            // Starts out with no turns, no gold, and three ammo, and sets name for this game
            gold = 0;
            ammo = 3;
            turns = 0;
            this.playerName = playerName;
        }

		public void AddGold()
		{
			// Adds one gold
			gold++;
		}

		public void SubtractGold()
		{
			// Subtracts gold one
			gold--;
		}

		public void AddTurn()
		{
			// Adds one turn
			turns++;
		}

		public void AddAmmo()
		{
			// Adds two ammo
            ammo += 2; ;
		}

		public void SubtractAmmo()
		{
			// Subtracts one ammo
			ammo--;
		}

		public int CurrentGold()
		{
			// Returns how much gold there is currently
			return gold;
		}

		public int CurrentTurn()
		{
			// Returns how many turns have been taken
			return turns;
		}

		public int CurrentAmmo()
		{
			// Returns how much ammo there is currently
			return ammo;
		}

		public string GetPlayerName()
		{
			// Gives the player's name
            return playerName;
		}

		public int CalculateScore()
		{
			// Calculates the score based on the 
			// current gold, ammo, and turns
			return 100 - turns + gold + 10 * ammo;
		}
	}
}
