using FluentValidation;
using Player.Domains.Models;

namespace Player.Api.Validators
{
    public class PlayerUpdateValidator : AbstractValidator<PlayerUpdate>
    {
        public PlayerUpdateValidator()
        {
            RuleFor(x => x.FullName)
               .NotEmpty()
               .MaximumLength(40)
               .MinimumLength(4);

            RuleFor(x => x.NickName)
               .NotEmpty()
                .MaximumLength(20)
                .MinimumLength(4);
        }
    }
}
