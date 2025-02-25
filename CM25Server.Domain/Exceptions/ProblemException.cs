using FluentValidation.Results;

namespace CM25Server.Domain.Exceptions;

public class ProblemException(string error, string message) : Exception(message)
{
    public string Error { get; private init; } = error;

    public static ProblemException FromValidationResult(ValidationResult validationResult) =>
        new("Validation failed", string.Join(' ', validationResult.Errors.Select(x => x.ErrorMessage)));
}