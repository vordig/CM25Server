using FluentValidation;

namespace CM25Server.Domain.Commands.Validators;

public class CreateIssueCommandValidator : AbstractValidator<CreateIssueCommand>
{
    public CreateIssueCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.ProjectId)
            .NotEmpty();
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(128);
        
        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}