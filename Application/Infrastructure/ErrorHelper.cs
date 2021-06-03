using System.Linq;

namespace MyRental
{
    public static class ErrorHelper
    {
        public static Result<Unit> CheckForErrors(this Condition[] conditions)
        {
            var errors = conditions.Aggregate(new string[] { }, (agg, curr) => curr.pedricate
                ? agg.Concat(new string[] { curr.msg }).ToArray()
                : agg);

            return errors.Count() == 0
                ? Result<Unit>.Succeed(new Unit())
                : Result<Unit>.Failure("Validation errors..." + errors.Aggregate((agg, curr) => agg + "\n" + curr));
        }
    }
}