﻿using System;

namespace Team.Service.Exceptions
{
    public class NotAllowedException : Exception 
    {
        public NotAllowedException(string message) : base(message)
        {

        }
    }
}
