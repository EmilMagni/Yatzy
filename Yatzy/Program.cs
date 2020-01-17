using System;

namespace Yatzy
{
    class Program
    {
        static void Main(string[] args)
        {
            TUI tui = new TUI();
            Console.WriteLine("Velkommen til det gode Yatzyspil");
            Console.WriteLine("Skriv roll for at rulle terningerne");
            tui.NewGame();
        }
    }
}

