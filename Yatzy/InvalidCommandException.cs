using System;
namespace Yatzy
{
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(string message):base(message)
        {
        }
    }
}
