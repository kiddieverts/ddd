using System.Collections.Generic;
using System.Linq;

namespace MyRental
{
    public static class ErrorHelper
    {
        public static List<ErrorType> CreateEmptyErrorList() => new List<ErrorType>();

        public static Result<Unit> CheckForErrors(this List<ErrorType> errors) =>
            errors.Count() == 0
                ? Result<Unit>.Succeed(new Unit())
                : Result<Unit>.Failure(ValidationError.Create(errors.ToArray()));

        public static List<ErrorType> Validate(this List<ErrorType> list, bool b, ErrorType s)
            => (b ? list.Concat(new List<ErrorType> { s }) : list).ToList();
    }
}