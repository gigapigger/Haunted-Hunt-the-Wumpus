using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus
{
	class ActiveWumpus
	{
        // The wumpus states
		enum wumpusState { lostTrivia, moving, asleep };
        wumpusState currentState = wumpusState.asleep;
		
        // ints for which turn the states should start/stop
        int endLostTrivia, startMoving = 0, stopMoving = 0;
		
        // Constructor
        public ActiveWumpus() { }

		public int SetWumpusPosition(int wumpusPosition, int turn, int[] rms2Away, int[] rms1Away)
		{
			// Calculates the wumpus' position

            Random rnd = new Random();
            // Sets the turn that the wumpus starts moving in, for the start of the game
            if (turn == 1) 
                startMoving = rnd.Next(6) + 5;
            
            // There is a 5% chance each turn that the wumpus randomly teleports to a new location
			if (rnd.Next(20) == 0)
				return rnd.Next(30) + 1;

			// If the wumpus is to begin moving, set for how long and change state to moving
			if (startMoving == turn)
			{
				stopMoving = startMoving + rnd.Next(3) + 1;
				currentState = wumpusState.moving;
			}

			switch (currentState)
			{
				case wumpusState.lostTrivia:
					{
                        // The wumpus moves randomly up to two rooms away while in lostTrivia state
                        if (turn <= endLostTrivia)
                            return rms2Away[rnd.Next(18)];
                        else
                        {
                            currentState = wumpusState.asleep; // Change the wumpusState to asleep if lostTrivia is over
                            return wumpusPosition; // The wumpus is now asleep and does not move
                        }
					}
				case wumpusState.moving:
					{
                        // The wumpus moves randomly one room away while in the moving state
                        if (turn <= stopMoving)
							return rms1Away[rnd.Next(6)];
						else
						{
							// If moving state has stopped, set the time for next moving state 
                            // and change wumpus state to asleep
                            startMoving = turn + rnd.Next(6) + 5;
							currentState = wumpusState.asleep;
                            return wumpusPosition; // The wumpus is now asleep and does not move
						}
					}
				default:
                    // If the wumpus is asleep it does not move
					return wumpusPosition;
			}
		}

		public void beatWumpus(int turn)
		{
			// When the wumpus loses trivia, it enters the lostTrivia state for up to three turns
			Random rnd = new Random();
			endLostTrivia = turn + 1 + rnd.Next(3); // Randomly sets when to end lostTrivia state
			startMoving = endLostTrivia + rnd.Next(6) + 5; // Postpones moving state
			currentState = wumpusState.lostTrivia; // Changes state to lostTrivia
		}
	}
}



