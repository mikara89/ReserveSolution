using System;

namespace Player.Service.Exceptions 
{
    public class AlreadyExistException : Exception
    { 
        public AlreadyExistException(string message):base(message)
        {

        }
    }
}
