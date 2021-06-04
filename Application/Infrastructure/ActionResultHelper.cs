using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyRental
{
    public static class ActionResultHelper
    {
        public static ActionResult<T> ToActionResult<T>(this Result<T> result, ApiControllerBase bs)
        {
            return result.Map(result => bs.Ok(result), errors =>
            {
                Func<IError[], string> concatErrors = err => err
                    .SelectMany(r => r.Errors)
                    .Aggregate("", (agg, curr) => agg + curr.ToString() + "\n");

                return errors.First() switch // TODO: Hmmm....
                {
                    ValidationError => bs.BadRequest(concatErrors(errors)),
                    DomainError => bs.BadRequest(concatErrors(errors)),
                    AuthorizationError => bs.Unauthorized(concatErrors(errors)),
                    SystemError => bs.StatusCode(StatusCodes.Status500InternalServerError, concatErrors(errors)),
                    _ => bs.StatusCode(StatusCodes.Status500InternalServerError, "Not supported")
                };
            });
        }
    }
}