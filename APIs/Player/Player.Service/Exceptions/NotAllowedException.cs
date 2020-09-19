using System;

namespace Player.Service.Exceptions 
{
    public class NotAllowedException : Exception 
    {
        public NotAllowedException(string message) : base(message)
        {

        }
    }
}
