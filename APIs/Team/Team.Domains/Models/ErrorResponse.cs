using System.Collections.Generic;

namespace Team.Domains.Models
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}
