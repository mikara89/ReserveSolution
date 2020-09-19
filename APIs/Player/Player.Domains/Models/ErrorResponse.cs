using System.Collections.Generic;

namespace Player.Domains.Models
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}
