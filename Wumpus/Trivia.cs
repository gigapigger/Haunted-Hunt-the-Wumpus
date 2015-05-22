using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Wumpus
{
    class Trivia
    {
        List<Question> trivias = new List<Question>();
        int triviaIndex = 0;

        public Trivia()
        {
            ReadFromFile();
            RandomizeTrivia();
        }

        public string[] GetTrivia()
        {
            Question t = trivias[triviaIndex];
            triviaIndex++;
            return new string[] {t.question, t.answer1, t.answer2, t.answer3, t.trueAnswer };
        }

        public string[] GetHint()
        {
            Random rnd = new Random();
            Question t = trivias[rnd.Next(triviaIndex)];
            return new string[2] { t.question, t.trueAnswer };
        }

        private void ReadFromFile()
        {
            StreamReader sr = new StreamReader("Trivia.txt");
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] data = line.Split(',');
                trivias.Add(new Question(data[0], data[1], data[2], data[3], data[4]));
            }
            sr.Close();
        }

        private void RandomizeTrivia()
        {
            Random rnd = new Random();
            trivias.OrderBy(r => rnd.Next());
        }

        struct Question
        {
            public string question;
            public string answer1;
            public string answer2;
            public string answer3;
            public string trueAnswer;
            public Question(string q, string a1, string a2, string a3, string ta)
            {
                this.question = q;
                this.answer1 = a1;
                this.answer2 = a2;
                this.answer3 = a3;
                this.trueAnswer = ta;
            }
        }
    }
}