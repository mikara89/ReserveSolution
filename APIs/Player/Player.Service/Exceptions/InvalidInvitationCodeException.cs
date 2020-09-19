using System;

namespace Player.Service.Exceptions
{
    public class InvalidInvitationCodeException : Exception 
    {
        public InvalidInvitationCodeException() : base("Invalid invitation code.") 
        {

        }
    }
}
