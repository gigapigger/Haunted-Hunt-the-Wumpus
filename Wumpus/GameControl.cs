using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstPersonWumpus.UI;
using System.Windows.Forms;
using System.IO;

namespace Wumpus
{
	class GameControl : UIControllerInterface
	{
		int correctAnswerIndex = 3;

		int correctAnswers = 0;
		int incorrectAnswers = 0;
		int numberOfQs = 0;
		
		Trivia trivia;
        ActiveWumpus activeWumpus;
        HighScore highScore;
        Cave cave;
        Player player;
        Map map;

        string lastTrivia;
        List<string> missedTrivia = new List<string>();

        public GameControl()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Launcher l = new Launcher(this);
            Application.Run(l);
        }

        #region Methods
        void UIControllerInterface.Turn(int room)
		{
			player.AddTurn();
			player.AddGold();
            map.PlayerPosition = room;
            map.WumpusPosition = activeWumpus.SetWumpusPosition(
                map.WumpusPosition,
                player.CurrentTurn(),
                map.RoomsNAway(room, 2).ToArray(),
                map.RoomsNAway(room, 1).ToArray()
                );
		}

        bool UIControllerInterface.Shoot(int targetRoom)
		{
			player.SubtractAmmo();
            if (map.ShootArrow(targetRoom))
            {
                highScore.AddScore(new HighScore.Score(player.GetPlayerName(), player.CalculateScore(), player.CurrentGold(),
                    player.CurrentAmmo(), player.CurrentTurn(), cave.Difficulty));
                highScore.SaveScores();
                return true;
            }
            else return false;
		}

        string[,] UIControllerInterface.GetHighScores()
		{
			List<HighScore.Score> tempHighScores = highScore.ReadScores();
			string[,] highScores = new string[10,6];
			foreach (HighScore.Score score in tempHighScores)
			{
				string difficulty;
				if (score.difficulty == 0) difficulty = "Easy";
				else if (score.difficulty == 1) difficulty = "Medium";
				else difficulty = "Hard";
				highScores[tempHighScores.IndexOf(score), 0] = score.name;
				highScores[tempHighScores.IndexOf(score), 1] = score.score.ToString();
				highScores[tempHighScores.IndexOf(score), 2] = difficulty;
				highScores[tempHighScores.IndexOf(score), 3] = score.turns.ToString();
				highScores[tempHighScores.IndexOf(score), 4] = score.gold.ToString();
				highScores[tempHighScores.IndexOf(score), 5] = score.arrows.ToString();
			}
			return highScores;
		}

        void UIControllerInterface.Initialize(string playerName, int difficulty)
        {
            map = new Map();
            highScore = new HighScore();
            player = new Player(playerName);
            trivia = new Trivia();
            cave = new Cave(difficulty);
            activeWumpus = new ActiveWumpus();
        }

        bool UIControllerInterface.WumpusHere(int room)
        {
            if (map.SameRoomWumpus(room) == true) return true;
            else return false;
        }

        bool UIControllerInterface.BatsHere(int room)
        {
            if (map.SameRoomBats(room) == true) return true;
            else return false;
        }

        bool UIControllerInterface.PitHere(int room)
        {
            if (map.SameRoomPit(room) == true) return true;
            else return false;
        }

        int[] UIControllerInterface.AdjacentRooms(int room)
        {
            return map.RoomsNAway(room - 1, 1).ToArray();
        }

        string[] UIControllerInterface.GetTriviaQuestion()
        {
            Random rnd = new Random();
            string[] triviaQuestion = trivia.GetTrivia();
            triviaQuestion.Reverse<string>().Take(4).OrderBy(r => rnd.Next()).ToList().Insert(0, triviaQuestion[0]);
            return triviaQuestion.ToArray();
        }

        bool UIControllerInterface.AnswerTrivia(int answerChoice)
        {
            bool correct;
            if (correctAnswerIndex == answerChoice)
            {
                correct = true;
                correctAnswers++;
            }
            else
            {
                correct = false;
                incorrectAnswers++;
            }

            if (!correct)
                missedTrivia.Add(lastTrivia);
            return correct;
        }

        void UIControllerInterface.BeginTrivia(int questions)
        {
            numberOfQs = questions;

            correctAnswers = 0;
            incorrectAnswers = 0;

            List<string[]> qs = new List<string[]>();

            for (int i = 0; i < numberOfQs; i++)
            {
                qs.Add(trivia.GetTrivia());
            }
        }

        int UIControllerInterface.EncounterBats(int room)
        {
            Random rnd = new Random();
            int carryTo = rnd.Next(30);
            map.PlayerPosition = carryTo;
            return carryTo;
        }

        int UIControllerInterface.EscapePit()
        {
            return 1;
        }

        void UIControllerInterface.BeatWumpus()
        {
            activeWumpus.beatWumpus(player.CurrentTurn());
        }

        bool[] UIControllerInterface.Doors(int room)
        {
			return cave.RoomDoors(room);
        }

        string UIControllerInterface.BuySecret()
        {
            Random rnd = new Random();
            int index = rnd.Next(6);
            switch(index)
            {
                case 0: return string.Format("Bats are in room {0}." , map.RndBatLocation());
                case 1: return string.Format("A trap is in room {0}." , map.RndTrapLocation());
                case 2: return string.Format((map.RoomsNAway(player.CurrentTurn(), 2).Contains(map.WumpusPosition)) ?
                "The wumpus is one or two rooms away" : "The wumpus is more than two rooms away");
                case 3: return string.Format("The wumpus is in room {0}." , map.WumpusPosition);
                case 4: return string.Format("You are in room {0}.", map.PlayerPosition);
                default:
                    string[] hint = trivia.GetHint();
                    return string.Format("{0} /nCorrect answer: /n{1}", hint[0], hint[1]);
            }
        }

        void UIControllerInterface.BuyArrow()
        {
            player.AddAmmo();
        }
        
        #endregion

        #region Properties
        int PlayerPosition
        {
            get { return PlayerPosition; }
            set { PlayerPosition = value; }
        }

        int UIControllerInterface.Gold
        {
            get { return player.CurrentGold(); }
        }

        int UIControllerInterface.Arrows
		{
			get { return player.CurrentAmmo(); }
		}

        int UIControllerInterface.Turns
		{
			get { return player.CurrentTurn(); }
		}

        int UIControllerInterface.Score
		{
			get { return player.CalculateScore(); }
		}

        int UIControllerInterface.WumpusLocation
        {
            get { return map.WumpusPosition; }
        }

        bool UIControllerInterface.WumpusNearby
        {
            get { return map.NextToWumpus(); }
        }

        bool UIControllerInterface.BatsNearby
        {
            get { return map.NextToBats(); }
        }

        bool UIControllerInterface.PitNearby
        {
            get { return map.NextToPits(); }
        }

        int UIControllerInterface.FailedQuestions
        {
			get { return incorrectAnswers; }
        }

        public int TotalSessionQuestions
        {
			get { return numberOfQs; }
        }

        public int TotalAnsweredQuestions
        {
			get { return correctAnswers + incorrectAnswers; }
        }

        bool UIControllerInterface.WonTrivia
        {
			get { return ((correctAnswers > incorrectAnswers) && (TotalAnsweredQuestions >= TotalSessionQuestions)); }
        }

        #endregion
    }
}
