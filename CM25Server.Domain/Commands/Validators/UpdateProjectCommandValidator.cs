using FluentValidation;

namespace CM25Server.Domain.Commands.Validators;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.Name)
            .MaximumLength(128);

        RuleFor(x => x.Code)
            .MaximumLength(3);
        
        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}