using System;
using System.Collections.Generic;
using System.Linq;
namespace Yatzy
{
    public class Scoreboard
    {
        public Dictionary<string, int> scores;

        public Scoreboard()
        {
            scores = new Dictionary<string, int>();
            //Dictionary med det som bruges i upper section af spillet
            scores.Add("Aces", -1);
            scores.Add("Twos", -1);
            scores.Add("Threes", -1);
            scores.Add("Fours", -1);
            scores.Add("Fives", -1);
            scores.Add("Sixes", -1);

            //Dictionary med det som bruges i lower section af spillet
            scores.Add("Pair", -1);
            scores.Add("Two Pair", -1);
            scores.Add("Three of a kind", -1);
            scores.Add("Four of a kind", -1);
            scores.Add("Full House", -1);
            scores.Add("Small Straight", -1);
            scores.Add("Large Straight", -1);
            scores.Add("Yatzy", -1);
            scores.Add("Chance", -1);
        }
        public int CountSum()
        {
            int sum = 0;
            foreach (var i in scores)
            {
                if (i.Value >= 0)
                {
                    sum += i.Value;
                }
            }
            return sum;
        }
        public int Bonus()
        {
            int bonus = 0;
            int sum = 0;
            for (int i = 0; i<=6; i++)
            {
                sum += scores.ElementAt(i).Value; // ElementAt kommer fra linq looper igennem de første 6 pladser i dictionary og tager summen for at se om der skal være bonus
            }
            if (sum>=63)
            {
                bonus = 50;
            }
            return bonus;
        }
        public bool GameFinished()
        {
            bool b = true;
            foreach (var i in scores)
            {
                if (i.Value == -1)
                {
                    b = false;
                    break;
                }
            }
            return b;
        }
        public bool UpperSectionFinished() //checker om de 6 første altså aces til og med sixes er streget
        {
            bool b = true;
            for (int i = 0; i < 6; i++)
            {
                int value = scores.ElementAt(i).Value;
                if (value == -1)
                {
                    b = false;
                }
            }
            return b;
        }
        public int TotalSum()
        {
           return CountSum() + Bonus();
        }
    }
}
