using System;
namespace Yatzy
{
    public class Dice
    {
        protected Random rand; //instansvariabel af datatypen Random. Protected så den kan bruges til nedarvninger.

        public Dice() // default constructor fordi den hedder det samme som klassen og parentesen er tom
        {
            rand = new Random(Guid.NewGuid().GetHashCode());
        }
        public int Current //property (en auto property da den både har get og set)
        { get; set; }
        public bool HoldState
        { get; set; }

        public virtual int Roll() {
            Current = rand.Next(1, 7);
            return Current;
        }
        public override string ToString() 
        {
            return "Current value is " + Current;
        }
    }
}
