using FluentValidation;

namespace CM25Server.Domain.Commands.Validators;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(3);
        
        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}