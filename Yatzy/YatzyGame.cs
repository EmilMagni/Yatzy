
using System;
namespace Yatzy
{
    /// <summary>
    /// Jeg har skrevet koden i samarbejde med min gruppe som består af: Nicolaj Lyth, Mads Møller, Patrick Hensberg, Christian Henriksen og Bogi Berg.
    /// Derudover har Jonas Lollesgaard deltaget i dele af kodningen.
    /// Dette Yatzy spil består af 6 klasser.
    /// Dice klassen definerer hvordan en enkelt terning opfører sig.
    /// BiasedDice arver fra Dice klassen, men i BiasedDice overskrives roll metoden så en Dice med positiv bias ruller flere gange til resultatet er minimum 4.
    /// Klassen TUI står for at håndtere input fra brugeren igennem konsollen. 
    /// Scoreboard står for at gemme resultaterne.
    /// I YatzyGame genereres terningerne og her findes logikken omkring yatzyspillet. 
    /// Den indeholder en række metoder såsom OnePair() der eks. tjekker om man har slået et par.
    /// Disse metoder tager alle udgangspunkt i metoden SortedArrayIterator.
    /// 
    /// SortedArrayIterator metoden:
    /// Der laves et array kaldet SortedArray som indeholder antallet af hver terningeværdi.
    /// Dvs. at på indeks 0 står antallet af 1'ere og på indeks 5 står antallet af 4'ere.
    /// Metoden SortedArrayIterator tager 3 parametre og checker i SortedArray for at undersøge hvad man har slået.
    /// SortedArrayIterator starter med at kigge på sidste plads i arrayet altså om der er mere end 1 6'er.
    /// Parametret condition er minimumsantal ens terninger vi vil checke for - number definerer hvor mange forskellige terningeværdier der skal opfylde betingelsen.
    /// Parametret offset bruges til at ignorere enten første eller sidste plads i arrayet ved small straight/large straight
    /// 
    /// Klassen InvalidCommandException laver en exception vi kan throw forskellige steder.
    /// Denne bruges hvis brugeren skriver forkert input ved hold eller save.
    /// 
    /// Der bruges 3 namespaces System, System.Collection.Generic og System.Linq
    /// System.Collection.Generic anvendes for at bruge datatypen Dictionary.
    /// System.Linq giver mulighed for at bruge ElementAt metoden som bruges til at checke de første 6 resultater i dictionary og beregne om der skal gives bonus i Scoreboard.
    /// </summary>
    public class YatzyGame
    {
        public TUI tui;
        private Dice[] dices; //instantierer et array som kun kan indeholde "terninger" af klassen Dice
        private int[] SortedArray; //Et array med resultatet af slaget talt op, så det indeholder antal 1'ere 2'ere 3'ere osv.
        public int TotalThrows { get; set; } //property der holder styr på hvor mange kast der er lavet
        public int NumberOfThrows { get; set; } = 3; //bestemmer hvor mange kast man må lave pr. runde. Default sat til 3.
        public int Turns { get; set; }
        public Scoreboard scoreboard;

        public YatzyGame() // default constructor der laver YatzyGame objekt
        {

            dices = new Dice[] //hver gang vi laver nyt YatzyGame objekt skal der laves et nyt array som indeholder 6 objekter af typen Dice dvs. 6 nye terninger.
            {
                new Dice(), new Dice(), new Dice(), 
                new Dice(), new Dice(), new Dice()
            };
            scoreboard = new Scoreboard();
        }

        public YatzyGame(int noDices)
        { //constructor til at vælge antal dices man spiller med.
            int n = noDices;

            dices = new Dice[n];
            for(int i = 0; i<n; i++)
            {
                dices[i] = new Dice();
            }
            scoreboard = new Scoreboard();
        }
        public void Turn()
        {
            TotalThrows = 0;
            ReleaseAll();
            Turns++;
        }

        public void Roll() 
        {
            foreach (Dice dice in dices) 
            {
                if (!dice.HoldState)
                {

                    dice.Roll(); 
                }
            }
            TotalThrows++;
            CountOfArray(); //Der laves et array kaldet SortedArray som ikke vises. I Arrayet gemmes antallet af 1'ere, 2'ere osv i sorteret rækkefølge.
        }

        public override string ToString()
        {
            string s = "Current throw: ";
            foreach (Dice dice in dices)
            {
                s = s + dice.Current + " ";
            }
            return s;
        }

        public void SetBiasedDices(bool isNegative, int degree, int howMany)
        {
            for (int i = 0; i < howMany; i++)
            {
                dices[i] = new BiasedDice(isNegative, degree);
            }
        }

        public void ResetAll() //sætter terningerne til at være normale igen
        {
            for (int i = 0; i < dices.Length; i++)
            {
                dices[i] = new Dice();
            }
        }
        public void HoldDices(int[] indexArray) //checker for hvert integer i i indexArray og skifter holdstate på hver dice
        {
            foreach(int i in indexArray)
            {
                dices[i-1].HoldState = true; //kigger i dices arrayet og i metoden har man ved index angivet hvilken terning der skal skifte HoldState property til true
            
            }
        }

        public void ReleaseDice(int index)
        {
            dices[index].HoldState = false;
        }
        public void ReleaseAll() //kører ReleaseDice på alle dices
        {
            for (int i = 0; i < dices.Length; i++)
            {
                ReleaseDice(i);
            }
        }


        // Metoder til at vise hvad man har slået
       
        public int CountOf(int number) //Tæller hvor mange gange man har slået "number"
        {
            int count = 0;

            foreach (Dice dice in dices) //checker alle terninger i vores dices array
            {
                if (dice.Current == number) //hvis værdien på terningen matcher det vi leder efter skal der tælles op
                {
                    count++;
                }
            }
            return count;
        }

        public int ValueOfDots(int number) 
        {
            int value = CountOf(number) * number;
            return value; //returnerer den samlede sum af en bestemt terning, eks. sum af alle 3'ere 
        }

        public void CountOfArray() //laver et nyt array der skal holde styr på hvor mange der er slået af hver nummer
        {
            int[] array = new int[6];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = CountOf(i + 1); //på indeks 0 står antallet af 1'ere osv.
            }
            SortedArray = array; //sætter SortedArray til at være et array der er sorteret efter 1'ere 2'ere 3'ere osv. På første plads i arrayet står antallet af 1'ere.
        }

        public int SortedArrayIterator(int condition, int number, int offset) //condition er minimumsantal ens terninger vi vil checke for - number definerer hvor mange forskellige terningeværdier der skal opfylde betingelsen. Offset
        {
            int count = 0;
            int value = 0;
            int startIndex = SortedArray.Length - 1; //sætter startindex til index 5
            int endIndex = 0;

            if (offset > 0) { endIndex += 1; } //hvis offset>0 stoppes der ved index 1 fremfor index 0 (large straight)
            if (offset < 0) { startIndex -= 1; } //hvis offset<0 startes der ved index 4 fremfor index 5 (small straight)

            for (int i = startIndex; i >= endIndex; i--)
            {
                if (SortedArray[i] >= condition)
                {
                    value += (i + 1) * condition;
                    count++;

                    if (count == number)
                    {
                        break;
                    }

                }
            }
            if (count < number) { value = 0; } //opsamler til hvis man leder efter to par og der kun er ét par skal værdien alligevel være 0
            return value;
        }

        public int OnePair()
        {
            int PairValue = SortedArrayIterator(2, 1, 0); //Checker om der er 2 ens (condition=2) af 1 slags (number=1) i hele arrayet (offset=0)
            return PairValue;
        }
        public int ThreeOfAKind()
        {
            int TripleValue = SortedArrayIterator(3, 1, 0);
            return TripleValue;
        }
        public int FourOfAKind()
        {
            int FourValue = SortedArrayIterator(4, 1, 0);
            return FourValue;
        }
        public int Yatzy()
        {
            int YatzyValue = SortedArrayIterator(dices.Length, 1, 0); //checker om alle er ens = yatzy
            if (YatzyValue > 0)
            {
                YatzyValue += 50; //Hvis man har slået yatzy får man bonus på 50 point
            }
            return YatzyValue;
        }
        public int TwoPairs() //Checker om der er 2 ens (condition=2) af 2 slags (number=2) i hele arrayet (offset=0)
        {
            int twoPairValue = SortedArrayIterator(2, 2, 0);
            return twoPairValue;
        }
        public int FullHouse()
        {
            int houseValue = 0;
            int triple = ThreeOfAKind(); //værdien af højeste 3 ens
            int onePair = OnePair(); //værdien af det højeste par
            int twoPairs = TwoPairs(); //værdien sum af de to højeste par 

            if (triple != 0 && twoPairs != 0) { //hvis der er tre ens og to par, hvilket er et krav for at der kan være full house.
                if (triple / 3 == onePair / 2) //checker om 3 ens stammer fra den samme terning som det højeste par
                {
                    houseValue = triple + (twoPairs - onePair); //housevalue sættes til sum af tre ens og laveste par
                }
                else
                {
                    houseValue = triple + onePair; //ellers sættes det til sum af tre ens og højeste par
                }
            }
            return houseValue;
        }
        public int LargeStraight()
        {
            int largeStraightValue = SortedArrayIterator(1, 5, 1); //checker om der er 1 af 5 forskellige slags og ignorerer index 0
            return largeStraightValue;
        }
        public int SmallStraight()
        {
            int smallStraightValue = SortedArrayIterator(1, 5, -1); //checker om der er 1 af 5 forskellige slags og ignorerer index 5
            return smallStraightValue;
        }
        public int Chance() //metode til at beregne summen af chance. Lægger værdien af hver terning sammen
        {
            int sum = 0;
            foreach (Dice dice in dices)
            {
                sum = sum + dice.Current;
            }
            return sum;
        }

    public void CallAllMethods()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            if (OnePair() > 0)
            {
                Console.WriteLine("Værdi af højeste par er: " + OnePair());
            }
            if (ThreeOfAKind() > 0)
            {
                Console.WriteLine("Værdi af højeste 3 ens er: " + ThreeOfAKind());
            }
            if (FourOfAKind() > 0)
            {
                Console.WriteLine("Værdi af 4 ens er: " + FourOfAKind());
            }
            if (TwoPairs() > 0)
            {
                Console.WriteLine("Værdi af to højeste par er: " + TwoPairs());
            }
            if (SmallStraight() > 0)
            {
                Console.WriteLine("Værdi af small straight er: " + SmallStraight());
            }
            if (LargeStraight() > 0)
            {
                Console.WriteLine("Værdi af large straight er: " + LargeStraight());
            }
            if (FullHouse() > 0)
            {
                Console.WriteLine("Værdi af full house er: " + FullHouse());
            }
            if (Yatzy() > 0)
            {
                Console.WriteLine("Værdi af yatzy er: " + Yatzy());
            }
            {
                Console.WriteLine("Værdi af chancen er: " + Chance());
            }
            Console.ResetColor();
            Console.WriteLine("Nuværende kast: ");
            foreach (Dice dice in dices)
            {
                if (dice.HoldState)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(dice.Current + " ");
                }
                else
                {
                    Console.ResetColor();
                    Console.Write(dice.Current + " ");
                }
            }
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}