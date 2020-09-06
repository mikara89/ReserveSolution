using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team.Data.Models.Entites;
using Team.Data.Persistence;
using Team.Domains.Models;

namespace Team.Api.Validators
{
    public class TeamCreateValidator:AbstractValidator<TeamCreate>
    {
        public TeamCreateValidator(TeamDBContext teamDbContext)
        {
            RuleFor(x => x.RegNumber)
                .NotEmpty()
                .Must(RegNumber => 
                {
                     return !teamDbContext.Teams.Any(t => t.RegNumber == RegNumber);
                }).WithMessage("Registration number already taken.")
                .LessThan(1000000000)
                .GreaterThan(99999999);

            RuleFor(x => x.TeamName)
               .NotEmpty()
               .Must(TeamName =>
               {
                   return !teamDbContext.Teams.Any(t => t.TeamName == TeamName);
               }).WithMessage("Team name already taken.")
               .MaximumLength(20)
               .MinimumLength(4);
        }
    }
}
