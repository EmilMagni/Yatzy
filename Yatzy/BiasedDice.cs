using System;
namespace Yatzy
{
    public class BiasedDice : Dice
    {
        private bool isNegative { get; set; }
        private int degree { get; set; }

        public BiasedDice()
        {
            degree = 1;
            isNegative = true;
        }

        public BiasedDice(bool isNegative, int degree) //constructor Hver objekt af klassen BiasedDice skal enten være dårligere (negative biased) eller bedre end normal terning. Degree afgører graden af bias.
        {
            this.isNegative = isNegative; // Kigger i klassen og ser om der er noget der hedder isNegative
            this.degree = degree;
        }

        /// <summary>
        /// Roll metoden overskrives. En positive altså !isNegative dice rulles igen 
        /// indtil der er slået minimum 4 
        /// eller til der er forsøgt degree antal gange.
        /// Negative biased dice har degree antal forsøg til at slå 3 eller lavere.
        /// </summary>

        public override int Roll()
        {
            base.Roll(); 
            int throws = degree;

            while (throws > 0)
            { 
                if (isNegative && Current > 3)
                { 
                    base.Roll();
                }
                else if (!isNegative && Current < 4) 
                {
                    base.Roll();
                }
                throws--;
            }
            return Current;
        }
    }
}
