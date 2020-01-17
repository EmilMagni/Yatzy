using System;

namespace Yatzy
{
    public class TUI //Text User Interface
    {
        public YatzyGame yatzy;
        public Scoreboard scoreboard;
        private bool TurnSaved { get; set; }
        public TUI()
       
        {
            yatzy = new YatzyGame(); 
        }

        private void NewTurn()
        {
            yatzy.Turn();
            TurnSaved = false;
            while (!TurnSaved)
            {
                try
                {
                    Console.WriteLine($"Du har {yatzy.NumberOfThrows - yatzy.TotalThrows} kast tilbage");
                    if (ThrowsLeft())
                    {
                        EnterCommands();
                    }
                    else
                    {
                        Save();
                    }
                }
                catch (InvalidCommandException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void NewGame()
        {
            while (!yatzy.scoreboard.GameFinished())
            {
                NewTurn();
            }
                Console.WriteLine("Spillet er færdigt");
                Console.WriteLine($"Din samlede score er: {yatzy.scoreboard.TotalSum()}");
        }

        private bool ThrowsLeft()
        {
            bool b = false;
            if (yatzy.TotalThrows < yatzy.NumberOfThrows)
            {
                b = true;
            }
            return b;
        }

        public string GetUserInput()
        {
            Console.WriteLine("Skriv kommando: ");
            string s = Console.ReadLine(); 
            return s;
        }
        public string GetUserInput(string message)
        {
            Console.WriteLine(message);
            string s = Console.ReadLine(); 
            return s;
        }
        public string[] SplitString(string stringToSplit)
        {
            string[] s = stringToSplit.Split(',');
            return s;
        }

        public int ParseString(string stringToParse)
        {
            int i = 0;
            try
            {
                i = int.Parse(stringToParse);
            }
            catch (FormatException)
            {
                Console.WriteLine($"'{stringToParse}' is not an integer");
            }
            catch (OverflowException) //hvis man har skrevet en for lang int der ikke passer i int32
            {
                Console.WriteLine($"'{stringToParse}' is too long");
            }
            return i;
        }

        public int[] ParseArray(string[] arrayToParse)
        {
            int[] intArray = new int[arrayToParse.Length]; //laver nyt array på samme længde uden indhold
            for (int i = 0; i < intArray.Length; i++)
            {
                intArray[i] = ParseString(arrayToParse[i]); //bruger ParseString metoden på alle pladser i intArray. Den string som parses er den string som står på i plads i arrayToParse
            }
            return intArray;
        }

        public int[] StringToIntArray()
        {
            string s = GetUserInput();
            return ParseArray(SplitString(s));
        }

        public int[] StringToIntArray(string message)
        {
            string s = GetUserInput(message);
            return ParseArray(SplitString(s));
        }

        public bool CheckTheInput(int[] arrayToCheck)
        {
            bool b = false;
            foreach (int i in arrayToCheck)
            {
                if (i >= 1 && i <= 6)
                {
                    b = true;
                }
                else if (i == 0)
                {
                    b = true;
                }
                else
                {
                    Console.WriteLine("Integers to hold must be between 1 and 6");
                    b = false;
                    break;
                }
            }
            return b;
        }
        public void EnterIntegersToHold()
        {
            int[] array;
            do
            {
                array = StringToIntArray("Indtast hvilke terninger du vil holde");
            }
            while (!CheckTheInput(array));
            yatzy.HoldDices(array);
        }

        public void EnterBiased()
        {
            bool isNegative = false;
            bool inputCheck = false;
            while (!inputCheck)
            {
                string s = GetUserInput("Indtast 'positive' for at lave bedre terninger eller 'negative' for at lave dårligere terninger");
                if (s == "positive")
                {
                    isNegative = false;
                    inputCheck = true;
                }
                else if (s == "negative")
                {
                    isNegative = true;
                    inputCheck = true;
                }
                else
                {
                    Console.WriteLine("forkert input: skriv 'negative' eller 'positive'");
                }
            }
            int degree = ParseString(GetUserInput("Indtast graden af bias"));
            int howMany = ParseString(GetUserInput("Indtast ønsket antal snydeterninger"));
            yatzy.SetBiasedDices(isNegative, degree, howMany);
        }
           
        public void EnterCommands()
        {
            bool CorrectCommand = false;
            while (!CorrectCommand)
            {
                string s = GetUserInput().ToLower();
                switch (s)
                {
                    case "roll":
                        yatzy.Roll();
                        CorrectCommand = true;
                        yatzy.CallAllMethods();
                        break;

                    case "hold":
                        EnterIntegersToHold();
                        CorrectCommand = true;
                        break;

                    case "release":
                        yatzy.ReleaseAll();
                        break;

                    case "reset":
                        yatzy.ResetAll();
                        break;

                    case "help":
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Mulige kommandoer: \n'roll' for at rulle terningerne. \n'hold' for at holde på terninger. \n'release' slipper de holdte terninger. \n'set biased' for at lave snydeterninger. \n'reset' for at lave terningerne fair igen. \n'save' for at gemme og strege et resultat. \n'print' for at vise score.");
                        Console.ResetColor();
                        break;

                    case "set biased":
                        EnterBiased();
                        CorrectCommand = true;
                        break;

                    case "cancel":
                        CorrectCommand = true;
                        break;

                    case "print":
                        PrintScore();
                        break;

                    case "save":
                        Save();
                        CorrectCommand = true; 
                        break;

                    default:
                        Console.Beep();
                        throw new InvalidCommandException($"Du har ikke skrevet en korrekt kommando (du skrev '{s}')");
                }
            }
        }
        public void Save()
        {
            bool CorrectCommand = false;
            TurnSaved = false;

            while (!CorrectCommand)
            {
                string s = GetUserInput("Skriv hvad du vil gemme").ToLower();

                if (!yatzy.scoreboard.UpperSectionFinished())
                {
                    switch (s)
                    {
                        default:
                            Console.Beep();
                            throw new InvalidCommandException($"Du har ikke skrevet en korrekt kommando (du skrev '{s}')");
                        
                        case "aces":
                            if (yatzy.scoreboard.scores["Aces"] == -1)
                            {
                                yatzy.scoreboard.scores["Aces"] = yatzy.ValueOfDots(1);
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt");
                                Console.Beep();
                            }
                            break;
                        case "twos":
                            if (yatzy.scoreboard.scores["Twos"] == -1)
                            {
                                yatzy.scoreboard.scores["Twos"] = yatzy.ValueOfDots(2);
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt");
                                Console.Beep();
                            }
                            break;
                        case "threes":
                            if (yatzy.scoreboard.scores["Threes"] == -1)
                            {
                                yatzy.scoreboard.scores["Threes"] = yatzy.ValueOfDots(3);
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt");
                                Console.Beep();

                            }
                            break;
                        case "fours":
                            if (yatzy.scoreboard.scores["Fours"] == -1)
                            {
                                yatzy.scoreboard.scores["Fours"] = yatzy.ValueOfDots(4);
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt");
                                Console.Beep();
                            }
                            break;
                        case "fives":
                            if (yatzy.scoreboard.scores["Fives"] == -1)
                            {
                                yatzy.scoreboard.scores["Fives"] = yatzy.ValueOfDots(5);
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt");
                                Console.Beep();
                            }
                            break;
                        case "sixes":
                            if (yatzy.scoreboard.scores["Sixes"] == -1)
                            {
                                yatzy.scoreboard.scores["Sixes"] = yatzy.ValueOfDots(6);
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt");
                                Console.Beep();
                            }
                            break;
                        case "help":
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("Mulige kommandoer: aces, twos, threes, fours, fives, sixes");
                            Console.ResetColor();
                            break;

                    }
                }
                else
                {
                    switch (s)
                    {
                        default:
                            Console.Beep();
                            throw new InvalidCommandException($"Du har ikke skrevet en korrekt kommando (du skrev '{s}')");

                        case "pair":
                            if (yatzy.scoreboard.scores["Pair"] == -1)
                            {
                                yatzy.scoreboard.scores["Pair"] = yatzy.OnePair();
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt.");
                                Console.Beep();
                            }
                            break;

                        case "two pairs":
                            if (yatzy.scoreboard.scores["Two Pair"] == -1)
                            {
                                yatzy.scoreboard.scores["Two Pair"] = yatzy.TwoPairs();
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt");
                                Console.Beep();
                            }
                            break;
                        case "three of a kind":
                            if (yatzy.scoreboard.scores["Three of a kind"] == -1)
                            {
                                yatzy.scoreboard.scores["Three of a kind"] = yatzy.ThreeOfAKind();
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt");
                                Console.Beep();
                            }
                            break;
                        case "four of a kind":
                            if (yatzy.scoreboard.scores["Four of a kind"] == -1)
                            {
                                yatzy.scoreboard.scores["Four of a kind"] = yatzy.FourOfAKind();
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt");
                                Console.Beep();
                            }
                            break;
                        case "full house":
                            if (yatzy.scoreboard.scores["Full House"] == -1)
                            {
                                yatzy.scoreboard.scores["Full House"] = yatzy.FullHouse();
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt");
                                Console.Beep();
                            }
                            break;
                        case "small straight":
                            if (yatzy.scoreboard.scores["Small Straight"] == -1)
                            {
                                yatzy.scoreboard.scores["Small Straight"] = yatzy.SmallStraight();
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt");
                                Console.Beep();
                            }
                            break;
                        case "large straight":
                            if (yatzy.scoreboard.scores["Large Straight"] == -1)
                            {
                                yatzy.scoreboard.scores["Large Straight"] = yatzy.LargeStraight();
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt");
                                Console.Beep();
                            }
                            break;
                        case "yatzy":
                            if (yatzy.scoreboard.scores["Yatzy"] == -1)
                            {
                                yatzy.scoreboard.scores["Yatzy"] = yatzy.Yatzy();
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt");
                                Console.Beep();
                            }
                            break;
                        case "chance":
                            if (yatzy.scoreboard.scores["Chance"] == -1)
                            {
                                yatzy.scoreboard.scores["Chance"] = yatzy.Chance();
                                CorrectCommand = true;
                                Console.WriteLine($"{s} er nu gemt");
                            }
                            else
                            {
                                Console.WriteLine($"{s} er allerede gemt");
                                Console.Beep();
                            }
                            break;
                        case "help":
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("Mulige kommandoer: pair, two pair, three of a kind, four of a kind, full house, small straight, large straight, yatzy, chance");
                            Console.ResetColor();
                            break;
                    }
                }
                TurnSaved = true;
            }
        }
        public void PrintScore()
        {
            foreach (var i in yatzy.scoreboard.scores)
            {
                Console.WriteLine("Værdien af " + i.Key + " er " + i.Value);
            }
            Console.WriteLine("Din samlede score er: " + yatzy.scoreboard.TotalSum());
        }

    }
}
        

