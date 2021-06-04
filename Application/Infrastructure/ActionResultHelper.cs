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
            return result.Map(result => bs.Ok(result), err =>
            {
                Func<IError, string> concatErrors = err => err.Errors
                    .Aggregate("", (agg, curr) => agg + curr.ToString() + "\n");

                return err switch
                {
                    ValidationError => bs.BadRequest(concatErrors(err)),
                    DomainError => bs.BadRequest(concatErrors(err)),
                    AuthorizationError => bs.Unauthorized(concatErrors(err)),
                    SystemError => bs.StatusCode(StatusCodes.Status500InternalServerError, concatErrors(err)),
                    NotFoundError => bs.NotFound("404"),
                    _ => bs.StatusCode(StatusCodes.Status500InternalServerError, "Not supported")
                };
            });
        }
    }
}