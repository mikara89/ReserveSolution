using System;

namespace Team.Service.Exceptions
{
    public class AlreadyExistException : Exception
    { 
        public AlreadyExistException(string message):base(message)
        {

        }
    }
}
