using FluentValidation;
using System.Linq;
using Team.Data.Persistence;
using Team.Domains.Models;

namespace Team.Api.Validators
{
    public class TeamUpdateValidator : AbstractValidator<TeamUpdate>
    {
        public TeamUpdateValidator(TeamDBContext teamDbContext)
        {
            RuleFor(x => x.RegNumber)
                .NotEqual(0)
                .Unless(m => !string.IsNullOrEmpty(m.TeamName))
                .Must(RegNumber =>
                {
                    return !teamDbContext.Teams.Any(t => t.RegNumber == RegNumber);
                }).WithMessage("Registration number already taken.")
                .Unless(m => m.RegNumber == 0)
                .LessThan(1000000000)
                .Unless(m=>m.RegNumber==0)
                .GreaterThan(99999999)
                .Unless(m => m.RegNumber == 0);

            RuleFor(x => x.TeamName)
                .NotEmpty()
                .Unless(m => m.RegNumber!=0)
               .Must(TeamName =>
               {
                   return !teamDbContext.Teams.Any(t => t.TeamName == TeamName);
               }).WithMessage("Team name already taken.")
               .MaximumLength(20)
               .MinimumLength(4);  
        }
    }
}
