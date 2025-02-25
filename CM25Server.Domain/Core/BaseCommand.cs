using CM25Server.Domain.Exceptions;
using FluentValidation;
using LanguageExt.Common;

namespace CM25Server.Domain.Core;

public record BaseCommand<TCommand, TValidator> 
    where TCommand : BaseCommand<TCommand, TValidator>
    where TValidator : AbstractValidator<TCommand>, new()
{
    public async Task<Result<TCommand>> ValidateAsync(CancellationToken cancellationToken)
    {
        TValidator validator = new();
        var validationResult = await validator.ValidateAsync((TCommand)this, cancellationToken);

        return !validationResult.IsValid
            ? new Result<TCommand>(ProblemException.FromValidationResult(validationResult))
            : (TCommand)this;
    }
}