using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Wumpus
{
    public class HighScore
    {
        // A list that tracks the high scores
        private List<Score> scores = new List<Score>();

        // HighScore constructor
        public HighScore()
        {
            scores = ReadScores();
        }

        public void AddScore(Score newScore)
        // Adds a new score to the file
        {
            // The ReadScores() method must be used first so scores will have correct data
            scores.Add(newScore);
            // Sorts list from highscore to low score
            scores.Sort();
            // If theres more than 10 highscore, delete the lowest scores
            if (scores.Count > 10)
                scores.RemoveRange(10, scores.Count - 10);
        }

        public List<Score> ReadScores()
        // Reads the high scores from file
        {
            // Sees if "HighScores" exists. If it doesnt, this is the first time the game
            // has been played so return a list with the names of our group members
            if (!File.Exists("HighScores"))
                return new List<Score>(){new Score("Oscar", 0, 0, 0, 0, 0), new Score("Jeffrey", 0, 0, 0, 0, 0),
                    new Score("Julia", 0, 0, 0, 0, 0), new Score("Rowan", 0, 0, 0, 0, 0), 
                    new Score("Chris", 0, 0, 0, 0, 0), new Score("Nick", 0, 0, 0, 0, 0)};
            BinaryReader reader = new BinaryReader(File.Open("HighScores", FileMode.OpenOrCreate));
            // Create a list to store scores
            List<Score> tempScores = new List<Score>();
            try
            {
                // Read the number of scores there are, which was written first
                int count = reader.ReadInt32();
                // Read the scores in the order that they were written
                for (int i = 0; i < count; i++)
                {
                    Score line = new Score(reader.ReadString(), reader.ReadInt32(), reader.ReadInt32(), 
                        reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                    tempScores.Add(line);
                }
            }
            catch (System.IO.EndOfStreamException ex)
            {
                Console.WriteLine(ex.Message);
                reader.Close();
                return new List<Score>();
            }
            reader.Close();
            return tempScores;
        }

        public void SaveScores()
        // Saves score to file
        {
            // If a file already exists, delete it so it can be replaced
            if (File.Exists("HighScores"))
                File.Delete("HighScores");
            BinaryWriter writer = new BinaryWriter(File.Open("HighScores", FileMode.OpenOrCreate));
            // First, write how many scores there are to save
            writer.Write(scores.Count);
            // Then, write each score in orderfrom highest to lowest, each one in that specific combo of variables
            foreach (Score s in scores)
            {
                writer.Write(s.name);
                writer.Write(s.score);
                writer.Write(s.gold);
                writer.Write(s.arrows);
                writer.Write(s.turns);
                writer.Write(s.difficulty);
            }
            writer.Close();
        }

        public struct Score : IComparable<Score>
        {
            // Score struct so each score can contain all relavent data
            // Inherites from IComparable<Score> so it uses Score.score when 
            // the sort method is used with a list of type Score
            public string name;
            public int score, gold, arrows, turns, difficulty;

            public Score(string name, int score, int gold, int arrows, int turns, int difficulty)
            {
                this.name = name;
                this.score = score;
                this.gold = gold;
                this.arrows = arrows;
                this.turns = turns;
                this.difficulty = difficulty;
            }

            int IComparable<Score>.CompareTo(Score s)
            {
                // Makes it so it will sort greatest to smallest
                return this.score.CompareTo(s.score) * -1;
            }
        }
    }
}