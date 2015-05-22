using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus
{
	class Map
	{
		// Location of player
        private int playerPosition;

		// Location of Wumpus
		private int wumpusLocation;

		// Locations of hazards
        private int bat1Location, bat2Location, trap1Location, trap2Location;

		// Constructor
		public Map() 
        {
            // Initialize game
            Initialize();
        }

        // Properties
        public int WumpusPosition
        {
            // Wumpus room number
            get { return wumpusLocation; }
            set { wumpusLocation = value; }
        }

        public int PlayerPosition
        {
            // Player room number
			get { return playerPosition; }
			set { playerPosition = value; }
        }

		// Methods
		public bool NextToWumpus()
		{
			// Checks if player is one room away from the Wumpus
            if (RoomsNAway(playerPosition, 1).Contains(wumpusLocation)) return true;
			else return false;
		}

		public bool NextToBats()
		{
			// Checks if player is one room away from one of the bats
            if (RoomsNAway(playerPosition, 1).Contains(bat1Location) || RoomsNAway(playerPosition, 1).Contains(bat2Location)) return true;
			else return false;
		}

		public bool NextToPits()
		{
			// Checks if the player is one room away from one of the pits
            if (RoomsNAway(playerPosition, 1).Contains(trap1Location) || RoomsNAway(playerPosition, 1).Contains(trap2Location)) return true;
			else return false;
		}

		public bool SameRoomBats(int playerRoom)
		{
			// Checks if player is in the same room as a bat hazard
			if (playerRoom == bat1Location || playerRoom == bat2Location)
			{
				// Makes list of possible rooms for bats to move to
                List<int> rooms = new List<int>();
				for (int i = 0; i < 30; i++) rooms.Add(i);
				// Cannot move to the location of an existing hazard
                rooms.Remove(trap1Location);
                rooms.Remove(trap2Location);
				rooms.Remove(bat1Location);
				rooms.Remove(bat2Location);
                
                // Moves bats to new location
                Random rnd = new Random();
                bat1Location = (playerRoom == bat1Location) ? rooms[rnd.Next(26)] : bat1Location;
                bat2Location = (playerRoom == bat2Location) ? rooms[rnd.Next(26)] : bat2Location;

				return true;
			}
			else return false;
		}

		public bool SameRoomPit(int playerRoom)
		{
			// Checks if player is in the same room as pit hazard
            if (playerRoom == trap1Location || playerRoom == trap2Location) return true;
			else return false;
		}

		public bool SameRoomWumpus(int playerRoom)
		{
			// Checks if player is in the same room as Wumpus
			if (playerRoom == wumpusLocation) return true;
			else return false;
		}

		public bool ShootArrow(int targetRoom)
		{
			// Returns whether arrow was shot into Wumpus room
            if (wumpusLocation == targetRoom) return true;
            else return false;
		}

		private void Initialize()
		{
			// Randomizes the positions of the Wumpus and hazards
			Random rnd = new Random();

            //Randomly orders a list of ints 1-30 and takes five of them
            List<int> rooms = new List<int>();
			for (int i = 2; i < 31; i++) rooms.Add(i);
            rooms = rooms.OrderBy(s => rnd.Next()).Take(5).ToList();

			//Assigns hazards to the rooms
            trap1Location = rooms[0];
            trap2Location = rooms[1];
			bat1Location = rooms[2];
			bat2Location = rooms[3];
			wumpusLocation = rooms[4];
		}

        public int RndBatLocation()
        {
            // Returns the room number of one of the bats
            Random rnd = new Random();
            return (rnd.Next(2) == 0) ? bat1Location : bat2Location;
        }

        public int RndTrapLocation()
        {
            // Returns the room number of one of the caves
            Random rnd = new Random();
            return (rnd.Next(2) == 0) ? trap1Location : trap2Location;
        }

        public List<int> RoomsNAway(int room, int nAway)
		{
			List<int> rooms = new List<int>();
			rooms.Add(room);
			// Information for what you must add to each room number from each door, starting at 
            // the top door and going cloockwise to get the room number of an adjacent room
            int[,] adjacentRoomDifferences = new int[,] 
                { { -6, -5, 1, 6, -1, -7 }, // For odd columns
                  { -6, 1, 7, 6, 5, -1 }, // For even columns
                  { -6, -5, 1, 6, 5, -1 } }; // For special case columns (1&6)
            List<int> forAdding = new List<int>();
			// Finds rooms adjacent to rooms in the the list for specified number away
            for (int n = 0; n < nAway; n++)
			{
				// Finds all rooms adjacent to each room in the list
                foreach (int rm in rooms)
				{
					for (int i = 0; i < 6; i++)
					{
						// Checks if its an even, odd, or special case column
                        int column;
                        if ((rm) % 2 == 1 && (rm) % 6 != 1) column = 0; // Odd column
                        else if ((rm) % 2 == 0 && (rm) % 6 != 0) column = 1; // Even column
                        else column = 2; // Special case column
                        int check = (rm + adjacentRoomDifferences[column, i] + 30) % 30;// room number of adjacent room
                        // only adds if not already in list
                        if (!rooms.Contains(check) && !forAdding.Contains(check))
						{
							forAdding.Add(check);
						}
					}
				}
				// Cannot directly change rooms list, so uses temporary list and adds that
                rooms.AddRange(forAdding);
				forAdding.Clear();
			}
			rooms.Remove(room); // Removes the room itself, it is not adjacent to itself
            // Room 0 does not exist, 30 does
            if (rooms.Contains(0))
			{
				rooms.Remove(0);
				rooms.Add(30);
			}
			// Returns list in order of least to greatest
            return rooms.OrderBy(r => r).ToList();
		}
	}
}

