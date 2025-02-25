using FluentValidation;

namespace CM25Server.Domain.Commands.Validators;

public class UpdateIssueCommandValidator : AbstractValidator<UpdateIssueCommand>
{
    public UpdateIssueCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.Name)
            .MaximumLength(128);
        
        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}