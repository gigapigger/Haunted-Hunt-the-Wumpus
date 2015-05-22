using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FirstPersonWumpus.UI//Do not change this namespace! Instead, add using FirstPersonWumpus.UI; to the top of the game control class
{
	/// <summary>
	/// The game control should implement this interface and implement all methods.
	/// This interface contains all the methods that the UI needs to interact with the game control.
	/// Be sure to make all methods and properties public
	/// </summary>
	public interface UIControllerInterface
	{
		#region Methods
		
		/// <summary>
		/// Called when a new game starts
		/// * Load/prepare the cave, map, hazards, trivia, and all variables/stats for a new game session
		/// </summary>
		/// <param name="playername">The player's name</param>
		/// <param name="difficulty">The game's difficulty</param>		
		void Initialize(string playername, int difficulty);
		
		/// <summary>
		/// Called when the player changes rooms (player takes a "turn").
		/// * Add 1 to the turn count and increase the player's gold by 1.
		/// </summary>
		/// <param name="room">The number of the room the player moved into. (Room number is 1-30)</param>		
		void Turn(int room);

		/// <summary>
		/// Called when the player shoots an arrow. 
		/// * Decrease arrow count by 1
		/// * Move the wumpus if the arrow misses
		/// </summary>
		/// <param name="targetRoom">The number of the room the player is shooting into. (Room number is 1-30)</param>
		/// <returns>Returns true if the wumpus is in targetRoom, otherwise returns false.</returns>
		bool Shoot(int targetRoom);

		/// <summary>
		/// Gets the numbers of the rooms that are touching a specified room
		/// </summary>
		/// <param name="room">The number of the room to get the set of adjacent rooms for. (Room number is 1-30)</param>
		/// /// <returns>
		/// An array of 6 integers containing the numbers of the rooms adjacent to 'room'.
		/// The array should be in clockwise order, with the room to the north of 'room' at index 0, and the room to the northwest at index 5.
		/// Ex: For room 15 this method returns {9,10,16,21,14,8}
		/// </returns>		
		int[] AdjacentRooms(int room);

		/// <summary>
		/// Gets a trivia question and answers
		/// * Store the correct answer so it can be compared when the question is answered
		/// </summary>
		/// <returns>
		/// An array of strings containing the question text and each of the answer choices.
		/// Ex: {"Question text", "Answer choice A", "Answer choice B", "Answer choice C", "Answer choice D"}
		/// </returns>
		string[] GetTriviaQuestion();

		/// <summary>
		/// Evaluates the player's answer to the current trivia question.
		/// * Increase the number of correctly answered trivia questions if the player's answer is correct.
		/// * Increase the number of incorrectly answered trivia questions if the player's answer is incorrect.
		/// * Remove 1 gold
		/// * If the player does not have any gold, always count the answer as incorrect
		/// </summary>
		/// <param name="answerChoice">The answer choice selected by the player</param>
		/// <returns>False if the player has no gold or answered incorrecly. True if the player answered correctly.</returns> 
		bool AnswerTrivia(int answerChoice);
		
		/// <summary>
		/// Starts a new trivia session
		/// * Reset the number of correct/incorrectly answered questions to 0
		/// * Set the number of questions that will be asked in this session to the input value
		/// </summary>
		/// <param name="questions">The number of questions that will be asked in the trivia session</param>
		void BeginTrivia(int questions);

		/// <summary>
		/// Called when the player enters a room that contains bats
		/// * Move the player to a random room.
		/// * Move the bats that are in 'room' to a random room
		/// </summary>
		/// <returns>The number of the random room that the player is moved to.</returns>
		int EncounterBats(int room);

		/// <summary>
		/// Called after the player wins a trivia session to escape a pit
		/// * Move the player to the starting room
		/// </summary>
		/// <returns>The number the room the player is moved to after escaping a pit.</returns>
		int EscapePit();

		/// <summary>
		/// Called when the player wins a trivia session for fighting the wumpus
		/// * Move the wumpus after beating it in a fight
		/// </summary>
		void BeatWumpus();

		/// <summary>
		/// /// Checks if the wumpus is in 'room'
		/// </summary>
		/// <param name="room">The room to check for a wumpus</param>
		/// <returns>True if 'room' contains a wumpus</returns>
		bool WumpusHere(int room);

		/// <summary>
		/// Checks if there are bats in in 'room'
		/// </summary>
		/// <param name="room">The room to check for bats</param>
		/// <returns>True if 'room' contains bats</returns>
		bool BatsHere(int room);

		/// <summary>
		/// Checks if there is a pit in 'room'
		/// </summary>
		/// <param name="room">The room to check for a pit</param>
		/// <returns>True if 'room' contains a pit</returns>
		bool PitHere(int room);

		/// <summary>
		/// Gets an array of booleans for the doorways and walls of a room. 
		/// </summary>
		/// <param name="room">The number of the room</param>
		/// <returns>
		/// An array of 6 booleans where a true value is a doorway and a false value is a wall.
		/// The array should be in clockwise order, with the north doorway at index 0, and northwest doorway at index 5.
		/// </returns>
		bool[] Doors(int room);

		/// <summary>
		/// Gets an array of the top 10 highscores
		/// </summary>
		/// <returns>
		/// An array of string arrays, where the first item in the array is the label for each index, and each item after that is the information for a hightscore
		/// Ex: { 
		///       { "Name", "Score", "Map", "Turns", "Gold", "Arrows" },
		///       { "Bob", "300", "Map1", "30", "11", "2"},
		///       { "Fred", "200", "Map3", "28", "7", "1"},
		///       { "Sam", "100", "Map2", "33", "6", "2"} 
		///     }
		/// </returns>
		string[,] GetHighScores();


        /// <summary>
        /// Called after winning purchase arrow trivia.
        /// * Gain 2 arrows
        /// </summary>
        void Buy2Arrows();

        /// <summary>
        /// Called after winning purchase secret trivia.
        /// </summary>
        /// <returns>The message/secret that will be displayed to the player.</returns>
        string BuySecret();
		#endregion

		#region Properties

        int PlayerPosition
        {
            get;
            set;
        }
        
        /// <summary>
		/// Gets the amount of gold the player has
		/// </summary>
		int Gold
		{
			get;
		}

		/// <summary>
		/// Gets the number of arrows the player has
		/// </summary>
		int Arrows
		{
			get;
		}

		/// <summary>
		/// Gets the number of turns the player has taken
		/// </summary>
		int Turns
		{
			get;
		}

		/// <summary>
		/// Gets the player's current score
		/// </summary>
		int Score
		{
			get;
		}

		/// <summary>
		/// Gets the number of the room containing the wumpus.
		/// </summary>
        int WumpusLocation
        {
            get;
        }

		/// <summary>
		/// Returns true if the wumpus is in an adjacent room. (Adjacent to the player's current room)
		/// Returns false if the wumpus is not in an adjacent room.
		/// </summary>
		bool WumpusNearby
		{
			get;
		}

		/// <summary>
		/// Returns true if bats are in an adjacent room. (Adjacent to the player's current room)
		/// Returns false if bats are not in an adjacent room.
		/// </summary>
		bool BatsNearby
		{
			get;
		}

		/// <summary>
		/// Returns true if a pit is in an adjacent room. (Adjacent to the player's current room)
		/// Returns false if a pit is not in an adjacent room.
		/// </summary>
		bool PitNearby
		{
			get;
		}
		
		/// <summary>
		/// Gets the number of trivia questions the player has answered incorrectly
		/// </summary>
		int FailedQuestions
		{
			get;
		}

		/// <summary>
		/// Gets the number of total trivia questions that will be asked in the current trivia session
		/// </summary>
		int TotalSessionQuestions
		{
			get;
		}

		/// <summary>
		/// Gets the number of total trivia questions that have been answered in the current session
		/// </summary>
		int TotalAnsweredQuestions
		{
			get;
		}

		/// <summary>
		/// Return true if the number of correctly answered questions is greater than the number of incorrectly answered questions
		/// </summary>
		bool WonTrivia
		{
			get;
		}       
        #endregion
	}
}


