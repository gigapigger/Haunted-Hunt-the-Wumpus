using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Wumpus
{
	class Cave
    {
        int difficulty; // The difficulty
        bool[][] doors; // Stores connectivity information

        public Cave(int difficulty) 
        {
            // Sets the difficulty for this game
            this.difficulty = difficulty;
            // Creates cave for this game
            CreateRndCave(difficulty);
			doors = new bool[30][];
			InitializeDoors();
        }

        /// <summary>
        /// Cave generator difficulty level (0, 1, and 2 for easy, medium and hard)
        /// </summary>
        public int Difficulty
        {
            get { return difficulty; }
        }

        /// <summary>
        /// Returns an array of doors for a room
        /// </summary>
        /// <param name="room">Number of the room</param>
        /// <returns>An array of 6 booleans</returns>
        public bool[] RoomDoors(int room)
        {
            return doors[room - 1];
        }
        
        private void InitializeDoors()
		{
			StreamReader sr = new StreamReader("Cave.txt");
			string line;
            int roomCounter = 0;
            while ((line = sr.ReadLine()) != null)
			{
				int doorIndex = 0;
				char[] doorsS = line.ToCharArray();
                doors[roomCounter] = new bool[6];
				foreach (char door in doorsS)
				{
                    doors[roomCounter][doorIndex] = ((door == '0') ? false : true);
					doorIndex++;
				}
                roomCounter++;
			}
			sr.Close();
		}

        private void CreateRndCave(int difficulty)
        // Creates a random cave
        {
            bool works = false; // Keeps making caves until one works
            int threeCounter = 0; // Keeps track of how many rooms have three doors, which determines difficulty
            // This array is information for what you must add to each room number to get
            // the room number of an adjacent room. The first number in each row corresponds to what you must add to
            // get the room to the top of the hexagon, and the others corresponding to in order going clockwise from there.
            int[,] adjacentRoomDifferences = new int[,]
                { { -6, -5, 1, 6, -1, -7 }, // For odd number columns
                  { -6, 1, 7, 6, 5, -1 }, // For even number colums
                  { -6, -5, 1, 6, 5, -1 } }; // For special case columns (1&6)
            List<int> rms = new List<int> { 0, 1, 2, 3, 4, 5 };
            int counter = 0;
            while (!works)
            // Loops through making maps until one meets the requirements
            {
                Random rnd = new Random();
                int[,] doors = new int[30, 6]; // Stores information for doors. 30 rooms, 6 0s/1s for wall/door
                for (int thisRoom = 0; thisRoom < 30; thisRoom++) // loops through once for each room.
                // Makes a map by randomly selecting door positions in each room
                {
                    // Randomly picks how many doors this room will have
                    int doorsInRoom = rnd.Next(3);
                    // Randomly picks which positions to put the rooms
                    for (int i = 0; i <= doorsInRoom; i++) // Loops through as many times as doors were selected to be in this room
                    {
                        int openDoor = rnd.Next(6); // randomly selects a position for a door
                        // Makes sure this room does not have more than three doors
                        for (int ii = 0; ii < 6; ii++) { counter += doors[thisRoom, ii]; }
                        if (counter < 3) doors[thisRoom, openDoor] = 1; // only adds the door if this room has less than three doors
                        counter = 0;
                    }
                    // Makes a list of which places in this room have doors
                    List<int> selectedDoors = rms.Where(r => doors[thisRoom, r] == 1).ToList(); // Finds positions of all doors for this room
                    // Determines if this room is in an odd number column, even number column, or special case column (1&6)
                    int column;
                    if ((thisRoom + 1) % 2 == 1 && (thisRoom + 1) % 6 != 1) column = 0; // odd number column
                    else if ((thisRoom + 1) % 2 == 0 && (thisRoom + 1) % 6 != 0) column = 1; // even number column
                    else column = 2; // special case column
                    foreach (int doorPos in selectedDoors) // Foreach door in this room
                    // Makes sure doors match up, so two adjacent rooms agree if there is a door between them.
                    // It does adding doors in the corresponding positions of doors in adjacent rooms to those in this room
                    {
                        // Sees how many rooms there are in adjecent room to doorPosition
                        for (int i = 0; i < 6; i++) { counter += doors[(thisRoom + adjacentRoomDifferences[column, doorPos] + 30) % 30, i]; }
                        // Adds door to adjacent room in corresponding position if the adjacent room has less than three doors
                        if (counter < 3) doors[((thisRoom + adjacentRoomDifferences[column, doorPos] + 30) % 30),
                            (doorPos + 3) % 6] = doors[thisRoom, doorPos];
                        // The special case is when there are three doors in the adjacent room, but one of them is in the 
                        // corresponding spot to the door position. For easy and medium if the adjecent room has three doors and 
                        // one of the doors is the correct spot, it still adds the door to this room
                        else if (doors[((thisRoom + adjacentRoomDifferences[column, doorPos] + 30) % 30),
                            (doorPos + 3) % 6] == 1 && difficulty - 1 > 0)
                        {
                            doors[thisRoom, doorPos] = 0;
                            doors[((thisRoom + adjacentRoomDifferences[column, doorPos] + 30) % 30), (doorPos + 3) % 6] = 0;
                        }
                        // For hard if the adjecent room has three doors and one of the doors is the correct spot,
                        // delete the door that is in the correct spot in the adjacent room so less rooms will have three doors
                        else if (doors[((thisRoom + adjacentRoomDifferences[column, doorPos] + 30) % 30),
                            (doorPos + 3) % 6] == 1) doors[thisRoom, doorPos] = 1;
                        // If the adjacent room to this door postion has three rooms and a door is not in the 
                        // corresponding spot to this door, this door will be changed to a wall
                        else doors[thisRoom, doorPos] = 0;
                        counter = 0;
                    }
                }
                int[] beenReached = new int[30]; // Array for storing which rooms are reachable from room 1 (1 for yes, 0 for no)
                beenReached[0] = 1; // Adds first room as reachable to see if other rooms can reach it
                int previous = 0; // Number of rooms that were reachable in previous round
                bool checking = true;
                while (checking)
                // Checks to see if each room is reachable
                // Loops through over and over, adding rooms that are reachable to already reached rooms until 
                // all have been reached or some cant be reached. It is essentially a floodfill algorithim
                {
                    for (int room = 0; room < 30; room++)
                    // Every time it loops, checks all 30 rooms for reachable rooms to already reached rooms
                    {
                        if (beenReached[room] == 1) // If this room has been reached
                        // For each room that has already been reached, add rooms that can be reached from those rooms
                        {
                            // Makes a list of positions of this rooms' doors
                            List<int> selectedDoors = rms.Where(r => doors[room, r] == 1).ToList();
                            if (selectedDoors.Count == 3) threeCounter++; // Adds to threeCounter room has  three doors
                            // Determines whether this room is in an odd number column, even number clolumn, or special case column
                            int column;
                            if ((room + 1) % 2 == 1 && (room + 1) % 6 != 1) column = 0; // Room is in an odd column
                            else if ((room + 1) % 2 == 0 && (room + 1) % 6 != 0) column = 1; // Room is in an even column
                            else column = 2; // Room is in a special case column
                            foreach (int r in selectedDoors)
                            // For each door in this room
                            {
                                // Adds the rooms that are adjacent to the doors of the this room, 
                                // which means they can be reached from this already reached room
                                beenReached[(room + adjacentRoomDifferences[column, r] + 30) % 30] = 1;
                            }
                        }
                    }
                    if (previous == beenReached.Sum())
                        checking = false; // There are unreachable rooms if another room wasnt touched this loop
                    previous = beenReached.Sum(); // Stores how many rooms have been touched this round
                    if (beenReached.Sum() == 30 && 20 - (10 * difficulty) <= threeCounter
                        && 25 - (10 * difficulty) >= threeCounter)
                    // Writes map to file if every room is reachable and there is an 
                    // acceptable number of rooms with three doors for the difficulty.
                    // Acceptable number of rooms with three doors: 20-25 for easy, 10-15 for medium, and 0-5 for hard
                    {
                        StreamWriter riter = new StreamWriter(File.Open("Cave.txt", FileMode.OpenOrCreate));
                        for (int i = 0; i < 30; i++)
                        {
                            for (int ii = 0; ii < 6; ii++)
                            {
                                // Writes each room as a line in a text file
                                riter.Write(doors[i, ii]);
                            }
                            riter.WriteLine();
                        }
                        riter.Close();
                        // Stops the looping
                        checking = false;
                        works = true;
                    }
                    threeCounter = 0; // Resets threeCounter for next round
                }
            }
        }
	}
}
